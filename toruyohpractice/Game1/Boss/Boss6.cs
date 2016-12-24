using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonPart
{
    class Boss6 : Boss
    {
        /// <summary>
        /// 頭部が回転する時、画像のどこを中心に回転するかを決める。1のときは画像の右側である。1/2の時は画像の中央縦線上となる。
        /// </summary>
        protected double head_rotatePercentX = 0.5;
        /// <summary>
        /// 頭部が回転する時、画像のどこを中心に回転するかを決める。1のときは画像の底辺である。1/2の時は画像の中央横線上となる。
        /// </summary>
        protected double head_rotatePercentY = 1;


        public Boss6(double _x, double _y, string _unitType_name) : base(_x, _y, _unitType_name)
        {
            body_max_index = -1;//funnelsNumber is not index,-1
            bodys = new Enemy[body_max_index+1];
            bodys_pos = new Vector[body_max_index + 1];
            
            maxLife = 24000;
            life = maxLife;
            setup_LoopSet(0, 0);
            for (int o = 1; o < 7; o++) // 0 - 6番目のLoopSetを作る,実際このボスは動かないのでこれで十分
            {
                setup_LoopSet(1, 1);
            }
            nowTime = -2 * 60;//
            maxPhaseIndex = 4;
            minPhaseIndex = 1;
        }

        public override void update(Player player)
        {
            base.update(player);

            #region bodys move, update
            for (int i = 0; i <= body_max_index; i++)
            {
                bodys[i].bulletsMove = bulletsMove;
                if (i <= 5)
                {
                    bodys[i].update(player);
                }
                else { bodys[i].moveToScreenPos_now(x, y); bodys[i].update(player); }
            }
            #endregion

            if (nowTime != 0)
                nowTime += nowTime < 0 ? +1 : -1;

            int n = 0; int nt = nowTime > 0 ? nowTime : -nowTime;
            string _3w1 = "b2-3wayshot-1", _2w1 = "b2-2wayshot-0.75", _1w1 = "b2-1wayshot-0.75";
            string _mis2 = "b2-x2shot-1", _blas1 = "b2-blaserDown1", _mlaP = "b2-laser-1";
            switch (motionLoopIndex)
            {
                #region phase 0
                case 0:

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
        }
        public override bool selectable()
        {
            return motionLoopIndex != 6 && base.selectable();
        }

        /// <summary>
        /// ボスのmotionLoopIndexを変更させる。またこれでphaseが決まる。
        /// </summary>
        /// <param name="p">どのフェースに行くかを決める、p=-1でrandom</param>
        public override void changePhase(int p = -1)
        {
            base.changePhase(p);
            int n = 0;
            #region phase
            #region record skillNames
            string _3w1 = "b2-3wayshot-1", _2w1 = "b2-2wayshot-0.75", _1w1 = "b2-1wayshot-0.75";
            string _mis2 = "b2-x2shot-1", _las1 = "b2-laserDown1-1";
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
                    nowTime = 3 * 60;
                    break;
                #endregion
                #region phase 1,2
                case 1:
                case 2:
                    //これはfunnelが展開のために時間を置いた
                    nowTime = -80 - 1;
                    
                    break;
                #endregion
                #region phase 3
                case 3:
                    //phase 3
                   
                    nowTime = 10 * 60;
                    break;
                #endregion
                #region phase 4
                case 4:
                    //phase 4
                    
                    break;
                #endregion
                #region phase 5
                case 5:
                    
                    nowTime = -80 - 17 * 60 - 1;
                    break;
                #endregion
                #region phase 6
                case 6:
                    
                    nowTime = -(1 + 3 + 8) * 60 - 1; //-12*60
                    break;
                #endregion
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
