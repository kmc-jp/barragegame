using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CommonPart
{
    abstract class StageData
    {
        protected BGMID[] bgmIDs;
        public string[] background_names;
        /// <summary>
        /// 現在0がleftside、1がrightside
        /// </summary>
        public List<string> texture_names = new List<string>();
        public string stageName;
        public StageData(string _stageName)
        {
            stageName = _stageName;
        }

        protected virtual void commonSetup()
        {
            Map.setup_textureNames(texture_names);//サイドの画像とか他の画像を用意する
            setupAllbackgroundWithNames();//背景を用意する。
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
            texture_names.Add("leftside1");
            texture_names.Add("rightside1");

            commonSetup();
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
                    Map.enemys.Last().set_skill_coolDown(0, 120,true);
                    Map.create_enemy(540, 0, "E1a-0");
                    Map.enemys.Last().set_skill_coolDown(0, 120,true);
                    break;
                case 840:
                    Map.create_enemy(0, 0, "E2a-0");
                    Map.create_enemy(720, 0, "E2a-1");
                    break;
                case 1260:
                    Map.create_enemy(360, 0, "E3-0");
                    Map.enemys.Last().set_skill_coolDown(0, 200,true);
                    break;
                case 1620:
                    Map.create_enemy(40, 100,"E1c-0a");
                    Map.create_enemy(680, 100, "E1c-0b");
                    break;
                case 1980:
                    Map.create_enemy(200, 0, "E3-1");
                    Map.enemys.Last().set_skill_coolDown(0, 60, true);
                    Map.create_enemy(520, 0, "E3-1");
                    Map.enemys.Last().set_skill_coolDown(0, 60, true);
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
                    Map.enemys.Last().set_skill_coolDown(0, 60, true);
                    Map.create_enemy(540, 0, "E1c-2");
                    Map.enemys.Last().set_skill_coolDown(0, 60, true);
                    break;
                case 3700:
                    Map.create_enemy(360, 0, "E1a-1");
                    Map.enemys.Last().set_skill_coolDown(0, 120, true);
                    break;
                case 4140:
                    Map.create_enemy(300, 0, "E1a-4");
                    Map.enemys.Last().set_skill_coolDown(0, 60, true);
                    Map.create_enemy(420, 0, "E1a-5");
                    Map.enemys.Last().set_skill_coolDown(0, 60, true);
                    break;
                case 5000:
                    Map.boss_mode = true;
                    Map.EngagingTrueBoss();
                    break;
                case 5020:
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
            texture_names.Add("leftside2");
            texture_names.Add("rightside2");

            commonSetup();
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
                    Map.enemys.Last().set_skill_coolDown(0, 180, true);
                    Map.create_enemy(180, 0, "E4-1a");
                    Map.enemys.Last().set_skill_coolDown(0, 180, true);
                    break;
                case 660:
                    Map.create_enemy(200, 0, "E4-1c");
                    Map.enemys.Last().set_skill_coolDown(0, 120, true);
                    Map.create_enemy(640, 0, "E4-1c");
                    Map.enemys.Last().set_skill_coolDown(0, 120, true);
                    break;
                case 840:
                    Map.create_enemy(360, 0, "E4-2b");
                    Map.enemys.Last().set_skill_coolDown(0, 60, true);
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
                case 2340:
                    Map.create_enemy(360, 0, "E4-2d");
                    Map.enemys.Last().set_skill_coolDown(0, 60, true);
                    break;
                case 2580:
                    Map.create_enemy(600, 0, "E4-1g");
                    Map.enemys.Last().set_skill_coolDown(0, 60, true);
                    Map.create_enemy(120, 0, "E4-2e");
                    Map.enemys.Last().set_skill_coolDown(0, 60, true);
                    break;
                case 3000:
                    Map.create_enemy(20, 0, "E4-2g");
                    break;
                case 3180:
                    Map.create_enemy(360, 0, "E2-3");
                    Map.enemys.Last().set_skill_coolDown(0, 60, true);
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
                case 4020:
                    Map.create_enemy(720, 270, "E5-g");
                    break;
                case 4200:
                    Map.create_enemy(360, 0, "E4-1h");
                    Map.enemys.Last().set_skill_coolDown(0, 120, true);
                    break;
                case 5260:
                    Map.EngagingTrueBoss();
                    break;
                case 5320:
                    playBGM(bgmIDs[1]);//BGMを流す。
                    Map.create_boss2(720, 0, "boss2");
                    break;
            }
            #endregion
        }
    }
    #endregion
    #region Stage3
    class Stage3Data : StageData
    {
        public Stage3Data(string _stageName) : base(_stageName)
        {
            bgmIDs = new BGMID[] { BGMID.Stage3onWay, BGMID.Stage3Boss }; //一応こうした、いつでも{}の中身を変更できる。
                                                                          //ただし、MusicPlayer2.cs 30行から登録済でないと流れません。
            background_names = new string[] { "background3" };
            texture_names.Add("leftside3");
            texture_names.Add("rightside3");

            commonSetup();
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
                    Map.create_enemy(100, 720, "3E7a");
                    Map.enemys.Last().add_skill("1shotfrom2point");
                    Map.create_enemy(620, 720, "3E7b");
                    Map.enemys.Last().add_skill("1shotfrom2point");
                    break;
                case 300:
                    Map.create_enemy(100, 720, "3E7a");
                    Map.enemys.Last().add_skill("1shotfrom2point");
                    Map.enemys.Last().sync_skill_coolDown(0, 60, 60);
                    Map.create_enemy(620, 720, "3E7b");
                    Map.enemys.Last().add_skill("1shotfrom2point");
                    Map.enemys.Last().sync_skill_coolDown(0, 60, 60);
                    break;
                case 360:
                    Map.create_enemy(100, 720, "3E7a");
                    Map.enemys.Last().add_skill("1shotfrom2point");
                    Map.enemys.Last().sync_skill_coolDown(0, 60, 120);
                    Map.create_enemy(620, 720, "3E7b");
                    Map.enemys.Last().add_skill("1shotfrom2point");
                    Map.enemys.Last().sync_skill_coolDown(0, 60, 120);
                    break;
                case 780:
                    Map.create_enemy(190, 0, "3E1a");
                    Map.enemys.Last().add_skill("yanagi-0");
                    break;
                case 840:
                    Map.create_enemy(530, 0, "3E1a");
                    Map.enemys.Last().add_skill("yanagi-0");
                    break;
                case 1500:
                    Map.create_enemy(80, 0, "3E8a");
                    Map.enemys.Last().add_skill("laser-down-1");
                    Map.create_enemy(180, 0, "3E8a");
                    Map.enemys.Last().add_skill("laser-down-1");
                    Map.create_enemy(280, 0, "3E8a");
                    Map.enemys.Last().add_skill("laser-down-1");
                    Map.create_enemy(360, 0, "3E8a");
                    Map.enemys.Last().add_skill("laser-down-1");
                    Map.create_enemy(440, 0, "3E8a");
                    Map.enemys.Last().add_skill("laser-down-1");
                    Map.create_enemy(540, 0, "3E8a");
                    Map.enemys.Last().add_skill("laser-down-1");
                    Map.create_enemy(640, 0, "3E8a");
                    Map.enemys.Last().add_skill("laser-down-1");
                    break;
                case 1920:
                    Map.create_enemy(360, 0, "3E2-f2a");
                    Map.enemys.Last().add_skill("ransya-3");
                    break;
                case 2620:
                    Map.create_enemy(0, 620, "3E7c");
                    Map.enemys.Last().add_skill("1wayshot-1");
                    break;
                case 2650:
                    Map.create_enemy(0, 620, "3E7c");
                    Map.enemys.Last().add_skill("1wayshot-1");
                    Map.enemys.Last().sync_skill_coolDown(0, 30, 30);
                    break;
                case 2680:
                    Map.create_enemy(0, 620, "3E7c");
                    Map.enemys.Last().add_skill("1wayshot-1");
                    Map.enemys.Last().sync_skill_coolDown(0, 30, 60);
                    break;
                case 2710:
                    Map.create_enemy(0, 620, "3E7c");
                    Map.enemys.Last().add_skill("1wayshot-1");
                    Map.enemys.Last().sync_skill_coolDown(0, 30, 90);
                    break;
                case 2740:
                    Map.create_enemy(0, 620, "3E7c");
                    Map.enemys.Last().add_skill("1wayshot-1");
                    Map.enemys.Last().sync_skill_coolDown(0, 30, 120);
                    break;
                case 2770:
                    Map.create_enemy(0, 620, "3E7c");
                    Map.enemys.Last().add_skill("1wayshot-1");
                    Map.enemys.Last().sync_skill_coolDown(0, 30, 150);
                    break;
                case 3000:
                    Map.create_enemy(400, 0, "3E3-1a");
                    Map.enemys.Last().add_skill("createbullet3way");
                    break;
                case 3600:
                    Map.create_enemy(100, 720, "3E8b");
                    Map.enemys.Last().add_skill("5way*5shot");
                    Map.create_enemy(620, 720, "3E8b");
                    Map.enemys.Last().add_skill("5way*5shot");
                    break;
                case 4080:
                    Map.create_enemy(100, 0, "3E4-2a");
                    Map.enemys.Last().add_skill("1downshotfrom2point");
                    Map.create_enemy(300, 0, "3E4-1a");
                    Map.enemys.Last().add_skill("1downshotfrom2point");
                    Map.create_enemy(420, 0, "3E4-1a");
                    Map.enemys.Last().add_skill("1downshotfrom2point");
                    Map.create_enemy(620, 0, "3E4-2a");
                    Map.enemys.Last().add_skill("1downshotfrom2point");
                    break;
                case 4320:
                    Map.create_enemy(0, 50, "3E1d");
                    break;
                case 4360:
                    Map.create_enemy(720, 50, "3E7d");
                    break;
                case 4400:
                    Map.create_enemy(0, 50, "3E1d");
                    break;
                case 4440:
                    Map.create_enemy(720, 50, "3E7d");
                    break;
                case 4480:
                    Map.create_enemy(0, 50, "3E1d");
                    break;
                case 4520:
                    Map.create_enemy(720, 50, "3E7d");
                    break;
                case 4560:
                    Map.create_enemy(0, 50, "3E1d");
                    break;
                case 5000:
                    Map.create_enemy(605, 0, "3E8c");
                    Map.enemys.Last().add_skill("ransya-4");
                    Map.create_enemy(115, 0, "3E8c");
                    Map.enemys.Last().add_skill("ransya-4");
                    break;
                case 5480:
                    Map.create_enemy(180, 0, "3E5a");
                    Map.enemys.Last().add_skill("laserfrom2point");
                    Map.create_enemy(360, 0, "3E5b");
                    Map.enemys.Last().add_skill("laerfrom2point");
                    Map.create_enemy(540, 0, "3E5a");
                    Map.enemys.Last().add_skill("laserfrom2point");
                    break;
                case 6200:
                    Map.create_enemy(360, 0, "3E7e");
                    Map.enemys.Last().add_skill("ransya-3");
                    Map.enemys.Last().add_skill("ransya-3^-1");
                    Map.enemys.Last().set_skill_coolDown(1,45);

                    break;
                case 7400:
                    Map.boss_mode = true;
                    Map.EngagingTrueBoss();
                    break;
                case 7420:
                    playBGM(bgmIDs[1]);//BGMを流す。
                    Map.create_boss3(360, 10, "boss3");
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
            texture_names.Add("leftside4");
            texture_names.Add("rightside4");

            commonSetup();
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
    #region Stage6
    class Stage6Data : StageData
    {
        public Stage6Data(string _stageName) : base(_stageName)
        {
            bgmIDs = new BGMID[] { BGMID.Stage6boss }; //一応こうした、いつでも{}の中身を変更できる。
                                                                          //ただし、MusicPlayer2.cs 30行から登録済でないと流れません。
            background_names = new string[] { "background6" };

            commonSetup();
            Map.scroll_speed = new Vector(0, 0);
        }

        public override void update()
        {
            #region 敵配置
            switch (Map.step[0])
            {
                case 0:
                    Map.boss_mode = false;
                    break;
                case 50:
                    Map.EngagingTrueBoss();
                    break;
                case 125:
                    playBGM(bgmIDs[0]);//最初のBGMを流す。
                    Map.create_boss6(640, 220, "boss6");
                    break;
            }
            #endregion
        }
    }
    #endregion
}// namespace CommonPart end

