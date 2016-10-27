using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CommonPart
{
    class LaserTop:Bullet
    {
        public double length = 0;
        public double angle;
        protected double omega;
        protected Color color;
        protected Enemy enemy;

        public LaserTop(double _x, double _y, MoveType _move_type, double _speed, double _acceleration, double _radian, string _anime, int _zoom_rate,double _radius, int _life, int _score, int _sword,
            double _angle,double _omega,Enemy _enemy,Color _color)
            : base(_x, _y, _move_type, _speed, _acceleration,_radian, _anime, _zoom_rate, _radius, _life, _score, _sword)
        {
            angle = _angle;
            omega = _omega;
            enemy = _enemy;
            color = _color;
        }

        public override void update(Player player)
        {
            update();

            switch (move_type)
            {
                case MoveType.chase_angle:
                    speed += acceleration;
                    length += speed;
                    int fix;
                    if (Math.Abs(x - enemy.x) < 0.01)
                    {
                        fix = y > enemy.y ? 1 : -1;
                        if (player.x > x)
                        {
                            angle -= omega*fix;
                        }
                        else
                        {
                            angle += omega*fix;
                        }
                    }
                    else
                    {
                        double k = (y - enemy.y) / (x - enemy.x);
                        fix = x > enemy.x ? 1 : -1;

                        if ((k * (player.x - enemy.x) + enemy.y > player.y))
                        {
                            angle -= omega*fix;
                        }
                        else
                        {
                            angle += omega*fix;
                        }
                    }

                    x = enemy.x+length * Math.Cos(angle);
                    y = enemy.y+length * Math.Sin(angle);
                    break;
                case MoveType.go_straight:
                    speed += acceleration;
                    length += speed;
                    angle = Math.PI / 2;
                    x = enemy.x+ length * Math.Cos(angle);
                    y = enemy.y+length * Math.Sin(angle);
                    break;
            }

            if (x < Map.leftside + animation.X / 2) { length-= Map.leftside + animation.X / 2-x; x = Map.leftside + animation.X/2; }
            if (x > Map.rightside - animation.X/2) { length += Map.rightside - animation.X / 2 - x; x = Map.rightside - animation.X/2; }
            if (y > DataBase.WindowSlimSizeY - animation.Y/2) { length+= DataBase.WindowSlimSizeY - animation.Y / 2-y; y = DataBase.WindowSlimSizeY - animation.Y / 2; }
            if (y < 0 + animation.Y / 2) {length-= animation.Y / 2-y; y = 0 + animation.Y / 2; }
        }

        public override void draw(Drawing d)
        {
            double dx, dy;
            dx = (radius) * Math.Cos(angle - Math.PI / 2)/2;
            dy = (radius) * Math.Sin(angle - Math.PI / 2) / 2;
            d.DrawLine(new Vector(enemy.x + dx, enemy.y + dy), new Vector(x + dx, y+dy), (float)radius, new Color(color,(int)(color.A*0.3)), DepthID.Player);
            dx = (radius*2/3) * Math.Cos(angle - Math.PI / 2) / 2;
            dy = (radius*2 /3) * Math.Sin(angle - Math.PI / 2) / 2;
            //d.DrawLine(new Vector(enemy.x, enemy.y), new Vector(x, y), (float)radius, new Color(color, (int)(color.A * 0.6)), DepthID.StateBack);
            d.DrawLine(new Vector(enemy.x+dx, enemy.y+dy), new Vector(x+dx, y+dy), (float)radius*2/3, new Color(color, (int)(color.A * 0.6)), DepthID.Player);
            dx = (radius / 4) * Math.Cos(angle - Math.PI / 2) / 2;
            dy = (radius / 4) * Math.Sin(angle - Math.PI / 2) / 2;
            d.DrawLine(new Vector(enemy.x+dx, enemy.y+dy), new Vector(x+dx, y+dy), (float)radius/4, color, DepthID.Player);

        }
    }
}
