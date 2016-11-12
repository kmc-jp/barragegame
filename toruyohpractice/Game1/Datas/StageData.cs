using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace CommonPart
{
    abstract class StageData
    {
        protected BGMID[] bgmIDs;
        public string[] background_names;
        public string stageName;
        public StageData(string _stageName)
        {
            stageName = _stageName;
        }

        abstract public void update();

        protected virtual void playBGM(BGMID _id)
        {
            Map.PlayBGM(_id);
        }

        protected virtual void setupAllbackgroundWithNames()
        {
            Map.setup_backgroundNamesFromNames(background_names);
        }

    }

    #region Stage1
    class Stage1Data :StageData
    {
        public Stage1Data(string _stageName) :base(_stageName)
        {
            bgmIDs = new BGMID[] { BGMID.Stage1onWay,BGMID.Stage1Boss}; //一応こうした、いつでも{}の中身を変更できる。
                                                                  //ただし、MusicPlayer2.cs 30行から登録済でないと流れません。
            background_names = new string[] { "background1"};

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
                case 240:
                    Map.create_enemy(180, 0, "E1a-0");
                    Map.create_enemy(540, 0, "E1a-0");
                    break;
                case 840:
                    Map.create_enemy(0, 0, "E2a-0");
                    Map.create_enemy(720, 0, "E2a-1");
                    break;
                case 1260:
                    Map.create_enemy(360, 0, "E3-0");
                    break;
                case 1620:
                    Map.create_enemy(0, 100,"E1c-0a");
                    Map.create_enemy(720, 100, "E1c-0b");
                    break;
                case 1980:
                    Map.create_enemy(200, 0, "E3-1");
                    Map.create_enemy(520, 0, "E3-1");
                    break;
                case 2480:
                    Map.create_enemy(720,120,"E1b-1");
                    break;
                case 2540:
                    Map.create_enemy(720, 120, "E1b-1");
                    break;
                case 2600:
                    Map.create_enemy(720, 120, "E1b-1");
                    break;
                case 2660:
                    Map.create_enemy(720, 120, "E1b-1");
                    break;
                case 2720:
                    Map.create_enemy(720, 120, "E1b-1");
                    break;
                case 3260:
                    Map.create_enemy(360, 0, "E1c-1");
                    break;
                case 3580:
                    Map.create_enemy(180, 0, "E1c-2");
                    Map.create_enemy(540, 0, "E1c-2");
                    break;
                case 3700:
                    Map.create_enemy(360, 0, "E1a-1");
                    break;
                case 4140:
                    Map.create_enemy(300, 0, "E1a-4");
                    Map.create_enemy(420, 0, "E1a-5");
                    break;
                case 4380:
                    Map.boss_mode = true;
                    break;
                case 4400:
                    playBGM(bgmIDs[1]);//BGMを流す。
                    Map.create_boss1(360, 10, "boss1");
                    break;
                    #endregion
            }
        }
    }
    #endregion
    #region Stage2
    class Stage2Data : StageData
    {
        public Stage2Data(string _stageName)
            : base(_stageName)
        {
            bgmIDs = new BGMID[] { BGMID.Stage2onWay, BGMID.Stage2Boss }; //一応こうした、いつでも{}の中身を変更できる。
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
                case 180:
                    Map.create_enemy(540, 0, "E4-1a");
                    Map.create_enemy(180, 0, "E4-1a");
                    break;
                case 480:
                    Map.create_enemy(360, 0, "E4-2a");
                    break;
                case 660:
                    Map.create_enemy(200, 0, "E4-1c");
                    Map.create_enemy(640, 0, "E4-1c");
                    break;
                case 840:
                    Map.create_enemy(360, 0, "E4-2b");
                    break;
                case 960:
                    Map.create_enemy(630, 0, "E5-a");
                    break;
                case 1200:
                    Map.create_enemy(30, 0, "E5-a");
                    break;
                case 1440:
                    Map.create_enemy(180, 0, "E2-2a");
                    Map.create_enemy(540, 0, "E2-2b");
                    break;
                case 1620:
                    Map.create_enemy(360, 720, "E2-f1");
                    break;
                case 1740:
                    Map.create_enemy(600, 0, "E4-2c");
                    break;
                case 1920:
                    Map.create_enemy(0, 320, "E4-1e");
                    Map.create_enemy(720, 320, "E4-1f");
                    break;
                case 2100:
                    Map.create_enemy(120, 0, "E5-c");
                    break;
                case 2160:
                    Map.create_enemy(600, 720, "E5-d");
                    break;
                case 2340:
                    Map.create_enemy(360, 0, "E4-2d");
                    break;
                case 2580:
                    Map.create_enemy(360, 0, "E5-e");
                    Map.create_enemy(600, 0, "E4-1g");
                    Map.create_enemy(120, 0, "E4-2e");
                    break;
                case 2820:
                    Map.create_enemy(700, 0, "E4-2f");
                case 3000:
                    Map.create_enemy(20, 0, "E4-2g");
                case 3180:
                    Map.create_enemy(360, 0, "E2-3");
                case 3360:
                    Map.create_enemy(600, 0, "E6-a");
                    Map.create_enemy(360, 0, "E6-b");
                case 3660:
                    Map.create_enemy(180, 0, "E6-c");
                case 3720:
                    Map.create_enemy(540, 0, "E6-d");
                case 3780:
                    Map.create_enemy(0, 320, "E5-f");
                case 4020:
                    Map.create_enemy(720, 270, "E5-g");
                case 4200:
                    Map.create_enemy(360, 0, "E4-h");
                case 4860:
                    Map.boss_mode = true;
                    break;
                case 4920:
                    playBGM(bgmIDs[1]);//BGMを流す。
                    Map.create_boss2(360, 10, "boss2");
                    break;
            #endregion
            }
        }
    }
    #endregion
}// namespace CommonPart end

