using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonPart
{
    abstract class Boss:Enemy {

        protected int body_max_index;
        public Enemy[] bodys;
        protected Vector[] bodys_pos;
        protected double height_percent;

        public Boss(double _x,double _y,string _unitType_name):base(_x,_y,_unitType_name)
        {   }

        protected virtual bool moving() { return true; }
        public override void damage(int atk)
        {
            base.damage(atk);
            Map.bossDamaged();
        }
    }
    class Boss1:Boss
    {
        /// <summary>
        /// 頭部が回転する時、画像のどこを中心に回転するかを決める。1のときは画像の右側である。1/2の時は画像の中央縦線上となる。
        /// </summary>
        protected double head_rotatePercentX = 0.5;
        /// <summary>
        /// 頭部が回転する時、画像のどこを中心に回転するかを決める。1のときは画像の底辺である。1/2の時は画像の中央横線上となる。
        /// </summary>
        protected double head_rotatePercentY = 1;
        public Boss1(double _x, double _y, string _unitType_name):base(_x,_y,_unitType_name)
        {
            body_max_index = 13;
            height_percent = 0.35;
            bodys = new Enemy[body_max_index+1];
            for (int i = 0; i < body_max_index; i++)
            {
                bodys[i] = new Enemy(x, y - 0 * i * height_percent, "boss1body" +  i % 3);
            }
            bodys[body_max_index] = new Enemy(x, y, "boss1tail");
            maxLife = 10000;
            life = maxLife;
        }
        public override void update(Player player)
        {
            base.update(player);

            if (moving())
            {
                for (int i = body_max_index; i > 0; i--)
                {
                    //Console.WriteLine(i+":"+Math.Sqrt(Function.distance(bodys[i - 1].x, bodys[i - 1].y, bodys[i].x, bodys[i].y)));
                    double s=Math.Sqrt(Function.distance(bodys[i - 1].x, bodys[i - 1].y, bodys[i].x, bodys[i].y)) - (bodys[i-1].animation.Y / 2 + bodys[i].animation.Y / 2)*height_percent;
                    if (s > 1) 
                    {
                        double e = Math.Sqrt(Function.distance(bodys[i].x, bodys[i].y, bodys[i-1].x, bodys[i-1].y));
                        double speed_x = (bodys[i-1].x - bodys[i].x) * s / e;
                        double speed_y = (bodys[i-1].y - bodys[i].y) * s / e;
                        bodys[i].x += speed_x;
                        bodys[i].y += speed_y;
                        bodys[i].angle = Math.Atan2( speed_y, speed_x);
                    }
                }
                double s1 = Math.Sqrt(Function.distance(x,y, bodys[0].x, bodys[0].y)) - (bodys[0].animation.Y + bodys[0].animation.Y )/4*height_percent;
                if (s1 > 1)
                {
                    double e = Math.Sqrt(Function.distance(bodys[0].x, bodys[0].y, x, y));
                    double speed_x = (x - bodys[0].x) * s1 / e;
                    double speed_y = (y - bodys[0].y) * s1 / e;
                    bodys[0].x += speed_x;
                    bodys[0].y += speed_y;
                    bodys[0].angle = Math.Atan2(speed_y, speed_x);
                }
            }
            for (int i = 0; i <= body_max_index ; i++)
            {
                bodys[i].bulletsMove = bulletsMove;
                bodys[i].update(player);
            }
        }

        public override void draw(Drawing d)
        {
            for(int i=body_max_index; i >=0; i--)
            {
                bodys[i].draw(d);
            }
            if (!texRotate)
            {
                animation.Draw(d, new Vector((x - animation.X / 2), (y - animation.Y / 2)), DepthID.Enemy);
            }
            else
            { // boss1 回転中心は特別に画像の下半分にする。
                float angle2 = (float)(angle);
                animation.Draw(d, new Vector((x + animation.Y *head_rotatePercentY * Math.Cos(angle2) + animation.X * head_rotatePercentX * Math.Sin(angle2)), (y + animation.Y * head_rotatePercentY * Math.Sin(angle2) - animation.X * head_rotatePercentX * Math.Cos(angle2))), DepthID.Enemy, 1, (float)(angle + Math.PI / 2));
            }
            for (int i = 0; i < bullets.Count; i++)
            {
                bullets[i].draw(d);
            }
        }

    }

    class Boss2 : Boss
    {
        /// <summary>
        /// 頭部が回転する時、画像のどこを中心に回転するかを決める。1のときは画像の右側である。1/2の時は画像の中央縦線上となる。
        /// </summary>
        protected double head_rotatePercentX = 0.5;
        /// <summary>
        /// 頭部が回転する時、画像のどこを中心に回転するかを決める。1のときは画像の底辺である。1/2の時は画像の中央横線上となる。
        /// </summary>
        protected double head_rotatePercentY = 1;
        private bool funnelsOut = false;

        private int nowTime=0;//0であれば、パターンの終了を意味する。

        public Boss2(double _x, double _y, string _unitType_name) : base(_x, _y, _unitType_name)
        {
            body_max_index = 7;
            bodys = new Enemy[body_max_index+1];
            bodys_pos = new Vector[body_max_index + 1];
            for(int i = 0; i < 3; i++)
            {
                bodys_pos[i] = new Vector(-110 - 35 * i,-30 + i % 2 * 25);
                bodys_pos[i + 3] = new Vector(110 + 35 * i, -30 + i % 2 * 25);
                bodys[i] = new Enemy(x +bodys_pos[i].X, y +bodys_pos[i].Y, "funnnel"+i%2);
                bodys[i+3] = new Enemy(x + bodys_pos[i+3].X, y + bodys_pos[i+3].Y, "funnnel" + i % 2);
            }
            bodys[6] = new Enemy(x, y, "boss2 head");
            bodys[7] = new Enemy(x , y, "boss2 body7");
            maxLife = 14000;
            life = maxLife;
        }

        public override void update(Player player)
        {
            base.update(player);

            #region move
            for (int i = 0; i <= body_max_index; i++)
            {
                bodys[i].bulletsMove = bulletsMove;
                if (i <= 5)
                {
                    if (!funnelsOut) {
                        bodys[i].moveToScreenPos_now(x + bodys_pos[i].X, y + bodys_pos[i].Y);
                    }
                    bodys[i].update(player);
                }else { bodys[i].moveToScreenPos_now(x,y); bodys[i].update(player); }
            }
            #endregion
            
        }

        public override void draw(Drawing d)
        {
            if (!texRotate)
            {
                animation.Draw(d, new Vector((x - animation.X / 2), (y - animation.Y / 2)), DepthID.Enemy);
            }
            for (int i = body_max_index; i >= 0; i--)
            {
                bodys[i].draw(d);
            }
            for (int i = 0; i < bullets.Count; i++)
            {
                bullets[i].draw(d);
            }
        }
    }
}
