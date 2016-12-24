﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonPart
{
    class stageData_forTestMap : StageData
    {
        public stageData_forTestMap(string _stageName) : base(_stageName)
        {
            bgmIDs = new BGMID[] { BGMID.Stage1onWay, BGMID.Stage1Boss }; //一応こうした、いつでも{}の中身を変更できる。
                                                                          //ただし、MusicPlayer2.cs 30行から登録済でないと流れません。
            background_names = new string[] { "background1" };

            setupAllbackgroundWithNames();//背景を用意する。
        }

        public override void update()
        {
            #region 敵配置
            switch (Map.step[0])
            {
                case 0:
                    playBGM(bgmIDs[0]);//最初のBGMを流す。
                    Map.boss_mode = false;
                    break;
                //case 10:
                    //Map.create_enemy(360, 0, "E3-1");
                    //Map.enemys.Last().add_skill("ransya-3");
                    //Map.enemys.Last().add_skill("ransya-3^-1");
                    Map.enemys.Last().set_skill_coolDown(1, 45);
                    break;
                    
                case 50:
                    Map.EngagingTrueBoss();
                    break;
                case 125:
                    playBGM(bgmIDs[0]);//最初のBGMを流す。
                    Map.create_boss3(640, 150, "boss3");
                    break;
                    /*case 80:
                        Map.boss_mode = true;
                        Map.EngagingTrueBoss();
                        break;
                    case 90:
                        playBGM(bgmIDs[1]);//BGMを流す。
                        Map.create_boss2(360, 640, "boss2");
                        break;*/
            }
            #endregion
        }
    }
}
