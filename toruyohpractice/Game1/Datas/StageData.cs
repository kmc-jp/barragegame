﻿using Microsoft.Xna.Framework;
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
        public StageData(BGMID id ,string backgroundname)
        {

        }

        abstract public void update();

        protected virtual void setBGM(BGMID _id)
        {
            Map.set_BGM(_id);
        }

        protected virtual void setupAllbackgroundWithNames()
        {
            foreach(string na in background_names)
            {
                Map.set_backgroundName(na);
            }
        }

    }

    #region Stage1
    class Stage1Data :StageData
    {
        public Stage1Data(BGMID id, string backgroundname) :base(id,backgroundname)
        {
            stageName = "stage1";
        }

        public override void update()
        {
            #region 敵配置
            switch (Map.step[0])
            {
                case 0:
                    Map.boss_mode = false;
                    break;
                case 240:
                    Console.WriteLine("a");
                    Map.create_enemy(180, 0, "E1a-0");
                    Map.create_enemy(180, 0, "E1a-0");
                    break;
                case 480:
                    Map.create_enemy(360, 0, "E1a-1");
                    Map.create_enemy(0, 0, "E2a-0");
                    Map.create_enemy(720, 0, "E2a-1");
                    break;
                case 720:
                    Map.create_enemy(100, 0, "E1b-0");
                    Map.create_enemy(620, 0, "E1b-0");
                    break;
                case 1200:
                    Map.create_enemy(360, 0, "E3-0");
                    break;
                case 1380:
                    Map.create_enemy(0, 100, "E1c-0");
                    Map.create_enemy(720, 100, "E1c-0");
                    break;
                case 1680:
                    Map.create_enemy(280, 0, "E1a-2");
                    Map.create_enemy(360, 0, "E1a-2");
                    Map.create_enemy(420, 0, "E1a-2");
                    break;
                case 1920:
                    Map.create_enemy(200, 0, "E3-1");
                    Map.create_enemy(520, 0, "E3-1");
                    break;
                case 2160:
                    Map.create_enemy(360, 0, "E1a-3");
                    break;
                case 2460:
                    Map.create_enemy(720,120,"E1b-1");
                    break;
                case 2520:
                    Map.create_enemy(720, 120, "E1b-1");
                    break;
                case 2580:
                    Map.create_enemy(720, 120, "E1b-1");
                    break;
                case 2640:
                    Map.create_enemy(720, 120, "E1b-1");
                    break;
                case 2700:
                    Map.create_enemy(720, 120, "E1b-1");
                    break;
                case 3180:
                    Map.create_enemy(360, 0, "E1c-1");
                    break;
                case 3400:
                    Map.create_enemy(180, 0, "E1c-2");
                    Map.create_enemy(540, 0, "E1c-2");
                    break;
                case 3960:
                    Map.create_enemy(300, 0, "E1a-4");
                    Map.create_enemy(420, 0, "E1a-5");
                    break;
                case 4620:
                    Map.boss_mode = true;
                    break;
                case 4680:
                    Map.create_boss1(360, 10, "boss1");
                    break;
                    #endregion
            }
        }
    }
    #endregion
}// namespace CommonPart end
