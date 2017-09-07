using System;
using System.Windows.Input;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tan4it
{
    using System.Net.Sockets;
    using GameObjects = List<GameObject>;//Псевдоним к типу списка объектов
    using GamePoint = System.Windows.Point;

    interface IPrintable
    {
        void Print(Graphics g);
    }

    abstract class GameObject : IPrintable
    {
        public GamePoint Position
        {
            get;
            set;
        }

        public int Health
        {
            get;
            protected set;
        }

        public GameObject(int health)
        {
            Health = health;
        }

        abstract public void applyDamage(int damage);

        abstract public void Print(Graphics g);

        abstract public void Update(int timeDelta);
    }

    class tan4itGame : IPrintable
    {
        GameObjects gameObjects;

        TcpClient tcpClient;

        public tan4itGame()
        {
            //Подключение к серваку
            try
            {
                tcpClient = new TcpClient("home.4it.me", 5222);
            }
            catch (Exception e)
            {

            }
            //Создание танка игрока
            Tank ftank = new Tank(100, gameObjects, new Random().Next(999999999));
            //Отправка id танка игрока на сервер
            if (tcpClient != null)
            {
                tcpClient.Client.Send(Encoding.ASCII.GetBytes("T:" + ftank.Id.ToString() + '\n'));
            }
            //Создание шасси
            ftank.TankChassis = new DefaultTankChassis();
            //Создание пушки
            ftank.TankWeapon = new DefaultTankWeapon();
            //Стратегия движения
            ftank.setWhereToGoTankStrategy((tank, timeDelta) => {
                int x = 0;
                int y = 0;
                y -= Keyboard.IsKeyDown(Key.Up) ? 1 : 0;
                y += Keyboard.IsKeyDown(Key.Down) ? 1 : 0;
                x += Keyboard.IsKeyDown(Key.Right) ? 1 : 0;
                x -= Keyboard.IsKeyDown(Key.Left) ? 1 : 0;
                System.Windows.Vector vector = new System.Windows.Vector(x, y);
                if (vector.LengthSquared != 0)
                {
                    tank.TankChassis.Angle = (int)System.Windows.Vector.AngleBetween(new System.Windows.Vector(0, 1), vector);//Угол поворота шасси
                    tank.Position += vector * timeDelta / 10;
                    tank.InMotion = true;
                }
                else
                {
                    tank.InMotion = false;
                }
                
            });
            //Стратегия стрельбы
            ftank.setWhereToShootTankStrategy((tank, timeDelta) => {

            });
            //Создать массив игровых объектов
            gameObjects = new GameObjects();
            //Добавить танк в массив игровых объектов
            gameObjects.Add(ftank);
        }

        string tcpReceivedString = "";

        public void Update(int timeDelta)
        {
            //Получение позиций танков с сервера
            if (tcpClient != null)//Если соединение ваще есть
            {
                int tcpAvaible = tcpClient.Available;//Получаем, сколько байтов пришло с сервера
                if (tcpAvaible > 0)//Если че нибудь пришло с сервера
                {
                    byte[] bytes = new byte[tcpAvaible];//Массив, в который считывается поток с сервера
                    tcpClient.Client.Receive(bytes);//Считать поток байтов с сервера
                    tcpReceivedString += Encoding.UTF8.GetString(bytes);//Перевести поток байтов в строку
                    Console.Write(tcpReceivedString);
                    if (tcpReceivedString.Last() == '\n')//Если конец строки
                    {
                        if (tcpReceivedString.IndexOf("T:") == 0)//Проверка, того ли типа это сообщение
                        {
                            string[] strArr = tcpReceivedString.Split(' ');
                            int receivedMessageTankId;
                            if (Int32.TryParse(strArr[0].Split(':')[1], out receivedMessageTankId))
                            {
                                Tank ftank = (Tank)gameObjects.Find((GameObject go)=> {
                                    if (go is Tank)
                                    {
                                        Tank tankItem = (Tank)go;
                                        return tankItem.Id == receivedMessageTankId;
                                    }
                                    else {
                                        return false;
                                    }
                                });
                                if (ftank == null)
                                {
                                    //Создание танка с сервера с полученным id
                                    ftank = new Tank(100, gameObjects, receivedMessageTankId);
                                    //Создание шасси
                                    ftank.TankChassis = new DefaultTankChassis();
                                    //Создание пушки
                                    ftank.TankWeapon = new DefaultTankWeapon();
                                    //Стратегия движения
                                    ftank.setWhereToGoTankStrategy((tank, ttimeDelta) => {

                                    });
                                    //Стратегия стрельбы
                                    ftank.setWhereToShootTankStrategy((tank, ttimeDelta) => {
   
                                    });

                                    gameObjects.Add(ftank);
                                }
                                if (strArr.Length == 3)
                                {
                                    Double receivedTankX;
                                    Double receicedTankY;
                                    if (Double.TryParse(strArr[1], out receivedTankX) && Double.TryParse(strArr[2], out receicedTankY))
                                    {
                                        ftank.Position = new GamePoint(receivedTankX, receicedTankY);
                                    }
                                }
                                
                            }
                        }
                        tcpReceivedString = "";
                    }
                }
            }

            foreach (GameObject go in gameObjects)
            {
                go.Update(timeDelta);
                if (go is Tank)
                {
                    Tank ftank = (Tank)go;
                    if (ftank.InMotion)
                    {
                        if (tcpClient != null)
                        {
                            tcpClient.Client.Send(Encoding.ASCII.GetBytes("T:" + ftank.Id.ToString() + ' ' + ftank.Position.X.ToString() + ' ' + ftank.Position.Y.ToString() + '\n'));
                        }
                    }
                    
                }
            }
        }

        public void Print(Graphics g)
        {
            Pen pen = new Pen(Color.Black, 2);
            g.DrawString("tan4it", new Font("Tahoma", 12), Brushes.Black, new Point(0, 0));
            foreach (GameObject go in gameObjects)
            {
                go.Print(g);
            }
        }
    }
}
