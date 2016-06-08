using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace ProjectCompass {
    /// <summary>
    /// BGM再生用クラス（winmm.dll利用、ループが微妙）
    /// </summary>
    //mp3,wav,midiを確認　他にも行けるかも
    class MusicPlayer {
        [DllImport("winmm.dll")]
        static extern int mciSendString(string lpstrCommand, StringBuilder lpstrReturnString, int uReturnLength, IntPtr hwndCallback);
        [DllImport("winmm.dll")]
        static extern bool mciGetErrorString(int fdwError, StringBuilder lpszErrorText, int cchErrorText);

        int SendString(string command) {
            return mciSendString(command, buffer = new StringBuilder(256), 256, handle);
        }
        int SendString(string command, string path, string command2 = "") {
            if(path == null) return -1;
            return SendString(command + " \"" + path + "\" " + command2);
        }
        int SendString(string command, BGMID id, string command2 = "") {
            if(id == BGMID.None) return -1;
            return SendString(command, fileNames[(int)id], command2);
        }

        static readonly int length = Enum.GetNames(typeof(BGMID)).Length - 1;
        string[] fileNames = new string[length];
        int[] loopPoint = new int[length];
        int[] loopEnd = new int[length];
        int[] volumes = new int[length];
        int[] start = new int[length];
        bool[] isMidi = new bool[length];
        BGMID playing = BGMID.None;
        BGMID interrupted = BGMID.None;

        IntPtr handle;
        StringBuilder buffer;
        /// <summary>
        /// 音量　0～1000で指定　MIDIは未対応
        /// </summary>
        public int Volume {
            get { return _volume; }
            set { _volume = Math.Max(0, Math.Min(1000, value)); ChangeBGMVolume(_volume); }
        }
        int _volume = 800;

        public MusicPlayer() {
            handle = new DummyHandler(this).Handle;
            Load();
        }
        void Load() {
            LoadBGM(BGMID.GrandGrass, "Content/Sounds/grand_glass2.mp3", 100, 27429, 126857);
            LoadBGM(BGMID.BottomRain, "Content/Sounds/bottomrain.mp3", 50, 25267, 202109);
            LoadBGM(BGMID.DarkStep, "Content/Sounds/darkstep.mp3", 40, 51892, 194595);
            LoadBGM(BGMID.Doukutu, "Content/Sounds/doukutu.mp3", 50, 0, 88889);
            LoadBGM(BGMID.PlaceToReturn, "Content/Sounds/place_to_return.mp3", 50, 2143, 70714);
            LoadBGM(BGMID.Result, "Content/Sounds/result.mp3", 100, 8727, 69818);
            LoadBGM(BGMID.CompassTitle, "Content/Sounds/compass-title.mp3", 100, 2424, 89697);
            LoadBGM(BGMID.Stepin, "Content/Sounds/stepin!.mp3", 100, 0, 109879);

            LoadBGM(BGMID.Otsukare, "Content/Sounds/otsukare.mp3", 50);
            LoadBGM(BGMID.CompassInMyPocket, "Content/Sounds/compass_in_my_pocket.mp3", 50);
            LoadBGM(BGMID.Gameover, "Content/Sounds/gameover.mp3", 100);
            LoadBGM(BGMID.Oops, "Content/Sounds/SE/!.mp3", 100);
            LoadBGM(BGMID.Levelup_e, "Content/Sounds/levelup_e.mp3", 100);
            LoadBGM(BGMID.Levelup_p, "Content/Sounds/levelup_p.mp3", 100);
            //MIDIの再生が遅いのはたぶん仕様だと思います
        }
        public void Close() {
            SendString("close all");
        }
        public void Update() {
            if(SendString("status", playing, "position") != 0) return;
            int i = (int)playing;
            if(i < 0) return;
            int le = loopEnd[i];
            if(le <= 0 || int.Parse(buffer.ToString()) < le - 4) return;
            int lp = loopPoint[i];
            if(lp < 0) { playing = BGMID.None; return; }
            SendString("play", playing, "from " + lp + " notify");
        }
        /// <summary>
        /// 音楽を読み込む
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="path">パス</param>
        /// <param name="loop">ループ開始点（midi：1/16小節、それ以外：ミリ秒、省略でループなし）</param>
        /// <param name="loopend">ループ終了点（midi：1/16小節、それ以外：ミリ秒、省略でファイルの最後からループ）</param>
        void LoadBGM(BGMID id, string path, int volume, int loop = -1, int loopend = -1) {
            int i = (int)id;
            fileNames[i] = path;
            int t = SendString("open", path);
            if(t != 0){
#if DEBUG
                mciGetErrorString(t, buffer = new StringBuilder(128), 128);
                System.Diagnostics.Debug.Print(fileNames[i] + " : " + buffer.ToString());
#endif
                fileNames[i] = null;
                return;
            }
            isMidi[i] = System.IO.Path.GetExtension(path) == ".mid";
            loopPoint[i] = loop;
            loopEnd[i] = loopend;
            volumes[i] = volume;
        }

        /// <summary>
        /// BGMを最初から再生する
        /// </summary>
        /// <param name="ignoreSame">再生中の音楽と同じ場合は無視するフラグ</param>
        public bool PlayBGM(BGMID id, bool ignoreSame) {
            if(ignoreSame && (id == playing || id == interrupted)) return false;
            if(playing != BGMID.None) StopBGM();
            playing = id;
            interrupted = BGMID.None;
            int t;
            int i = (int)id;
                t = SendString("play", id, "from " + start[i] + " notify");
            ChangeBGMVolume(Volume * volumes[i] / 100);
            return t == 0;
        }
        /// <summary>
        /// BGMを割り込ませる（ジングル用）
        /// </summary>
        /// <param name="ignoreSame">再生中の音楽と同じ場合は無視するフラグ</param>
        public bool InteruptBGM(BGMID id, bool ignoreSame) {
            if(ignoreSame && id == playing) return false;
            if(playing != BGMID.None) PauseBGM();
            if(interrupted == BGMID.None)
                interrupted = playing;
            playing = id;
            int t;
            int i = (int)id;
            if(i >= 0 && loopEnd[i] != -1)
                t = SendString("play", id, "from 0 to " + loopEnd[i] + " notify");
            else
                t = SendString("play", id, "from 0 notify");
            ChangeBGMVolume(Volume);
            return t == 0;
        }
        /// <summary>
        /// 音楽を止める
        /// </summary>
        public bool StopBGM() {
            int t = SendString("stop", playing);
            playing = BGMID.None;
            return t == 0;
        }
        /// <summary>
        /// 音楽を一時停止する
        /// </summary>
        public bool PauseBGM() {
            return SendString("pause", playing) == 0;
        }
        /// <summary>
        /// 一時停止した音楽を再生する
        /// </summary>
        public bool ResumeBGM() {
            return SendString("resume", playing) == 0;
        }
        /// <summary>
        /// 現在の再生位置を得る（midi：midi：1/16小節、それ以外：ミリ秒）
        /// </summary>
        public string GetNowTime() {
            if(SendString("status", playing, "position") != 0) return "0";
            if(isMidi[(int)playing])
                return TimeToStr2(int.Parse(buffer.ToString()));
            return TimeToStr(int.Parse(buffer.ToString()));
        }
        /// <summary>
        /// 曲の長さを得る（midi：midi：1/16小節、それ以外：ミリ秒）
        /// </summary>
        public string GetLength() {
            if(SendString("status", playing, "length") != 0) return "0";
            if(isMidi[(int)playing])
                return TimeToStr2(int.Parse(buffer.ToString()));
            return TimeToStr(int.Parse(buffer.ToString()));
        }

        string TimeToStr(int miliseconds) {
            return miliseconds / 60000 + ":" + (miliseconds / 1000 % 60).ToString("00") + "." + (miliseconds % 1000).ToString("000");
        }
        string TimeToStr2(int time) {
            return time / 16 + ":" + (time % 16).ToString("00");
        }
        bool ChangeBGMVolume(int value) {
            return SendString("setaudio", playing, "volume to " + value) == 0;
        }
        /// <summary>
        /// イベント受取り（ループ）用のダミーフォーム
        /// </summary>
        class DummyHandler: System.Windows.Forms.Form {
            MusicPlayer parent;
            public DummyHandler(MusicPlayer parent) {
                this.parent = parent;
            }
            protected override void WndProc(ref System.Windows.Forms.Message m) {
                switch(m.Msg) {
                    case 0x03B9:
                        if(m.WParam.ToInt32() == 1) Loop();
                        break;
                }
                base.WndProc(ref m);
            }
            void Loop() {
                string str = parent.GetNowTime();
                if(parent.interrupted != BGMID.None) {
                    parent.playing = parent.interrupted;
                    parent.ResumeBGM();
                    parent.interrupted = BGMID.None;
                    return;
                }
                /*int i = (int)parent.playing;
                if(i < 0) return;
                int lp = parent.loopPoint[i];
                if(lp < 0) { parent.playing = BGMID.None; return; }
                int le = parent.loopEnd[i];
                if(le >= 0)
                    parent.SendString("play", parent.playing, "from " + lp + " to " + le + " notify");
                else
                    parent.SendString("play", parent.playing, "from " + lp + " notify");*/
            }
        }
    }
    enum BGMID {
        None = -1, GrandGrass, Levelup_e, Levelup_p,
        BottomRain, CompassInMyPocket, DarkStep, Doukutu,
        Otsukare, PlaceToReturn, Result, Gameover,
        CompassTitle, Stepin, Oops,
    }
}
