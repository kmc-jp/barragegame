using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using CommonPart;

namespace CommonPart {
    /// <summary>
    /// 全体で使う関数などを扱うクラス
    /// </summary>
    static class Function {
        static RandomXS rand = new RandomXS();
        public static void SetRandomSeed(uint seed1, uint seed2, uint seed3, uint seed4) { rand = new RandomXS(seed1, seed2, seed3, seed4); }
        public static void SetRandomSeed(long l) { rand = new RandomXS(l); }
        /// <summary>
        /// 0からmax - 1までの乱数を返す
        /// </summary>
        public static int GetRandomInt(int max) { return rand.NextInt(max); }
        /// <summary>
        /// minからmax - 1までの乱数を返す
        /// </summary>
        public static int GetRandomInt(int min, int max) { return min + rand.NextInt(max - min); }
        /// <summary>
        /// 0からmaxまでの乱数を返す
        /// </summary>
        public static double GetRandomDouble(double max) { return rand.NextDouble(max); }
        public static double GetRandomDouble(double min, double max) { return min + rand.NextDouble(max - min); }

        public static int GetEnumLength(Type t) { return Enum.GetNames(t).Length; }
        public static int GetEnumLength<T>() { return Enum.GetNames(typeof(T)).Length; }
        public static List<T> GetEnumValues<T>() { return Enum.GetValues(typeof(T)).Cast<T>().ToList(); }

        //度数法関数定義
        const double tr = Math.PI / 180;
        public static double Sin(double d) { return Math.Sin(d * tr); }
        public static double Cos(double d) { return Math.Cos(d * tr); }
        public static double Tan(double d) { return Math.Tan(d * tr); }
        public static double Atan2(double y, double x) { return Math.Atan2(y, x) / tr; }

        public static float ToRadian(double v) { return (float)(v * tr); }

        /// <summary>
        /// 度数法の角度をそれと等価なｰ180°以上180°未満の値に変換する
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double AdjustAngle(double x) { return x - Math.Floor((x + 180) / 360) * 360; }

        public static IOrderedEnumerable<T> GetRandomSort<T>(this IEnumerable<T> list) {
            return list.OrderBy(_ => Guid.NewGuid());
        }

        public static double distance(double x1, double y1, double x2, double y2)
        {
            return (x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2);
        }

        public static bool hitcircle(double x1, double y1,double radius1, double x2, double y2,double radius2)
        {
            return distance(x1, y1, x2, y2) <= (radius1 + radius2) * (radius1 + radius2);
        } 

    }//function.end
    static class EnumExtension {
        /// <summary>
        /// Direction4の反転した方向を求める。
        /// </summary>
        public static Direction4 Reverse(this Direction4 d) {
            return (Direction4)(((int)d + 2) % 4);
        }
    }
}
