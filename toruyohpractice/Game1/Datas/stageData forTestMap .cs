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
                case 5:
                    Map.create_enemy(320, 100, "testE1");
                    Map.enemys.Last().add_skill("5way*3shot");
                    break;
               /* case 60:
                    Map.create_enemy(400, 100, "testE1");
                    Map.enemys.Last().add_skill("createbullet");
                    break;
                case 4700:
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
