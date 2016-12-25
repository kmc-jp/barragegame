using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CommonPart
{
    class Window
    {
        #region public Variable
        public int x, y;
        public int w, h;
        public bool mouse_dragable = false;
        /// <summary>
        /// このwindowを持つSceneなどに伝えたいCommandが書いている。
        /// </summary>
        public Command commandForTop=Command.nothing;
        #endregion

        #region private Variable
        private List<RichText> richTexts = new List<RichText>();
        /// <summary>
        /// i+1番目のrichtextは　i番目のRichTextの左下の点+richTextsRelativePos[i+1]　の位置にある。
        /// </summary>
        private List<Vector> richTextsRelativePos = new List<Vector>();
        private List<string> texturePaths = new List<string>();
        /// <summary>
        /// スクリーン上ではなく、windowの左上の(x,y)座標を(0,0)とした座標である。
        /// </summary>
        private List<Vector> texturesPos = new List<Vector>();
        private int NumberOfCharasEachLine = 22;
        #endregion

        #region protected variable
        bool useImageAsBackGround = false;
        string BackGroundImageName=null;
        /// <summary>
        /// Sceneから注目されているかされていないか
        /// </summary>
        protected bool selected = false;
        #endregion


        #region constructor
        public Window(int _x, int _y, int _w, int _h) {
            x = _x;
            y = _y;
            w = _w;
            h = _h;
        }
        #endregion
        public virtual void assignBackgroundImage(string bgi_name)
        {
            BackGroundImageName = bgi_name;
            useImageAsBackGround = true;
            w = DataBase.getTexD(BackGroundImageName).w_single;
            h = DataBase.getTexD(BackGroundImageName).h_single;
        }
        #region update
        public virtual void update(KeyManager k = null, MouseManager m = null)
        {
            if (k != null)
            {
                selected = true;
                update_with_key_manager(k);
            }
            if (m != null)
            {
                selected = true;
                update_with_mouse_manager(m);
            }
            if(k==null && m == null) { selected = false; }
        }
        public virtual void update_with_key_manager(KeyManager k)
        { }
        /// <summary>
        /// only deal with "mouse dragable"
        /// </summary>
        /// <param name="m"></param>
        public virtual void update_with_mouse_manager(MouseManager m)
        {
            if (mouse_dragable == true && PosInside(m.MousePosition()) && m.IsButtomDown(MouseButton.Left))
            {
                x += (int)(m.MousePosition().X - m.OldMousePosition().X);
                y += (int)(m.MousePosition().Y - m.OldMousePosition().Y);
            }
        }
        #endregion update
        public virtual void draw(Drawing d)
        {
            if (useImageAsBackGround)
            {
                d.Draw(new Vector(x, y), DataBase.getTex(BackGroundImageName), DepthID.BackGroundFloor);
            }
            else
            {
                d.DrawBox(new Vector(x, y), new Vector(w, h), Color.Black, DepthID.Message);
            }
            if (richTexts.Count() > 0)
            {
                richTexts[0].Draw(d, new Vector(x + richTextsRelativePos[0].X, y + richTextsRelativePos[0].Y), DepthID.Message);
                double ix = x + richTextsRelativePos[0].X; double iy = y + richTextsRelativePos[0].Y;
                for (int i = 1; i < richTexts.Count(); i++)
                {
                    ix += richTextsRelativePos[i].X;
                    iy += richTextsRelativePos[i].Y+ richTexts[i-1].Y;
                    richTexts[i].Draw(d, new Vector(ix, iy), DepthID.Message);
                }
            }
            if (texturePaths.Count() > 0)
            {
                for (int i = 0; i < texturePaths.Count(); i++)
                {
                    d.Draw(new Vector(x+texturesPos[0].X, y+texturesPos[0].Y), DataBase.getTex(texturePaths[i]), DepthID.Message);
                }
            }
        }
        #region Method
        public bool PosInside(Vector pos)
        {
            //Console.WriteLine("pos inside:" + pos.X + " " + pos.Y + " in " + x + "+" + w + " " + y + "+" + h);
            if(pos.X<x+w && pos.X > x && pos.Y<y+h && pos.Y>y) { return true; }
            return false;
        }
        public void AddRichText(string text, Vector _vector) {
            AddRichText(text, _vector, NumberOfCharasEachLine);
        }
        public void AddRichText(string text, Vector _vector, int m) {
            // m is "max number of chars in a line"
            richTexts.Add(new RichText(new PoorString(text, m).str));
            richTextsRelativePos.Add(_vector);
        }

        public void AddTexture(string texture_path, Vector _vector)
        {
            AddTexture(texture_path, _vector, NumberOfCharasEachLine);
        }
        public void AddTexture(string texture_path, Vector _vector, int m)
        {
            // m is "max number of chars in a line"
            texturePaths.Add(texture_path);
            texturesPos.Add(_vector);
        }
        
        public virtual void AddColoum(Coloum c) { }
        public virtual string getColoumiStr_string(int i){   return DataBase.InvaildColoumContentReply_string;}
        public virtual int getColoumiStr_int(int i) { return DataBase.InvaildColoumContentReply_int; }
        public virtual int getColoumiContent_int(int i) { return DataBase.InvaildColoumContentReply_int; }
        public virtual string getColoumiContent_string(int i) { return DataBase.InvaildColoumContentReply_string; }
        /// <summary>
        /// 常にfalse
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public virtual bool getColoumiContent_bool(int i){ return false; }
        public virtual string getNowColoumStr_string() { return DataBase.InvaildColoumContentReply_string; }
        public virtual int getNowColoumStr_int() { return DataBase.InvaildColoumContentReply_int; }
        public virtual string getNowColoumContent_string() { return DataBase.InvaildColoumContentReply_string; }
        public virtual int getNowColoumContent_int() { return DataBase.InvaildColoumContentReply_int; }
        /// <summary>
        /// 常にfalse
        /// </summary>
        /// <returns></returns>
        public virtual bool getNowColoumContent_bool() { return false; }
        #endregion
    } // class Window end

    class Window_WithColoum : Window
    {
        public int now_coloums_index = 0;
        /// <summary>
        /// Window uses key/mouseManager. Not its coloums.
        /// </summary>
        public bool keyResponseToWindow = true, mouseResponseToWindow = true;

        public List<Coloum> coloums = new List<Coloum>();
        public Window_WithColoum(int _x, int _y, int _w, int _h) : base(_x, _y, _w, _h)
        { }

        public override void AddColoum(Coloum c) { coloums.Add(c); }

        public override string getColoumiStr_string(int i)
        {
            if (coloums.Count > i)
            {
                return coloums[i].str;
            }
            return DataBase.InvaildColoumContentReply_string;
        }
        public override int getColoumiStr_int(int i)
        {
            if (coloums.Count > i)
            {
                int ans;
                if (int.TryParse(coloums[i].str, out ans))
                {
                    return ans;
                }
            }
            return DataBase.InvaildColoumContentReply_int;
        }
        public override int getColoumiContent_int(int i)
        {
            if (coloums.Count > i) {
                int ans;
                if (int.TryParse(coloums[i].content, out ans))
                {
                    return ans;
                }
            }
            return DataBase.InvaildColoumContentReply_int;
        }
        public override string getColoumiContent_string(int i)
        {
            if (coloums.Count > i)
            {
                return coloums[i].content;
            }
            return DataBase.InvaildColoumContentReply_string;
        }
        public override bool getColoumiContent_bool(int i)
        {
            if (coloums[i].content == true.ToString()) { return true; }
            else { return false; }
        }

        public override string getNowColoumStr_string() { return getColoumiStr_string(now_coloums_index); }
        public override int getNowColoumStr_int() { return getColoumiStr_int(now_coloums_index); }
        public override string getNowColoumContent_string() { return getColoumiContent_string(now_coloums_index); }
        public override int getNowColoumContent_int() { return getColoumiContent_int(now_coloums_index); }
        public override bool getNowColoumContent_bool()
        {
            return getColoumiContent_bool(now_coloums_index);
        }
        protected Blank create_blank(Command c, int x, int ny, string str, string content)
        {
            return new Blank(x, ny, str, content, c);
        }
        protected Button create_button(Command c, int x, int ny, string str, string content, bool useTexture)
        {
            return new Button(x, ny, str, content, c, useTexture);
        }
        /// <summary>
        /// このclassのコマンド処理はwindow内での処理が済んだ前提で、Sceneなどにコマンドを伝達するためにある。
        /// </summary>
        /// <param name="c">処理してほしいコマンド</param>
        protected virtual void deal_with_command(Command c)
        {
            if (c != Command.nothing && c != Command.Scroll) // Scroll--scroll barを移動しているだけだと、leftcoloumしない
            {
                left_coloum();
                commandForTop = c;
            } else { c = Command.nothing; }

        }
        public override void draw(Drawing d)
        {
            base.draw(d);
            for (int i = 0; i < coloums.Count; i++) {
                if (!selected)  //this Window is not selected. Its Coloums will not be Highlighted
                {
                    if (now_coloums_index == i && coloums[now_coloums_index].selected)
                    {
                        coloums[now_coloums_index].selected = false;
                        coloums[i].draw(d, x, y);
                        coloums[i].selected = true;
                    }
                    else
                    {
                        coloums[i].draw(d, x, y);
                    }
                }
                //This window is selected. Its Coloums will be Highlighted if it is selected
                else if (now_coloums_index == i && !coloums[now_coloums_index].selected) {
                    coloums[now_coloums_index].selected = true;
                    coloums[i].draw(d, x, y);
                    coloums[i].selected = false;
                }
                else
                {
                    coloums[i].draw(d, x, y);
                }
            }
        }
        #region update
        public override void update(KeyManager k, MouseManager m)
        {
            base.update(k, m);
            //現在注目のcoloumに対して、deal with command
            for (int i = 0; i < coloums.Count; i++)
            {
                if (i == now_coloums_index)
                {
                    if (!keyResponseToWindow && !mouseResponseToWindow)
                    {
                        if (m != null && coloums[now_coloums_index].PosInside(m.MousePosition(), x, y))
                        {
                            deal_with_command(coloums[now_coloums_index].update(k, m));
                        }
                        else { deal_with_command(coloums[now_coloums_index].update(k, null)); }
                    } else if (m != null && coloums[now_coloums_index].PosInside(m.MousePosition(), x, y))
                    {
                        //deal_with_command(coloums[now_coloums_index].update(null, m));//クリックしなくても適用するものがある。
                        coloums[now_coloums_index].update(null, m);
                    } else
                    {
                        coloums[now_coloums_index].update(null, null);
                    }
                }
                else { coloums[i].update(null, null); }
            }
        }
        public override void update_with_key_manager(KeyManager k) {
            if (keyResponseToWindow)
            {
                if (coloums.Count > 0)
                {
                    if (k.IsKeyDownOnce(KeyID.Down))
                    {
                        coloums[now_coloums_index].is_left();
                        now_coloums_index++;
                        if (now_coloums_index >= coloums.Count) { now_coloums_index = 0; }
                        coloums[now_coloums_index].is_selected();
                    }
                    else if (k.IsKeyDownOnce(KeyID.Up))
                    {
                        coloums[now_coloums_index].is_left();
                        now_coloums_index--;
                        if (now_coloums_index < 0) { now_coloums_index = coloums.Count - 1; }
                        coloums[now_coloums_index].is_selected();
                    }
                    if (k.IsKeyDownOnce(KeyID.Select))
                    {
                        selectColoum();
                    }
                }// if has any coloum or not
            }
        }//update_with_key_manager end
        public override void update_with_mouse_manager(MouseManager m)
        {
            if (coloums.Count > 0)
            {
                //bool mouseInsideColoums = false;
                for (int ii = 0; ii < coloums.Count; ii++)
                {
                    if (coloums[ii].PosInside(m.MousePosition(), x, y))
                    // PosInsideは画面上の絶対座標を使って判定している。windowの位置によって描画位置が変わるcoloumsにはx,y補正が必要 
                    {
                        //mouseInsideColoums = true;
                        if (m.MousePosition() != m.OldMousePosition())
                        {
                            if (now_coloums_index < coloums.Count && now_coloums_index >= 0 && now_coloums_index != ii)
                            {
                                coloums[now_coloums_index].is_left();
                            }
                            now_coloums_index = ii;
                        }
                        if (m.IsButtomDownOnce(MouseButton.Left))
                        {
                            selectColoum();
                        }
                        return; // coloumsの上だと、mouse dragableの処理を飛ばす
                    }
                }
                /*
                if (!mouseInsideColoums)
                {
                    left_coloum();
                }
                */
            }//if has any coloum or not
            base.update_with_mouse_manager(m); // mouse dragableの処理だけと思われる
        }//update_with_mouse_manager end
        #endregion
        /// <summary>
        /// coloums[now_coloums_index]に対して、is_selected()を呼ぶ。
        /// </summary>
        protected virtual void selectColoum()
        {
            keyResponseToWindow = false;
            mouseResponseToWindow = false;
            coloums[now_coloums_index].is_selected();
        }
        private void left_coloum() { keyResponseToWindow = mouseResponseToWindow = true;
            if (now_coloums_index < coloums.Count && now_coloums_index >= 0) {coloums[now_coloums_index].is_left(); }
        }
    }//class Window_WithColoum end

    /// <summary>
    /// 未使用
    /// </summary>
    abstract class WindowAsPages
    {
        public int x, y;
        public int maximumWindowIndex = 0;
        public int nowWindowIndex = 0;
        protected List<Window> WindowPages = new List<Window>();
        protected Blank nowWindowIndexBlank;
        protected Button toPrevious, toNext;
        /// <summary>
        /// Pageの番号指定の欄とボタン、　とWindow実体の距離。
        /// </summary>
        protected int dx = 10, dy = 10;
        public WindowAsPages(int _x, int _y)
        {
            x = _x; y = _y;
            toPrevious = new Button(x, y, "", "pre", Command.previousPage,false, 0, 0);
            nowWindowIndexBlank = new Blank(x + dx, y, "now page:", "0", Command.apply_int);
            toNext = new Button(x + nowWindowIndexBlank.w + 2 * dx, y, "", "next", Command.nextPage, false,0, 0);
        }
        public virtual void update(KeyManager k, MouseManager m)
        {
            Command c = Command.nothing;
            if (WindowPages.Count > 0) {
                if (WindowPages.Count > 1)
                {
                    if (m != null)
                    {
                        if (m.IsButtomDown(MouseButton.Left))
                        {
                            if (toPrevious.PosInside(m.MousePosition())) { c = toPrevious.is_applied(); }
                            if (toNext.PosInside(m.MousePosition())) { c = toNext.is_applied(); }
                            if (nowWindowIndexBlank.PosInside(m.MousePosition())) { c = nowWindowIndexBlank.is_applied(); }
                            deal_with_command(c);
                        }
                    }
                }
                WindowPages[nowWindowIndex].update(k, m);
            }
        }
        protected virtual void deal_with_command(Command c)
        {
            if (c == Command.nothing) { return; }
            if (c == Command.previousPage) {
                c = Command.apply_int;
                nowWindowIndexBlank.change_content( (nowWindowIndex-1).ToString()  );
            }
            if (c == Command.nextPage)
            {
                c = Command.apply_int;
                nowWindowIndexBlank.change_content((nowWindowIndex + 1).ToString());
            }
            if (c == Command.apply_int)
            {
                nowWindowIndex = int.Parse(nowWindowIndexBlank.content) ;
            }
        }
        public virtual void draw(Drawing d)
        {
            if (WindowPages.Count > 0) { WindowPages[nowWindowIndex].draw(d); }
        }

    }

    /// <summary>
    /// DataBaes.utDataBase is required; a poorly made
    /// </summary>
    class Window_utsList : Window_WithColoum
    {
        protected const int white_space_size = 40;
        /// <summary>
        /// UTDutButtonはUnitTypeDataBaseとUnitTypeが存在することが前提で使えるもの
        /// </summary>
        protected List<UTDutButton> UTDutButtons = new List<UTDutButton>();
        #region constructor
        public Window_utsList(int _x, int _y, int _w, int _h) : base(_x, _y, _w, _h)
        {
            setUp_UTDutButtons();
            mouse_dragable = true;
        }
        #endregion

        private void setUp_UTDutButtons()
        {
            if (UTDutButtons.Count > 0) { foreach (UTDutButton ub in UTDutButtons) { ub.clear(); } }
            UTDutButtons.Clear();
            //now UTDutButtons is an empty list
            int nx = white_space_size, ny = white_space_size;
            int max_dy = 0;
            for (int i = 0; i < DataBase.getUTDcount(); i++)
            {
                AddUTDut(nx, ny, DataBase.utDataBase.UnitTypeList[i].typename);
                max_dy = Math.Max(max_dy, UTDutButtons[i].h);
                nx += UTDutButtons[i].w;
                if (nx + white_space_size + UTDutButtons[i].w > this.w)
                {
                    nx = white_space_size;
                    ny += max_dy;
                }
            }
        }
        public void reloadUTDutButtons() {
            if (DataBase.getUTDcount() == UTDutButtons.Count) {
                int i = 0;
                foreach (UnitType ut in DataBase.utDataBase.UnitTypeList) {
                    UTDutButtons[i].changeStrTo(ut.typename);
                }
            }
        }
        public void AddUTDut(int x, int y, string str, string content = "", Command c = Command.UTDutButtonPressed)
        {
            UTDutButtons.Add(createUTDutButton(x, y, str, content, c));
        }
        public UTDutButton createUTDutButton(int x, int y, string str,string content="",Command c=Command.UTDutButtonPressed) {
            return new UTDutButton(x,y,str,content,c);
        }
    }// class Window_utsList end
    
}// namespace CommonPart End
