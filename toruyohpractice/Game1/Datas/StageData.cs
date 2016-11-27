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
                    Map.create_enemy(40, 100,"E1c-0a");
                    Map.create_enemy(680, 100, "E1c-0b");
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
                case 4700:
                    Map.boss_mode = true;
                    Map.EngagingTrueBoss();
                    break;
                case 4720:
                    playBGM(bgmIDs[1]);//BGMを流す。
                    Map.create_boss1(360, 10, "boss1");
                    break;
            }
            #endregion
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
            background_names = new string[] { "background2" };
            setupAllbackgroundWithNames();
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
                    break;
                case 3000:
                    Map.create_enemy(20, 0, "E4-2g");
                    break;
                case 3180:
                    Map.create_enemy(360, 0, "E2-3");
                    break;
                case 3360:
                    Map.create_enemy(600, 0, "E6-a");
                    Map.create_enemy(360, 0, "E6-b");
                    break;
                case 3660:
                    Map.create_enemy(180, 0, "E6-c");
                    break;
                case 3720:
                    Map.create_enemy(540, 0, "E6-d");
                    break;
                case 3780:
                    Map.create_enemy(0, 320, "E5-f");
                    break;
                case 4020:
                    Map.create_enemy(720, 270, "E5-g");
                    break;
                case 4200:
                    Map.create_enemy(360, 0, "E4-1h");
                    break;
                case 4860:
                    Map.EngagingTrueBoss();
                    break;
                case 4920:
                    playBGM(bgmIDs[1]);//BGMを流す。
                    Map.create_boss2(720, 10, "boss2");
                    break;
            }
            #endregion
        }
    }
    #endregion
    #region Stage4
    class Stage4Data : StageData{

        public Stage4Data(string _stageName) : base(_stageName)
        {
            bgmIDs = new BGMID[] { BGMID.Stage4onWay, BGMID.Stage4Boss }; //一応こうした、いつでも{}の中身を変更できる。
                                                                          //ただし、MusicPlayer2.cs 30行から登録済でないと流れません。
            background_names = new string[] { "background4" };

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
                    Map.create_enemy(360, 0, "4E2-f2a"); //便宜上、UnitType名は[4][敵画像名][順番(a-)]とした
                    break;
                case 660:
                    Map.create_enemy(150, 0, "4E2-f2b"); //一つ減らした
                    break;
                case 900:
                    Map.create_enemy(0, 240, "4E4-2a");
                    break;
                case 1020:
                    Map.create_enemy(720, 240, "4E4-1a");
                    break;
                case 1140:
                    Map.create_enemy(0, 240, "4E4-2a"); //1a,2aはそれぞれ共通の画像と動きに変更
                    break;
                case 1260:
                    Map.create_enemy(720, 240, "4E4-1a");
                    break;
                case 1620:
                    Map.create_enemy(320, 0, "4E1a");
                    Map.create_enemy(380, 0, "4E1b");
                    break;
                case 2040:
                    Map.create_enemy(70, 720, "4E2-2a");
                    Map.create_enemy(170, 720, "4E2-2a");
                    Map.create_enemy(550, 720, "4E2-2a");
                    Map.create_enemy(650, 720, "4E2-2a");
                    break;
                case 2460:
                    Map.create_enemy(0, 200, "4E5a");
                    Map.create_enemy(720, 200, "4E5b");
                    break;
                case 2640:
                    Map.create_enemy(360, 720, "4E3-2a");
                    break;
                case 2800:
                    Map.create_enemy(360, 0, "4E2-f1a");
                    break;
                case 3180:
                    Map.create_enemy(60, 0, "4E2-1a");
                    Map.create_enemy(120, 0, "4E2-1a");
                    Map.create_enemy(180, 0, "4E2-1a");
                    Map.create_enemy(240, 0, "4E2-1a");
                    Map.create_enemy(300, 0, "4E2-1a");
                    Map.create_enemy(420, 0, "4E2-1a");
                    Map.create_enemy(480, 0, "4E2-1a");
                    Map.create_enemy(540, 0, "4E2-1a");
                    Map.create_enemy(600, 0, "4E2-1a");
                    Map.create_enemy(660, 0, "4E2-1a");
                    break;
                case 4020:
                    Map.create_enemy(100, 720, "4E2-f2d");
                    Map.create_enemy(200, 720, "4E2-f2d");
                    Map.create_enemy(520, 720, "4E2-f2d");
                    Map.create_enemy(620, 720, "4E2-f2d"); //たぶんこの弾幕きつすぎるから間引きか全削除
                    break;
                case 4640:
                    Map.create_enemy(360, 0, "4E4-2c");
                    break;
                case 4700:
                    Map.create_enemy(360, 0, "4E4-2c");
                    break;
                case 4760:
                    Map.create_enemy(360, 0, "4E4-2c");
                    break;
                case 5040:
                    Map.create_enemy(0, 200, "4E5c");
                    Map.create_enemy(720, 200, "4E5d");
                    break;
                case 5520:
                    Map.create_enemy(100, 0, "4E1c");
                    Map.create_enemy(620, 0, "4E1c");
                    break;
                case 5640:
                    Map.create_enemy(230, 0, "4E1c");
                    Map.create_enemy(490, 0, "4E1c");
                    break;
                case 5780:
                    Map.create_enemy(360, 0, "4E1c");
                    break;
                case 6300:
                    Map.create_enemy(360, 0, "4E4-1b");
                    Map.create_enemy(360, 0, "4E4-1c");
                    break;
                case 6900:
                    Map.boss_mode = true;
                    break;
                case 6920:
                    playBGM(bgmIDs[1]);//BGMを流す。
                    Map.create_boss1(360, 10, "boss4"); 
                    break;
            }
            #endregion
        }
    }
    #endregion
}// namespace CommonPart end

