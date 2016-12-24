using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonPart
{
    class TitleSceneWithWindows:SceneWithWindows
    {
        private string titleWindowBackGroundNames = "タイトル画面C91";
        string _difficulty, _play_mode;
        public TitleSceneWithWindows(SceneManager scene) : base(scene) {
            setup_windows();
            SoundManager.Music.PlayBGM(BGMID.title,true);
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
            if (Game1.difficulty == -1) { _difficulty = "NORMAL"; }else { _difficulty = "HARD"; }
            if (Game1.play_mode == -1) { _play_mode = "ARCADE"; } else { _play_mode = "PRACTICE"; }
            //windows[0] starts
            windows.Add(new Window_WithColoum(0, 0, DataBase.WindowDefaultSizeX, DataBase.WindowDefaultSizeY));
            windows[0].assignBackgroundImage(titleWindowBackGroundNames);
            //windows[0].AddColoum(new Coloum(nx, ny, "version: " + DataBase.ThisSystemVersionNumber.ToString(), Command.nothing));
            nx = 5; ny += dy;
            //windows[0].AddColoum(new Button(nx, ny, "open MapEditor", "", Command.openMapEditor, false));
            ny += 2 * dy; nx += dx;
            nx = 100;
            ny += 0 * dy;
	        //windows[0].AddColoum(new AnimationButton(nx, ny, "TestMap", DataBase.getAniD("NewGame-selected"), Command.buttonPressed3, 80, 0));
            ny += 1 * dy;
            windows[0].AddColoum(new AnimationButton(nx, ny, /*"ＨＡＲＤ"*/"", DataBase.getAniD("NewGame-selected"), Command.buttonPressed1,0,0));
            ny += 2*dy;
            windows[0].AddColoum(new Button(nx, ny, "難易度変更", _difficulty, Command.buttonPressed2, false,80));
            ny += dy;
            windows[0].AddColoum(new Button(nx, ny, "プレイモード変更", _play_mode, Command.buttonPressed4, false,100));
            /* ny += 2*dy;
             windows[0].AddColoum(new AnimationButton(nx, ny, "ＮＯＲＭＡＬ", DataBase.getAniD("NewGame-selected"), Command.buttonPressed2, 80, 0));
             */
            //ny += dy;
            //windows[0].AddColoum(new AnimationButton(nx, ny, "", DataBase.getAniD("LoadGame-selected"), Command.buttonPressed2,0,0));
            ny += 3*dy/2;
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
                    scenem.Exit();
                    break;
                case Command.openMapEditor:
                    openMapEditorScene();
                    break;
                case Command.openMusicGallery:
                    openMusicGallery();
                    break;
                case Command.buttonPressed1: //1 new game
                    #region　難易度変更
                    if (Game1.difficulty == 1)
                    {
                        Game1.enemyBullets_update_fps = 60;
                        Game1.enemySkills_update_fps = 60;
                        Game1.playerLife = 4 * Map.lifesPerPiece;
                    }
                    else if (Game1.difficulty == -1)
                    {
                        Game1.enemyBullets_update_fps = 45;
                        Game1.enemySkills_update_fps = 40;
                        Game1.playerLife = 5 * Map.lifesPerPiece;
                    }

                    #endregion
                    #region プレイモード変更
                    if (Game1.play_mode == 1)
                    {
                        openStageSelectScene();

                    }
                    else if (Game1.play_mode == -1)
                    {
                        new MapScene(scenem, 1);
                    }
                    #endregion
                    break;
                case Command.buttonPressed2:
                    Game1.difficulty *= -1;
                    if (Game1.difficulty == -1) { _difficulty = "NORMAL"; } else { _difficulty = "HARD"; }
                    ((Window_WithColoum)windows[i]).coloums[3].content = _difficulty;
                    break;
                case Command.buttonPressed4:
                    Game1.play_mode *= -1;
                    if (Game1.play_mode == -1) { _play_mode = "ARCADE"; } else { _play_mode = "PRACTICE"; }
                    ((Window_WithColoum)windows[i]).coloums[4].content = _play_mode;
                    break;
                case Command.buttonPressed3:
                    new MapScene(scenem, -1);
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
