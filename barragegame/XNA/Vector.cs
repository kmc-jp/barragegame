using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace barragegame {
    /// <summary>
    /// 位置、速度などを扱う構造体
    /// </summary>
    struct Vector {
        public double X;
        public double Y;
        public Vector(double px, double py) {
            X = px;
            Y = py;
        }
        /// <summary>
        /// 角度と長さからVectorを得る
        /// </summary>
        /// <param name="angle">角度（度数法）</param>
        /// <param name="length">長さ</param>
        /// <returns></returns>
        public static Vector GetFromAngleLength(double angle, double length) {
            return new Vector(Function.Cos(angle), Function.Sin(angle)) * length;
        }
        /// <summary>
        /// 長さを得る
        /// </summary>
        /// <returns></returns>
        public double GetLength() {
            return Math.Pow(X * X + Y * Y, 0.5);
        }
        /// <summary>
        /// 角度を得る
        /// </summary>
        /// <returns></returns>
        public double GetAngle() {
            return Function.Atan2(Y, X);
        }
        /// <summary>
        /// 長さの2乗を得る
        /// </summary>
        /// <returns></returns>
        public double GetLengthSquare() {
            return X * X + Y * Y;
        }
        /// <summary>
        /// 方向が同じ単位ベクトルを返す
        /// </summary>
        /// <returns></returns>
        public Vector GetUnit() {
            double leng = GetLength();
            return leng == 0 ? new Vector(1, 0) : (this / GetLength());
        }
        /// <summary>
        /// 回転したベクトルを返す
        /// </summary>
        /// <param name="a">回転角（度数法）</param>
        /// <returns></returns>
        public Vector Rotate(double a) {
            double cos = Function.Cos(a);
            double sin = Function.Sin(a);
            return new Vector(X * cos - Y * sin, X * sin + Y * cos);
        }
        /// <summary>
        /// 内積を得る
        /// </summary>
        public double InnerProduct(Vector v) {
            return X * v.X + Y * v.Y;
        }
        /// <summary>
        /// fromからtoへの角度を取得する（度数法）
        /// </summary>
        public static double GetAngle(Vector from, Vector to) { return Math.Atan2(to.Y - from.Y, to.X - from.X) * 180 / Math.PI; }
        /// <summary>
        /// fromからtoへのlengthのベクトルを取得する
        /// </summary>
        public static Vector GetAdjustedVector(Vector from, Vector to, double length) { return (to - from).GetUnit() * length; }
        public static Vector operator +(Vector v1, Vector v2) {
            return new Vector(v1.X + v2.X, v1.Y + v2.Y);
        }
        public static Vector operator -(Vector v1, Vector v2) {
            return new Vector(v1.X - v2.X, v1.Y - v2.Y);
        }
        public static Vector operator *(Vector v1, double k) {
            return new Vector(v1.X * k, v1.Y * k);
        }
        public static Vector operator *(double k, Vector v1) {
            return new Vector(v1.X * k, v1.Y * k);
        }
        public static Vector operator /(Vector v1, double k) {
            return new Vector(v1.X / k, v1.Y / k);
        }
        public static bool operator ==(Vector v1, Vector v2) {
            return (v1.X == v2.X && v1.Y == v2.Y);
        }
        public static bool operator !=(Vector v1, Vector v2) {
            return !(v1 == v2);
        }
        public static Vector operator -(Vector v1) {
            return new Vector(-v1.X, -v1.Y);
        }
        //implicitとexplicitは本来は意味的に逆にすべきだが楽をするための手抜き
        //（Vector→Vector2は情報が落ちるが、Vector2→Vectorは情報が落ちない）
        //若干の演算誤差を吸収する仕組みを追加した
        public static implicit operator Vector2(Vector v) {
            return new Vector2((float)Math.Floor(v.X + 0.00390625), (float)Math.Floor(v.Y + 0.00390625));
        }
        public static explicit operator Vector(Vector2 v) {
            return new Vector(v.X, v.Y);
        }
        public override bool Equals(object obj) {
            if(obj == null || obj.GetType() != this.GetType()) return false;
            Vector v = (Vector)obj;
            return (X == v.X && Y == v.Y);
        }
        public override int GetHashCode() {
            return X.GetHashCode() ^ Y.GetHashCode();
        }
        public override string ToString() {
            return "(" + X.ToString() + "," + Y.ToString() + ")";
        }
        /// <summary>
        /// 小数点以下の桁数を指定して、決められた文字数で文字列に
        /// </summary>
        /// <param name="l">桁数</param>
        /// <param name="num">文字数</param>
        /// <returns></returns>
        public string ToString(int l, int num) {
            return "(" + String.Format("{0:f" + l + "}", X).PadLeft(num) + "," + String.Format("{0:f" + l + "}", Y).PadLeft(num) + ")";
        }
    }
    enum Direction4 {
        Right, Down, Left, Up, None = -1
    }
}
