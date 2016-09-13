using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonPart {
    /// <summary>
    /// XorShift法による乱数の生成
    /// </summary>
    class RandomXS {
        uint x;
        uint y;
        uint z;
        uint w;
        const long pow2_32 = 0x100000000;
        public RandomXS()
            : this(System.DateTime.Now.Ticks) { }
        /// <summary>
        /// longのseed値（TimerのTicks想定から適当にシードを決める）
        /// </summary>
        public RandomXS(long ticks)
            : this(
                (uint)(ticks % pow2_32),
                (uint)(ticks * 3 / 0x20 % pow2_32),
                (uint)(ticks / 0x400 % pow2_32),
                (uint)(ticks / 0x8000 % pow2_32)) { }
        public RandomXS(uint seed1, uint seed2, uint seed3, uint seed4) {
            x = seed1;
            y = seed2;
            z = seed3;
            w = seed4;
            //初期値がまずければwをずらす
            if(unchecked(x + y + z + w) == 0) w++;
            //最初の128個は捨てる
            for(int i = 0; i < 128; i++)
                NextUInt();
        }
        /// <summary>
        /// 0～0xffffffffの整数の乱数を返す
        /// </summary>
        public uint NextUInt() {
            uint t = x ^ (x << 11);
            x = y; y = z; z = w;
            w = (w ^ (w >> 19)) ^ (t ^ (t >> 8));
            return w;
        }
        /// <summary>
        /// 0～max-1の整数の乱数を返す
        /// </summary>
        // メモ：負の数を渡された場合、絶対値を渡されたようにふるまいます
        public int NextInt(int max) {
            if(max == 0) return 0;
            //乱数2個消費でmaxが大きいときでも偏りを減らせているはず
            return (int)(((ulong)NextUInt() * pow2_32 + NextUInt()) % (ulong)max);
        }
        /// <summary>
        /// 0～maxの浮動小数の乱数を返す
        /// </summary>
        //メモ：負の数を渡された場合、max～0の乱数を返します
        //2^960≒10^289以上の値を渡すとInfinityが帰ってくる可能性あり
        //0でない絶対値が最小の値はmax / 2^64です（maxが極端に小さくないとき）
        public double NextDouble(double max) {
            return ((ulong)NextUInt() * pow2_32 + NextUInt()) * max / pow2_32 / pow2_32;
        }
    }
}
