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
                case 5:
                    Map.create_enemy(320, 0, "testE1");
                    Map.enemys.Last().add_skill("createbullet2way-2");
                    //Map.enemys.Last().add_skill("boss6-createzyuzi");

                    break;
                //case 60:
                    //Map.create_enemy(400, 50, "testE1");
                    //Map.enemys.Last().add_skill("1wayshot-2");
                    //Map.enemys.Last().add_skill("createzyuzi-0");
                    //break;
                    
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
