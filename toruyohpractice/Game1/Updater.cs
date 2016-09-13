using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace MyUpdaterLib {
    /* updateフォルダの構造
     * main.dat（仮） 中身はテキストでいいや
     * 固有識別子
     * 基準exe名
     * 最新製品バージョン:最新アセンブリバージョン
     * 対応バージョン数
     * 対応バージョン名:zip名 x num
     */
    public class Updater : IDisposable {
        /// <summary>
        /// 一覧ファイルのダウンロード状況
        /// </summary>
        public UpdateState Progress_Data { get; private set; }
        /// <summary>
        /// 更新ファイルのダウンロード状況
        /// </summary>
        public UpdateState Progress_UpdateFile { get; private set; }
        /// <summary>
        /// ダウンロード状況（共通）
        /// </summary>
        public DownloadProgressChangedEventArgs Args { get; private set; }
        /// <summary>
        /// 最新バージョン名（製品バージョン）　特殊な値：未取得・失敗・最新（すでに最新版である）・更新不能（このバージョン用のパッチが存在しない）
        /// </summary>
        public string NewestVersion {
            get {
                if(Progress_Data == UpdateState.None) return "未取得";
                if(Progress_Data != UpdateState.Success) return "失敗";
                if(newest == ver) return "最新";
                if(fileName == null) return "更新不能";
                return newest_str;
            }
        }

        public bool CanUpdate { get { return Progress_Data == UpdateState.Success && newest != ver && fileName != null; } }

        WebClient client;
        string baseUri;
        /// <summary>
        /// 一時ファイルの名前
        /// </summary>
        public string TempFile = "temp.dat";
        /// <summary>
        /// 一覧ファイルの名前（サーバ上）
        /// </summary>
        public string MainFileOnServer = "main.dat";
        Version ver;
        Version newest;
        string newest_str;
        string fileName;
        string asmName;
        Task task;
        public Updater(string uri, Assembly asm) {
            client = new WebClient();
            client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
            client.DownloadDataCompleted += new DownloadDataCompletedEventHandler(client_DownloadDataCompleted);
            client.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler(client_DownloadFileCompleted);
            baseUri = uri;

            ver = asm.GetName().Version;
            asmName = Path.GetFileName(asm.Location);
            //System.Diagnostics.FileVersionInfo.GetVersionInfo("").
        }

        public void CancelDownload() {
            if(task != null && !task.IsCompleted) {
                Progress_Data = UpdateState.Cancelled;
            }
            client.CancelAsync();
        }

        void client_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e) {
            if(e.Cancelled) { Progress_UpdateFile = UpdateState.Cancelled; return; }
            if(e.Error != null) { Progress_UpdateFile = UpdateState.Error; return; }
            Progress_UpdateFile = UpdateState.Success;
        }

        void client_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e) {
            if(e.Cancelled) { Progress_Data = UpdateState.Cancelled; return; }
            if(e.Error != null) { Progress_Data = UpdateState.Error; return; }
            Progress_Data = UpdateState.Success;

            try {
                using(var stm = new StreamReader(new MemoryStream(e.Result))) {
                    if(stm.ReadLine() != "** Update Data List **") { Progress_Data = UpdateState.ErrorParse; return; }
                    if(stm.ReadLine() != asmName) { Progress_Data = UpdateState.ErrorParse; return; }
                    var str = stm.ReadLine().Split(':');
                    newest_str = str[0];
                    newest = new Version(str[1]);
                    //最新版
                    if(newest == ver) return;
                    int num;
                    if(!int.TryParse(stm.ReadLine(), out num)) { Progress_Data = UpdateState.ErrorParse; return; }
                    for(int i = 0; i < num; i++) {
                        str = stm.ReadLine().Split(':');
                        //対応するバージョンならアップデートファイル名をセット
                        if(ver == new Version(str[0])) fileName = str[1];
                    }
                }
            } catch {
                Progress_Data = UpdateState.ErrorParse;
            }
        }

        void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e) {
            Args = e;
            //Console.WriteLine(e.ProgressPercentage);
        }
        /// <summary>
        /// アップデートがあるかチェック
        /// </summary>
        public void CheckUpdate() {
            task = new Task(CheckUpdate_);
            task.Start();
        }
        void CheckUpdate_() {
            try {
                Progress_Data = UpdateState.Downloading;
                client.DownloadDataAsync(Concat(baseUri, MainFileOnServer));
            } catch {
                Progress_Data = UpdateState.Error;
            }

        }
        /// <summary>
        /// アップデートファイルをダウンロード（要CheckUpdate）
        /// </summary>
        public void Update() {
            if(Progress_Data != UpdateState.Success) return;
            Args = null;
            try {
                Progress_UpdateFile = UpdateState.Downloading;
                client.DownloadFileAsync(Concat(baseUri, fileName), TempFile);
            } catch(TaskCanceledException) {
                Progress_UpdateFile = UpdateState.Cancelled;
            } catch {
                Progress_UpdateFile = UpdateState.Error;
            }
        }
        Uri Concat(string s1, string s2) { return new Uri(s1.Last() != '/' ? s1 + "/" + s2 : s1 + s2); }
        public void Dispose() {
            if(task != null) task.Dispose();
            if(client != null) client.Dispose();
        }

        public enum UpdateState {
            None, Downloading, Success, Error, ErrorParse, Cancelled
        }
    }
}
