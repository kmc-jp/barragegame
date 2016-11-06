using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonPart
{
    class TitleSceneWithWindows:SceneWithWindows
    {
        private string titleWindowBackGroundNames = "タイトル画面NF";
        public TitleSceneWithWindows(SceneManager scene) : base(scene) {
            setup_windows();
        }
        protected void openStageSelectScene()
        {
            close();
            new StageSelectScene(scenem);
        }
        protected void openMapEditorScene()
        {
            close();
            new MapEditorScene(scenem);
        }
        protected void openMusicGallery()
        {
            new MusicGalleryScene(scenem);
        }
        override protected void setup_windows()
        {
            int nx = 0; int ny = 0;
            int dx = 40; int dy = 50;
            //windows[0] starts
            windows.Add(new Window_WithColoum(0, 0, DataBase.WindowDefaultSizeX, DataBase.WindowDefaultSizeY));
            windows[0].assignBackgroundImage(titleWindowBackGroundNames);
            windows[0].AddColoum(new Coloum(nx, ny, "version: " + DataBase.ThisSystemVersionNumber.ToString(), Command.nothing));
            nx = 5; ny += dy;
            windows[0].AddColoum(new Button(nx, ny, "open MapEditor", "", Command.openMapEditor, false));
            ny += 2 * dy; nx += dx;
            nx = 100;
            windows[0].AddColoum(new AnimationButton(nx, ny, "", DataBase.getAniD("NewGame-selected"), Command.buttonPressed1,0,0));
            ny += dy;
            windows[0].AddColoum(new AnimationButton(nx, ny, "", DataBase.getAniD("LoadGame-selected"), Command.buttonPressed2,0,0));
            ny += dy;
            windows[0].AddColoum(new AnimationButton(nx, ny, "", DataBase.getAniD("Gallery-selected"), Command.openMusicGallery, 0, 0));
            ny += 2*dy;
            windows[0].AddColoum(new AnimationButton(nx, ny, "", DataBase.getAniD("Exit-selected"), Command.exit,0,0));
            ny += dy;
            // windows[0] is finished.
        }

        override protected void switch_windowsIcommand(int i)
        {
            switch (windows[i].commandForTop)
            {
                case Command.exit:
                    close();
                    break;
                case Command.openMapEditor:
                    openMapEditorScene();
                    break;
                case Command.buttonPressed2:
                    
                    break;
                case Command.openMusicGallery:
                    openMusicGallery();
                    break;
                case Command.buttonPressed1: //1 new game
                    openStageSelectScene();
                    break;
                case Command.nothing:
                    break;
                default:
                    Console.WriteLine("window" + i + " " + windows[i].commandForTop);
                    break;
            }
        }
    }
}
