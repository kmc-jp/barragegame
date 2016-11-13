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
        protected double omega;
        protected Color color;
        protected Enemy enemy;

        public LaserTop(double _x, double _y, MoveType _move_type, double _speed, double _acceleration, double _radian, string _anime, int _zoom_rate,double _radius, int _life, int _score, int _sword,
            double _omega,Enemy _enemy,Color _color)
            : base(_x, _y, _move_type, _speed, _acceleration,_radian, _anime, _zoom_rate, _radius, _life, _score, _sword)
        {
            omega = _omega;
            enemy = _enemy;
            color = _color;
        }

        public override void update(Player player)
        {
            update();
            #region Laser Motion: length is not changed here!
            switch (move_type)
            {
                case MoveType.chase_angle:
                    speed += acceleration;
                    int fix;
                    if (Math.Abs(x - enemy.x) < 0.01)
                    {
                        fix = y > enemy.y ? 1 : -1;
                        if (player.x > x)
                        {
                            radian -= omega*fix;
                        }
                        else
                        {
                            radian += omega*fix;
                        }
                    }
                    else
                    {
                        double k = (y - enemy.y) / (x - enemy.x);
                        fix = x > enemy.x ? 1 : -1;

                        if ((k * (player.x - enemy.x) + enemy.y > player.y))
                        {
                            radian -= omega*fix;
                        }
                        else
                        {
                            radian += omega*fix;
                        }
                    }

                    x = enemy.x+length * Math.Cos(radian);
                    y = enemy.y+length * Math.Sin(radian);
                    break;
                case MoveType.go_straight:
                    speed += acceleration;
                    radian = Math.PI / 2;
                    x = enemy.x+ length * Math.Cos(radian);
                    y = enemy.y+length * Math.Sin(radian);
                    break;
            }
            #endregion
            #region Laser cannot getout of Window;  changes length and limit it
            /*
            if ( x < Map.leftside + animation.X / 2) { length-= Map.leftside + animation.X / 2-x; x = Map.leftside + animation.X/2; }
            if (x > Map.rightside - animation.X/2) { length -= x- Map.rightside + animation.X / 2;  x = Map.rightside - animation.X/2; }
            if (y > DataBase.WindowSlimSizeY - animation.Y/2) { length-= y- DataBase.WindowSlimSizeY + animation.Y / 2; y = DataBase.WindowSlimSizeY - animation.Y / 2; }
            if (y < 0 + animation.Y / 2) {length-= animation.Y / 2-y; y = 0 + animation.Y / 2; }
            */
            if ((x < Map.leftside + animation.X / 2) || (x > Map.rightside - animation.X / 2) || (y > DataBase.WindowSlimSizeY - animation.Y / 2)
                || (y < 0 + animation.Y / 2) )
            { // もしレーザーの先頭がすでに画面外に出ているなら、lengthを増やさない
    
            }else
            {
                length += speed;
            }
            if (length <= 0) { length = 0; }
            #endregion

            if (hit_jugde(player) == true)
            {
                player.damage(atk);
                //レーザーはキャラクターにダメージを与えても消えない。
            }
        }

        public override bool hit_jugde(Player player)
        {
            bool close;
            bool hit;
            double k;
            double d;


            if (Math.Abs(enemy.x - x) > 0.05)
            {
                if (enemy.x <= x)
                {
                    if (enemy.y <= y)
                    {
                        close = (player.x > enemy.x && player.x < x) && (player.y > enemy.y && player.y < y);
                    }
                    else
                    {
                        close = (player.x > enemy.x && player.x < x) && (player.y < enemy.y && player.y > y);
                    }
                }
                else
                {
                    if (enemy.y <= y)
                    {
                        close = (player.x < enemy.x && player.x > x) && (player.y > enemy.y && player.y < y);
                    }
                    else
                    {
                        close = (player.x < enemy.x && player.x > x) && (player.y < enemy.y && player.y > y);
                    }
                }
                k = (enemy.y - y) / (enemy.x - x);
                d = (Math.Abs(k * player.x - player.y - k * x + y)) / Math.Sqrt(k * k + 1);
                hit = d < (player.radius + radius);
            }
            else
            {
                close = (player.y > enemy.y && player.y < y) || (player.y < enemy.y && player.y > y);
                hit = Math.Abs(x - player.x) < (player.radius + radius);
            }
            
            if(hit && close)
            {
                if (player.avoid_mode == true)
                {
                    length = 0;

                    Map.make_chargePro(x, y,sword,Map.caculateBulletScore(score));
                    return false;
                }
                return true;
            }else { return false; }
        }

        public override void draw(Drawing d)
        {
            double dx, dy;
            dx = (radius) * Math.Cos(radian - Math.PI / 2)/2;
            dy = (radius) * Math.Sin(radian - Math.PI / 2) / 2;
            d.DrawLine(new Vector(enemy.x + dx, enemy.y + dy), new Vector(x + dx, y+dy), (float)radius, new Color(color,(int)(color.A*0.3)), DepthID.Player);
            dx = (radius*2/3) * Math.Cos(radian - Math.PI / 2) / 2;
            dy = (radius*2 /3) * Math.Sin(radian - Math.PI / 2) / 2;

            d.DrawLine(new Vector(enemy.x+dx, enemy.y+dy), new Vector(x+dx, y+dy), (float)radius*2/3, new Color(color, (int)(color.A * 0.6)), DepthID.Player);
            dx = (radius / 4) * Math.Cos(radian - Math.PI / 2) / 2;
            dy = (radius / 4) * Math.Sin(radian - Math.PI / 2) / 2;
            d.DrawLine(new Vector(enemy.x+dx, enemy.y+dy), new Vector(x+dx, y+dy), (float)radius/4, color, DepthID.Player);
        }
    }
}
