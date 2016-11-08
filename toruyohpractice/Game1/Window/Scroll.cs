using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace CommonPart
{
    /// <summary>
    /// Scrollをwindowのcoloumsにいれて、update()していけば上手く作動するようにしたい。
    /// またこれはwindowみたいにcoloumを持ち、scroll barもある.contentは今選択している内容
    /// </summary>
    class Scroll : Coloum
    {
        // pos は始点のx,y。常にスクロール全体の左上の座標に当たる
        //w, hは scrollの全長となる,大きい方が自動にスクロールする方向となる。スクロールbarの長さは今表示できる項目数/総項目数になる

        /// <summary>
        /// 今のスクロールbarが全体のどこに位置するかを決める。0から1を前から後ろと扱い、最初の見える項目の番号と関連する
        /// </summary>
        public double percent = 0;

        /// <summary>
        /// 今のスクロールbarの位置のxを取得できるandスクロールbarの位置のxを設定できる
        /// </summary>
        public double nx { get { if (vertical) return pos.X + dx; else return pos.X + dx + (l-barlength) * percent; }
            protected set
            {
                if (vertical) { //pos.X = value+dx;
                } else
                {
                    if (value >= pos.X + dx+l -barlength) { percent = 1; }
                    else { percent = (value - dx - pos.X) / (l-barlength);
                        if(percent > 1) { percent = 1; }
                        if(percent < 0) { percent = 0; }
                    }
                }
            }
        }
        /// <summary>
        /// 今のスクロールbarの位置.yを取得できandスクロールbarの位置.yを設定できる
        /// </summary>
        public double ny { get { if (!vertical) return pos.Y + dy; else return pos.Y+dy+(l - barlength)*percent ; }
            protected set {
                //barlength is visiable_n * h / (coloums.Count + 1)
                if (!vertical) {   //pos.Y = value;
                } else
                {
                    if (value >= pos.Y+dy  +l- barlength) { percent = 1; }
                    else {
                        percent = (value -pos.Y-dy) / (l-barlength);
                        if (percent > 1) { percent = 1; }
                        if (percent < 0) { percent = 0; }
                    }

                }
                //Console.WriteLine((ny+l-barlength)+" "+value+" "+l + " "+(l-barlength)+" "+percent );
            }
        }
        /// <summary>
        /// scroll bar の長さ
        /// </summary>
        public int barlength {
            get {
                if (coloums.Count > visiable_n) return (visiable_n * l) / (coloums.Count + 1);
                else { return l; }
            }
        }
        /// <summary>
        /// スクロールの全長を取得する
        /// </summary>
        public int l { get { if (vertical) return h-dy; else return w-dx; } }
        /// <summary>
        /// 今スクロールの中で選ばれている要項の番号.PosInsideで定義される
        /// </summary>
        public int now_coloum_index = -1;//-1の時はスクロールbarを操っている。
        const int scrollbar_index = -1;
        protected int now_firstVisiableIndex { get {
                if ( (int)((coloums.Count - visiable_n) * percent)<1)
                {
                    return 0;
                }else
                {
                    return (int)((coloums.Count - visiable_n) * percent);
                }
            }//get end
        }
        protected int now_lastVisiableIndex { get { return Math.Min(coloums.Count-1, now_firstVisiableIndex + visiable_n-1); } }
        protected int scrollStartX { get { return (int)(pos.X + dx); } }
        protected int scrollStartY { get { return (int)(pos.Y + dy); } }
        /// <summary>
        /// スクロールによって、coloumsのXにかかる校正
        /// </summary>
        protected int coloumsAdjustX { get
            {
                if (!vertical) { return scrollStartX-now_firstVisiableIndex * coloumsDistance; }else { return scrollStartX; }
            }
        }
        /// <summary>
        /// スクロールによって、coloumsのYにかかる校正
        /// </summary>
        protected int coloumsAdjustY
        {
            get
            {
                if (vertical) { return scrollStartY-now_firstVisiableIndex * coloumsDistance; } else { return scrollStartY; }
            }
        }
        /// <summary>
        /// 画面上に見えるこのスクロールの要項の数
        /// </summary>
        public int visiable_n;
        /// <summary>
        /// このスクロールにある全要項.ここのcoloumはscroll内部での座標を持たねばならない
        /// </summary>
        public List<Coloum> coloums = new List<Coloum>();
        /// <summary>
        /// 要項の総数
        /// </summary>
        public int Count { get { return coloums.Count; } }
        /// <summary>
        /// 要項間の間隔
        /// </summary>
        public int coloumsDistance;
        const int min_bw = 16;
        /// <summary>
        /// タイトルとscrollの距離である. strはタイトルになる
        /// </summary>
        const int default_title_dx = 5, default_title_dy = 25;
        protected bool vertical
        {
            get
            {
                if (h >= w) { return true; }
                else { return false; }
            }
        }

        /// <summary>
        /// 要項の個別サイズと、画面に見える要項の数でscrollを作る
        /// </summary>
        /// <param name="ox">スクロールの始点x</param>
        /// <param name="oy">スクロールの始点x</param>
        /// <param name="w">横スクロールの場合に長さになる、以外はスクロールバーの幅とみなせる</param>
        /// <param name="_str">タイトル</param>
        /// <param name="_sh">要項間の間隔</param>
        /// <param name="_n">全長に対応する要項の数</param>
        public Scroll(int _ox, int _oy, string _str, int _sh, int _n, int _w = min_bw,bool _vertical=true,int _dx=default_title_dx,int _dy=default_title_dy)
          : base(_ox, _oy, _str, Command.Scroll)
        {
            visiable_n = _n;
            coloumsDistance = _sh;
            Console.WriteLine(_sh);
            if (_vertical) { h = coloumsDistance * visiable_n; w = _w; }
            else { w = coloumsDistance * visiable_n; h = _w; }
            dx = _dx; dy = _dy;
            h += dy;w += dx;
            Console.Write(h);
        }

        public virtual void addColoum(Coloum c) {
            coloums.Add(c);
            if (coloumsDistance * visiable_n > l)
            {
                if (vertical) { h = coloumsDistance * visiable_n-dy; }
                else { w = coloumsDistance * visiable_n-dx; }
            }
        }
        public void clear_all_coloums()
        {
            coloums.Clear();
        }
        /// <summary>
        /// make this scroll completely useless
        /// </summary>
        public override void clear()
        {
            clear_all_coloums();
            coloums = null;
            visiable_n = 0;
            percent = 0; w = h = 0;
            base.clear();
        }
        public override bool PosInside(Vector v, int ax, int ay) {
            if (coloums == null) { return false; }
            if (v.X < nx + ax + w && v.X >= nx + ax - 1 && v.Y < ny + ay + h && v.Y >= ny + ay - 1)
            {
                now_coloum_index = scrollbar_index;
                return true;
            }
            else {
                for (int i = now_firstVisiableIndex; i <=now_lastVisiableIndex; i++)
                {
                    //if (coloums[i].PosInside(v, (int)(ax - nx + 2 * (dx + pos.X)), (int)(ay - ny + 2 * (dy + pos.Y)) ) )
                    if(coloums[i].PosInside(v, (int)(ax+coloumsAdjustX), (int)(ay+coloumsAdjustY) ))
                    {
                        select_index(i);
                        return true;
                    }
                }
            }
            return false;
        }
        public override bool PosInside(Vector v) { return PosInside(v, 0, 0); }
        protected void select_index(int i)
        {
            now_coloum_index = i;
            if (now_coloum_index != scrollbar_index)
            {
                content = coloums[i].content;
            }
        }
        public override Command update_with_key_manager(KeyManager k)
        {
            if (k.IsKeyDown(KeyID.Select))
            {
                coloums[now_coloum_index].is_selected();
                return is_applied();
            }
            return Command.nothing;
        }
        public override Command update_with_mouse_manager(MouseManager m) {
            Command c = Command.nothing;
            if (m != null)
            {
                if (now_coloum_index == scrollbar_index && m.IsButtomDown(MouseButton.Left) ) {
                    // scroll bar の移動
                    if (vertical) { ny += m.MousePosition().Y - m.OldMousePosition().Y;
                    }
                    else { nx += m.MousePosition().X - m.OldMousePosition().X; }
                    c = Command.Scroll; // Scrollを返しても、leftcoloum()にならないように書いているかな？
                }
                else if(m.IsButtomDownOnce(MouseButton.Left)) // Mouseがscroll全体の中に入っているかは、windowで判断している。
                {
                    // とある要項を選択した。
                    content = coloums[now_coloum_index].content;
                    c = coloums[now_coloum_index].is_applied();
                    //Console.WriteLine("scroll-coloums" + now_coloum_index + " " + content);
                }
            }
            return c;
        }

        public override void draw(Drawing d,int ax,int ay)
        {
            base.draw(d, ax, ay);
            d.DrawBoxFrame(new Vector(pos.X+dx+ax,pos.Y+dy+ay), vertical ? new Vector2(w, l) : new Vector2(l, h), Color.RosyBrown, DepthID.Message);
            if (coloums != null)
            {
                d.DrawBox(new Vector2((float)nx + ax, (float)ny + ay), vertical ? new Vector2(w, barlength) : new Vector2(barlength, h)
                    , Color.LimeGreen, DepthID.Message);
                for (int i = now_firstVisiableIndex; i <= now_lastVisiableIndex; i++)
                {
                    if (coloums[i].pos.X + ax+coloumsAdjustX >= ax + scrollStartX-2 &&
                        coloums[i].pos.Y + ay+coloumsAdjustY >= ay + scrollStartY-2)
                    {
                        coloums[i].draw(d, (int)(ax+coloumsAdjustX), (int)(ay+coloumsAdjustY));
                    }
                }
            } 
            //Console.WriteLine("draw "+(ax-nx)+" "+(ay-ny) );
        }
        public override void draw(Drawing d) { draw(d, 0, 0); }
    }
}
