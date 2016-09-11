using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CommonPart {
    /// <summary>
    /// 描画を任せるクラス（基準点はすべて中心）
    /// </summary>
    class Drawing {
        /// <summary>
        /// 描画に必要なもの
        /// </summary>
        readonly SpriteBatch sb;
        public readonly Drawing3D D3D;
        /// <summary>
        /// 基準を画像中心にするか（回転時に画像位置がずれない　falseの場合は左上）
        /// </summary>
        public bool CenterBased = true;
        /// <summary>
        /// 描画位置をずらす
        /// </summary>
        public Vector2 Camera { get { return drawAbs ? Vector2.Zero : camera + quakeVector; } set { camera = new Vector2((float)Math.Floor(value.X), (float)Math.Floor(value.Y)); } }
        Vector2 camera;
        /// <summary>
        /// アニメーションを動かすか　使うのはアニメーション側
        /// </summary>
        public bool Animate;
        /// <summary>
        /// 描画色　一部関数には無効
        /// </summary>
        public Color Color = Color.White;
        /// <summary>
        /// 描画倍率　値を変更するとウィンドウのサイズを変えることが可能
        /// </summary>
        public int DrawStyle {
            get { return drawStyle; }
            set {
                drawStyle = value;
                game.ChangeWindowSize(drawStyle);
            }
        }
        int drawStyle = 1;
        /// <summary>
        /// 反転フラグ
        /// </summary>
        public SpriteEffects Flip = SpriteEffects.None;
        /// <summary>
        /// 絶対位置描画（カメラ無視）フラグ
        /// </summary>
        bool drawAbs = false;
        Game1 game;
        int quakeTime;
        double quakeVolume;
        Vector2 quakeVector;
        public Drawing(SpriteBatch batch, Drawing3D d3d, Game1 g) {
            sb = batch;
            D3D = d3d;
            game = g;
        }
        /// <summary>
        /// 画面を揺らす
        /// </summary>
        /// <param name="time">持続時間</param>
        /// <param name="volume">揺れの大きさ</param>
        public void SetQuake(int time, double volume) {
            quakeTime = time + 1;
            quakeVolume = volume;
        }
        public void Update() {
            if(quakeTime > 0) {
                quakeTime--;
                quakeVolume *= 0.99f;
                quakeVector = Vector.GetFromAngleLength(Function.GetRandomDouble(360), Function.GetRandomDouble(0, quakeVolume));
            } else quakeVector = new Vector();
        }
        /// <summary>
        /// カメラ無視描画
        /// </summary>
        public void SetDrawAbsolute() { if(drawAbs) return; drawAbs = true; CenterBased = false; }
        /// <summary>
        /// カメラ有効描画
        /// </summary>
        public void SetDrawNormal() { if(!drawAbs) return; drawAbs = false; CenterBased = true; }
        /// <summary>
        /// 画像を描画
        /// </summary>
        /// <param name="pos">中心点</param>
        /// <param name="tex">描画する画像</param>
        /// <param name="size">画像の倍率</param>
        /// <param name="depth">画像の表示優先度（小さいほうが優先される）</param>
        /// <param name="angle">画像の回転角度</param>
        public void Draw(Vector2 pos, Texture2D tex, Depth depth, float size = 1, float angle = 0) {
            sb.Draw(tex, GetDrawPos(pos), null, Color, angle, CenterBased ? new Vector2(tex.Width, tex.Height) / 2 : Vector2.Zero, size, Flip, depth.Value);
        }
        public void Draw(Vector2 pos, Texture2D tex, Depth depth, Vector origin, float size = 1, float angle = 0) {
            sb.Draw(tex, GetDrawPos(pos), null, Color, angle, origin, size, Flip, depth.Value);
        }
        /// <summary>
        /// 画像を描画
        /// </summary>
        /// <param name="pos">中心点</param>
        /// <param name="tex">描画する画像</param>
        /// <param name="size">画像の倍率</param>
        /// <param name="depth">画像の表示優先度（小さいほうが優先される）</param>
        /// <param name="angle">画像の回転角度</param>
        public void Draw(Vector2 pos, Texture2D tex, Rectangle rect, Depth depth, float size = 1, float angle = 0) {
            sb.Draw(tex, GetDrawPos(pos), rect, Color, angle, CenterBased ? new Vector2(rect.Width, rect.Height) / 2 : Vector2.Zero, size, Flip, depth.Value);
        }
        /// <summary>
        /// 画像を描画 倍率ベクトル版
        /// </summary>
        /// <param name="pos">中心点</param>
        /// <param name="tex">描画する画像</param>
        /// <param name="size">画像の拡大ベクトル</param>
        /// <param name="depth">画像の表示優先度（小さいほうが優先される）</param>
        /// <param name="angle">画像の回転角度</param>
        public void Draw(Vector2 pos, Texture2D tex, Depth depth, Vector2 size, float angle = 0) {
            sb.Draw(tex, GetDrawPos(pos), null, Color, angle, CenterBased ? new Vector2(tex.Height, tex.Width) / 2 : Vector2.Zero, size, Flip, depth.Value);
        }
        public void Draw(Vector2 pos, Texture2D tex, Rectangle rect, Depth depth, Vector2 size, float angle = 0) {
            sb.Draw(tex, GetDrawPos(pos), rect, Color, angle, CenterBased ? new Vector2(rect.Height, rect.Width) / 2 : Vector2.Zero, size, Flip, depth.Value);
        }
        /// <summary>
        /// 文字を描画
        /// </summary>
        /// <param name="pos">位置</param>
        /// <param name="font">フォント</param>
        /// <param name="text">表示する文字</param>
        /// <param name="color">色</param>
        /// <param name="size">文字の拡大倍率</param>
        /// <param name="depth">文字の表示優先度（小さいほうが優先される）</param>
        public void DrawText(Vector2 pos, SpriteFont font, string text, Color color, Depth depth, float size = 1, float angle = 0) {
            sb.DrawString(font, text, GetDrawPos(pos), color, angle, CenterBased ? font.MeasureString(text) / 2 : Vector2.Zero, size, Flip, depth.Value);
        }
        /// <summary>
        /// 文字を描画
        /// </summary>
        /// <param name="pos">位置</param>
        /// <param name="font">フォント</param>
        /// <param name="text">表示する文字</param>
        /// <param name="color">色</param>
        /// <param name="size">文字の拡大ベクトル</param>
        /// <param name="depth">文字の表示優先度（小さいほうが優先される）</param>
        public void DrawText(Vector2 pos, SpriteFont font, string text, Color color, Depth depth, Vector2 size, float angle = 0) {
            sb.DrawString(font, text, GetDrawPos(pos), color, angle, CenterBased ? font.MeasureString(text) / 2 : Vector2.Zero, size, Flip, depth.Value);
        }
        /// <summary>
        /// 中が塗りつぶされた長方形を描画
        /// </summary>
        /// <param name="pos">位置</param>
        /// <param name="size">大きさ</param>
        /// <param name="color">色</param>
        /// <param name="depth">表示優先度（小さいほうが優先される）</param>
        public void DrawBox(Vector2 pos, Vector2 size, Color color, Depth depth, float angle = 0) {
            sb.Draw(TextureManager.GetTexture(TextureID.White), GetDrawPos(pos), null, color, angle, CenterBased ? new Vector2(1, 1) / 2 : Vector2.Zero, size, Flip, depth.Value);
        }
        /// <summary>
        /// pos1とpos2をつなぐ中が塗りつぶされた直線を描画
        /// </summary>
        /// <param name="pos1">位置1</param>
        /// <param name="pos2">位置2</param>
        /// <param name="width">線の太さ</param>
        /// <param name="color">色</param>
        /// <param name="depth">表示優先度（小さいほうが優先される）</param>
        public void DrawLine(Vector2 pos1, Vector2 pos2, float width, Color color, Depth depth) {
            sb.Draw(TextureManager.GetTexture(TextureID.White), GetDrawPos(pos1), null, color, (float)Math.Atan2(pos2.Y - pos1.Y, pos2.X - pos1.X), Vector2.Zero, new Vector2((pos2 - pos1).Length(), width), Flip, depth.Value);
        }
        /// <summary>
        /// pos中心の粗末な疑似円を描画
        /// </summary>
        /// <param name="pos">位置</param>
        /// <param name="range">半径</param>
        /// <param name="width">線の太さ</param>
        /// <param name="split">分割数</param>
        /// <param name="color">色</param>
        /// <param name="depth">表示優先度（小さいほうが優先される）</param>
        public void DrawCircle(Vector pos, double range, float width, int split, Color color, Depth depth) {
            double deg = 360.0 / split;
            Vector p1 = pos + Vector.GetFromAngleLength(0, range);
            Vector p2 = pos + Vector.GetFromAngleLength(deg, range);
            for(int i = 0; i < split; i++) {
                DrawLine(p1, p2, width, color, depth);
                p1 = p2;
                p2 = pos + Vector.GetFromAngleLength(deg * (i + 2), range);
            }
        }
        /// <summary>
        /// 中が塗りつぶされていない長方形を描画（回転の動作未確認）
        /// </summary>
        /// <param name="pos">位置</param>
        /// <param name="size">大きさ</param>
        /// <param name="color">色</param>
        /// <param name="depth">表示優先度（小さいほうが優先される）</param>
        public void DrawBoxFrame(Vector2 pos, Vector2 size, Color color, Depth depth, float angle = 0) {
            float s = (float)Math.Sin(angle);
            float c = (float)Math.Cos(angle);
            Vector2 px = new Vector2(c, s) * size.X;
            Vector2 py = new Vector2(-s, c) * size.Y;
            if(CenterBased) {
                DrawBox(pos - py / 2, new Vector2(size.X, 1), color, depth, angle);
                DrawBox(pos - px / 2, new Vector2(1, size.Y), color, depth, angle);
                DrawBox(pos + py / 2, new Vector2(size.X, 1), color, depth, angle);
                DrawBox(pos + px / 2, new Vector2(1, size.Y), color, depth, angle);
            } else {
                DrawBox(pos, new Vector2(size.X, 1), color, depth, angle);
                DrawBox(pos, new Vector2(1, size.Y), color, depth, angle);
                DrawBox(pos + py, new Vector2(size.X, 1), color, depth, angle);
                DrawBox(pos + px, new Vector2(1, size.Y), color, depth, angle);
            }
        }
        /// <summary>
        /// 中が塗りつぶされていない長方形を描画（回転の動作未確認）
        /// </summary>
        /// <param name="pos">位置</param>
        /// <param name="size">大きさ</param>
        /// <param name="size">幅</param>
        /// <param name="color">色</param>
        /// <param name="depth">表示優先度（小さいほうが優先される）</param>
        public void DrawBoxFrame(Vector2 pos, Vector2 size, int width, Color color, Depth depth, float angle = 0) {
            float s = (float)Math.Sin(angle);
            float c = (float)Math.Cos(angle);
            Vector2 px = new Vector2(c, s) * size.X;
            Vector2 py = new Vector2(-s, c) * size.Y;
            if(CenterBased) {
                DrawBox(pos - py / 2, new Vector2(size.X + width, width), color, depth, angle);
                DrawBox(pos - px / 2, new Vector2(width, size.Y + width), color, depth, angle);
                DrawBox(pos + py / 2, new Vector2(size.X + width, width), color, depth, angle);
                DrawBox(pos + px / 2, new Vector2(width, size.Y + width), color, depth, angle);
            } else {
                DrawBox(pos, new Vector2(size.X + width, width), color, depth, angle);
                DrawBox(pos, new Vector2(width, size.Y + width), color, depth, angle);
                DrawBox(pos + py, new Vector2(size.X + width, width), color, depth, angle);
                DrawBox(pos + px, new Vector2(width, size.Y + width), color, depth, angle);
            }
        }
        public void DrawBegin() {
            Depth.DepthReset();
            //3D用描画範囲
            D3D.SetViewport(new Viewport(0, 0, Game1._WindowSizeX, Game1._WindowSizeY));
            //ここの引数を変えれば加算描画や拡大方法の変更が可能
            sb.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);
        }
        public void DrawEnd() {
            //2D用描画範囲
            D3D.SetViewport(new Viewport(0, 0,Game1._WindowSizeX, Game1._WindowSizeY));
            //実際に2Dが描かれるのはここ
            sb.End();
        }
        Vector2 GetDrawPos(Vector2 v) { return new Vector2((int)((v.X - Camera.X)), (int)((v.Y - Camera.Y))); }
    }

    /// <summary>
    /// 描画深度を管理します
    /// 同じ描画深度IDでも、描画順によって微妙に値を変えてあとに呼ばれた描画の方を上にすることで、
    /// 描画順序を一定化して画面がちらつかないように＋自然に書けるように
    /// </summary>
    class Depth {
        /// <summary>
        /// 同描画深度許容数
        /// </summary>
        //（0x3f800000 / sameDepth）> DepthIDの数になるように設定してください
        const int sameDepth = 0x100000;

        /// <summary>
        /// 利用済みdepthの回数の一覧
        /// </summary>
        static int[] depthUsed;
        /// <summary>
        /// DepthIDの長さ
        /// </summary>
        static readonly int depthNum = Enum.GetNames(typeof(DepthID)).Length;
        public float Value { get; private set; }
        private Depth(float v) {
            Value = v;
        }
        /// <summary>
        /// IDをDepthとして利用するための物
        /// </summary>
        public static implicit operator Depth(DepthID id) {
            return new Depth(ConvertInt((int)id * sameDepth + depthUsed[(int)id]++));
        }
        /// <summary>
        /// Depth利用数のリセット
        /// 描画前に呼ぶ
        /// </summary>
        public static void DepthReset() {
            depthUsed = new int[depthNum];
        }
        /// <summary>
        /// int（0 ~ 1065353216）から順序を逆にしたfloat(0 ~ 1)へ変換
        /// </summary>
        static float ConvertInt(int d) {
            int c = 0x3f800000 - (int)d;
            var bit = BitConverter.GetBytes(c);
            return BitConverter.ToSingle(bit, 0);
        }
    }
    /// <summary>
    /// 描画深度を管理するためのIDです（順番のみ　後ろの方が優先度高）
    /// </summary>
    enum DepthID {
        BackGroundFloor, BackGroundWall, Item, StateBack, Enemy, Player, StateFront,
        Message, Effect, Status, Map, Fade, Debug
    }
}
