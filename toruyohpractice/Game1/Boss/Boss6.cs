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
            body_max_index = 1;//funnelsNumber is not index,-1
            bodys = new Enemy[body_max_index+1];
            bodys_pos = new Vector[body_max_index + 1];
            maxLife = 24000;
            life = maxLife;
            setup_LoopSet(0, 0);
            bodys_pos[0] = new Vector(-90, -92);
            bodys_pos[1] = new Vector(50, 28);
            bodys[0] = new Enemy(x + bodys_pos[0].X, y + bodys_pos[0].Y, "boss6 up ball");
            bodys[1] = new Enemy(x + bodys_pos[1].X, y + bodys_pos[1].Y, "boss6 down ball");
            for (int j = 0; j <= body_max_index; j++)
            {
                bodys[j].maxLife = maxLife;
                bodys[j].life = life;
            }
            for (int o = 1; o < 6; o++) // 0 - 5番目のLoopSetを作る
            {
                setup_LoopSet(0, 0);
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
                bodys[i].skillsUpdate = skillsUpdate;
                bodys[i].update(player);
                
                bodys[i].moveToScreenPos_now(x+bodys_pos[i].X, y+bodys_pos[i].Y); 
            }
            #endregion

            if (nowTime != 0)
                nowTime += nowTime < 0 ? +1 : -1;

            int n = 0; int nt = nowTime > 0 ? nowTime : -nowTime;
            
            switch (motionLoopIndex)
            {
                #region phase 0
                case 0:
                    if (nowTime == 0)
                    {
                        for(int j = 0; j < 2; j++)
                        {
                            for(int k=0;k< bodys[j].bullets.Count; k++)
                            {
                                bodys[j].bullets[k].dead();
                            }
                        }
                        changePhase();
                    }
                    break;
                #endregion
                default:
                    if(nowTime==0)
                        changePhase(0);
                    break;
            }
        }

        public override void damage(int atk)
        {
            base.damage(atk);
            for(int j = 0; j <= body_max_index; j++)
            {
                bodys[j].life = life;
            }
            if (life <= maxLife / 2) { maxPhaseIndex = 5; }
            if (life <= maxLife / 4) { maxPhaseIndex = 4; }
        }

        /// <summary>
        /// ボスのmotionLoopIndexを変更させる。またこれでphaseが決まる。
        /// </summary>
        /// <param name="p">どのフェースに行くかを決める、p=-1でrandom</param>
        public override void changePhase(int p = -1)
        {
            base.changePhase(p);
            int n = 0;
            #region switch phase
            switch (motionLoopIndex)
            {
                #region phase 0
                case 0:
                    nowTime = 2 * 60;
                    break;
                #endregion

                default:
                    nowTime = 10*60;
                    break;
            }
            #endregion
            for(int j = 0; j < 2; j++)
            {
                bodys[j].motionLoopIndex = motionLoopIndex;
            }
        }
        
    }
}
