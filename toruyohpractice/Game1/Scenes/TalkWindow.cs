using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace CommonPart {
    /// <summary>
    /// 会話メッセージを管理します
    /// </summary>
    class TalkWindow : Scene {
        List<MessageBase> showing;
        MessageManager mes;
        int index = 0;
        int length;

        public TalkWindow(SceneManager s, MessageManager message, params MessageBase[] md)
            : base(s) {
            showing = md.ToList();
            mes = message;
            length = md.Length;
            s.BackSceneNumber++;
        }

        public override void SceneUpdate() {
            //if(Input.GetKeyPressed(KeyID.Wait)) { new MessageHistory(scenem, mes, 2); return; }
            showing[index].Update(manager.Input, mes);
            if(showing[index].IsEnd) {
                index++;
                if(index == length) Delete = true;
            }
        }
        public override void SceneDraw(Drawing d) {
            showing[index].Draw(d);
        }

        public void AddMessage(params MessageBase[] md) {
            showing.AddRange(md);
            length = showing.Count;
        }
        public abstract class MessageBase {
            public bool IsEnd { get; protected set; }

            public abstract void Update(InputManager input, MessageManager message);
            public abstract void Draw(Drawing d);

            public abstract void Reset();
        }
        public static void DrawMessage(Drawing d, Image face, string name, RichText text) {
            d.SetDrawAbsolute();
            if(face != null) {
                //new SimpleTexture(TextureID.FaceBack).Draw(d, new Vector2(20, 246), DepthID.Message);
                face.Draw(d, new Vector2(-100, 0), DepthID.Message);
            }
            DrawMessageBack(d, new Vector2(600, 120), new Vector2(20, 356), DepthID.Message);
            //new SimpleTexture(TextureID.MessageBack).Draw(d, new Vector2(20, 350), DepthID.Message);
            if(name != null && name != "") {
                int leng = Math.Max(RichText.Length(name) * 10 + 64, 200);
                DrawMessageBack(d, new Vector2(leng, 30), new Vector2(20, 326), DepthID.Message);
                new RichText(name, FontID.Medium, Color.Yellow).Draw(d, new Vector2(50, 328), DepthID.Message);
            }
            text.Draw(d, new Vector2(40, 360), DepthID.Message);
            if(Settings.Explain != Settings.ExplainType.None) {
                new RichText("[" + KeyConfig.GetKeyString(KeyID.Select) + "]:進める・選ぶ ["
                    + KeyConfig.GetKeyString(KeyID.Cancel) + "]:メッセージ履歴 ["
                    + KeyConfig.GetKeyString(KeyID.Wait) + "]:スキップ", FontID.Medium).Draw(d, new Vector2(40, 454), DepthID.Message, 0.7f);
            }
            d.SetDrawNormal();
        }
        public static void DrawMessageBack(Drawing d, Vector2 size, Vector2 pos, DepthID id) {
            bool f = d.CenterBased;
            d.CenterBased = false;
            var tex = TextureManager.GetTexture(TextureID.MessageBack);
            if(tex == null) {
                new FilledBox(size, Color.FromNonPremultiplied(Settings.WindowR, Settings.WindowG, Settings.WindowB, Settings.WindowA)).Draw(d, pos, id);
                new BoxFrame(size, Color.White).Draw(d, pos, id);
                d.CenterBased = f;
                return;
            }
            //==画像によって変わる
            const int grid = 3;
            const int imageWidth = 128;
            const int imageHeight = 128;
            //==ここまで
            const int yBase = imageHeight - grid * 2;
            const int xBase = imageWidth - grid * 2;
            int x = (int)size.X - grid * 2;
            int y = (int)size.Y - grid * 2;
            if(x < 0 || y < 0) return;
            d.Draw(pos, TextureManager.GetTexture(TextureID.MessageBack), new Rectangle(0, 0, grid, grid), id);
            d.Draw(pos + new Vector2(grid, 0), TextureManager.GetTexture(TextureID.MessageBack), new Rectangle(grid, 0, xBase, grid), id, new Vector2((float)x / xBase, 1));
            d.Draw(pos + new Vector2(x + grid, 0), TextureManager.GetTexture(TextureID.MessageBack), new Rectangle(xBase + grid, 0, grid, grid), id);
            d.Draw(pos + new Vector2(0, grid), TextureManager.GetTexture(TextureID.MessageBack), new Rectangle(0, grid, grid, yBase), id, new Vector2(1, (float)y / xBase));
            Color colortemp = d.Color;
            d.Color = Color.FromNonPremultiplied(Settings.WindowR, Settings.WindowG, Settings.WindowB, Settings.WindowA);
            d.Draw(pos + new Vector2(grid, grid), TextureManager.GetTexture(TextureID.MessageBack), new Rectangle(grid, grid, xBase, yBase), id, new Vector2((float)x / xBase, (float)y / xBase));
            d.Color = colortemp;
            d.Draw(pos + new Vector2(x + grid, grid), TextureManager.GetTexture(TextureID.MessageBack), new Rectangle(xBase + grid, grid, grid, yBase), id, new Vector2(1, (float)y / xBase));
            d.Draw(pos + new Vector2(0, y + grid), TextureManager.GetTexture(TextureID.MessageBack), new Rectangle(0, yBase + grid, grid, grid), id);
            d.Draw(pos + new Vector2(grid, y + grid), TextureManager.GetTexture(TextureID.MessageBack), new Rectangle(grid, yBase + grid, xBase, grid), id, new Vector2((float)x / xBase, 1));
            d.Draw(pos + new Vector2(x + grid, y + grid), TextureManager.GetTexture(TextureID.MessageBack), new Rectangle(xBase + grid, yBase + grid, grid, grid), id);
            d.CenterBased = f;
        }
        public static Animation GetCursorAnimation() {
            return new Animation(new SingleTextureAnimationDataWithFrames(new int[] { 15,15,15,15 }, TextureID.Scroll, 1, 1));
        }
        public class MessageData : MessageBase {
            readonly string[] mes;
            readonly string name;
            readonly SimpleTexture[] graphic;

            readonly int maxPage;
            int page = -1;
            int wait;

            MessageData(string[] message, string name) {
                mes = message;
                this.name = name;
                maxPage = mes.Length;
            }
            /// <param name="message">メッセージ。1行最大29文字が目安。複数要素でページ送り。</param>
            /// <param name="name">発言者の名前</param>
            /// <param name="graphicids">表示するテクスチャ</param>
            public MessageData(string[] message, string name, TextureID[] graphicids) : this(message, name) {
                graphic = new SimpleTexture[maxPage];
                if(graphicids == null) return;
                for(int i = 0; i < graphicids.Length; i++)
                    if(graphicids[i] != TextureID.None)
                        graphic[i] = new SimpleTexture(graphicids[i]);
            }
            /// <param name="message">メッセージ。1行最大29文字が目安。複数要素でページ送り。</param>
            /// <param name="name">発言者の名前</param>
            /// <param name="graphicid">表示するテクスチャ</param>
            public MessageData(string[] message, string name, TextureID graphicid) : this(message, name)  {
                graphic = new SimpleTexture[maxPage];
                if(graphicid != TextureID.None)
                    for(int i = 0; i < maxPage; i++)
                        graphic[i] = new SimpleTexture(graphicid);
            }
            public override void Reset() {
                page = -1;
                wait = 0;
                IsEnd = false;
            }

            public override void Update(InputManager input, MessageManager message) {
                if(IsEnd) return;
                if((input.GetKeyPressed(KeyID.Select) && wait >= 10)
                    || (input.IsKeyDown(KeyID.Wait) && wait >= 2 ) || page == -1/* || wait == 300*/) {
                    page++; wait = 0;
                    if(page >= maxPage) { IsEnd = true; page--;
                    } else { message.Add("#ffff00" + name + "#ffffff : " + mes[page], true); }
                }
                wait++;
            }
            public override void Draw(Drawing d) {
                if(page >= 0)
                    DrawMessage(d, graphic[page], name, new RichText(mes[page]));
            }
        }
        public class SelectMessage : MessageBase {
            protected string[] choice;
            protected int length;
            /// <summary>
            /// 現在のページの長さ
            /// </summary>
            protected int length2;
            Action<int> action;

            readonly string mes;
            readonly string name;
            readonly SimpleTexture graphic;
            /// <summary>
            /// メッセージに追加したか
            /// </summary>
            bool mesFlag;
            
            protected int index = 0;
            int wait;
            Vector2 delta;
            protected Vector2 pos;
            protected bool DrawBack = true;
            protected int defX;
            protected int sizeX;
            protected int defY;
            protected int detY;
            bool cancelable;
            Animation cursor;

            public SelectMessage(string message, string name, TextureID texid, bool cancel, string[] choice, Action<int> act) {
                this.choice = choice;
                length2 = length = choice.Length;
                sizeX = choice.Max(x => RichText.Length(x)) * FontID.Medium.GetDefaultFontSizeX() + 64;
                defX = 600 - sizeX;
                detY = FontID.Medium.GetDefaultFontSizeY() + 4;
                defY = 334 - detY * choice.Length;
                if(defY < 12) defY = 12;
                action = act;

                mes = message;
                this.name = name;
                if(texid != TextureID.None)
                    graphic = new SimpleTexture(texid);

                pos = new Vector2(defX, defY);
                cancelable = cancel;
                cursor = GetCursorAnimation();
            }

            //複数ページ用
            bool multiPaged;
            int pageNum;
            int page;
            int pageLength;
            int frame = 0;
            public SelectMessage(string message, string name, TextureID texid, bool cancel, int pageLength, string[] choice, Action<int> act)
                :this(message,name,texid,cancel,choice,act) {
                pageNum = (length + pageLength - 1) / pageLength;
                if(!(multiPaged = pageNum > 1)) return;   //1ページで収まるならそのまま
                this.pageLength = pageLength;
                sizeX = choice.Max(x => RichText.Length(x)) * FontID.Medium.GetDefaultFontSizeX() + 64;
                defX = 600 - sizeX;
                detY = FontID.Medium.GetDefaultFontSizeY() + 4;
                defY = 334 - detY * pageLength;
                if(defY < 12) defY = 12;
                PageChanged();
            }
            void PageChanged() {
                length2 = Math.Min(pageLength, length - page * pageLength);
                if(index >= length2) index = length2 - 1;
                pos = new Vector2(defX, defY);
            }

            public virtual void SetIndex(int index) {
                if(index < -length) throw new ArgumentOutOfRangeException();
                this.index = (index + length) % length;
                pos = new Vector2(defX, defY + detY * this.index);
                
            }
            public void SetPos(int x, int y, int width, int height) {
                defX = x;
                defY = y;
                detY = height + 4;
                sizeX = width;

                pos = new Vector2(defX, defY);
            }
            public override void Update(InputManager input, MessageManager message) {
                frame++;
                cursor.Update();
                const int Time = 1; //カーソル移動時間
                if(!mesFlag) { message.Add("#ffff00" + name + "#ffffff : " + mes, true); mesFlag = true; }  //メッセージ追加
                if(wait > 0) { pos += delta; wait--; return; }  //カーソル移動待ち
                //カーソル移動処理
                if(input.IsPressedForMenu(KeyID.Up, 30, 15)) {
                    SoundManager.PlaySE(SoundEffectID.Cursor_Move);
                    index--;
                    if(index < 0) index = length2 - 1;
                    delta = (new Vector2(defX, defY + detY * index) - pos) / Time;
                    wait = Time;
                } else if(input.IsPressedForMenu(KeyID.Down, 30, 15)) {
                    SoundManager.PlaySE(SoundEffectID.Cursor_Move);
                    index++;
                    if(index >= length2) index = 0;
                    delta = (new Vector2(defX, defY + detY * index) - pos) / Time;
                    wait = Time;
                }
                if(multiPaged) {
                    if(input.IsPressedForMenu(KeyID.Left, 30, 15)) {
                        SoundManager.PlaySE(SoundEffectID.Cursor_Move);
                        page--;
                        if(page < 0) page = pageNum - 1;
                        PageChanged();
                    } else if(input.IsPressedForMenu(KeyID.Right, 30, 15)) {
                        SoundManager.PlaySE(SoundEffectID.Cursor_Move);
                        page++;
                        if(page >= pageNum) page = 0;
                        PageChanged();
                    }
                }

                if(input.GetKeyPressed(KeyID.Select)) {
                    SoundManager.PlaySE(SoundEffectID.Cursor_OK);
                    Select(index + page * pageLength, message);
                } else if(cancelable && input.GetKeyPressed(KeyID.Cancel)) {
                    SoundManager.PlaySE(SoundEffectID.Cursor_Cancel);
                    Select(-1, message);
                }
            }
            public override void Reset() {
                index = 0;
                wait = 0;
                IsEnd = false;
            }
            protected virtual void Select(int index, MessageManager message) {
                IsEnd = true;
                StringBuilder sb = new StringBuilder("");
                int l = 0;
                for(int i = 0; i < length; i++) {
                    int p = RichText.Length(sb.ToString());
                    int q = RichText.Length(choice[i]);
                    if(p + q - l > 50) { sb.Append("\n"); l = RichText.Length(sb.ToString()); }  //文字数見て適当に改行（適当すぎる）
                    sb.Append(i == index ? " <" : "  ").Append(choice[i]).Append(i == index ? "> " : "  ");
                }
                message.Add(sb.ToString(), true);
                if(action != null) action(index);
            }
            public override void Draw(Drawing d) {
                d.SetDrawAbsolute();
                if(DrawBack)
                    DrawMessageBack(d, new Vector2(sizeX + 20, detY * length2 + 24), new Vector2(defX - 10, defY - 10), DepthID.Message);
                if(multiPaged) {
                    new PageScroll(Direction4.Left).Draw(frame, d, new Vector2(defX - 2, defY + detY * length2 / 2), DepthID.Message);
                    new PageScroll(Direction4.Right).Draw(frame, d, new Vector2(defX + sizeX - 4, defY + detY * length2 / 2), DepthID.Message);
                }
                d.SetDrawNormal();
                DrawMessage(d, graphic, name, new RichText(mes));
                d.SetDrawAbsolute();
                DrawSelection(d);
                d.SetDrawNormal();
            }
            public void DrawSelection(Drawing d) {
                for(int i = 0; i < length2; i++)
                    new RichText(choice[i + page * pageLength]).Draw(d, new Vector2(defX + 24, defY + detY * i), DepthID.Message);
                cursor.Draw(d, new Vector2(defX + 2, defY + detY * index + 4), DepthID.Message);
            }
        }
        public class SelectValueMessage: MessageBase {
            Action<int> action;
            int digit;
            string unit;
            int max;

            readonly string mes;
            readonly string name;
            readonly SimpleTexture graphic;
            bool mesFlag;

            protected int value = 0;
            protected int index;
            int wait;
            Vector2 delta;
            protected Vector2 pos;
            protected bool DrawBack = true;
            protected int defX;
            protected int sizeX;
            protected int defY;
            protected int detX;
            bool cancelable;
            Animation cursor;

            public SelectValueMessage(string message, string name, TextureID texid, bool cancel, int digit, int max, string unit, Action<int> act) {
                this.digit = digit;
                this.unit = unit;
                this.max = max;
                sizeX = (digit + System.Text.Encoding.GetEncoding("Shift-JIS").GetByteCount(unit)) * FontID.Medium.GetDefaultFontSizeX() + 12 + digit * 2;
                defX = 600 - sizeX;
                detX = FontID.Medium.GetDefaultFontSizeX() + 2;
                defY = 300;
                action = act;

                mes = message;
                this.name = name;
                if(texid != TextureID.None)
                    graphic = new SimpleTexture(texid);

                pos = new Vector2(defX + 6, defY);
                cancelable = cancel;
                cursor = GetCursorAnimation();
            }
            public SelectValueMessage(string message, string name, TextureID texid, bool cancel, int digit, string unit, Action<int> act)
                : this(message, name, texid, cancel, digit, Pow10(digit) - 1, unit, act) { }
            public void SetValue(int val) {
                if(val < 0) throw new ArgumentOutOfRangeException();
                value = val;
            }
            public override void Update(InputManager input, MessageManager message) {
                cursor.Update();
                const int Time = 1;
                if(!mesFlag) { message.Add("#ffff00" + name + "#ffffff : " + mes, true); mesFlag = true; }
                if(wait > 0) { pos += delta; wait--; return; }
                if(input.IsPressedForMenu(KeyID.Left, 30, 15)) {
                    SoundManager.PlaySE(SoundEffectID.Cursor_Move);
                    index--;
                    if(index < 0) index = digit - 1;
                    delta = (new Vector2(defX + detX * index + 6, defY) - pos) / Time;
                    wait = Time;
                } else if(input.IsPressedForMenu(KeyID.Right, 30, 15)) {
                    SoundManager.PlaySE(SoundEffectID.Cursor_Move);
                    index++;
                    if(index >= digit) index = 0;
                    delta = (new Vector2(defX + detX * index + 6, defY) - pos) / Time;
                    wait = Time;
                } else if(input.IsPressedForMenu(KeyID.Down, 15, 5)) {
                    value -= Pow10(digit - index - 1);
                } else if(input.IsPressedForMenu(KeyID.Up, 15, 5)) {
                    value += Pow10(digit - index - 1);
                }
                if(value < 0) value = 0;
                if(value > max) value = max;

                if(input.GetKeyPressed(KeyID.Select)) {
                    SoundManager.PlaySE(SoundEffectID.Cursor_OK);
                    Select(value, message);
                } else if(cancelable && input.GetKeyPressed(KeyID.Cancel)) {
                    SoundManager.PlaySE(SoundEffectID.Cursor_Cancel);
                    Select(-1, message);
                }
            }
            static int Pow10(int x) {
                int t = 1;
                while(x > 0) { x--; t *= 10; }
                return t;
            }
            public override void Reset() {
                index = 0;
                wait = 0;
                value = 0;
                IsEnd = false;
            }
            protected virtual void Select(int v, MessageManager message) {
                IsEnd = true;

                message.Add(" [" + value.ToString().PadLeft(digit) + "] ", true);
                if(action != null) action(v);
            }
            public override void Draw(Drawing d) {
                d.SetDrawAbsolute();
                if(DrawBack)
                    DrawMessageBack(d, new Vector2(sizeX + 20, 64), new Vector2(defX - 10, defY - 10), DepthID.Message);
                d.SetDrawNormal();
                DrawMessage(d, graphic, name, new RichText(mes));
                d.SetDrawAbsolute();
                DrawSelection(d);
                d.SetDrawNormal();
            }
            public void DrawSelection(Drawing d) {
                for(int i = 0; i < digit; i++)
                    new RichText(((value % Pow10(i + 1)) / Pow10(i)).ToString()).NoNum().Draw(d, new Vector2(defX + 12 + detX * (digit - i - 1), defY + 12), DepthID.Message);
                new RichText(unit, FontID.Medium, Color.LightBlue).Draw(d, new Vector2(defX + detX * digit + 16, defY + 12), DepthID.Message);
                cursor.Draw(d, pos, DepthID.Message);
            }
        }
    }
    class PageScroll: AnimationData {
        public override int X { get { return direction == Direction4.Up || direction == Direction4.Down ? width : height; } }
        public override int Y { get { return direction == Direction4.Left || direction == Direction4.Right ? height : width; } }

        Direction4 direction;
        public PageScroll(Direction4 dir) { direction = dir; }

        public override void Draw(int f, Drawing d, Vector2 pos, DepthID depth, Vector2 size, float angle = 0) {
            //実装は必要になったら
            throw new NotImplementedException();
        }

        const int width = 12;
        const int height = 6; 
        public override void Draw(int f, Drawing d, Vector2 pos, DepthID depth, float size = 1, float angle = 0) {
            const int moveFrame = 8;
            const int maxDet = 6;
            if(d.CenterBased) {
                new SimpleTexture(TextureID.Scroll).Draw(d, pos + (Vector2)new Vector(0, f / moveFrame % maxDet).Rotate(((int)direction - 1) * 90), DepthID.Message, 1, MathHelper.PiOver2 * ((int)direction - 1));
                return;
            }
            switch(direction) {
                case Direction4.Down:
                    new SimpleTexture(TextureID.Scroll).Draw(d, pos + new Vector2(0, f / moveFrame % maxDet), DepthID.Message);
                    break;
                case Direction4.Up:
                    new SimpleTexture(TextureID.Scroll).Draw(d, pos + new Vector2(width, height - f / moveFrame % maxDet), DepthID.Message, 1, MathHelper.Pi);
                    break;
                case Direction4.Right:
                    new SimpleTexture(TextureID.Scroll).Draw(d, pos + new Vector2(f / moveFrame % maxDet, width), DepthID.Message, 1, MathHelper.PiOver2 * 3);
                    break;
                case Direction4.Left:
                    new SimpleTexture(TextureID.Scroll).Draw(d, pos + new Vector2(height - f / moveFrame % maxDet, 0), DepthID.Message, 1, MathHelper.PiOver2);
                    break;
            }
        }
    }
}
