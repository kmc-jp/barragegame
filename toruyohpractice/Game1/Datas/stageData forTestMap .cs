using System;
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
            background_names = new string[] { "background3" };

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
                /*case 5:
                    Map.EngagingTrueBoss();
                    Map.create_boss1(320, 0, "boss1");
                    break;
                    /*
                case 60:
                    Map.create_boss2(DataBase.WindowDefaultSizeX / 2, 0, "boss2");*/                    
                case 5:
                    Map.create_enemy(320, 0, "testE1");
                    Map.enemys.Last().add_skill("boss3onfire-longbless");
                    //Map.enemys.Last().add_skill("1wayshot-2");
                    Map.enemys.
                        Last().set_skill_coolDown(0, 60);
                    //Map.enemys.Last().add_skill("ransya-3^-1");

                    /*Map.enemys.Last().add_skill("createbullet2way-2");
                    //Map.enemys.Last().add_skill("boss6-createzyuzi");
                    break;
                    */
                /*case 45:
                    Map.create_enemy(320, 0, "testE2");
                    Map.enemys.Last().add_skill("ransya-3^-1");
                    //Map.enemys.Last().add_skill("createzyuzi-0");
                    break*/
                    
                /*case 4700:
                    Map.boss_mode = true;
                    Map.EngagingTrueBoss();
                    break;
                case 4720:
                    playBGM(bgmIDs[1]);//BGMを流す。
                    Map.create_boss1(360, 10, "boss1");
                    break;*/
            }
            #endregion
        }
    }
}
