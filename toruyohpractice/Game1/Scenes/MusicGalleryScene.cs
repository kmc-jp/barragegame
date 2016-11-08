using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CommonPart {
    /// <summary>
    /// マップ作成のシーンのクラス
    /// </summary>
    class MusicGalleryScene : SceneWithWindows {
        public MusicGalleryScene(SceneManager s) : base(s)
        {
            setup_windows();
        }
        /// <summary>
        /// once changed this, make sure of data -those saved in file and used to edit
        /// </summary>
        protected override void setup_windows() {
            int nx = 0;int ny = 0;
            int dx = 40;int dy = 30; 
            //windows[0] starts
            windows.Add(new Window_WithColoum(0, 0, DataBase.WindowDefaultSizeX, DataBase.WindowDefaultSizeY));
            windows[0].AddColoum(new Coloum(nx, ny, "version: "+DataBase.ThisSystemVersionNumber.ToString(), Command.nothing));
            nx = 5; ny += dy;
            windows[0].AddColoum(new Button(nx, ny, "close MusicGallery","", Command.closeThis, false));
            ny += 2*dy; nx += dx;
            windows[0].AddRichText("BGMの名前をクリックすると再生されます。ループする曲はループし続けます", new Vector(nx, ny));
            //windows[0].AddColoum(new Coloum(nx, ny, "BGMの名前をクリックすると再生されます。\nループする曲はループし続けます", Command.nothing));
            ny += 2*dy;
            windows[0].AddColoum(new Button(nx, ny, "stop", "", Command.buttonPressed1, false));
            ny += dy;
            int i = 0;
            foreach (BGMdata bd in SoundManager.Music.bgmDatas)
            {
                windows[0].AddColoum(new Button(nx, ny, i.ToString(), bd.BGMname, Command.buttonPressed2, false));
                i++;
                ny += dy;
            }
            // windows[0] is finished.

        }

        protected void stopBGM()
        {
            SoundManager.Music.StopBGM();
        }
        protected override void switch_windowsIcommand(int i)
        {
            switch (windows[i].commandForTop)
            {
                case Command.closeThis:
                    close();
                    break;
                case Command.buttonPressed2:
                    //Console.WriteLine(SoundManager.Music.bgmDatas[windows[i].getNowColoumStr_int()].BGMname);
                    SoundManager.Music.PlayBGM(SoundManager.Music.bgmDatas[windows[i].getNowColoumStr_int()].bgmId, true);
                    break;
                case Command.buttonPressed1:
                    stopBGM();
                    break;
                case Command.nothing:
                    break;
                default:
                    Console.WriteLine("window" + i + " " + windows[i].commandForTop);
                    break;
            }
        }// method end

      
    }//class MusicGalleryScene end


}//namespace CommonPart End
