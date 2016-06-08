using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace barragegame {
    /// <summary>
    /// 当たり判定を管理するクラス 境界を含みます
    /// </summary>
    abstract class HitShape {
        public abstract HitType Shape { get; }
        protected HitShape() { }
        public bool IsHit(Vector v1, Vector v2, HitShape h2) {
            return IsHit(v1, this, v2, h2);
        }
        public static bool IsHit(Vector v1, HitShape h1, Vector v2, HitShape h2) {
            if(h1.Shape == HitType.Circle) {
                if(h2.Shape == HitType.Circle) return IsHit(v1, (HitCircle)h1, v2, (HitCircle)h2);
                if(h2.Shape == HitType.Rect) return IsHit(v1, (HitCircle)h1, v2, (HitRect)h2);
            }
            if(h1.Shape == HitType.Rect) {
                if(h2.Shape == HitType.Circle) return IsHit(v2, (HitCircle)h2, v1, (HitRect)h1);
                if(h2.Shape == HitType.Rect) return IsHit(v1, (HitRect)h1, v2, (HitRect)h2);
                if(h2.Shape == HitType.Donut) return IsHit(v1, (HitRect)h1, v2, (HitDonut)h2);
            }
            if(h1.Shape == HitType.Donut) {
                if(h2.Shape == HitType.Rect) return IsHit(v2, (HitRect)h2, v1, (HitDonut)h1);
            }
            return false;
        }
        public static bool IsHit(Vector v1, HitCircle h1, Vector v2, HitCircle h2) {
            return Pow2(v2.X - v1.X) + Pow2(v2.Y - v1.Y) <= Pow2(h1.Range + h2.Range);
        }
        public static bool IsHit(Vector v1, HitCircle h1, Vector v2, HitRect h2) {
            //座標変換　v2中心　h2を軸に沿わせる
            Vector p = (v1 - v2).Rotate(-h2.Angle);
            double x = p.X;
            double y = p.Y;
            double w = h2.Width / 2;
            double h = h2.Height / 2;
            double r = h1.Range;
            //半径+長方形の幅に入ってなかったらあたってない
            if(!(Between(x, -r - w, r + w) && Between(y, -r - h, r + h))) return false;
            //角の丸い部分考慮
            //左端
            if(x < -w) {
                if(y < -h) return Pow2(x + w) + Pow2(y + h) <= Pow2(r);
                if(y > h) return Pow2(x + w) + Pow2(y - h) <= Pow2(r);
                return true;
            }
            //右端
            if(x > w) {
                if(y < -h) return Pow2(x - w) + Pow2(y + h) <= Pow2(r);
                if(y > h) return Pow2(x - w) + Pow2(y - h) <= Pow2(r);
                return true;
            }
            return true;
        }
        public static bool IsHit(Vector v1, HitRect h1, Vector v2, HitRect h2) {
            //v2中心でh2=0°にしたのときのv1の座標
            Vector p = (v1 - v2).Rotate(-h2.Angle);
            Vector vh = Vector.GetFromAngleLength(h1.Angle - h2.Angle + 90, h1.Height / 2);
            Vector vw = Vector.GetFromAngleLength(h1.Angle - h2.Angle, h1.Width / 2);
            double h = h2.Height / 2;
            double w = h2.Width / 2;
            Vector[] x = new[] { p - vh - vw, p + vh - vw, p + vh + vw, p - vh + vw, p - vh - vw };
            //h2にh1が含まれている
            for(int i = 0; i < 4; i++)
                if(Between(x[i].X, -w, w) && Between(x[i].Y, -h, h)) return true;
            //h1とh2は交点を持つ
            Vector[] y = new[] { new Vector(-w, -h), new Vector(w, -h), new Vector(w, h), new Vector(-w, h), new Vector(-w, -h) };
            for(int i = 0; i < 4; i++)
                for(int j = 0; j < 4; j++)
                    if(IsHitLine(x[i], x[i + 1], y[j], y[j + 1])) return true;
            //h1にh2が含まれている
            p = (v2 - v1).Rotate(-h1.Angle);
            vh = Vector.GetFromAngleLength(h2.Angle - h1.Angle + 90, h2.Height / 2);
            vw = Vector.GetFromAngleLength(h2.Angle - h1.Angle, h2.Width / 2);
            h = h1.Height;
            w = h1.Width;
            for(int i = 0; i < 4; i++)
                if(Between(x[i].X, -w, w) && Between(x[i].Y, -h, h)) return true;
            return false;
        }
        public static bool IsHit(Vector v1, HitRect h1, Vector v2, HitDonut h2) {
            //交点を持つ場合
            if(IsHitCircleFrame(v1, h1, v2, h2.RangeHoll)) return true;
            if(IsHitCircleFrame(v1, h1, v2, h2.Range)) return true;
            //四角に円が含まれる
            //v1中心でh1=0°にしたのときのv2の座標
            Vector p = (v2 - v1).Rotate(-h1.Angle);
            if(Between(p.X, -h1.Width / 2 + h2.Range, h1.Width / 2 - h2.Range)
                && Between(p.Y, -h1.Height / 2 + h2.Range, h1.Height / 2 - h2.Range)) return true;

            //四角の1点（v1）のv2からの距離
            double d = (v2 - v1).GetLengthSquare();
            if(d < Pow2(h2.RangeHoll)) return false;
            if(d <= Pow2(h2.Range)) return true;
            return false;
        }
        public static bool IsHitCircleFrame(Vector v1, HitRect h1, Vector v2, double range) {
            //v1中心でh1=0°にしたのときのv2の座標
            Vector p = (v2 - v1).Rotate(-h1.Angle);
            //対称性から変換
            p.X = Math.Abs(p.X);
            p.Y = Math.Abs(p.Y);
            double w = h1.Width / 2;
            double h = h1.Height / 2;
            range *= range;
            //(x-px)^2+(y-py)^2=r^2 = xv+yv=r^2とする
            //xv+yv=r^2をみたすx<-[-w,w],y<-[-h,h]が存在すればよい
            double minxv = (p.X <= w) ? 0 : Pow2(w - p.X);
            double maxxv = Pow2(w + p.X);
            double minyv = (p.Y <= h) ? 0 : Pow2(h - p.Y);
            double maxyv = Pow2(h + p.Y);
            return Between(range, minxv + minyv, maxxv + maxyv);
        }
        /// <summary>
        /// 線分x1 x2と線分y1 y2は交点を持つか
        /// </summary>
        public static bool IsHitLine(Vector x1, Vector x2, Vector y1, Vector y2) {
            //x1 x2はy軸に平行
            if(x1.X == x2.X) {
                //y1 y2はy軸に平行
                if(y1.X == y2.X)
                    if(x1.X == y1.X) return Meet2Lines(x1.Y, x2.Y, y1.Y, y2.Y);
                    else return false;
                return IsHitLine(y1, y2, x1, x2);
            }
            //x1 x2の傾き
            double slashx = (x2.Y - x1.Y) / (x2.X - x1.X);
            //y1 y2はy軸に平行
            if(y1.X == y2.X) return Between(y1.X, x1.X, x2.X) && Between(slashx * (y1.X - x1.X) + x1.Y, y1.Y, y2.Y);
            double slashy = (y2.Y - y1.Y) / (x2.Y - y1.Y);
            //x1 x2,y1 y2は平行
            if(slashx == slashy) return false;
            double min = Math.Max(Math.Min(x1.X, x2.X), Math.Min(y1.X, y2.X));
            double max = Math.Min(Math.Max(x1.X, x2.X), Math.Max(y1.X, y2.X));
            if(min >= max) return false;
            return Between((y1.Y - x1.Y + slashx * x1.X - slashy * y1.X) / (slashx - slashy), min, max);
        }

        static double Pow2(double x) { return x * x; }
        /// <summary>
        /// 範囲[r1,r2]にxが含まれるかどうか
        /// </summary>
        static bool Between(double x, double r1, double r2) { return (r1 <= x && x <= r2) || (r2 <= x && x <= r1); }
        /// <summary>
        /// 2つの線分は共通部分を持つのか
        /// </summary>
        static bool Meet2Lines(double x1, double x2, double y1, double y2) { return Between(y1, x1, x2) || Between(y2, x1, x2) || Between(x1, y1, y2); }
    }
    class HitCircle: HitShape {
        public override HitType Shape { get { return HitType.Circle; } }
        public double Range { get; private set; }
        public HitCircle(double r) {
            Range = r;
        }
    }
    /// <summary>
    /// 中心を基準とする長方形の当たり判定
    /// </summary>
    class HitRect: HitShape {
        public override HitType Shape { get { return HitType.Rect; } }
        public double Width { get; private set; }
        public double Height { get; private set; }
        public Vector Size { get { return new Vector(Width, Height); } }
        public double Angle { get; private set; }
        public HitRect(double width, double height, double angle) {
            Width = width;
            Height = height;
            Angle = angle;
        }
        public static HitRect From2PointWidth(Vector pos1, Vector pos2, double width) {
            return new HitRect((pos2 - pos1).GetLength(), width, Vector.GetAngle(pos1, pos2));
        }
    }
    /// <summary>
    /// あなのあいた円　実装は一部のみ
    /// </summary>
    class HitDonut: HitShape {
        public override HitType Shape { get { return HitType.Donut; } }
        /// <summary>
        /// 穴の半径
        /// </summary>
        public double RangeHoll { get; private set; }
        /// <summary>
        /// 幅
        /// </summary>
        public double Width { get; private set; }
        /// <summary>
        /// 穴の半径
        /// </summary>
        public double Range { get; private set; }
        public HitDonut(double rh, double w) {
            RangeHoll = rh;
            Width = w;
            Range = rh + w;
        }
    }
    enum HitType { Circle, Rect, Donut }
}
