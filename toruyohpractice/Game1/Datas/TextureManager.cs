using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace CommonPart {
    /// <summary>
    /// 画像を管理するクラス
    /// </summary>
    static class TextureManager {
        #region 変数
        /// <summary>
        /// フォント一覧
        /// </summary>
        static SpriteFont[] font;
        /// <summary>
        /// テクスチャ一覧
        /// </summary>
        static Texture2D[] texture;
        /// <summary>
        /// 読み込みに必要な何か
        /// </summary>
        static ContentManager content;
        #endregion
        #region 関数
        /// <summary>
        /// 画像を全て読み込む
        /// </summary>
        public static void Load(ContentManager cont) {
            content = cont;
            //配列のサイズ指定
            font = new SpriteFont[Enum.GetNames(typeof(FontID)).Length];
            texture = new Texture2D[Enum.GetNames(typeof(TextureID)).Length];

            //フォントをセット
            SetFont(FontID.Medium, "medium");

            //画像をセット
            SetTexture(TextureID.White, "white");
            SetTexture(TextureID.Scroll, "scroll");
        }

        /// <summary>
        /// 画像をセットする
        /// </summary>
        /// <param name="id">対応するTextureID</param>
        /// <param name="path">Contentからのパス（拡張子除く、厳密にはちょっと違う）</param>
        static void SetTexture(TextureID id, string path) { texture[(int)id] = content.Load<Texture2D>(path); }
        /// <summary>
        /// フォントをセットする
        /// </summary>
        /// <param name="id">対応するFontID</param>
        /// <param name="path">Contentからのパス（拡張子除く、厳密にはちょっと違う）</param>
        static void SetFont(FontID id, string path) { font[(int)id] = content.Load<SpriteFont>(path); }
        

        /// <summary>
        /// フォントを呼び出す
        /// </summary>
        /// <param name="id">対応するFontID</param>
        public static SpriteFont GetFont(FontID id) { return font[(int)id]; }
        /// <summary>
        /// 画像を呼び出す
        /// </summary>
        /// <param name="id">対応するTextureID</param>
        public static Texture2D GetTexture(TextureID id) { return texture[(int)id]; }
        #endregion
    }
    enum FontID{ Medium }
    enum TextureID {
        None, White, Scroll, MessageBack
    }
    /// <summary>
    /// 過去の遺産
    /// </summary>
    enum AnimationID {
    }
    static class TextureIDExtension {
        static double[] hitsize;
        static double?[] rotationSpeed;
        static int[] fontsizeX;
        static int[] fontsizeY;
        static AnimationData[] animedata;
        static double[] animehitsize;
        static TextureIDExtension() {
            hitsize = new double[Enum.GetNames(typeof(TextureID)).Length];
            rotationSpeed = new double?[Enum.GetNames(typeof(TextureID)).Length];

            //RichFont用　使わないフォントは設定しなくてよい
            fontsizeX = new int[Enum.GetNames(typeof(FontID)).Length];
            fontsizeY = new int[Enum.GetNames(typeof(FontID)).Length];
            SetFontSize(FontID.Medium, 10, 20);

            animedata = new AnimationData[Enum.GetNames(typeof(AnimationID)).Length];
            animehitsize = new double[Enum.GetNames(typeof(AnimationID)).Length];
        }
        public static void SetHitSize(TextureID id, double value) { hitsize[(int)id] = value; }
        public static void SetRotation(TextureID id, double? value) { rotationSpeed[(int)id] = value; }
        static void SetAnimetionData(AnimationID id, double size, AnimationData value) { animehitsize[(int)id] = size; animedata[(int)id] = value; }
        static void SetFontSize(FontID id, int x, int y) { fontsizeX[(int)id] = x; fontsizeY[(int)id] = y; }
        public static AnimationData GetAnimationData(this AnimationID a) {
            return animedata[(int)a];
        }
        public static double GetDefaultHitSize(this TextureID t) {
            return hitsize[(int)t];
        }
        public static double GetDefaultHitSize(this AnimationID t) {
            return animehitsize[(int)t];
        }
        public static double? GetDefaultRotation(this TextureID t) {
            return rotationSpeed[(int)t];
        }
        public static int GetDefaultFontSizeX(this FontID t) {
            return fontsizeX[(int)t];
        }
        public static int GetDefaultFontSizeY(this FontID t) {
            return fontsizeY[(int)t];
        }
    }
}
