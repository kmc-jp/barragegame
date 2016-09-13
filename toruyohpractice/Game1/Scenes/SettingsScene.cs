using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace CommonPart {
    static class Settings {
        public static int BGMVolume { get { return SoundManager.Music.Volume / 10; } set { SoundManager.Music.Volume = value * 10; } }
        public static int SEVolume { get { return SoundManager.SE.Volume; } set { SoundManager.SE.Volume = value; } }

        public static int WindowR, WindowG, WindowB, WindowA;
        static Settings() { SetDefaultColor(); }

        public static void SetDefaultColor() {
            WindowR = 16;
            WindowG = 48;
            WindowB = 96;
            WindowA = 224;
        }

        public static int WindowStyle = 1;
        //これは保存しなくてよい
        public static int WindowStyleOld = 1;


        public static ExplainType Explain = ExplainType.Keyboard;

        public enum ExplainType { Keyboard, Pad, None, Length }
    }
    class SettingsScene : MenuScene {
        readonly static string[] messages;
        readonly static int length;
        static SettingsScene() {
            messages = new string[length = Enum.GetNames(typeof(SettingID)).Length];
            messages[(int)SettingID.BGMVolume] = "BGM音量";
            messages[(int)SettingID.SEVolume] = "SE音量";
            messages[(int)SettingID.WindowSize] = "ウィンドウの大きさ";
            messages[(int)SettingID.Explain] = "操作説明表示";
            messages[(int)SettingID.ColorSetting] = "ウィンドウ色設定";
            messages[(int)SettingID.KeyConfig] = "キー設定";
            messages[(int)SettingID.PadConfig] = "ゲームパッド設定";
            messages[(int)SettingID.Close] = "閉じる";
        }
        Animation cursor = TalkWindow.GetCursorAnimation();
        bool _delete;
        public SettingsScene(SceneManager s) : base(s, length) { s.BackSceneNumber++; Settings.WindowStyleOld = Settings.WindowStyle; }
        public override void SceneUpdate() {
            cursor.Update();
            base.SceneUpdate();
            switch((SettingID)Index) {
                case SettingID.BGMVolume:
                    if(Input.IsPressedForMenu(KeyID.Left, 15, 2)) Settings.BGMVolume--;
                    else if(Input.IsPressedForMenu(KeyID.Right, 15, 2)) Settings.BGMVolume++;
                    break;
                case SettingID.SEVolume:
                    if(Input.IsPressedForMenu(KeyID.Left, 15, 2)) Settings.SEVolume--;
                    else if(Input.IsPressedForMenu(KeyID.Right, 15, 2)) Settings.SEVolume++;
                    break;
                case SettingID.WindowSize:
                    if(Input.IsPressedForMenu(KeyID.Left, 30, 15) || Input.IsPressedForMenu(KeyID.Right, 30, 15))
                        Settings.WindowStyle = 1 - Settings.WindowStyle;
                    break;
                case SettingID.Explain:
                    if(Input.IsPressedForMenu(KeyID.Left, 40, 20)) Settings.Explain--;
                    else if(Input.IsPressedForMenu(KeyID.Right, 40, 20)) Settings.Explain++;
                    Settings.Explain = (Settings.ExplainType)(((int)Settings.Explain + (int)Settings.ExplainType.Length) % (int)Settings.ExplainType.Length);
                    break;
            }
            if(Input.GetKeyPressed(KeyID.Cancel)) {
                SoundManager.PlaySE(SoundEffectID.Cursor_Cancel);
                Close();
            }
        }
        void Close() {
            _delete = true;
            //Save.SaveConfig();
        }
        protected override void Choosed(int i) {
            switch((SettingID)i) {
                case SettingID.ColorSetting: new ColorSettings(scenem); break;
                case SettingID.KeyConfig: new KeyConfig(scenem); break;
                case SettingID.PadConfig: new PadConfig(scenem); break;
                case SettingID.Close: Close(); break;
            }
        }
        public override void SceneDraw(Drawing d) {
            if(_delete) { Delete = true; if (Settings.WindowStyle != Settings.WindowStyleOld) { d.DrawStyle = Settings.WindowStyle; Settings.WindowStyleOld = Settings.WindowStyle; } }
            TalkWindow.DrawMessageBack(d, new Vector2(500, 24 + length * 26), new Vector2(20, 20), DepthID.Message);
            for(int i = 0; i < length; i++) {
                Vector2 pos = new Vector2(52, 30 + 26 * i);
                new RichText(messages[i]).Draw(d, pos, DepthID.Message);
            }
            cursor.Draw(d, new Vector2(30, 34 + Index * 26), DepthID.Message);
            DrawMeter(d, new Vector2(240, 30 + 26 * (int)SettingID.BGMVolume), Settings.BGMVolume, Settings.BGMVolume.ToString(), 0, 100);
            DrawMeter(d, new Vector2(240, 30 + 26 * (int)SettingID.SEVolume), Settings.SEVolume, Settings.SEVolume.ToString(), 0, 100);
            string str = "";
            switch(Settings.Explain) {
                case Settings.ExplainType.None: str = "表示しない"; break;
                case Settings.ExplainType.Keyboard: str = "キーボード"; break;
                case Settings.ExplainType.Pad: str = "パッド"; break;
            }
            new RichText(str).Draw(d, new Vector2(240, 30 + 26 * (int)SettingID.Explain), DepthID.Message);

            string show;
            if(Settings.WindowStyle == 1)
                show = String.Format("{0}x{1} (4 : 3)", 1280, 720);
            else
                show = String.Format("{0}x{1} (16 : 9)", 1280, 720);
            new RichText(show).Draw(d, new Vector2(240, 30 + 26 * (int)SettingID.WindowSize), DepthID.Message);
        }
        public static void DrawMeter(Drawing d, Vector2 pos, int value, string str, int min, int max) {
            //new SimpleTexture(TextureID.ConfigMeterBack).Draw(d, pos + new Vector2(0, 4), DepthID.Message);
            //new SimpleTexture(TextureID.ConfigMeter).Draw(d, pos + new Vector2(2 + (value - min) * 188 / (max - min), 0), DepthID.Message);
            new Gauge(new Vector2(200, 12), Color.Blue, min, max, value, Color.Black).Draw(d, pos + new Vector2(0, 6), DepthID.Message);
            new RichText(str).Draw(d, pos + new Vector2(220, 0), DepthID.Message);
        }
        enum SettingID {
            BGMVolume, SEVolume, Explain, WindowSize, ColorSetting, KeyConfig, PadConfig, Close
        }
    }
    class ColorSettings: MenuScene {
        readonly static string[] messages;
        readonly static int length;
        static ColorSettings() {
            messages = new string[length = Enum.GetNames(typeof(C_SettingID)).Length];
            messages[(int)C_SettingID.R] = "赤";
            messages[(int)C_SettingID.G] = "緑";
            messages[(int)C_SettingID.B] = "青";
            messages[(int)C_SettingID.A] = "透明度";
            messages[(int)C_SettingID.Default] = "デフォルトに戻す";
            messages[(int)C_SettingID.Close] = "閉じる";
        }
        Animation cursor = TalkWindow.GetCursorAnimation();

        public ColorSettings(SceneManager s) : base(s, length) { s.BackSceneNumber++; }

        public override void SceneUpdate() {
            cursor.Update();
            base.SceneUpdate();
            switch((C_SettingID)Index) {
                case C_SettingID.R:
                    if(Input.IsPressedForMenu(KeyID.Left, 10, 1) && Settings.WindowR > 0) Settings.WindowR--;
                    else if(Input.IsPressedForMenu(KeyID.Right, 10, 1) && Settings.WindowR < 255) Settings.WindowR++;

                    break;
                case C_SettingID.G:
                    if(Input.IsPressedForMenu(KeyID.Left, 10, 1) && Settings.WindowG > 0) Settings.WindowG--;
                    else if(Input.IsPressedForMenu(KeyID.Right, 10, 1) && Settings.WindowG < 255) Settings.WindowG++;
                    break;
                case C_SettingID.B:
                    if(Input.IsPressedForMenu(KeyID.Left, 10, 1) && Settings.WindowB > 0) Settings.WindowB--;
                    else if(Input.IsPressedForMenu(KeyID.Right, 10, 1) && Settings.WindowB < 255) Settings.WindowB++;
                    break;
                case C_SettingID.A:
                    if(Input.IsPressedForMenu(KeyID.Left, 10, 1) && Settings.WindowA > 0) Settings.WindowA--;
                    else if(Input.IsPressedForMenu(KeyID.Right, 10, 1) && Settings.WindowA < 255) Settings.WindowA++;
                    break;
            }
            if(Input.GetKeyPressed(KeyID.Cancel)) {
                SoundManager.PlaySE(SoundEffectID.Cursor_Cancel);
                Delete = true;
            }
        }
        protected override void Choosed(int i) {
            switch((C_SettingID)i) {
                case C_SettingID.Default:
                    Settings.SetDefaultColor();
                    break;
                case C_SettingID.Close: Delete = true; break;
            }
        }

        public override void SceneDraw(Drawing d) {
            TalkWindow.DrawMessageBack(d, new Vector2(500, 320), new Vector2(20, 20), DepthID.Message);
            for(int i = 0; i < length; i++) {
                Vector2 pos = new Vector2(52, 30 + 26 * i);
                new RichText(messages[i]).Draw(d, pos, DepthID.Message);
            }
            cursor.Draw(d, new Vector2(30, 34 + Index * 26), DepthID.Message);
            SettingsScene.DrawMeter(d, new Vector2(240, 30 + 26 * (int)C_SettingID.R), Settings.WindowR, Settings.WindowR.ToString(), 0, 255);
            SettingsScene.DrawMeter(d, new Vector2(240, 30 + 26 * (int)C_SettingID.G), Settings.WindowG, Settings.WindowG.ToString(), 0, 255);
            SettingsScene.DrawMeter(d, new Vector2(240, 30 + 26 * (int)C_SettingID.B), Settings.WindowB, Settings.WindowB.ToString(), 0, 255);
            SettingsScene.DrawMeter(d, new Vector2(240, 30 + 26 * (int)C_SettingID.A), Settings.WindowA, Settings.WindowA.ToString(), 0, 255);
        }

        enum C_SettingID {
            R, G, B, A, Default, Close
        }
    }
}
