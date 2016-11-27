using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CommonPart {
    /// <summary>
    /// マップ作成のシーンのクラス
    /// </summary>
    class UTDEditorScene : BasicEditorScene {
        static public Window_UnitType window_ut;

        public UTDEditorScene(SceneManager s) : base(s)
        {
            setup_windows();
        }
        /// <summary>
        /// once changed this, make sure of data -those saved in file and used to edit
        /// </summary>
        protected override void setup_windows() {
            int nx = 0;int ny = 0;
            int dx = 0; int dy = 30;
            //windows[0] starts
            windows.Add(new Window_WithColoum(20, 20, 110, 130));
            ((Window_WithColoum) windows[0] ).AddColoum(new Coloum(nx, ny, "version: "+DataBase.ThisSystemVersionNumber.ToString(), Command.nothing));
            nx = 5; ny += 15;dx = 30; 
            windows[0].AddColoum(new Button(nx, ny, "", "open MapEdi", Command.closeThis, false));
            ny += dy;
            windows[0].AddColoum(new Button(nx, ny, "", "add Texture", Command.addTex, false));
            // windows[0] is finished.

            // windows[1] starts
            windows.Add(new Window_utsList(20, ny +dy, 150, 150));

            // windows[2] starts
            window_ut= new Window_UnitType(DataBase.getUnitType(null),60, ny + 20, 150, 150);
        }

        public override void SceneDraw(Drawing d) {
            base.SceneDraw(d);
        }//SceneDraw
        protected void openAniD()
        {
            new AniDEditorScene(scenem);
            close();
        }
        protected void openMapEditorScene()
        {
            close();
            new MapEditorScene(scenem);
        }
        protected override void switch_windowsIcommand(int i)
        {
            switch (windows[i].commandForTop)
            {
                case Command.openUTD:
                    openAniD();
                    break;
                case Command.closeThis:
                    openMapEditorScene();
                    break;
                case Command.addTex:
                    addTex();
                    break;
                case Command.UTDutButtonPressed:
                    window_ut.setup_unitType_window(DataBase.getUnitType(windows[1].getNowColoumContent_string())  );
                    break;
                case Command.nothing:
                    break;
                default:
                    Console.WriteLine("window" + i + " " + windows[i].commandForTop);
                    break;
            }
        }// method End

    }//class UTDEditorScene end


}//namespace CommonPart End
