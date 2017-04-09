using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tan4it
{
    using GameObjects = List<GameObject>;
    using GamePoint = System.Windows.Point;

    interface TankChassis //Шасси танка
    {
        int Angle //Угол поворота
        {
            get; set;
        }
        void Print(Graphics g, GamePoint point);
    }

    interface TankWeapon //Пушка танка
    {
        int Angle //Угол поворота
        {
            get; set;
        }
        void Print(Graphics g, GamePoint point);
    }

    delegate System.Windows.Vector WhereToGoTankStrategy(Tank tank);//Стратегия выбора направления и скорости движения
    delegate int WhereToShootTankStrategy(Tank tank);//Стратегия выбора направления пушки

    class DefaultTankChassis : TankChassis
    {
        public int Angle
        {
            get; set;
        }
        public void Print(Graphics g, GamePoint point)
        {
            g.DrawImage(Properties.Resources.defaultTankChassis, (float)point.X, (float)point.Y, 20,40);
            //g.DrawRectangle(new Pen(Brushes.Black), (float)point.X, (float)point.Y, 10, 10);
        }
    }

    class DefaultTankWeapon : TankWeapon
    {
        public int Angle
        {
            get; set;
        }
        public void Print(Graphics g, GamePoint point)
        {
            g.DrawRectangle(new Pen(Brushes.Gray), (float)point.X, (float)point.Y, 10, 3);
        }
    }

    class Tank : GameObject
    {
        public GameObjects gameObjects
        {
            get; set;
        }

        public TankChassis TankChassis
        {
            get; set;
        }

        public TankWeapon TankWeapon
        {
            get; set;
        }

        WhereToGoTankStrategy whereToGoTankStrategy;
        WhereToShootTankStrategy whereToShootTankStrategy;

        public Tank(int health, GameObjects gameObjects) : base(health)
        {
            this.gameObjects = gameObjects;
            Position = new GamePoint(100, 100);
        }

        public override void Print(Graphics g)
        {
            TankChassis.Print(g, Position);
            TankWeapon.Print(g, Position);
        }

        public override void applyDamage(int damage)
        {
            Health -= damage;
        }

        public override void Update(int timeDelta)
        {
            Position += whereToGoTankStrategy(this) * timeDelta/10;
        }

        public void setWhereToShootTankStrategy(WhereToShootTankStrategy wtsts)
        {
            whereToShootTankStrategy = wtsts;
        }

        public void setWhereToGoTankStrategy(WhereToGoTankStrategy wtgts)
        {
            whereToGoTankStrategy = wtgts;
        }
    }

}
