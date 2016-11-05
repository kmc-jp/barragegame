using System;
using Microsoft.Xna.Framework;

namespace CommonPart
{
    /// <summary>
    /// AnimationをWindow内に配置するためのColoum;widthはanimationのwidth
    /// </summary>
    class AnimationColoum : Coloum
    {
        AnimationAdvanced animationAdvanced;
        bool updated=true;
        #region constructor
        /// <summary>
        /// strがタイトルに当たる ,contentが描画するanimationを指定している。
        /// </summary>
        /// <param name="_str">このcoloumの文字</param>
        /// <param name="_content">このcoloumのanimationを示す</param>
        /// <param name="_reply">このcoloumを選択された時の返り値</param>
        public AnimationColoum(int _x, int _y, string _str, AnimationDataAdvanced _content, Command _reply, int _dx = 0, int _dy = default_distance)
            : base(_x, _y, _str, _reply)
        {
            //Console.WriteLine(_str+"-"+str);
            content = _content.animationDataName;
            animationAdvanced = new AnimationAdvanced(_content);
            dx = _dx; dy = _dy;
            setup_W_H(0, 0, null);
        }
        #endregion

        #region method
        /// <summary>
        /// animationを更新するboolをfalseにする。
        /// </summary>
        public void stop() { updated = false; }
        public void play(bool _shifted=false) {
            if (updated) { stop(); }
            else
            {
                if (_shifted) { replay(); }
                else { updated = true; }
            }
        }
        /// <summary>
        /// animationのループ源か自分の最初に戻る
        /// </summary>
        public void replay()
        {
            animationAdvanced.backToTop();
        }

        /// <summary>
        /// _cの指定に従い、AnimationColoumのwidthとheightを作り上げる。もしくはw,hをそのまま指定する.
        /// </summary>
        /// <param name="_w"></param>
        /// <param name="_h"></param>
        /// <param name="_c">animationのwidthとheightの固定に照準となるアニメーションを指定できる.nullで自分のanimationで設定</param>
        public override void setup_W_H(int _w, int _h, string _c)
        {
            dy = (pstr.CountChar() + 1) * pstr.getCharSizeY();
            if (_w <= 0 || _h <= 0)
            {
                w = 0; h = 0;
                h += (pstr.CountChar() + 1) * pstr.getCharSizeY();
                w += dx; h += dy;
                if (_c != null)
                {
                    if (DataBase.existsAniD(_c,null))
                    {
                        w += DataBase.getAniD(_c).X;
                        h += DataBase.getAniD(_c).Y;
                    }
                    else { w += pstr.Width; }
                } else {
                    if (animationAdvanced != null)
                    {
                        w += (int)animationAdvanced.X;
                        h += (int)animationAdvanced.Y;
                    }
                    else { w += pstr.Width; }
                }
            }
            else
            {
                w = _w; h = _h;
            }
        }

        /// <summary>
        /// coloumの文字部分とアニメーションを変える。nullのものは変更しないと判定する
        /// </summary>
        /// <param name="_str">coloumの文字部分</param>
        /// <param name="_content">アニメーションの名前</param>
        /// <param name="_dx">文字部分とアニメーション部分のx変位</param>
        /// <param name="_dy">文字部分とアニメーション部分のy変位</param>
        public void changeTo(string _str, string _content, int _dx = default_distance, int _dy = 0)
        {
            if (_str != null) { str = _str; }
            if (_content != null && DataBase.existsAniD(_content, null))
            {
                content = _content;
                animationAdvanced = new AnimationAdvanced(DataBase.getAniD(content));
            }
            else if (_content != null)
            {
                Console.WriteLine(_str + " isNotFoundInAniD");
            }
            dx = _dx; dy = _dy;
            setup_W_H(-1, -1, null);
        }
        #endregion

        #region update
        public override Command update(KeyManager k, MouseManager m)
        {
            if (updated) { animationAdvanced.Update(); }
            if (m != null) { return update_with_mouse_manager(m); }
            return Command.nothing;
        }
        public override Command update_with_mouse_manager(MouseManager m)
        {
            if (PosInside(m.MousePosition()) && m.IsButtomDown(MouseButton.Left))
            {
                return is_applied();
            }
            return Command.nothing;
        }
        #endregion
    
        #region draw
        public override void draw(Drawing d)
        {
            base.draw(d);
            if (content != null && content != "")
            {
                animationAdvanced.Draw(d, new Vector(pos.X + dx, pos.Y + dy), DepthID.Message);
            }

        }
        public override void draw(Drawing d, int x, int y)
        {
            Vector posNew = new Vector(pos.X + x, pos.Y + y);
            if (str != null && str != "")
            {
                new RichText(str, default_fontId, selected ? Color.Yellow : Color.White).Draw(d, posNew, DepthID.Message);
            }
            if (content != null && content != "")
            {
                animationAdvanced.Draw(d, new Vector(posNew.X + dx, posNew.Y + dy), DepthID.Message);
            }
        }
        #endregion
    }
}
