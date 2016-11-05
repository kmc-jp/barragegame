using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CommonPart {
    /// <summary>
    /// マップ作成のシーンのクラス
    /// </summary>
    class AniDEditorScene : BasicEditorScene {
        const int aniIndex= 2;
        const int aniDListIndex = 1;
        static public Window_Animation window_Ani { get { if (windows.Count < 2) return null; else return ((Window_Animation)windows[aniIndex]); } }
        static public Window_AniDList window_AniDList { get { if (windows.Count < 1) return null; else return ((Window_AniDList)windows[aniDListIndex]); } }
        public AniDEditorScene(SceneManager s) : base(s)
        {
            setup_windows();
        }
        /// <summary>
        /// once changed this, make sure of data -those saved in file and used to edit
        /// </summary>
        protected override void setup_windows() {
            int nx = 0;int ny = 0;
            int dx = 0;int dy = 30; 
            //windows[0] starts
            windows.Add(new Window_WithColoum(5, 5, 200, 160));
            windows[0].AddColoum(new Coloum(nx, ny, "version: "+DataBase.ThisSystemVersionNumber.ToString(), Command.nothing));
            nx = 5; ny += dy;dx = 110;
            windows[0].AddColoum(new Button(nx, ny, "", "close AniD", Command.closeThis, false));
            nx += dx;
            windows[0].AddColoum(new Button(nx, ny, "", "add Texture", Command.addTex, false));
            nx = 5; ny += dy; 
            windows[0].AddColoum(new Button(nx, ny, "", "new AniD", Command.newAniD, false));
            ny += dy;
            windows[0].AddColoum(new Button(nx, ny, "", "reload Scroll", Command.reloadScroll, false));
            ny += dy;
            // windows[0] is finished.

            // windows[1] starts
            nx = 115;
            windows.Add(new Window_AniDList(nx, ny, 150, 320));
            

            //-------------
            // window_AniD 
            nx =300; ny =5;
            windows.Add(new Window_Animation(DataBase.defaultBlankAnimationData, nx, ny, 350, 440));

        }

        public override void SceneDraw(Drawing d) {
            base.SceneDraw(d);
            //window_AniD.draw(d);
        }//SceneDraw
        protected void openUTD()
        {
            new UTDEditorScene(scenem);
            close();
        }
        protected override void switch_windowsIcommand(int i)
        {
            //Console.WriteLine(((Window_AniDList)windows[1]).getAniScrollContent_str());
            //Console.WriteLine(window_AniDList.commandForTop);
            switch (windows[i].commandForTop)
            {
                case Command.openUTD:
                    openUTD();
                    break;
                case Command.closeThis:
                    close();
                    break;
                case Command.addTex:
                    addTex();
                    break;
                case Command.selectInScroll:
                    window_Ani.set_animation_name(window_AniDList.getAniScrollContent_str() );
                    break;
                case Command.reloadScroll:
                    window_AniDList.reloadAniDscroll();
                    break;
                case Command.newAniD:
                    createNewAniD();
                    window_AniDList.reloadAniDscroll();
                    break;
                case Command.nothing:
                    break;
                default:
                    Console.WriteLine("window" + i + " " + windows[i].commandForTop);
                    break;
            }
        }// method end

        protected void createNewAniD()
        {
            Console.WriteLine("Please Type In datas:");
            #region Read AnimationDataName Or Make A Copy
            string _aniName;string _texName;
            Console.Write("Animation Data name: ");
            while (true)
            {
                _aniName = Console.ReadLine();
                if (!DataBase.existsAniD(_aniName, null)) { break; }
                else { Console.Write(" is already Exists In Dictionary. Make A Copy? yes/no ");
                    string _copy=null;
                    while (true)
                    {
                        _copy = Console.ReadLine();
                        if (_copy=="yes") {
                            window_Ani.set_animation(AnimationDataAdvanced.getAcopyFromDataBaseByName(_aniName));
                            return;
                        }else if (_copy == "no") { _copy = null; break; }
                    }// while asking copy or not
                    if (_copy != null) { break; }
                } 
            }
            #endregion
            #region Read TextureName
            Console.Write("Texture name: ");
            while (true)
            {
                _texName = Console.ReadLine();
                if (DataBase.TexturesDataDictionary.ContainsKey(_texName)) { break; }
                else { Console.Write(" is not Found In Textures' Dictionary."); }
            }
            #endregion
            #region Read min,max Index
            int _min_index, _max_index;
            Console.Write("min Index: ");
            while ( !(int.TryParse(Console.ReadLine(), out _min_index)) )
            { }
            Console.Write("max Index: ");
            while (!(int.TryParse(Console.ReadLine(), out _max_index)) && _max_index>=_min_index)
            { }
            #endregion
            #region Read Frames
            int[] _frames = new int[_max_index - _min_index+1];
            for(int i = 0; i <= _max_index - _min_index; i++)
            {
                Console.Write("frame "+i +" : ");
                while (!(int.TryParse(Console.ReadLine(), out _frames[i])) )
                { }
            }
            #endregion
            string _repeat_str;
            Console.Write("repeat? -> true / false:");
            while (true)
            {
                _repeat_str = Console.ReadLine();
                if (_repeat_str == "true") {
                    window_Ani.set_animation(new AnimationDataAdvanced(_aniName, _frames, _min_index, _texName,true) );
                    break;
                }
                else if(_repeat_str=="false"){
                    window_Ani.set_animation(new AnimationDataAdvanced(_aniName, _frames, _min_index, _texName,false) );
                    break;
                }
                else { Console.WriteLine("Use Only true or false"); }
            }
        }

    }//class AniDEditorScene end


}//namespace CommonPart End
