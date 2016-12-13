using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace CommonPart
{
    class StageSelectScene : Scene
    {
        /// <summary>
        /// indexではない
        /// </summary>
        int stage_select = 1;
        bool[] stageAvailable;
        Vector player_pos = new Vector();
        const string playerIconName = "130 149-player";
        int pw=DataBase.getTexD(playerIconName).w_single;
        int ph = DataBase.getTexD(playerIconName).h_single;
        Vector[] stagesPos = new Vector[] {
            new Vector(450, 450), new Vector(100, 200),
            new Vector(400, 80),new Vector(750, 150),
            new Vector(1080, 25),
        };
        string stageButtonAniDName="stageSelectButton";
        AnimationAdvanced[] animations; 

        public StageSelectScene(SceneManager scenem) : base(scenem) {
            stageAvailable = new bool[stagesPos.Length];
            SoundManager.Music.PlayBGM(BGMID.map, true);
            animations = new AnimationAdvanced[stagesPos.Length];
            for(int i = 0; i < stagesPos.Length; i++)
            {
                stageAvailable[i] = false;
            }
            stageAvailable[0] = true; stageAvailable[1] = true;
            for(int j = 0; j < stagesPos.Length; j++)
            {
                if (stageAvailable[j] == false)
                {
                    animations[j] = new AnimationAdvanced(DataBase.getAniD(stageButtonAniDName, DataBase.defaultAnimationNameAddOn));
                }
                else
                {
                    if (stage_select == j+1)
                    {
                        animations[j] = new AnimationAdvanced(DataBase.getAniD(stageButtonAniDName, DataBase.aniNameAddOn_spell));
                    }
                    else
                    {
                        animations[j] = new AnimationAdvanced(DataBase.getAniD(stageButtonAniDName, DataBase.aniNameAddOn_spellOff));
                    }
                }
            }
        }

        public override void SceneUpdate()
        {
            bool changed=false;
            int add=0;
            if (Input.GetKeyPressed(KeyID.Up) == true || Input.GetKeyPressed(KeyID.Right) == true)
            {
                animations[stage_select - 1] = null;
                if (!stageAvailable[stage_select - 1])
                {
                    animations[stage_select - 1] = new AnimationAdvanced(DataBase.getAniD(stageButtonAniDName,
                        DataBase.defaultAnimationNameAddOn));
                }else {
                    animations[stage_select - 1] = new AnimationAdvanced(DataBase.getAniD(stageButtonAniDName,
                    DataBase.aniNameAddOn_spellOff));
                }
                changed = true;
                add = 1;
            }
            if (Input.GetKeyPressed(KeyID.Down) == true || Input.GetKeyPressed(KeyID.Left) == true)
            {
                animations[stage_select - 1] = null;
                if (!stageAvailable[stage_select - 1])
                {
                    animations[stage_select - 1] = new AnimationAdvanced(DataBase.getAniD(stageButtonAniDName,
                        DataBase.defaultAnimationNameAddOn));
                }
                else
                {
                    animations[stage_select - 1] = new AnimationAdvanced(DataBase.getAniD(stageButtonAniDName,
                    DataBase.aniNameAddOn_spellOff));
                }
                changed = true;
                add = -1;
            }
            int number = 0;
            while (add!=0 && number <stagesPos.Length)
            {
                number++;
                stage_select += add;
                if (stage_select > stagesPos.Length) { stage_select = 1; }
                if (stage_select < 1) { stage_select = stagesPos.Length; }
                if (stage_select <= stagesPos.Length && stageAvailable[stage_select - 1])
                {
                    break;
                }
            }
            if (changed) {
                animations[stage_select - 1] = null;
                if (!stageAvailable[stage_select - 1])
                {
                    animations[stage_select - 1] = new AnimationAdvanced(DataBase.getAniD(stageButtonAniDName,
                        DataBase.defaultAnimationNameAddOn));
                }
                else
                {
                    animations[stage_select - 1] = new AnimationAdvanced(DataBase.getAniD(stageButtonAniDName,
                    DataBase.aniNameAddOn_spell));
                }
            }
            if (Input.IsKeyDownOnce(KeyID.Select) == true)
            {
                new MapScene(scenem,stage_select);
            }

            if (Input.IsKeyDownOnce(KeyID.Escape) == true || Input.IsKeyDownOnce(KeyID.Cancel))
            {
                Delete=true;
                new TitleSceneWithWindows(scenem);
            }
            player_pos.X = stagesPos[stage_select-1].X-pw/2;
            player_pos.Y = stagesPos[stage_select - 1].Y-ph/2;
            for(int j = 0; j < stagesPos.Length; j++)
            {
                animations[j].Update();
            }
        }

        public override void SceneDraw(Drawing d)
        {
            d.Draw(new Vector(0, 0), DataBase.getTex("stageselect"), DepthID.BackGroundWall);
            for(int i = 0; i < stagesPos.Length-1; i++)
            {
                d.DrawLine(stagesPos[i], stagesPos[i + 1], 2, Color.Wheat, DepthID.Status);
            }
            for(int j=0;j<stagesPos.Length;j++){
                animations[j].Draw(d, new Vector(stagesPos[j].X-animations[j].X/2,stagesPos[j].Y-animations[j].Y/2), DepthID.Status);
            }
            d.Draw(player_pos, DataBase.getTex(playerIconName), DepthID.Status);
        }
    }
}
