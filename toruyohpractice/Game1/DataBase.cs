using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace CommonPart {
    /// <summary>
    /// 不変なデータをまとめたクラス
    /// </summary>
    class DataBase　{

        #region Window
        // ウィンドウの元のサイズ(4：3)
        public static readonly int WindowDefaultSizeX = Game1.WindowSizeX;
        public static readonly int WindowDefaultSizeY = Game1.WindowSizeY;
        // 16：9にした時のウィンドウの縦のサイズ
        public static readonly int WindowSlimSizeY = 720;
        #endregion

        #region Map
        // へクス画像のリスト
        public static List<Texture2D> hex_tex;
        // マップのサイズは MAP_MAX×MAP_MAX
        public static readonly int MAP_MAX = 20;
        // マップの倍率の配列
        public static readonly double[] MapScale = new[] { 0.15d, 0.3d, 0.4d, 0.5d, 0.6d, 0.7d, 0.8d, 0.9d, 1.0d, 1.2d, 1.5d, 2.0d, 3.0d };
        // デフォルトのマップの倍率
        public static readonly int DefaultMapScale = 8;
        // へクス画像の横幅と縦幅
        public static readonly int HexWidth = 180;
        public static readonly int HexHeight = 200;
        #endregion

        #region Bar&Box
        // バー画像のリスト
        public static List<Texture2D> bar_frame_tex;
        // ボックス画像のリスト
        public static List<Texture2D> box_frame_tex;
        // バー・ボックスの名前　※要らないけど名前と番号のメモ用に
        public enum BarName
        {
            Study, Unit, Minimap, Status, Arrange, Product
        }
        // それぞれのバー・ボックスの横幅の個数と縦幅の個数リスト
        public static readonly int[] BarWidth = new[] { 22, 22, 22, 40, 18, 18 };
        public static readonly int[] BarHeight = new[] { 6, 23, 16, 4, 6, 23 };
        // それぞれのバー・ボックスの左上の画面上での座標のリスト
        public static readonly Vector[] BarPos = new[] {
            new Vector(0d, 0d), new Vector(0d, 96d), new Vector(0d, 704d), new Vector(352d, 0d), new Vector(992d, 0d), new Vector(992d, 96d)
        };
        #endregion

        #region Unit
        // ユニット画像のリスト
        public static List<Texture2D> unit_tex;
        // ユニットの名前　※要らないけど名前と番号のメモ用に
        public enum MyUnitName
        {
            Kochu, Macro, Jujo, Kosan, NK, HelperT, KillerT, B, Plasma
        }
        public enum EnemyUnitName
        {
            
        }
        // ユニット各種類ごとの固有値
        public static readonly int[] MyUnitMAX_HP = new[] { 100, 100, 100, 100, 100, 100, 100, 100 };
        public static readonly int[] MyUnitMAX_LP = new[] { 100, 100, 100, 100, 100, 100, 100, 100 };
        public static readonly int[] MyUnitMAX_EXP = new[] { 100, 100, 100, 100, 100, 100, 100, 100 };

        #endregion

        #region Method
        // マップ上の位置から現在の画面上の座標を求める
        public static Vector WhereDisp(int x_index, int y_index, Vector camera, int scale)
        {
            if (y_index % 2 == 1)
                return new Vector((HexWidth * x_index + HexWidth / 2 - camera.X) * MapScale[scale],
                                (HexHeight * 3 / 4 * y_index - camera.Y) * MapScale[scale]);

            return new Vector((HexWidth * x_index - camera.X) * MapScale[scale],
                            (HexHeight * 3 / 4 * y_index - camera.Y) * MapScale[scale]);
        }
        // 左上の描画位置を与えるとそのユニットが画面に入るかどうかを返す
        public static bool IsInDisp(Vector p, int scale)
        {
            return p.X >= -HexWidth * MapScale[scale] && p.X <= Game1._WindowSizeX && p.Y >= -HexHeight * MapScale[scale] && p.Y <= Game1._WindowSizeY;
        }
        // 画面上の位置 (x,y) がへクスのある位置の上にあるどうかを返す
        public static bool IsOnHex(int x_index, int y_index, int x, int y, Vector camera, int scale)
        {
            double dx = camera.X + x / MapScale[scale] - HexWidth * x_index;
            double dy = camera.Y + y / MapScale[scale] - HexHeight * 3 / 4 * y_index;
            if (y_index % 2 == 1) dx -= HexWidth / 2;

            return dx >= 0 && dx <= HexWidth &&
                dy + dx * HexHeight / HexWidth / 2 >= HexHeight / 4 && dy + dx * HexHeight / HexWidth / 2 <= HexHeight / 4 * 5 &&
                dy + HexHeight / 4 >= dx * HexHeight / HexWidth / 2 && dy <= HexHeight / 4 * 3 + dx * HexHeight / HexWidth / 2;
        }
        #endregion

    }// DataBase end
}// namespace end
