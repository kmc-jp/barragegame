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
        /// <summary>
        /// random時とり得るphaseの設定
        /// </summary>
        protected int maxPhaseCount;
        public Boss(double _x,double _y,string _unitType_name):base(_x,_y,_unitType_name)
        {   }

        protected virtual bool moving() { return true; }
        public override void damage(int atk)
        {
            base.damage(atk);
            Map.bossDamaged();
        }

        /// <summary>
        /// ボスのmotionLoopIndexを変更させる。またこれでphaseが決まる。
        /// </summary>
        /// <param name="p">どのフェースに行くかを決める、p=-1でrandom</param>
        public virtual void changePhase(int p=-1)
        {
            if (p < -1) { return; }
            else if (p == -1) {
                motionLoopIndex = Function.GetRandomInt(maxPhaseCount);
            }else { motionLoopIndex = p; }
            #region phase
            switch (motionLoopIndex)
            {
                case 0:
                    //phase 1


                    break;
                case 1:
                    //phase 2


                    break;
                case 2:
                    //phase 3

                    break;
                default:

                    break;
            }
            #endregion
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
        private int funnelsNumber = 6;
        private int nowTime=0;//0であれば、パターンの終了を意味する。

        public Boss2(double _x, double _y, string _unitType_name) : base(_x, _y, _unitType_name)
        {
            body_max_index = funnelsNumber+2 - 1;//funnelsNumber is not index,-1
            bodys = new Enemy[body_max_index+1];
            bodys_pos = new Vector[body_max_index + 1];
            for(int i = 0; i < funnelsNumber/2; i++)
            {
                bodys_pos[i] = new Vector(-110 - 70 * (2-i),-30 + i % 2 * 25);
                bodys_pos[i + 3] = new Vector(110 + 70 * i, -30 + i % 2 * 25);
                bodys[i] = new Enemy(x + bodys_pos[i].X, y + bodys_pos[i].Y, "funnel");// +i%2);
                bodys[i + funnelsNumber / 2] = new Enemy(x + bodys_pos[i + 3].X, y + bodys_pos[i + 3].Y, "funnel");// + i % 2);
            }
            bodys[funnelsNumber] = new Enemy(x, y, "boss2 head");
            bodys[body_max_index] = new Enemy(x , y, "boss2 body7"); // the green platform
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

            changePhase();
        }

        /// <summary>
        /// ボスのmotionLoopIndexを変更させる。またこれでphaseが決まる。
        /// </summary>
        /// <param name="p">どのフェースに行くかを決める、p=-1でrandom</param>
        public override void changePhase(int p = -1)
        {
            base.changePhase();
            int n = 0;
            #region phase
            #region record skillNames
            string _3w1 = "b2-3wayshot-1",_2w1="b2-2wayshot-0.75", _1w1 = "b2-1wayshot-0.75";
            string _mis2 = "b2-x2shot-1",_las1="b2-laserDown1-1";
            string _1wd1 = "b2-1wayDown-0.5";
            #endregion
            switch (motionLoopIndex)
            {
                case 1:
                    //phase 1
                    funnelsOut = true;
                    bodys[n].setup_extra_motion(MoveType.go_straight,PointType.pos_on_screen,
                        new Vector(100,360),120);
                    bodys[n].add_skill(_3w1);
                    n++;
                    bodys[funnelsNumber-1].setup_extra_motion(MoveType.go_straight, PointType.pos_on_screen,
                        new Vector(1100, 360), 120);
                    bodys[funnelsNumber-1].add_skill(_3w1);
                    for (int j=n ; j < funnelsNumber-1; j++)
                    {
                        bodys[n].setup_extra_motion(MoveType.go_straight, PointType.pos_on_screen,
                        new Vector(x-180*(funnelsNumber/2-n), 100), 120);
                        bodys[n].add_skill(_1w1);
                        n++;
                    }
                    for (int j = 0; j < funnelsNumber; j++)
                    {
                        bodys[j].set_skill_coolDown(_3w1,120, true);
                        bodys[j].set_skill_coolDown(_1w1, 120, true);
                    }
                    nowTime = 8 * 60+1;
                    break;
                case 2:
                    //phase 2
                    funnelsOut = true;
                    bodys[n].setup_extra_motion(MoveType.go_straight, PointType.pos_on_screen,
                        new Vector(100, 360), 120);
                    bodys[n].add_skill(_2w1);
                    n++;
                    bodys[funnelsNumber - 1].setup_extra_motion(MoveType.go_straight, PointType.pos_on_screen,
                        new Vector(1100, 360), 120);
                    bodys[funnelsNumber - 1].add_skill(_2w1);
                    for (int j = n; j < funnelsNumber - 1; j++)
                    {
                        bodys[n].setup_extra_motion(MoveType.go_straight, PointType.pos_on_screen,
                        new Vector(x - 180 * (funnelsNumber / 2 - n), 100), 120);
                        bodys[n].add_skill(_2w1);
                        n++;
                    }
                    for (int j = 0; j < funnelsNumber; j++)
                    {
                        bodys[j].set_skill_coolDown(_2w1, 120, true);
                    }
                    add_skill(_mis2);
                    nowTime = 8 * 60 + 1;
                    break;
                case 3:
                    //phase 3
                    bodys[funnelsNumber].add_skill(_las1);
                    for(int j = 0; j < funnelsNumber; j++)
                    {
                        bodys[j].add_skill(_1wd1);
                    }
                    nowTime = 10 * 60;
                    break;
                case 4:
                    //phase 4
                    bodys[funnelsNumber].add_skill(_las1);
                    for (int j = 0; j < funnelsNumber; j++)
                    {
                        bodys[j].add_skill(_2w1);
                    }
                    nowTime = 9 * 60;
                    break;
                case 5:

                default:

                    break;
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
