using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CommonPart {
    /// <summary>
    /// Padのボタン設定シーン
    /// </summary>
    class PadConfig: MenuScene {
        /// <summary>
        /// （設定可能な）ボタンの表示名
        /// </summary>
        public static readonly string[] ButtonNames = new string[] { "決定・攻撃", "キャンセル・メニュー", "待機", "スキップ" };
        /// <summary>
        /// （設定可能な）ボタンのID
        /// </summary>
        public static readonly KeyID[] ButtonIDs = new KeyID[] { KeyID.Select, KeyID.Cancel, KeyID.Wait, KeyID.Skip };
        /// <summary>
        /// 選択肢
        /// </summary>
        static readonly string[] choice = ButtonNames.Concat(new string[] { "動作テスト・遊び調整", "終了" }).ToArray();
        static readonly KeyID[] ids = ButtonIDs;
        int setting = -1;
        Animation cursor = TalkWindow.GetCursorAnimation();
        public PadConfig(SceneManager s) : base(s, choice.Length) { s.BackSceneNumber++; JoyPadManager.GetPad(); }
        protected override void Choosed(int i) {
            if(i == MaxIndex - 2) { new PadTestScene(scenem); return; }
            if(i == MaxIndex - 1) { Delete = true; return; }
            setting = i;
        }
        public override void SceneUpdate() {
            cursor.Update();
            if(setting >= 0) {
                int x = JoyPadManager.FirstPressedKey();
                if(x >= 0) {
                    int id = (int)ids[setting] - 4;
                    //既に設定済みのキーなら入れ替え
                    int y = Array.IndexOf(JoyPadManager.JoyButtons, (byte)x);
                    if(y >= 0) { JoyPadManager.JoyButtons[y] = JoyPadManager.JoyButtons[id]; }
                    JoyPadManager.JoyButtons[id] = (byte)x;
                    setting = -3;
                }
            } else if(setting < -1) setting++;
            else
                base.SceneUpdate();
        }
        public override void Focused() {
            JoyPadManager.GetPad();
            JoyPadManager.Update();
        }
        public override void SceneDraw(Drawing d) {
            TalkWindow.DrawMessageBack(d, new Vector2(500, 24 + 26 * MaxIndex), new Vector2(40, 40), DepthID.Message);
            for(int i = 0; i < MaxIndex; i++) {
                Vector2 pos = new Vector2(72, 50 + 26 * i);
                new RichText(choice[i], FontID.Medium, i == setting ? Color.Yellow : Color.White).Draw(d, pos, DepthID.Message);
                if(i < MaxIndex - 2)
                    new RichText((JoyPadManager.JoyButtons[(int)ids[i] - 4] + 1).ToString(), FontID.Medium, i == setting ? Color.Yellow : Color.White).NoNum().Draw(d, pos + new Vector2(300, 0), DepthID.Message);
            }
            cursor.Draw(d, new Vector2(50, 54 + Index * 26), DepthID.Message);
        }
    }
}
