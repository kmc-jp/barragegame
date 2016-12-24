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
        /// motionLoopIndexをphaseの代わりに使っている、大抵 0 を中継のphaseとして利用する.　そのphaseを経過したかを記録するリスト
        /// </summary>
        protected List<bool> phasesAlready;
        /// <summary>
        /// random時とり得るphaseの設定. motionLoopIndexをphaseの代わりに使っている、大抵 0 を中継のphaseとして利用する.
        /// </summary>
        protected int maxPhaseIndex,minPhaseIndex;
        protected int nowTime = 0;//0であれば、パターンの終了を意味する。
        public Boss(double _x,double _y,string _unitType_name):base(_x,_y,_unitType_name)
        {   }

        protected virtual bool moving() { return true; }
        public override void damage(int atk)
        {
            base.damage(atk);
            Map.bossDamaged();
        }
        public override void setup_LoopSet(int _loopStart, int _loopEnd, int _loopIndex = -1)
        {
            if (!motionLooped)
            {
                phasesAlready = new List<bool>();
            }
            base.setup_LoopSet(_loopStart, _loopEnd, _loopIndex);
            if (_loopIndex >= 0) // index 指定
            {
                if (phasesAlready.Count <= _loopIndex)
                {
                    for (int i = 0; i < _loopIndex + 1 - phasesAlready.Count; i++)
                        phasesAlready.Add(false);
                    Console.Write("phasesAlready: 0 added due to outOfIndex");
                }
            }else { phasesAlready.Add(false); }
        }

        public virtual int getProperMotionLoopIndex()
        {
            int p;
            bool hasleft=false;
            for(int j = minPhaseIndex; j <=maxPhaseIndex ; j++)
            {
                if(!phasesAlready[j])
                    hasleft = true;
            }
            if (!hasleft) {
                for (int j = 0; j < phasesAlready.Count; j++)
                {
                    phasesAlready[j] = false;
                }
            }
            while (true)
            {
                p = minPhaseIndex + Function.GetRandomInt(maxPhaseIndex + 1 - minPhaseIndex);
                if (p != motionLoopIndex &&( phasesAlready.Count<p || !phasesAlready[p]))
                {
                    if (phasesAlready.Count > p)
                    {
                        phasesAlready[p] = true;
                    }
                    return p;
                }
            }
        }
        /// <summary>
        /// ボスのmotionLoopIndexを変更させる。またこれでphaseが決まる。
        /// </summary>
        /// <param name="p">どのフェースに行くかを決める、p=-1でrandom</param>
        public virtual void changePhase(int p=-1)
        {
            if (p < -1) { return; }
            else if (p == -1) {
                p = getProperMotionLoopIndex();
            }
            motionLoopIndex = p;
            //Console.Write(motionLoopIndex + " index /");
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
            maxLife = 10000;
            life = maxLife;
            bodys = new Enemy[body_max_index+1];
            for (int i = 0; i < body_max_index; i++)
            {
                bodys[i] = new Enemy(x, y - 0 * i * height_percent, "boss1body" +  i % 3);
                bodys[i].maxLife = maxLife;
                bodys[i].life = maxLife;
            }
            bodys[body_max_index] = new Enemy(x, y, "boss1tail");
            bodys[body_max_index].maxLife = maxLife;
            bodys[body_max_index].life = maxLife;

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
                bodys[i].life = life;
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
        private bool funnelsOut = true;
        private int funnelsNumber = 6;

        private bool phaseSixUsed=false;
        public Boss2(double _x, double _y, string _unitType_name) : base(_x, _y, _unitType_name)
        {
            body_max_index = funnelsNumber+2 - 1;//funnelsNumber is not index,-1
            bodys = new Enemy[body_max_index+1];
            bodys_pos = new Vector[body_max_index + 1];
            for(int i = 0; i < funnelsNumber/2; i++)
            {
                bodys_pos[i] = new Vector(-110 - 70 * (2-i),-30 + i % 2 * 25);
                bodys_pos[i + funnelsNumber/2] = new Vector(110 + 70 * i, -30 + i % 2 * 25);
                bodys[i] = new Enemy(x + bodys_pos[i].X, y + bodys_pos[i].Y, "funnel");// +i%2);
                bodys[i + funnelsNumber / 2] = new Enemy(x + bodys_pos[i + 3].X, y - 100, "funnel");// + i % 2);
                bodys[i].playAnimation(DataBase.aniNameAddOn_alterOff);
                bodys[i+funnelsNumber/2].playAnimation(DataBase.aniNameAddOn_alterOff);
                
            }
            bodys[funnelsNumber] = new Enemy(x, y, "boss2 head");
            bodys[body_max_index] = new Enemy(x , y, "boss2 body7"); // the green platform
            maxLife = 14000;
            life = maxLife;
            setup_LoopSet(0, 0);
            for(int o = 1; o < 7; o++) // 0 - 6番目のLoopSetを作る,実際このボスは動かないのでこれで十分
            {
                setup_LoopSet(1, 1);
            }
            nowTime = -2*60;//これは下のupdateを見たら、funnelがボスに向かう
            maxPhaseIndex = 4;
            minPhaseIndex = 1;
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
                        //bodys[i].moveToScreenPos_now(x + bodys_pos[i].X, y + bodys_pos[i].Y);
                    }
                    bodys[i].update(player);
                }else { bodys[i].moveToScreenPos_now(x,y); bodys[i].update(player); }
            }
            #endregion

            if(nowTime!=0 )
                nowTime+= nowTime<0 ? +1: -1;

            int n = 0; int nt = nowTime > 0 ? nowTime : -nowTime;
            string _3w1 = "b2-3wayshot-1", _2w1 = "b2-2wayshot-0.75", _1w1 = "b2-1wayshot-0.75";
            string _mis2 = "b2-x2shot-1", _blas1="b2-blaserDown1",_mlaP= "b2-laser-1";
            switch (motionLoopIndex)
            {
                #region phase 0
                case 0:
                    if (nowTime == 80)
                    {
                        for (int j = 0; j < funnelsNumber; j++)
                        {
                            bodys[j].playAnimation(DataBase.aniNameAddOn_alterOff);
                        }
                    }
                    else if (nowTime >= 40 || nowTime<0)
                    {
                        for (int j = 0; j < funnelsNumber; j++)
                        {
                            bodys[j].setup_extra_motion(MoveType.go_straight, PointType.pos_on_screen, 
                                new Vector(x + bodys_pos[j].X, y + bodys_pos[j].Y), nt- 40);
                        }
                    }
                    else if (nowTime == 0)
                    {
                        changePhase();
                    }
                    break;
                #endregion
                #region phase 1
                case 1:
                    #region nonwTime==-1
                    if (nowTime == -1)
                    {
                        funnelsOut = true;
                        bodys[0].setup_extra_motion(MoveType.go_straight, PointType.pos_on_screen,
                            new Vector(100, 360), 120);
                        bodys[0].add_skill(_3w1);
                        bodys[funnelsNumber - 1].setup_extra_motion(MoveType.go_straight, PointType.pos_on_screen,
                            new Vector(1100, 360), 120);
                        bodys[funnelsNumber - 1].add_skill(_3w1);
                        for (int j = 1; j < funnelsNumber - 1; j++)
                        {
                            bodys[j].setup_extra_motion(MoveType.go_straight, PointType.pos_on_screen,
                            new Vector(x - 180 * ((funnelsNumber-1) / 2.0 - j), 100), 120);
                            bodys[j].add_skill(_1w1);
                            n++;
                        }
                        for (int j = 0; j < funnelsNumber; j++)
                        {
                            bodys[j].set_skill_coolDown(_3w1, 120, true);
                            bodys[j].set_skill_coolDown(_1w1, 120, true);
                        }
                        nowTime = 8 * 60;
                    }
                    #endregion
                    else if (nowTime == 0) { changePhase(0); }
                    break;
                #endregion
                #region phase 2
                case 2:
                    #region nowTime<0
                    if (nowTime < 0)
                    {
                        if (nowTime == -1)
                        {
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
                                new Vector(x - 180 * ((funnelsNumber-1) / 2.0 - n), 100), 120);
                                bodys[n].add_skill(_2w1);
                                n++;
                            }
                            for (int j = 0; j < funnelsNumber; j++)
                            {
                                bodys[j].set_skill_coolDown(_2w1, 120, true);
                            }
                            add_skill(_mis2);
                            nowTime = 11 * 60;
                        }
                    }
                    #endregion
                    else if (nowTime == 0) { changePhase(0); }
                    break;
                #endregion
                #region phase 3,4
                case 3:
                case 4:
                    if (nowTime == 0) { changePhase(0); }
                    break;
                #endregion
                #region phase 5
                case 5:
                    if (nowTime == -17 * 60-1)
                    {
                        bodys[0].setup_extra_motion(MoveType.go_straight, PointType.pos_on_screen, new Vector(100, 600), 120);
                        bodys[1].setup_extra_motion(MoveType.go_straight, PointType.pos_on_screen, new Vector(100, 100), 120);
                        bodys[4].setup_extra_motion(MoveType.go_straight, PointType.pos_on_screen, new Vector(1080, 600), 120);
                        bodys[5].setup_extra_motion(MoveType.go_straight, PointType.pos_on_screen, new Vector(1080, 100), 120);
                    }
                    else if (nowTime == -15 * 60 - 1)
                    {
                        for (int j = 0; j < funnelsNumber; j++)
                        {
                            if (j != 2 && j != 3)
                                bodys[j].add_skill(_1w1);
                        }
                        bodys[0].setup_extra_motion(MoveType.go_straight, PointType.pos_on_screen, new Vector(100, 100), 5*60);
                        bodys[5].setup_extra_motion(MoveType.go_straight, PointType.pos_on_screen, new Vector(1080, 600), 5 * 60);

                    }
                    else if (nowTime == -13*60-1)
                    { 
                        bodys[1].setup_extra_motion(MoveType.go_straight, PointType.pos_on_screen, new Vector(1080, 100), 5 * 60);
                        bodys[4].setup_extra_motion(MoveType.go_straight, PointType.pos_on_screen, new Vector(100, 600), 5 * 60);
                        
                    }
                    else if (nowTime == -8*60- 1)
                    {
                        bodys[1].setup_extra_motion(MoveType.go_straight, PointType.pos_on_screen, new Vector(1080, 600), 5 * 60);
                        bodys[4].setup_extra_motion(MoveType.go_straight, PointType.pos_on_screen, new Vector(100, 100), 5 * 60);
                    }
                    else if (nowTime == -6 * 60 - 1)
                    {
                        bodys[0].setup_extra_motion(MoveType.go_straight, PointType.pos_on_screen, new Vector(1080, 100), 5 * 60);
                        bodys[5].setup_extra_motion(MoveType.go_straight, PointType.pos_on_screen, new Vector(100, 600), 5 * 60);
                    }
                    else if (nowTime == 0) { changePhase(0); }
                    break;
                #endregion
                #region phase 6
                case 6:
                    if (nowTime == -9 * 60 - 1)
                    {
                        add_skill(_blas1);
                        for (int j = 0; j < funnelsNumber; j++)
                        {
                            bodys[j].add_skill(_mlaP);
                            bodys[j].add_skill(_2w1);
                        }
                    }
                    else if (nowTime == - 1)
                    {
                        for (int j = 0; j < funnelsNumber; j++)
                        {
                            bodys[j].delete_all_skills();
                        }
                        nowTime = 60;
                        Map.CutInTexture(DataBase.Boss2phase6Texture, 0, 0,
                        0, -DataBase.getTex(DataBase.Boss2phase6Texture).Height, 80, 
                        DataBase.getTex(DataBase.Boss2phase6Texture).Height / 60);

                    }
                    else if (nowTime == 0) { changePhase(0); }
                    break;
                #endregion
                default:
                    break;
            }
        }

        public override void damage(int atk)
        {
            base.damage(atk);
            if (life <= maxLife / 2) { maxPhaseIndex = 5; }
            if (life <= maxLife * 1 / 3 && !phaseSixUsed) { phaseSixUsed = true; changePhase(6); }
        }
        public override bool selectable()
        {
            return motionLoopIndex!=6 &&base.selectable();
        }

        /// <summary>
        /// ボスのmotionLoopIndexを変更させる。またこれでphaseが決まる。
        /// </summary>
        /// <param name="p">どのフェースに行くかを決める、p=-1でrandom</param>
        public override void changePhase(int p = -1)
        {
            base.changePhase(p);
            int n = 0;
            funnelsOut = false;
            #region phase
            #region record skillNames
            string _3w1 = "b2-3wayshot-1",_2w1="b2-2wayshot-0.75", _1w1 = "b2-1wayshot-0.75";
            string _mis2 = "b2-x2shot-1",_las1="b2-laserDown1-1";
            string _1wd1 = "b2-1wayDown-0.5";
            #endregion
            switch (motionLoopIndex)
            {
                #region phase 0
                case 0:
                    for (int j = 0; j <= body_max_index; j++)
                    {
                        bodys[j].delete_all_skills();
                    }
                    delete_all_skills();
                    nowTime = 3*60;
                    break;
                #endregion
                #region phase 1,2
                case 1:
                case 2:
                    //これはfunnelが展開のために時間を置いた
                    nowTime = -80 - 1;
                    for(int j = 0; j < funnelsNumber; j++)
                    {
                        bodys[j].playAnimation(DataBase.aniNameAddOn_alterOn);
                    } 
                    break;
                #endregion
                #region phase 3
                case 3:
                    //phase 3
                    bodys[funnelsNumber].add_skill(_las1);
                    for(int j = 0; j < funnelsNumber; j++)
                    {
                        bodys[j].add_skill(_1wd1);
                    }
                    nowTime = 10 * 60;
                    break;
                #endregion
                #region phase 4
                case 4:
                    //phase 4
                    bodys[funnelsNumber].add_skill(_las1);
                    for (int j = 0; j < funnelsNumber; j++)
                    {
                        bodys[j].add_skill(_2w1);
                    }
                    nowTime = 9 * 60;
                    break;
                #endregion
                #region phase 5
                case 5:
                    for(int j = 0; j < funnelsNumber; j++)
                    {
                        bodys[j].playAnimation(DataBase.aniNameAddOn_alterOn);
                    }
                    bodys[2].add_skill(_2w1);
                    bodys[3].add_skill(_2w1);
                    bodys[funnelsNumber].add_skill(_1w1);
                    nowTime = -80-17*60-1;
                    break;
                #endregion
                #region phase 6
                case 6:
                    for(int j = 0; j < funnelsNumber; j++)
                    {
                        bodys[j].setup_extra_motion(MoveType.go_straight, PointType.displacement, new Vector(0, 60), 1 * 60);
                    }
                    Map.CutInTexture(DataBase.Boss2phase6Texture, 0, -DataBase.getTex(DataBase.Boss2phase6Texture).Height,
                        0, 0, 9999, DataBase.getTex(DataBase.Boss2phase6Texture).Height / 60);

                    nowTime = -(1 + 3 + 8) * 60 - 1; //-12*60
                    break;
                #endregion
                default:

                    break;
            }
            #endregion
        }
        
    }

    class Boss3:Boss
    {
        /// <summary>
        /// 頭部が回転する時、画像のどこを中心に回転するかを決める。1のときは画像の右側である。1/2の時は画像の中央縦線上となる。
        /// </summary>
        protected double head_rotatePercentX = 0.5;
        /// <summary>
        /// 頭部が回転する時、画像のどこを中心に回転するかを決める。1のときは画像の底辺である。1/2の時は画像の中央横線上となる。
        /// </summary>
        protected double head_rotatePercentY = 1;

        public Boss3(double _x, double _y, string _unitType_name) : base(_x, _y, _unitType_name)
        {
            maxLife = 14000;
            life = maxLife;
            setup_LoopSet(0, 0);
            for (int o = 1; o < 7; o++) // 0 - 6番目のLoopSetを作る,実際このボスは動かないのでこれで十分
            {
                setup_LoopSet(1, 1);
            }
            nowTime = -2 * 60;//これは下のupdateを見たら、funnelがボスに向かう
            maxPhaseIndex = 4;
            minPhaseIndex = 1;
        }

        
        
    }
}
