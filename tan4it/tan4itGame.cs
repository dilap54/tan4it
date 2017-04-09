using System;
using System.Windows.Input;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tan4it
{
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
            protected set;
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

        public tan4itGame()
        {
            gameObjects = new GameObjects();
            Tank ftank = new Tank(100, gameObjects);
            ftank.TankChassis = new DefaultTankChassis();
            ftank.TankWeapon = new DefaultTankWeapon();
            ftank.setWhereToGoTankStrategy((tank) => {
                int x = 0;
                int y = 0;
                y -= Keyboard.IsKeyDown(Key.Up) ? 1 : 0;
                y += Keyboard.IsKeyDown(Key.Down) ? 1 : 0;
                x += Keyboard.IsKeyDown(Key.Right) ? 1 : 0;
                x -= Keyboard.IsKeyDown(Key.Left) ? 1 : 0;
                return new System.Windows.Vector(x, y);
            });
            gameObjects.Add(ftank);
        }

        public void Update(int timeDelta)
        {
            foreach (GameObject go in gameObjects)
            {
                go.Update(timeDelta);
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
