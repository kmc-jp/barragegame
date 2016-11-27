using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace CommonPart
{
    /// <summary>
    /// Coloumのx,y座標は絶対であるが、drawに訂正x,yを渡せる
    /// </summary>
    class Coloum
    {
        #region Variables
        ///<summary>
        ///absolute Position (may be in the window/game screen).
        ///</summary>
        public Vector pos;
        protected PoorString pstr=null;
        /// <summary>
        /// Coloum' text explanation
        /// </summary>
        public string str
        {
            get { if (pstr == null) return null; else return pstr.old_str; }
            set { if (pstr != null) pstr = new PoorString(value, pstr); else pstr = new PoorString(value,maximumOfCharsEachLine,false); }
        }
        /// <summary>
        /// Coloum' text explanation that draws!
        /// </summary>
        public string nstr
        {
            get { if (pstr == null) return null; else return pstr.str; }
        }
        /// <summary>
        /// a int enum "Command" command returned if the coloum gots "apply" 
        /// </summary>
        public Command reply;
        /// <summary>
        /// it is selected in a window or not.
        /// </summary>
        public bool selected;
        /// <summary>
        /// default distance that between str and content. is used in Blank
        /// </summary>
        protected const int default_distance = 10;
        /// <summary>
        /// Coloum size
        /// </summary>
        public int w, h;
        protected int dx = 0, dy = 0;
        /// <summary>
        /// Coloumでは無意味。書き換えの内容に当たる。
        /// </summary>
        public string content =null;
        protected const int maximumOfCharsEachLine = 18;
        protected const FontID default_fontId = FontID.Medium;
        #endregion
        /// <summary>
        /// Textureを使うButton用の簡易コンストラクタ、w hの設定がない
        /// </summary>
        /// <param name="_x"></param>
        /// <param name="_y"></param>
        /// <param name="_str"></param>
        /// <param name="_reply"></param>
        protected Coloum(int _x, int _y, string _str, Command _reply)
        {
            pos = new Vector(_x, _y);
            str = _str;
            reply = _reply;
        }

        public Coloum(int _x, int _y, string _str, Command _reply, int _w = -1, int _h = -1,string _content=""):this(_x,_y,_str,_reply)
        {
            setup_W_H(_w, _h,_content);
        }
        #region activity
        public void change_content(string c) { content = c; }
        public virtual Command update(KeyManager k, MouseManager m)
        {
            Command c = Command.nothing;
            if (k != null && selected) { c = update_with_key_manager(k); }
            if (m != null) { c= update_with_mouse_manager(m); }
            return c;
        }
        public virtual Command update_with_key_manager(KeyManager k) {
            if (k.IsKeyDown(KeyID.Select))
            {
                return is_applied();
            }
            if (k.IsKeyDown(KeyID.Cancel))
            {
                return is_left();
            }
            return Command.nothing;
        }
        public virtual Command update_with_mouse_manager(MouseManager m) {
            if (m.IsButtomDownOnce(MouseButton.Left))
            {
                if (selected == false)
                {
                    is_selected();
                }else
                {
                    return is_applied();
                }
            }

            return Command.nothing;
        }

        public virtual bool PosInside(Vector v){
            if(v.X < pos.X+w && v.X> pos.X && v.Y < pos.Y + h && v.Y > pos.Y) { return true; }
            return false;    
        }
        public virtual bool PosInside(Vector v,int addx,int addy)
        {
            if (v.X < pos.X + w+addx && v.X >= pos.X+addx-2 && v.Y < pos.Y + h+addy && v.Y >= pos.Y+addy-2) { return true; } //Console.Write(addx+"+"+pos.X.ToString()+"+"+w);
            return false;
        }
        public virtual void is_selected() {
            selected = true;
        }
        public virtual Command is_left()
        {
            selected = false;
            return Command.left_and_go_back;
        }
        public virtual Command is_applied()
        {
            selected = false;
            return reply;
        }
        #endregion

        #region draw
        public virtual void draw(Drawing d)
        {
            if (str != null && str != "")
            {
                new RichText(nstr, default_fontId, selected ? Color.Yellow : Color.White).Draw(d, pos, DepthID.Message);
            }
        }
        /// <summary>
        /// 絶対指定しているColoumの座標をx,yで訂正できる。
        /// </summary>
        /// <param name="x">ColoumのXにこのxの値だけ増加させる</param>
        /// <param name="y">このyの値だけ増加させる</param>
        public virtual void draw(Drawing d,int x, int y)
        {
            if (str != null && str != "")
            {
                Vector posNew = new Vector(pos.X + x, pos.Y + y);
                new RichText(nstr, default_fontId, selected ? Color.Yellow : Color.White).Draw(d, posNew, DepthID.Message);
            }
        }
        #endregion

        /// <summary>
        /// set str, content to null
        /// </summary>
        public virtual void clear() {
            pstr = null;
            str = null;
            content = null;
        } 
        /// <summary>
        /// _w=_h= -1 の時strの文字列の大きさに従う;  = -2 の時,str+contentの文字の大きさに従う
        /// </summary>
        public virtual void setup_W_H(int _w,int _h,string _c) {
            w = _w; h = _h;
            #region type 1- only str
            if (w == -1)
            {
                w = pstr.Width;
            }
            if (h == -1)
            {
                h = (pstr.CountChar() + 1) * pstr.getCharSizeY();
            }
            #endregion
            #region type 2- str and content
            if (_c != null)
            {
                PoorString pco = new PoorString(_c, maximumOfCharsEachLine, default_fontId, false);
                if (w==-2)
                {
                    w = pstr.Width;
                    w += pco.Width;
                }
                if (h == -2)
                {
                    h = (pstr.CountChar() + 1) * pstr.getCharSizeY();
                    h = Math.Max(h,pco.Height);
                    h += dy;
                }
            }else if (w == -2) // content is null
            {
                w = pstr.Width;
                w += dx;
            }
            #endregion
            if(w>0 && w < 70) { w = 70; }
            //Console.WriteLine(str + ":" + w + " " + h);
        }//setup_W_H end
    }// class Coloum end

    class Blank : Coloum
    {
        /// <summary>
        /// 内容書き換え可能の項目
        /// </summary>
        /// <param name="_x"></param>
        /// <param name="_y"></param>
        /// <param name="_str">この項目の名前</param>
        /// <param name="_content">書き換えできる内容</param>
        /// <param name="_reply"></param>
        /// <param name="_w">-1でstrによって、-2でstrとcontentのサイズによって 自身のw,hを求める</param>
        /// <param name="_dx">名前と内容の距離差</param>
        public Blank(int _x, int _y, string _str, string _content, Command _reply, 
            int _w=-2,int _h=-2,int _dx = default_distance, int _dy = 0) : base(_x, _y, _str, _reply,_w,_h,_content)
        {
            content = _content;
            dx = _dx;
            dy = _dy;
        }

        public override void draw(Drawing d) {
            if (str != null && str != "")
            {
                new RichText(str, FontID.Medium, selected ? Color.Yellow : Color.White).Draw(d, pos, DepthID.Message);
            }
            if (content != null && content != "")
            {
                new RichText(content, FontID.Medium, Color.White).Draw(d, new Vector(pos.X+pstr.Width + dx, pos.Y + dy), DepthID.Message);
            }
        }
        public override void draw(Drawing d, int x, int y)
        {
            Vector posNew = new Vector(pos.X + x, pos.Y + y);
            if (str != null && str != "")
            {
                new RichText(str, FontID.Medium, selected ? Color.Yellow : Color.White).Draw(d, posNew, DepthID.Message);
            }
            if (content != null && content != "")
            {
                new RichText(content, FontID.Medium, Color.White).Draw(d, new Vector(posNew.X+pstr.Width + dx, posNew.Y + dy), DepthID.Message);
            }
        }
        public override Command is_applied()
        {
            if (reply == Command.nothing) { return reply; }
            while (true) {
                Console.WriteLine(str);
                Console.Write("changes to : ");
                content = Console.ReadLine();
                int ap;
                if (content == DataBase.BlankDefaultContent || 
                    ( (reply==Command.apply_int || reply==Command.specialIntChange1||reply==Command.specialIntChange2) 
                    && int.TryParse(content, out ap) ) 
                    || content!="" )
                {
                    break;
                }

            }
            return base.is_applied();
        }
    }// class Blank end

    class Button : Coloum
    {
        // TextureDictionary内の画像をボタンにもできる。画像がボタンの実際大きさになる。

        /// <summary>
        /// Is str a key of Texture. true= it is. false= it is not
        /// </summary>
        protected bool useTexture = false;
        /// <summary>
        /// Texture画像内の区画の番号。
        /// </summary>
        protected int TexIndex ;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_x"></param>
        /// <param name="_y"></param>
        /// <param name="_str"></param>
        /// <param name="_content"></param>
        /// <param name="_reply"></param>
        /// <param name="_useTexture">画像を使うかのbool.使う時はtrueで,contentが画像名になる</param>
        /// <param name="_dx"></param>
        /// <param name="_dy"></param>
        /// <param name="index">画像の何コマ目を指定する</param>
        public Button(int _x, int _y, string _str, string _content, Command _reply, bool _useTexture, int _dx = default_distance, int _dy = 0,int index=0) 
            : base(_x, _y, _str, _reply)
        {
            useTexture = _useTexture;
            content = _content;
            dx = _dx;
            dy = _dy;
            TexIndex = index;
            if (useTexture) {
                w = DataBase.getTexD(str).w_single; h = DataBase.getTexD(str).h_single;
            }
            else { setup_W_H(-2, -2, content); }
        }

        public override Command is_applied()
        {
            switch (reply)
            {
                case Command.tru_fals:
                    if (content == true.ToString()) { content = false.ToString(); }
                    else { content = true.ToString(); }
                    break;
                case Command.button_off:
                    reply = Command.button_on;
                    break;
                case Command.button_on:
                    reply = Command.button_off;
                    break;
                default:
                    break;
            }
            return base.is_applied();
        }
        public override void draw(Drawing d)
        {
            if (!useTexture)
            {
                base.draw(d);
                if (content != null && content != "")
                {
                    new RichText(content, FontID.Medium, selected ? Color.Yellow : Color.White).Draw(d, new Vector(pos.X + dx, pos.Y + dy), DepthID.Message);
                }
            }
            else
            {
                if (str != null)
                {
                    d.Draw(pos, DataBase.getTex(str), DepthID.Message);
                }
                if (content != null && content != "")
                {
                    new RichText(content, FontID.Medium, Color.White).Draw(d, new Vector(pos.X + dx, pos.Y + dy), DepthID.Message);
                }
            }
        }
        public override void draw(Drawing d, int x, int y)
        {
            Vector posNew = new Vector(pos.X + x, pos.Y + y);
            if (!useTexture)
            {
                if (str != null && str != "")
                {
                    new RichText(str, default_fontId, selected ? Color.Yellow : Color.White).Draw(d, posNew, DepthID.Message);
                }
                if (content != null && content != "")
                {
                    new RichText(content, FontID.Medium, selected ? Color.Yellow : Color.White).Draw(d, new Vector(posNew.X +pstr.Width+dx, posNew.Y + dy), DepthID.Message);
                }
            }
            else
            {
                if (str != null)
                {
                    d.Draw(posNew, DataBase.getTex(str), DepthID.Message);
                }
                if (content != null && content != "")
                {
                    new RichText(content, FontID.Medium, Color.White).Draw(d, new Vector(posNew.X + pstr.Width + dx, posNew.Y + dy), DepthID.Message);
                }
            }
        }
    }

    /// <summary>
    /// UnitTypeDataBaseとUnitTypeが存在する前提の、専用unitTypeの画像のbutton
    /// </summary>
    class UTDutButton : Coloum {
        private UnitType ut;
        /// <summary>
        /// コンストラクタ。UnitTypeDataBaseがあることを確認してください。
        /// </summary>
        /// <param name="_str">UnitTypeDataBaseの中身にアクセスするためのkey string,UnitTypeの名前</param>
        /// <param name="_content">もし文字を書くなら</param>
        /// <param name="_reply">Buttonが押された時の返事Command</param>
        public UTDutButton(int _x, int _y, string _str, string _content, Command _reply,int _dx = default_distance, int _dy = 0) : base(_x, _y, _str, _reply)
        {
            content = _content;
            dx = _dx; dy = _dy;
            ut = DataBase.getUnitType(str);
            w = DataBase.getTexD(ut.texture_name).w_single;
            h = DataBase.getTexD(ut.texture_name).h_single;
        }

        #region method 
        public void changeStrTo(string s, string _content = null, int _dx = default_distance, int _dy= 0)
        {
            if (DataBase.utDataBase.UnitTypeDictionary.ContainsKey(s))
            {
                str = s; ut = DataBase.getUnitType(str);
                w = DataBase.getTexD(ut.texture_name).w_single;
                h = DataBase.getTexD(ut.texture_name).h_single;
            }
            else {
                Console.WriteLine(s+" isNotFoundInUTD");
            }
            content = _content;
            dx = _dx; dy = _dy;
        }
        #endregion

        #region update
        public override Command update(KeyManager k, MouseManager m)
        {
            if (m != null) { return update_with_mouse_manager(m); }
            return Command.nothing;
        }
        public override Command update_with_mouse_manager(MouseManager m) { 
            if(PosInside(m.MousePosition()) && m.IsButtomDown(MouseButton.Left) ){
                return is_applied();
            }
            return Command.nothing;
        }
        #endregion
        #region draw
        public override void draw(Drawing d)
        {
            if (str != null && str != "")
            {
                ut.drawIcon(d, pos);
            }
            if (content != null && content != "")
            {
                new RichText(content, FontID.Medium, Color.White).Draw(d, new Vector(pos.X + dx, pos.Y + dy), DepthID.Message);
            }

        }
        public override void draw(Drawing d, int x, int y)
        {
            Vector posNew = new Vector(pos.X + x, pos.Y + y);
            if (str != null && str != "")
            {
                d.Draw(posNew, DataBase.getTex(ut.texture_name), DataBase.getRectFromTextureNameAndIndex(ut.texture_name, ut.texture_min_id), DepthID.Message);
            }
            if (content != null && content != "")
            {
                new RichText(content, FontID.Medium, Color.White).Draw(d, new Vector(posNew.X + dx, posNew.Y + dy), DepthID.Message);
            }
        }
        #endregion

    }// UTDutButton class end
}//namespace CommonPart End
