using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonPart
{
    /// <summary>
    /// AnimationAdvancedを自分の中に描画できるwindow.
    /// </summary>
    class Window_Animation : Window_WithColoum
    {
        AnimationDataAdvanced ad;
        private int old_animeFramesLength;
        /// <summary>
        /// アニメーションデータの名前から作れたAnimatioAdvanced,を含むcoloumsのwindow
        /// </summary>
        /// <param name="ad_name">アニメーションデータのフルネーム</param>
        /// <param name="_x">このwindowの画面上のx</param>
        /// <param name="_y">windowの画面上のy</param>
        /// <param name="_w">windowの幅,アニメーションに応じて自動調整しません</param>
        /// <param name="_h">windowの高さ</param>
        public Window_Animation(string ad_name,int _x, int _y, int _w, int _h) : base(_x, _y, _w, _h)
        {
            ad = DataBase.getAniD(ad_name);
            setup_window();
        }
        public Window_Animation(AnimationDataAdvanced aDad, int _x, int _y, int _w, int _h) : base(_x, _y, _w, _h)
        {
            ad = aDad;
            setup_window();
        }

        /// <summary>
        /// change Window_Animation's ad to a specific one and re setup_window(); 
        /// </summary>
        /// <param name="_aDAd"></param>
        public void set_animation(AnimationDataAdvanced _aDAd) { ad = _aDAd; setup_window(); }
        
        /// <summary>
        /// change Window_Animation's ad using ad_name, re setup_window();
        /// </summary>
        /// <param name="_ani_name"></param>
        public void set_animation_name(string _ani_name) {
            if (DataBase.existsAniD(_ani_name, null)) { ad = DataBase.getAniD(_ani_name); setup_window(); }
            else { Console.WriteLine("Window_Animation: "+_ani_name + " does not exist in Dictionary."); }
        }
        
        protected void apply_animation()
        {
            if(DataBase.existsAniD(ad.animationDataName,null))  DataBase.RemoveAniD(ad.animationDataName, null);
            int n = 1;
            bool repeat = getColoumiContent_bool(n);
            n++;
            int min_index = getColoumiContent_int(n);
            n++;
            int max_index = getColoumiContent_int(n);
            n++;
            int framesLength = getColoumiContent_int(n);
            int[] frames = new int[framesLength];
            n++;
            if (old_animeFramesLength > framesLength)
            {
                for (int i = 0; i < old_animeFramesLength; i++)
                {
                    if (i < framesLength)
                    {
                        frames[i] = getColoumiContent_int(n);
                    }
                    n++;
                }
            }else{
                for (int i = 0; i < framesLength; i++)
                {
                    if (i < old_animeFramesLength)
                    {
                        frames[i] = getColoumiContent_int(n);
                        n++;
                    }
                    else
                    {
                        frames[i] = frames[old_animeFramesLength - 1];
                    }
                }
            }
            string ani_name = getColoumiContent_string(n);
            n++;
            string texture_name = getColoumiContent_string(n);
            n++;
            string pre_ani_name = getColoumiContent_string(n);
            n++;
            string next_ani_name = getColoumiContent_string(n);

            DataBase.addAniD(new AnimationDataAdvanced(ani_name, frames,
                min_index, texture_name, repeat));
            DataBase.getAniD(ani_name).assignAnimationName(pre_ani_name, false);
            DataBase.getAniD(ani_name).assignAnimationName(next_ani_name, true);
            ad = DataBase.getAniD(ani_name);
            setup_window();
        }

        /// <summary>
        /// このclassのcommand処理はこのwindow内部の処理を行い、後はSceneに任せたいためbase.を呼ぶ
        /// </summary>
        /// <param name="c"></param>
        protected override void deal_with_command(Command c)
        {
            switch (c)
            {
                case Command.specialIntChange1:// min\max_index changed
                    //coloums.Insert(new Blank(nx, ny, i.ToString() + ":", ad.frames[i].ToString(), Command.apply_int)););
                    break;
                case Command.playAnimation:
                    ((AnimationColoum)coloums[0]).play();
                    break;
                case Command.applyAniD:
                    apply_animation();
                    break;
                case Command.button_on:
                    
                default:
                    break;
            }
            base.deal_with_command(c);
        }
        public void setup_window()
        {
            coloums.Clear();
            //repeat,min,max,length,frames,name,texname,pre,next
            int nx = 100, ny = 200; int dy = 30;
            AddColoum(new AnimationColoum(nx, ny, ad.animationDataName, ad, Command.playAnimation));
            nx = 10; ny = 10;
            AddColoum(new Button(nx, ny, "repeat:", ad.repeat.ToString(), Command.tru_fals, false));
            ny += dy;
            AddColoum(new Blank(nx, ny, "min_tex:", ad.min_texture_index.ToString(), Command.specialIntChange1));
            ny += dy;
            AddColoum(new Blank(nx, ny, "max_tex:", ad.max_texture_index.ToString(), Command.specialIntChange1));
            ny += dy;
            AddColoum(new Blank(nx, ny, "frames:",ad.frames.Length.ToString(),  Command.specialIntChange2));
            old_animeFramesLength = ad.frames.Length;
            //Console.WriteLine(old_animeFramesLength);
            ny += dy;
            for(int i = 0; i < old_animeFramesLength;i++) {
                AddColoum(new Blank(nx, ny, i.ToString()+":", ad.frames[i].ToString(), Command.apply_int));
                ny += dy;
            }
            nx = 160; ny = 10;
            AddColoum(new Blank(nx, ny, "name:",ad.animationDataName, Command.apply_string));
            nx = 120; ny += dy;
            AddColoum(new Blank(nx, ny, "texname:", ad.texture_name, Command.apply_string));
            ny += dy;
            if (ad.pre_animation_name != null)
            {
                AddColoum(new Blank(nx, ny, "pre_ani_name:", ad.pre_animation_name, Command.apply_string));
            }
            ny += dy;
            if (ad.next_animation_name != null)
            {
                AddColoum(new Blank(nx, ny, "next_ani_name:", ad.next_animation_name, Command.apply_string));
            }
            ny += dy;
            AddColoum(new Button(nx, ny, "apply Animation:","", Command.applyAniD,false));
        }

    }
}
