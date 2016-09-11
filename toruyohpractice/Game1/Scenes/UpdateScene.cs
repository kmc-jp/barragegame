using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyUpdaterLib;
using System.Reflection;
using System.IO;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace CommonPart {
    /// <summary>
    /// アップデート用のファイルをダウンロードするシーン
    /// </summary>
    class UpdateScene : Scene {
        Updater updater;
        public static readonly string TempExe = "temp.exe";
        public UpdateScene(SceneManager s, Updater u) : base(s) {
            s.BackSceneNumber++;
            updater = u;
            u.Update();
        }
        public override void SceneUpdate() {
            switch(updater.Progress_UpdateFile) {
                case Updater.UpdateState.Downloading:
                    if(Input.GetKeyPressed(KeyID.Cancel)) updater.CancelDownload();
                    break;
                case Updater.UpdateState.Error:
                case Updater.UpdateState.Cancelled:
                    if(Input.GetKeyPressed(KeyID.Cancel)) Delete = true;
                    break;
                case Updater.UpdateState.Success:
                    if(Input.GetKeyPressed(KeyID.Select)) UpdateUnZip(updater.TempFile);
                    break;
            }
        }
        public override void SceneDraw(Drawing d) {
            new FilledBox(new Vector2(10000, 10000), Color.Black * 0.3f).Draw(d, new Vector2(0, 0), DepthID.BackGroundFloor);
            switch(updater.Progress_UpdateFile) {
                case Updater.UpdateState.Downloading:
                    if(updater.Args == null) {
                        new RichText("更新ファイルのダウンロード中…").Draw(d, new Vector2(100, 120), DepthID.Message);
                    } else {
                        new RichText("更新ファイルのダウンロード中…(" + updater.Args.ProgressPercentage + "%)").Draw(d, new Vector2(100, 120), DepthID.Message);
                        new Gauge(new Vector2(400, 16), Color.Aquamarine, 0, (int)updater.Args.TotalBytesToReceive, (int)updater.Args.BytesReceived, Color.Black).Draw(d, new Vector2(100, 150), DepthID.Message);
                        new RichText(string.Format("{0,15:#,0}byte/{1,15:#,0}byte", updater.Args.BytesReceived, updater.Args.TotalBytesToReceive), FontID.Medium).Draw(d, new Vector2(100, 180), DepthID.Message, 0.7f);
                    }
                    new RichText("[" + KeyConfig.GetKeyString(KeyID.Cancel) + "]：キャンセル", FontID.Medium).Draw(d, new Vector2(100, 220), DepthID.Message, 0.7f);
                    break;
                case Updater.UpdateState.Cancelled:
                    new RichText("キャンセルしました").Draw(d, new Vector2(100, 120), DepthID.Message);
                    new RichText("[" + KeyConfig.GetKeyString(KeyID.Cancel) + "]：閉じる", FontID.Medium).Draw(d, new Vector2(100, 220), DepthID.Message, 0.7f);
                    break;
                case Updater.UpdateState.Error:
                    new RichText("失敗しました").Draw(d, new Vector2(100, 120), DepthID.Message);
                    new RichText("[" + KeyConfig.GetKeyString(KeyID.Cancel) + "]：閉じる", FontID.Medium).Draw(d, new Vector2(100, 220), DepthID.Message, 0.7f);
                    break;
                case Updater.UpdateState.Success:
                    new RichText("ダウンロードに成功　再起動します").Draw(d, new Vector2(100, 120), DepthID.Message);
                    new RichText("[" + KeyConfig.GetKeyString(KeyID.Select) + "]：再起動", FontID.Medium).Draw(d, new Vector2(100, 220), DepthID.Message, 0.7f);
                    break;
            }
        }
        void UpdateUnZip(string archive) {
            //解凍用ファイルの書き出し
            using(var stm = Assembly.GetExecutingAssembly().GetManifestResourceStream("ProjectCompass.UpdateUnziper.exe")) {
                int length = (int)stm.Length;
                byte[] buffer = new byte[length];
                stm.Read(buffer, 0, length);
                File.WriteAllBytes(TempExe, buffer);
            }
            using(var stm = Assembly.GetExecutingAssembly().GetManifestResourceStream("ProjectCompass.UpdateUnziper.exe.config")) {
                int length = (int)stm.Length;
                byte[] buffer = new byte[length];
                stm.Read(buffer, 0, length);
                File.WriteAllBytes(TempExe + ".config", buffer);
            }
            //終了させて解凍用exeを起動
            scenem.Exit();
            Process.Start(TempExe, archive + " " + Assembly.GetExecutingAssembly().Location);
        }
    }
}
