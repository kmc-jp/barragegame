using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace CommonPart {
    /// <summary>
    /// 画像を表示するだけのWorker
    /// </summary>
    class Picture: WorkerWithPos {
        #region 変数
        /// <summary>
        /// 画像
        /// </summary>
        public Image Image;
        /// <summary>
        /// 消去時間
        /// </summary>
        protected int Time;
        /// <summary>
        /// 描画深度
        /// </summary>
        DepthID depth;
        /// <summary>
        /// 移動速度
        /// </summary>
        public Vector Speed;
        /// <summary>
        /// 拡大率
        /// </summary>
        public float Size = 1;
        /// <summary>
        /// 回転角(rad)
        /// </summary>
        public float Angle;
        /// <summary>
        /// 中心基準か
        /// </summary>
        public bool CenterBased;
        /// <summary>
        /// Workerのタイプ
        /// </summary>
        public override WorkerType Type { get { return WorkerType.Picture; } }
        #endregion
        #region 関数
        /// <summary>
        /// 画像表示だけのクラスを作成
        /// </summary>
        /// <param name="w">WorkerManager</param>
        /// <param name="pos">位置</param>
        /// <param name="img">表示する画像</param>
        /// <param name="t">継続時間（無入力でたぶん無限）</param>
        public Picture(WorkerManager w, Vector pos, Image img, DepthID depth, int t = -1)
            : base(w, pos) {
            Image = img;
            Time = t;
            this.depth = depth;
        }
        public override void Update() { if(Frame == Time) Delete = true; Pos += Speed; }
        public override void Draw(Drawing d) {
            bool cb = d.CenterBased;
            d.CenterBased = CenterBased;
            Image.Draw(d, Pos, depth, Size, Angle);
            d.CenterBased = cb;
        }
        #endregion
    }
    /// <summary>
    /// 画像を汎用的に扱うための基底クラス
    /// </summary>
    abstract class Image {
        #region 変数
        /// <summary>
        /// X方向の大きさ
        /// </summary>
        public abstract float X { get; }
        /// <summary>
        /// Y方向の大きさ
        /// </summary>
        public abstract float Y { get; }
        /// <summary>
        /// 大きさ
        /// </summary>
        public Vector2 Size { get { return new Vector2(X, Y); } }
        #endregion
        #region 関数
        /// <summary>
        /// 描画（簡易）
        /// </summary>
        /// <param name="d">Drawing</param>
        /// <param name="pos">位置</param>
        public abstract void Draw(Drawing d, Vector2 pos, DepthID depth, float size = 1, float angle = 0);
        public abstract void Draw(Drawing d, Vector2 pos, DepthID depth, Vector2 size, float angle = 0);
        public virtual double GetDefaultHitSize() {
            return 0;
        }
        #endregion
    }
    /// <summary>
    /// 複数のImageを一つのImageとして扱うためのクラス
    /// </summary>
    class ImageCluster: Image {
        #region 変数
        /// <summary>
        /// X方向の大きさ
        /// </summary>
        public override float X { get { return image.Max(img => img.X); } }
        /// <summary>
        /// Y方向の大きさ
        /// </summary>
        public override float Y { get { return image.Max(img => img.Y); } }
        /// <summary>
        /// 表示するImage
        /// </summary>
        List<Image> image;
        #endregion
        #region 関数
        public ImageCluster(List<Image> img) {
            image = img;
        }
        public ImageCluster(params Image[] img) {
            image = img.ToList();
        }
        public override void Draw(Drawing d, Vector2 pos, DepthID depth, float size = 1, float angle = 0) {
            foreach(Image img in image) img.Draw(d, pos, depth, size, angle);
        }
        public override void Draw(Drawing d, Vector2 pos, DepthID depth, Vector2 size, float angle = 0) {
            foreach(Image img in image) img.Draw(d, pos, depth, size, angle);
        }
        #endregion
    }
    /// <summary>
    /// ただのTextureを扱うImage
    /// </summary>
    class SimpleTexture: Image {
        #region 変数
        /// <summary>
        /// X方向の大きさ
        /// </summary>
        public override float X { get { return texture.Width; } }
        /// <summary>
        /// Y方向の大きさ
        /// </summary>
        public override float Y { get { return texture.Height; } }
        /// <summary>
        /// 表示するTexture
        /// </summary>
        Texture2D texture;

        public readonly TextureID ID;
        #endregion
        #region 関数
        SimpleTexture(Texture2D tex) {
            texture = tex;
        }
        public SimpleTexture(TextureID id) : this(TextureManager.GetTexture(id)) { ID = id; }
        public static SimpleTexture CreateFromTexture2D(Texture2D t) { return new SimpleTexture(t); }
        public override void Draw(Drawing d, Vector2 pos, DepthID depth, float size = 1, float angle = 0) {
            d.Draw(pos, texture, depth, size, angle);
        }
        public override void Draw(Drawing d, Vector2 pos, DepthID depth, Vector2 size, float angle = 0) {
            d.Draw(pos, texture, depth, size, angle);
        }
        public override double GetDefaultHitSize() {
            return ID.GetDefaultHitSize();
        }
        #endregion
    }
    /// <summary>
    /// Textureの一部分なImage
    /// </summary>
    class TrimmedTexture: Image {
        #region 変数
        /// <summary>
        /// X方向の大きさ
        /// </summary>
        public override float X { get { return rectangle.Width; } }
        /// <summary>
        /// Y方向の大きさ
        /// </summary>
        public override float Y { get { return rectangle.Height; } }
        /// <summary>
        /// 表示するTexture
        /// </summary>
        Texture2D texture;
        /// <summary>
        /// 描画部分
        /// </summary>
        Rectangle rectangle;

        public readonly TextureID ID;
        #endregion
        #region 関数
        TrimmedTexture(Texture2D tex, Rectangle rect) {
            texture = tex;
            rectangle = rect;
        }
        public TrimmedTexture(TextureID id, Rectangle rect) : this(TextureManager.GetTexture(id), rect) { ID = id; }
        TrimmedTexture(Texture2D tex, int x, int y, int xNum, int yNum) {
            texture = tex;
            int w = tex.Width / xNum;
            int h = tex.Height / yNum;
            rectangle = new Rectangle(x * w, y * h, w, h);
        }
        public TrimmedTexture(TextureID id, int x, int y, int xNum, int yNum) : this(TextureManager.GetTexture(id), x, y, xNum, yNum) { ID = id; }
        public override void Draw(Drawing d, Vector2 pos, DepthID depth, float size = 1, float angle = 0) {
            d.Draw(pos, texture, rectangle, depth, size, angle);
        }
        public override void Draw(Drawing d, Vector2 pos, DepthID depth, Vector2 size, float angle = 0) {
            d.Draw(pos, texture, rectangle, depth, size, angle);
        }
        public override double GetDefaultHitSize() {
            return ID.GetDefaultHitSize();
        }
        #endregion
    }
    /// <summary>
    /// アニメーションを管理するImage
    /// </summary>
    class Animation: Image {
        public override float X { get { return data.X; } }
        public override float Y { get { return data.Y; } }
        AnimationData data;
        int frame;
        const bool animateWithUpdate = true;
        public Animation(AnimationData d) {
            data = d;
            //data = d.GetAnimationData();
        }
        public void Update() {
            if(animateWithUpdate) frame++;
        }
        public override void Draw(Drawing d, Vector2 pos, DepthID depth, float size = 1, float angle = 0) {
            data.Draw(frame, d, pos, depth, size, angle);
            if(!animateWithUpdate && d.Animate)
                frame++;
        }
        public override void Draw(Drawing d, Vector2 pos, DepthID depth, Vector2 size, float angle = 0) {
            data.Draw(frame, d, pos, depth, size, angle);
            if(d.Animate)
                frame++;
        }
    }
    /// <summary>
    /// アニメーションのデータを管理するクラス
    /// </summary>
    abstract class AnimationData {
        public abstract int X { get; }
        public abstract int Y { get; }
        public abstract void Draw(int f, Drawing d, Vector2 pos, DepthID depth, float size = 1, float angle = 0);
        public abstract void Draw(int f, Drawing d, Vector2 pos, DepthID depth, Vector2 size, float angle = 0);
    }
    /// <summary>
    /// 複数画像版AnimationData
    /// </summary>
    class MultiTextureAnimationData: AnimationData {
        public override int X { get { return width; } }
        public override int Y { get { return height; } }
        readonly Texture2D[] textures;
        readonly int frame;
        readonly int number;
        readonly int width;
        readonly int height;
        public MultiTextureAnimationData(int frame, params TextureID[] t) {
            this.frame = frame;
            number = t.Length;
            textures = new Texture2D[number];
            for(int i = 0; i < number; i++)
                textures[i] = TextureManager.GetTexture(t[i]);
            width = textures[0].Width;
            height = textures[0].Height;
        }
        public override void Draw(int f, Drawing d, Vector2 pos, DepthID depth, float size = 1, float angle = 0) {
            d.Draw(pos, textures[f / frame % number], depth, size, angle);
        }
        public override void Draw(int f, Drawing d, Vector2 pos, DepthID depth, Vector2 size, float angle = 0) {
            d.Draw(pos, textures[f / frame % number], depth, size, angle);
        }
    }
    /// <summary>
    /// 1画像分割版AnimationData
    /// </summary>
    class SingleTextureAnimationData: AnimationData {
        public override int X { get { return width; } }
        public override int Y { get { return height; } }
        readonly Texture2D texture;
        readonly int frame;
        readonly int xNum;
        readonly int yNum;
        readonly int width;
        readonly int height;
        readonly int number;
        public SingleTextureAnimationData(int frame, TextureID t, int xNum, int yNum) {
            this.frame = frame;
            texture = TextureManager.GetTexture(t);
            this.xNum = xNum;
            this.yNum = yNum;
            number = xNum * yNum;
            width = texture.Width / xNum;
            height = texture.Height / yNum;
        }
        public override void Draw(int f, Drawing d, Vector2 pos, DepthID depth, float size = 1, float angle = 0) {
            int x = f / frame % number;
            d.Draw(pos, texture, new Rectangle(x % xNum * width, x / xNum * height, width, height), depth, size, angle);
        }
        public override void Draw(int f, Drawing d, Vector2 pos, DepthID depth, Vector2 size, float angle = 0) {
            int x = f / frame % number;
            d.Draw(pos, texture, new Rectangle(x % xNum * width, x / xNum * height, width, height), depth, size, angle);
        }

    }
    /// <summary>
    /// 1画像分割版AnimationData
    /// </summary>
    class SingleTextureAnimationDataWithOrder: AnimationData {
        public override int X { get { return width; } }
        public override int Y { get { return height; } }
        readonly Texture2D texture;
        readonly int frame;
        readonly int xNum;
        readonly int yNum;
        readonly int width;
        readonly int height;
        readonly int number;
        readonly int[] order;
        public SingleTextureAnimationDataWithOrder(int frame, TextureID t, int xNum, int yNum, int[] order) {
            this.frame = frame;
            texture = TextureManager.GetTexture(t);
            this.xNum = xNum;
            this.yNum = yNum;
            this.order = order;
            number = order.Length;
            width = texture.Width / xNum;
            height = texture.Height / yNum;
        }
        public override void Draw(int f, Drawing d, Vector2 pos, DepthID depth, float size = 1, float angle = 0) {
            int x = order[f / frame % number];
            d.Draw(pos, texture, new Rectangle(x % xNum * width, x / xNum * height, width, height), depth, size, angle);
        }
        public override void Draw(int f, Drawing d, Vector2 pos, DepthID depth, Vector2 size, float angle = 0) {
            int x = f / frame % number;
            d.Draw(pos, texture, new Rectangle(x % xNum * width, x / xNum * height, width, height), depth, size, angle);
        }
    }
    /// <summary>
    /// 1画像分割版AnimationData
    /// </summary>
    class SingleTextureAnimationDataWithFrames: AnimationData {
        public override int X { get { return width; } }
        public override int Y { get { return height; } }
        readonly Texture2D texture;
        readonly int[] frame;
        readonly int xNum;
        readonly int yNum;
        readonly int width;
        readonly int height;
        readonly int totalFrame;
        public SingleTextureAnimationDataWithFrames(int[] frame, TextureID t, int xNum, int yNum) {
            this.frame = frame;
            texture = TextureManager.GetTexture(t);
            this.xNum = xNum;
            this.yNum = yNum;
            totalFrame = frame.Sum();
            width = texture.Width / xNum;
            height = texture.Height / yNum;
        }
        public override void Draw(int f, Drawing d, Vector2 pos, DepthID depth, float size = 1, float angle = 0) {
            int t = f % totalFrame;
            int x = 0;
            for(int s = frame[0]; s < t; s += frame[++x]) ;
            d.Draw(pos, texture, new Rectangle(x % xNum * width, x / xNum * height, width, height), depth, size, angle);
        }
        public override void Draw(int f, Drawing d, Vector2 pos, DepthID depth, Vector2 size, float angle = 0) {
            int t = f % totalFrame;
            int x = 0;
            for(int s = frame[0]; s < f; s += frame[++x]) ;
            d.Draw(pos, texture, new Rectangle(x % xNum * width, x / xNum * height, width, height), depth, size, angle);
        }
    }
    /// <summary>
    /// テキストとフォントを扱うImage
    /// </summary>
    /*class TextAndFont: Image {
        #region 変数
        /// <summary>
        /// X方向の大きさ
        /// </summary>
        public override float X { get { return font.MeasureString(str).X; } }
        /// <summary>
        /// Y方向の大きさ
        /// </summary>
        public override float Y { get { return font.MeasureString(str).Y; } }
        /// <summary>
        /// 描画する文字列
        /// </summary>
        string str;
        /// <summary>
        /// 描画用フォント
        /// </summary>
        SpriteFont font;
        /// <summary>
        /// 描画色
        /// </summary>
        Color color;
        Color baseColor;
        /// <summary>
        /// フォントを省略するとこれになる
        /// </summary>
        const FontID defaultFont = FontID.Test;
        #endregion
        #region 関数
        public TextAndFont(string mes) : this(mes, defaultFont, Color.White) { }
        public TextAndFont(string mes, FontID fontid) : this(mes, fontid, Color.White) { }
        public TextAndFont(string mes, FontID fontid, Color c) : this(mes, TextureManager.GetFont(fontid), c) { }
        public TextAndFont(string mes, Color c) : this(mes, defaultFont, c) { }
        public TextAndFont(string mes, SpriteFont f, Color c) {
            str = mes;
            font = f;
            color = baseColor = c;
        }
        /// <summary>
        /// フォント色を変更する
        /// </summary>
        public void ChangeColor(Color c) {
            color = baseColor = c;
        }
        /// <summary>
        /// フォント色を保ちつつ透明度を変更する
        /// </summary>
        public void ChangeAlpha(float f) {
            color = baseColor * f;
        }
        public override void Draw(Drawing d, Vector2 pos, DepthID depth, float size = 1, float angle = 0) {
            d.DrawText(pos, font, str, color, depth, size, angle);
        }
        public override void Draw(Drawing d, Vector2 pos, DepthID depth, Vector2 size, float angle = 0) {
            d.DrawText(pos, font, str, color, depth, size, angle);
        }
        #endregion
    }*/
    /// <summary>
    /// 1文字毎に別描画なImage
    /// </summary>
    class RichText: Image {
        /* 特殊文字一覧
         * #(6桁の16進数)　文字色変更
         * #_(8桁の16進数)　文字色変更（透明度も指定）
         * #- デフォルト文字色に変更
         * #? 数字の色固定解除(色変更のたびにリセット)
         * &(3桁の10進数)　文字サイズ変更
         * \n　改行
         * $type[arg1,arg2...]　機能拡張用
         * #, &, $ を描画する場合はそれぞれ重ねてください
         */
        #region 変数
        /// <summary>
        /// X方向の大きさ
        /// </summary>
        public override float X {
            get {
                if(_x == -1) SetSize();
                return _x;
            }
        }
        float _x = -1;
        /// <summary>
        /// Y方向の大きさ
        /// </summary>
        public override float Y {
            get {
                if(_y == -1) SetSize();
                return _y;
            }
        }
        float _y = -1;

        readonly int length;
        /// <summary>
        /// 描画する文字列
        /// </summary>
        readonly string str;
        /// <summary>
        /// 描画用フォント
        /// </summary>
        SpriteFont font;
        /// <summary>
        /// 強制等幅化
        /// </summary>
        bool sameWidth;
        /// <summary>
        /// デフォルト色
        /// </summary>
        Color colorBase = Color.White;
        /// <summary>
        /// 影付き
        /// </summary>
        public Color? Shadow = Color.Black;
        /// <summary>
        /// 数字の色固定
        /// </summary>
        public bool Numeric = true;
        /// <summary>
        /// 数字の色固定(テキスト側での指定)
        /// </summary>
        public bool NumericT = true;
        /// <summary>
        /// フォントを省略するとこれになる
        /// </summary>
        const FontID defaultFont = FontID.Medium;

        int defaultSize;
        int fx, fy;
        byte alpha;
        #endregion
        #region 関数
        public RichText(string mes, bool same = true) : this(mes, defaultFont, same) { }
        public RichText(string mes, FontID fontid, bool same = true) {
            str = mes;
            sameWidth = same;
            font = TextureManager.GetFont(fontid);
            fx = defaultSize = fontid.GetDefaultFontSizeX();
            fy = fontid.GetDefaultFontSizeY();
            length = str.Length;
        }
        public RichText(string mes, FontID fontid, Color c, bool same = true) :this(mes, fontid, same) {
            colorBase = c;
        }
        /// <summary>
        /// サイズを計算します
        /// </summary>
        void SetSize() {
            Draw(null, new Vector2(), DepthID.Debug);
        }
        /// <summary>
        /// 数字に色を付けないようにする
        /// </summary>
        public RichText NoNum() {
            Numeric = false;
            return this;
        }
        /// <summary>
        /// 都合上angleは機能しません
        /// </summary>
        public override void Draw(Drawing d, Vector2 pos, DepthID depth, float size = 1, float angle = 0) {
            Draw(d, pos, -1, depth, size, angle);
        }
        /// <summary>
        /// dがnullの時はダミーの描画を呼びます（サイズの計算用）
        /// </summary>
        public void Draw(Drawing d, Vector2 pos, int part, DepthID depth, float size = 1, float angle = 0) {
            float px = 0;
            float py = 0;
            Color color = colorBase;
            alpha = color.A;
            float xsize = 1;
            int dummy;
            for(int i = 0; i < length; i++) {
                //特殊文字のチェック
                if(str[i] == '#') {
                    if(str[i + 1] == '_') {
                        int r = int.Parse(str.Substring(i + 2, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
                        int g = int.Parse(str.Substring(i + 4, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
                        int b = int.Parse(str.Substring(i + 6, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
                        int a = int.Parse(str.Substring(i + 8, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
                        NumericT = true;
                        i += 9;
                        color = new Color(r, g, b) * (a / 255f);
                        alpha = (byte)a;
                    } else if(str[i + 1] == '-') {
                        color = colorBase;
                        NumericT = true;
                        i += 1;
                    } else if(str[i + 1] == '?') {
                        NumericT = false;
                        i += 1;
                    } else if(str[i + 1] == '#') {
                        i += 1;
                        goto drawchar;
                    } else {
                        int r = int.Parse(str.Substring(i + 1, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
                        int g = int.Parse(str.Substring(i + 3, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
                        int b = int.Parse(str.Substring(i + 5, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
                        NumericT = true;
                        i += 6;
                        color = new Color(r, g, b) * (alpha / 255f);
                    }
                    continue;
                }
                if(str[i] == '&') {
                    if(str[i + 1] == '&') {
                        i += 1;
                        goto drawchar;
                    }
                    int s = int.Parse(str.Substring(i + 1, 3));
                    i += 3;
                    xsize = s / 100f;
                    continue;
                }
                if(str[i] == '$') {
                    if(str[i + 1] == '$') {
                        i += 1;
                        goto drawchar;
                    }
                    string type = str.Substring(i);
                    int l = type.IndexOf(']');
                    if(l == -1) continue;
                    type = type.Substring(0, l);
                    int index = type.IndexOf('[');
                    if(index == -1) continue;
                    string[] args = new string[1]{ type.Substring(index + 1) };
                    type = type.Substring(0, index);
                    args = args[0].Split(',');
                    i += l;

                    //ここに処理を追加する　$type[args]の形

                    continue;
                }
                if(str[i] == '\n') {
                    if(_x < px) _x = px;
                    px = 0;
                    py += fy * size;
                    continue;
                }
drawchar:
                string x = str[i].ToString();
                float dy = (1 - xsize) * size * fy;
                if(d != null) {
                    if(Shadow != null)
                        d.DrawText(pos + new Vector2(px + 1, py + 1 + dy), font, x, (Color)Shadow, depth, xsize * size);
                    d.DrawText(pos + new Vector2(px, py + dy), font, x, Numeric && NumericT && (int.TryParse(x, out dummy)) ? Color.Yellow * (alpha / 255f) : color, depth, xsize * size);
                }
                if(sameWidth)
                    px += fx * xsize * size * System.Text.Encoding.GetEncoding("Shift_JIS").GetByteCount(x);
                else
                    px += font.MeasureString(x).X * xsize * size;
                if(part == 0) return;
                part--;
            }
            if(_x < px) _x = px;
            _y = py + fy * size;
        }
        /// <summary>
        /// 与えられた文字列は半角何文字分の大きさを持つか
        /// </summary>
        public static int Length(string s) {
            //$による拡張分はここで補正
            s = System.Text.RegularExpressions.Regex.Replace(s, "\\$.+?\\[.*?\\]", "");
            s = System.Text.RegularExpressions.Regex.Replace(s, "#(-|_[0-9a-fA-F]{8}|[0-9a-fA-F]{6})", "");
            return System.Text.Encoding.GetEncoding("Shift_JIS").GetByteCount(s) - s.Count(x => x == '&') * 4;
        }
        public static string ColorToString(Color c) { return String.Format("#{0:x2}{1:x2}{2:x2}", c.R, c.G, c.B); }
        public static string ColorToStringWithAlpha(Color c) { return String.Format("#_{0:x2}{1:x2}{2:x2}{3:x2}", c.R, c.G, c.B, c.A); }
        public override void Draw(Drawing d, Vector2 pos, DepthID depth, Vector2 size, float angle = 0) {
            //未対応
            Draw(d, pos, depth);
        }
        #endregion
    }
    /// <summary>
    /// 中が塗りつぶされた長方形を扱うImage
    /// </summary>
    class FilledBox: Image {
        #region 変数
        /// <summary>
        /// X方向の大きさ
        /// </summary>
        public override float X { get { return size.X; } }
        /// <summary>
        /// Y方向の大きさ
        /// </summary>
        public override float Y { get { return size.Y; } }
        /// <summary>
        /// サイズ
        /// </summary>
        Vector2 size;
        /// <summary>
        /// 色
        /// </summary>
        Color color;
        #endregion
        #region 関数
        public FilledBox(Vector2 size, Color color) {
            this.size = size;
            this.color = color;
        }
        public override void Draw(Drawing d, Vector2 pos, DepthID depth, float size = 1, float angle = 0) {
            d.DrawBox(pos, this.size * size, color, depth, angle);
        }
        public override void Draw(Drawing d, Vector2 pos, DepthID depth, Vector2 size, float angle = 0) {
            d.DrawBox(pos, this.size * size, color, depth, angle);
        }
        #endregion
    }
    /// <summary>
    /// 中が塗りつぶされていない長方形を扱うImage
    /// </summary>
    class BoxFrame: Image {
        #region 変数
        /// <summary>
        /// X方向の大きさ
        /// </summary>
        public override float X { get { return size.X; } }
        /// <summary>
        /// Y方向の大きさ
        /// </summary>
        public override float Y { get { return size.Y; } }
        /// <summary>
        /// サイズ
        /// </summary>
        Vector2 size;
        /// <summary>
        /// 色
        /// </summary>
        Color color;
        #endregion
        #region 関数
        public BoxFrame(Vector2 size, Color color) {
            this.size = size;
            this.color = color;
        }
        public override void Draw(Drawing d, Vector2 pos, DepthID depth, float size = 1, float angle = 0) {
            d.DrawBoxFrame(pos, this.size * size, color, depth, angle);
        }
        public override void Draw(Drawing d, Vector2 pos, DepthID depth, Vector2 size, float angle = 0) {
            d.DrawBoxFrame(pos, this.size * size, color, depth, angle);
        }
        #endregion
    }
    /// <summary>
    /// ゲージを扱うImage
    /// </summary>
    class Gauge: Image {
        #region 変数
        /// <summary>
        /// X方向の大きさ
        /// </summary>
        public override float X { get { return frame.X; } }
        /// <summary>
        /// Y方向の大きさ
        /// </summary>
        public override float Y { get { return frame.Y; } }
        /// <summary>
        /// 実態（枠）
        /// </summary>
        BoxFrame frame;
        /// <summary>
        /// 実態（中身）
        /// </summary>
        FilledBox bar;
        /// <summary>
        /// 実態（背景）
        /// </summary>
        FilledBox back;
        #endregion
        #region 関数
        public Gauge(Vector2 size, Color? color, int min, int max, int value, Color? backColor) {
            int lengthmax = (int)size.X - 1;
            int barlength = lengthmax * (value - min) / (max - min);
            if(barlength < 0) barlength = 0;
            if(barlength > lengthmax) barlength = lengthmax;
            Color c = color ?? new Color(Math.Min(Math.Max(255 + 192 - 192 * 4 * barlength / lengthmax, 63), 255)
                , Math.Min(Math.Max(255 + 192 - 192 * 4 * Math.Abs(barlength - (lengthmax / 2)) / lengthmax, 63), 255)
                , Math.Min(Math.Max(255 + 192 - 192 * 4 * (lengthmax - barlength) / lengthmax, 63), 255));
            frame = new BoxFrame(size, Color.White);
            if(backColor != null)
                back = new FilledBox(size, (Color)backColor);
            bar = new FilledBox(new Vector2(barlength, size.Y - 1), c);
        }
        public override void Draw(Drawing d, Vector2 pos, DepthID depth, float size = 1, float angle = 0) {
            if(back != null) back.Draw(d, pos, depth, size, angle);
            bar.Draw(d, pos + new Vector2(1, 1), depth, size, angle);
            frame.Draw(d, pos, depth, size, angle);
        }
        public override void Draw(Drawing d, Vector2 pos, DepthID depth, Vector2 size, float angle = 0) {
            if(back != null) back.Draw(d, pos, depth, size, angle);
            bar.Draw(d, pos + new Vector2(1, 1), depth, size, angle);
            frame.Draw(d, pos, depth, size, angle);
        }
        #endregion
    }

    /// <summary>
    /// テクスチャ付ゲージを扱うImage
    /// </summary>
    class TexturedGauge: Image {
        #region 変数
        /// <summary>
        /// X方向の大きさ
        /// </summary>
        public override float X { get { return _frame.X; } }
        /// <summary>
        /// Y方向の大きさ
        /// </summary>
        public override float Y { get { return _frame.Y; } }
        /// <summary>
        /// 実態（枠）
        /// </summary>
        SimpleTexture _frame;
        /// <summary>
        /// 実態（背景）
        /// </summary>
        SimpleTexture _back;
        /// <summary>
        /// 実態（中身）
        /// </summary>
        Texture2D bar;
        Vector2 offset;
        Vector2 offset2;
        int barlength;
        #endregion
        #region 関数
        public TexturedGauge(TextureID? frame, TextureID? back, TextureID front, Vector2 offset, int min, int max, int value, Vector2 offsetback = new Vector2()) {
            if(frame != null)
                _frame = new SimpleTexture((TextureID)frame);
            if(back != null)
                _back = new SimpleTexture((TextureID)back);
            bar = TextureManager.GetTexture(front);

            int lengthmax = bar.Width;
            barlength = lengthmax * (value - min) / (max - min);
            this.offset = offset;
            offset2 = offsetback;
            if(barlength < 0) barlength = 0;
            if(barlength > lengthmax) barlength = lengthmax;

        }
        public override void Draw(Drawing d, Vector2 pos, DepthID depth, float size = 1, float angle = 0) {
            if(_back != null)
                _back.Draw(d, pos + offset2 * size, depth, size, angle);
            d.Draw(pos + offset * size, bar, new Rectangle(0, 0, barlength, bar.Height), depth, size, angle);
            if(_frame != null)
                _frame.Draw(d, pos, depth, size, angle);
        }
        public override void Draw(Drawing d, Vector2 pos, DepthID depth, Vector2 size, float angle = 0) {
            //実装略
            Draw(d, pos, depth);
        }
        #endregion
    }
}
