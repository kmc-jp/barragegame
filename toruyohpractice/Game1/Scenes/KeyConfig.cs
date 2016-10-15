using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace CommonPart {
    /// <summary>
    /// キー設定のシーン
    /// </summary>
    class KeyConfig : MenuScene {
        /// <summary>
        /// 選択肢
        /// </summary>
        static readonly string[] choice = new string[] { "上", "下", "左", "右" }.Concat(PadConfig.ButtonNames.Concat(new string[] { "デフォルトに戻す", "保存せずに終了", "保存して終了" })).ToArray();
        /// <summary>
        /// 設定するキー一覧
        /// </summary>
        static readonly KeyID[] ids = new KeyID[] { KeyID.Up, KeyID.Down, KeyID.Left, KeyID.Right }.Concat(PadConfig.ButtonIDs).ToArray();
        /// <summary>
        /// 設定中キーインデックス
        /// </summary>
        int setting = -1;
        /// <summary>
        /// 新たな設定
        /// </summary>
        Keys[] sets;

        Animation cursor = TalkWindow.GetCursorAnimation();
        public KeyConfig(SceneManager s) : base(s, choice.Length) {
            s.BackSceneNumber++;
            sets = (Keys[])InputManager.GetKeys().Clone();
        }
        protected override void Choosed(int i) {
            if(i == MaxIndex - 3) { InputManager.SetDefault(); return; }
            if(i == MaxIndex - 2) { Delete = true; return; }
            if(i == MaxIndex - 1) { InputManager.SetKeys(sets); Delete = true; return; }
            setting = i;
        }
        /// <summary>
        /// セットしないキー一覧
        /// </summary>
        static readonly Keys[] notforuse = new Keys[] { Keys.OemAuto, Keys.OemEnlW, Keys.OemCopy, (Keys)240, (Keys)241 };   //このあたりは「全角／半角」キーなどに相当しますが、全角にしている間などはずっと押されている扱いになるため面倒です
        public override void SceneUpdate() {
            cursor.Update();
            //セット中
            if(setting >= 0) {
                //決定キーが離されていて、有効なキーが押されている場合にセットする
                Keys key = Keyboard.GetState().GetPressedKeys().FirstOrDefault(x => !notforuse.Contains(x));
                if(key != Keys.None && Input.GetKeyState(KeyID.Select) != KeyState.Down) {
                    //既に設定済みのキーなら入れ替え
                    int x = Array.IndexOf(sets, key);
                    int id = (int)ids[setting];
                    if(x >= 0) {
                        sets[x] = sets[id];
                    }
                    sets[id] = key;
                    setting = -3;
                }
            } else if(setting < -1) setting++;  //Wait中
            else {
                base.SceneUpdate();
            }
        }
        public override void SceneDraw(Drawing d) {
            TalkWindow.DrawMessageBack(d, new Vector2(500, 24 + 26 * MaxIndex), new Vector2(40, 40), DepthID.Message);
            for(int i = 0; i < MaxIndex; i++) {
                Vector2 pos = new Vector2(72, 50 + 26 * i);
                new RichText(choice[i], FontID.Medium, i == setting ? Color.Yellow : Color.White).Draw(d, pos, DepthID.Message);
                if(i < MaxIndex - 3) {
                    new RichText(KeysToStr(InputManager.GetKey(ids[i])), FontID.Medium, Color.Gray).NoNum().Draw(d, pos + new Vector2(240, 0), DepthID.Message);
                    new RichText(KeysToStr(sets[(int)ids[i]]), FontID.Medium, i == setting ? Color.Yellow : Color.White).NoNum().Draw(d, pos + new Vector2(360, 0), DepthID.Message);
                }
            }
            cursor.Draw(d, new Vector2(50, 54 + Index * 26), DepthID.Message);
        }
        /// <summary>
        /// キーの名前を得る
        /// </summary>
        public static string GetKeyString(KeyID id) {
            if(Settings.Explain == Settings.ExplainType.Pad){
                switch(id) {
                    case KeyID.Up: return "#ffff00↑#ffffff";
                    case KeyID.Down: return "#ffff00↓#ffffff";
                    case KeyID.Left: return "#ffff00←#ffffff";
                    case KeyID.Right: return "#ffff00→#ffffff";
                    default: return "(" + (JoyPadManager.JoyButtons[(int)id - 4] + 1) + ")";
                }
            }
            return "#ffff00" + KeysToStr(InputManager.GetKey(id)) + "#ffffff";
        }
        static string KeysToStr(Keys k) {
            switch(k) {
                case Keys.Up: return "↑";
                case Keys.Down: return "↓";
                case Keys.Left: return "←";
                case Keys.Right: return "→";
                case Keys.D0:
                case Keys.D1:
                case Keys.D2:
                case Keys.D3:
                case Keys.D4:
                case Keys.D5:
                case Keys.D6:
                case Keys.D7:
                case Keys.D8:
                case Keys.D9:
                    return k.ToString().Substring(1);
                case Keys.NumPad0:
                case Keys.NumPad1:
                case Keys.NumPad2:
                case Keys.NumPad3:
                case Keys.NumPad4:
                case Keys.NumPad5:
                case Keys.NumPad6:
                case Keys.NumPad7:
                case Keys.NumPad8:
                case Keys.NumPad9:
                    return "テンキー" + k.ToString().Substring(6);
                case Keys.Add: return "テンキー+";
                case Keys.Subtract: return "テンキー-";
                case Keys.Multiply: return "テンキー*";
                case Keys.Divide: return "テンキー/";
                case Keys.Decimal: return "テンキー.";
                case Keys.LeftControl: return "左Ctrl";
                case Keys.LeftAlt: return "左Alt";   //Altは効かないもの？
                case Keys.LeftWindows: return "左Windows";
                case Keys.LeftShift: return "左Shift";   //Monogameの仕様のため、Shiftとの同時押しがうまく動かないのはキーボード依存っぽい
                case Keys.RightControl: return "右Ctrl";
                case Keys.RightAlt: return "右Alt";
                case Keys.RightWindows: return "右Windows";
                case Keys.RightShift: return "右Shift";
                case Keys.Apps: return "アプリ";
                case Keys.ImeConvert: return "変換";
                case Keys.ImeNoConvert: return "無変換";
                case Keys.Back: return "BackSpace";
                case Keys.OemBackslash: return "\\";
                case Keys.OemPipe: return "|";
                case Keys.OemMinus: return "-";
                case Keys.OemPlus: return ";";
                case Keys.OemSemicolon: return ":"; //この辺名前が間違っているように見えますが、英字配列と日本語配列の違いのせいです
                case Keys.OemQuestion: return "/";
                case Keys.OemTilde: return "@";
                case Keys.OemQuotes: return "^";
                case Keys.OemOpenBrackets: return "[";
                case Keys.OemCloseBrackets: return "]";
                case Keys.OemComma: return ",";
                case Keys.OemPeriod: return ".";
                default: return k.ToString();
            }
        }
    }
}
