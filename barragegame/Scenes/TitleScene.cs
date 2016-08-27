using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using System.Reflection;
using MyUpdaterLib;

namespace barragegame {
    class TitleScene: MenuScene {
        enum TitleIndex {
            Start, Config, Jukebox, Update, Quit
        }
        static readonly string[] choiceDefault = new[] { "----", "設定を変更する", "サウンドテストをする", "ゲームを更新する", "ゲームを終了する" };
        readonly string[] choice = (string[])choiceDefault.Clone();
        bool[] enabled = new bool[] { false, true, false, false, true };
        Color[] defaultColor = new Color[] { Color.White, Color.White, Color.White, Color.Gold, Color.White };
        Animation cursor = TalkWindow.GetCursorAnimation();
        string version;

        Updater updater;
        public TitleScene(SceneManager s) : base(s, choiceDefault.Length) {

            Focused();
            
            version = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion;

            updater = new Updater("あどれす", Assembly.GetExecutingAssembly());
            updater.CheckUpdate();

            enabled[(int)TitleIndex.Jukebox] = Function.GetEnumLength<BGMID>() > 1;
        }
        public override void Deleted() {
            if(updater != null) updater.Dispose();
        }
        public override void SceneUpdate() {
            if(!enabled[(int)TitleIndex.Update] && updater.CanUpdate) {
                enabled[(int)TitleIndex.Update] = true;
            }
            cursor.Update();
            SoundManager.Music.PlayBGM(BGMID.None, true);
            base.SceneUpdate();
        }
        protected override void Choosed(int i) {
            if(!enabled[i]) return;
            switch((TitleIndex)i) {
                case TitleIndex.Start:
                    break;
                case TitleIndex.Config:
                    new SettingsScene(scenem);
                    break;
                case TitleIndex.Jukebox:
                    new SoundTest(scenem);
                    break;
                case TitleIndex.Update:
                    new UpdateScene(scenem, updater);
                    break;
                case TitleIndex.Quit:
                    Delete = true;
                    break;
            }
        }
        public override void Focused() {
            JoyPadManager.GetPad();
            JoyPadManager.Update();
        }
        public override void SceneDraw(Drawing d) {
            if(scenem.IsTopScene(this) && Settings.WindowSizeOld != Settings.WindowSize) d.DrawRate = Settings.WindowSize / 10f;

            Vector2 basePos = new Vector2(218, 234);
            TalkWindow.DrawMessageBack(d, new Vector2(274, 28 + MaxIndex * 25), basePos, DepthID.Message);
            for(int i = 0; i < MaxIndex; i++) {
                new RichText(choice[i], FontID.Medium, enabled[i] ? defaultColor[i] : Color.Gray).Draw(d, basePos + new Vector(34, 10 + i * 26), DepthID.Message);
            }
            cursor.Draw(d, basePos + new Vector2(12, 16 + Index * 26), DepthID.Message);

            new RichText(version, FontID.Medium).Draw(d, new Vector(20, 10), DepthID.Message, 0.7f);
            string str = "更新情報：";
            switch(updater.Progress_Data) {
                case Updater.UpdateState.Downloading: str += "取得中…"; break;
                case Updater.UpdateState.Error: str += "失敗"; break;
                case Updater.UpdateState.ErrorParse: str += "サーバ上のファイルに異常"; break;
                case Updater.UpdateState.Success:
                    if(updater.CanUpdate) str += "更新可能：";
                    str += updater.NewestVersion;
                    break;
            }
            new RichText(str, FontID.Medium).Draw(d, new Vector(20, 30), DepthID.Message, 0.7f);
            
        }
    }
}
