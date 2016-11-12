using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using CommonPart;

namespace CommonPart {
    /// <summary>
    /// 効果音を再生するクラス
    /// </summary>
    class SEPlayer {
        //mci
        [DllImport("winmm.dll")]
        static extern int mciSendString(string lpstrCommand, StringBuilder lpstrReturnString, int uReturnLength, IntPtr hwndCallback);
        [DllImport("winmm.dll")]
        static extern bool mciGetErrorString(int fdwError, StringBuilder lpszErrorText, int cchErrorText);
        /*[DllImport("winmm.dll")]
        static extern int waveOutSetVolume(IntPtr hwo, uint dwVolume);
        [DllImport("winmm.dll")]
        static extern int waveOutSetPitch(IntPtr hwo, uint dwPitch);
        [DllImport("winmm.dll")]
        static extern int waveOutSetPlaybackRate(IntPtr hwo, uint dwRate);*/

        int SendString(string command) {
            return mciSendString(command, buffer = new StringBuilder(256), 256, IntPtr.Zero);
        }
        int SendString(string command, string path, string command2 = "") {
            if(path == null) return -1;
            return SendString(command + " \"" + path + "\" " + command2);
        }
        int SendString(string command, SoundEffectID id, string command2 = "") {
            if(id == SoundEffectID.None) return -1;
            return SendString(command, id.ToString(), command2);
        }
        string ErrorString(int num) {
            mciGetErrorString(num, buffer = new StringBuilder(256), 256);
            return buffer.ToString();
        }
        /// <summary>
        /// 音量　0～100で指定　wavのみ
        /// </summary>
        public int Volume {
            get { return _volume; }
            set { _volume = Math.Max(0, Math.Min(100, value)); }
        }
        int _volume = 80;
        string[] soundEffects;
        /// <summary>
        /// 同フレームに多重再生させないフラグ
        /// </summary>
        bool[] soundEffectsPlayed;
        StringBuilder buffer;
        static readonly int seNum = Enum.GetNames(typeof(SoundEffectID)).Length;

        public SEPlayer() {
            Load();
        }
        void Load(){
            soundEffects = new string[seNum];
            soundEffectsPlayed = new bool[seNum];
            SetSE(SoundEffectID.Cursor_Move, "");
            SetSE(SoundEffectID.Cursor_Cancel, "");
            SetSE(SoundEffectID.Cursor_OK, "");
        }
        public void Update() {
            for(int i = 0; i < seNum; i++)
                soundEffectsPlayed[i] = false;
        }

        /// <summary>
        /// ファイルをセットする　※ファイルが存在しない場合も例外は出ません　出力->デバッグを確認
        /// </summary>
        void SetSE(SoundEffectID id, string fileName) {
            soundEffects[(int)id] = fileName;
            int t = SendString("open", fileName, " type mpegvideo alias " + id.ToString());
            if(t != 0) System.Diagnostics.Debug.WriteLine(id + ":\"" + fileName + "\"" + " エラー:" + ErrorString(t));
        }
        public bool Play(SoundEffectID id, int volume, float pitch) {
            if(id == SoundEffectID.None) return false;
            soundEffectsPlayed[(int)id] = true;

            //uint v = (uint)(volume * Volume * 0xffff / 10000);    こっちはBGMまでつられてダメでした…が今はこっちでも行けると思います
            //waveOutSetVolume(IntPtr.Zero, v * 0x10001u);
            //waveOutSetPlaybackRate(handle, (uint)(pitch * 0x10000));

            int t = SendString("play", id, "from 0");
            SendString("setaudio", id, "volume to " + volume * Volume / 10);
            SendString("set", id, "speed " + (int)(1000 * pitch));
            return t == 0;
        }
    }
    enum SoundEffectID {
        None,
        Cursor_Move, Cursor_OK, Cursor_Cancel,
        
    }
}
