using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Microsoft.Xna.Framework;
using NAudio;
using NAudio.Wave;
using NAudio.CoreAudioApi;

// NAudio（Ms-PL ライセンス）http://naudio.codeplex.com　を利用しています。
// 上記の程度の内容をクレジットやマニュアルに記載したほうがよいでしょう。 

namespace CommonPart
{
    enum BGMID
    {
        None = -1, title, Stage1onWay, Stage1Boss, Stage2onWay, Stage2Boss, Stage3onWay, Stage3Boss, Stage4onWay, Stage4Boss,
    }
    /// <summary>
    /// BGMを再生するためのクラス（with WASAPI in NAudio）
    /// </summary>
    //mp3のみを想定していますがちょっと変えればwaveも行けます
    //2016/11/03 wavにしています
    //2016/11/04 ここにはbgmDatasを用意する.  これの元でBGMを登録する
    // 実際MusicPlayer2のload()はDataBaseのstaticやstaticなコンストラクターより先に実行される
    class MusicPlayer2
    {
        //BGMIDを同じにしないでください。曲名を省略するとファイルパスからファイル名を取得し、それをそのまま曲名とする。
        public BGMdata[] bgmDatas = new BGMdata[] {
            new BGMdata("Content/barragetitle.wav",BGMID.title,100), // ループしない曲はループ情報を省略するだけ
            new BGMdata("Content/Stage2_Boss.wav",BGMID.Stage2Boss,100,6479,232083), //ファイル名が曲名になる
            new BGMdata("Content/stage４ボス.wav",BGMID.Stage4Boss,100,"花弁",47340,207970),
        };


        //このプロパティはゲージ用のプログラムを手抜きするためで、普通はいらないと思います
        public PlayerSet GetPlayingSet { get { return playingSet; } }
        public BGMID GetPlayingID { get { return playing; } }

        static readonly int length = Enum.GetNames(typeof(BGMID)).Length - 1;
        string[] fileNames = new string[length];
        long[] loopPoint = new long[length];
        long[] loopEnd = new long[length];
        int[] volumes = new int[length];
        BGMID playing = BGMID.None;
        BGMID interrupted = BGMID.None;
        PlayerSet playingSet;
        PlayerSet interruptedSet;

        bool setEnable { get { return playingSet != null && playingSet.Enable; } }

        /// <summary>
        /// 音量　0～1000で指定
        /// </summary>
        public int Volume
        {
            get { return _volume; }
            set { _volume = Math.Max(0, Math.Min(1000, value)); ChangeBGMVolume(_volume); }
        }
        int _volume = 800;

        public MusicPlayer2()
        {
            Load();
        }
        public void Close()
        {
            if (interruptedSet != null) interruptedSet.Close();
            Close_();
        }
        void Close_()
        {
            if (playingSet != null) playingSet.Close();
        }
        void Load()
        {
            foreach (BGMdata bd in bgmDatas)
            {
                SetBGM(bd.bgmId, bd.filePath, bd.volume, bd.millisecond_loopStart, bd.millisecond_loopEnd);
            }
            //SetBGM(BGMID.Stage4Boss, "Content/stage４ボス.wav", 100);
            //SetBGM(BGMID, "Content/....." filePath, volume, millisecond_loopStart, millisecond_loopEnd);
        }
        /// <summary>
        /// 音楽の情報をセットする
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="path">パス</param>
        /// <param name="loop">ループ開始点（1チャンネル当たりのサンプル単位、省略でループなし）</param>
        /// <param name="loopend">ループ終了点（1チャンネル当たりのサンプル単位、省略でファイルの最後からループ）</param>
        void SetBGM(BGMID id, string path, int volume, long loop = -1, long loopend = -1)
        {
            int i = (int)id;
            fileNames[i] = path;
            loopPoint[i] = loop;
            loopEnd[i] = loopend;
            volumes[i] = volume;
        }

        bool SetPlayerSet(BGMID id)
        {
            try
            {
                playing = id;
                int i = (int)id;
                playingSet = new PlayerSet(fileNames[i], loopPoint[i], loopEnd[i]);
                return true;
            }
            catch
            {
                if (playingSet != null) playingSet.Close();
                playing = BGMID.None;
                playingSet = null;
                return false;
            }
        }

        /// <summary>
        /// BGMを最初から再生する
        /// </summary>
        /// <param name="ignoreSame">再生中の音楽と同じ場合は無視するフラグ</param>
        public bool PlayBGM(BGMID id, bool ignoreSame)
        {
            if (interrupted != BGMID.None)
            { //割り込み中
                if (id == playing)
                {
                    if (ignoreSame) return false;
                    else
                    { //再生中BGMなら割り込み状態を解除し、割り込まれBGMを開放し、最初に戻す
                        playingSet.Player.PlaybackStopped -= new EventHandler<StoppedEventArgs>(player_PlaybackStopped);
                        interruptedSet.Close();
                        interrupted = BGMID.None;
                        playingSet.Stream.ToBegin();
                        return true;
                    }
                }
                else if (id == interrupted)
                {
                    if (ignoreSame) return false;
                    else { StopBGM(); return true; }    //割り込まれているBGMなら割り込みを終了させる
                }
                else if (playing == BGMID.None)
                {   //BGMが割り込みだけなら割り込まれBGMを開放する
                    interruptedSet.Close();
                    interrupted = BGMID.None;
                }
                else
                { //関係ないBGMなら割り込み状態を解除し、再生中BGMと割り込まれBGMを開放し、BGMを流す
                    playingSet.Player.PlaybackStopped -= new EventHandler<StoppedEventArgs>(player_PlaybackStopped);
                    interruptedSet.Close();
                    interrupted = BGMID.None;
                    StopBGM();
                }
            }
            if (ignoreSame && id == playing) return false;
            if (playing != BGMID.None) StopBGM();
            if (!SetPlayerSet(id)) return false;
            playingSet.Player.Play();
            ChangeBGMVolume(Volume);
            return true;
        }
        /// <summary>
        /// BGMを割り込ませる（ジングル用）
        /// </summary>
        /// <param name="ignoreSame">再生中の音楽と同じ場合は無視するフラグ</param>
        public bool InteruptBGM(BGMID id, bool ignoreSame)
        {
            if (ignoreSame && id == playing) return false;
            if (interrupted == BGMID.None)
            { //割り込みが無い場合
                interrupted = playing;
                interruptedSet = playingSet;
                PauseBGM();
                if (!SetPlayerSet(id)) return false;
                playingSet.Player.PlaybackStopped += new EventHandler<StoppedEventArgs>(player_PlaybackStopped);
                playingSet.Player.Play();
                ChangeBGMVolume(Volume);
            }
            else if (id == playing)
            { //割り込み中BGMと同じ場合、最初に戻す
                playingSet.Stream.ToBegin();
            }
            else
            { //割り込み中BGMと異なる場合、割り込みBGMを差し替える
                playingSet.Player.PlaybackStopped -= new EventHandler<StoppedEventArgs>(player_PlaybackStopped);
                StopBGM();
                if (!SetPlayerSet(id)) return false;
                playingSet.Player.PlaybackStopped += new EventHandler<StoppedEventArgs>(player_PlaybackStopped);
                playingSet.Player.Play();
                ChangeBGMVolume(Volume);
            }
            return true;
        }

        /// <summary>
        /// 割り込みから元の曲に戻す用のイベント関数
        /// </summary>
        void player_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            Close_();
            playing = interrupted;
            playingSet = interruptedSet;
            interrupted = BGMID.None;
            interruptedSet = null;
            //if(playingSet != null)
            playingSet.Player.Play();
        }
        /// <summary>
        /// 音楽を止める
        /// </summary>
        public bool StopBGM()
        {
            Close_();
            playing = BGMID.None;
            return true;
        }
        /// <summary>
        /// 音楽を一時停止する
        /// </summary>
        public bool PauseBGM()
        {
            if (playing == BGMID.None) return false;
            playingSet.Player.Pause();
            return true;
        }
        /// <summary>
        /// 一時停止した音楽を再生する
        /// </summary>
        public bool ResumeBGM()
        {
            if (playing == BGMID.None) return false;
            playingSet.Player.Play();
            return true;
        }
        //本当はEnumとか使うべきですが、手を抜いてます
        /// <summary>
        /// 再生位置を変更する
        /// </summary>
        /// <param name="value">値</param>
        /// <param name="type">基準　0:曲の先頭　1:ループ開始　2:ループ終了　3:ファイルの終わり　+0:サンプル単位（1ch）　+100:ミリ秒単位</param>
        public void SetPosition(int value, int type)
        {
            if (playingSet == null) return;
            if (type >= 100)
            {
                value *= playingSet.Stream.WaveFormat.AverageBytesPerSecond / 1000;
                type -= 100;
            }
            else
                value *= playingSet.Stream.BytesPer1chSample;
            switch (type)
            {
                case 0: playingSet.Stream.Position = value; break;
                case 1: playingSet.Stream.Position = playingSet.Stream.LoopBegin + value; break;
                case 2: playingSet.Stream.Position = playingSet.Stream.LoopEnd + value; break;
                case 3: playingSet.Stream.Position = playingSet.Stream.Length + value; break;
            }
        }
        /// <summary>
        /// 現在の再生位置を得る
        /// </summary>
        public string GetNowTime()
        {
            return playingSet == null ? "-" : TimeToStr(playingSet.Stream.Position, playingSet.Stream.WaveFormat.SampleRate, playingSet.Stream.BytesPer1chSample);
        }
        /// <summary>
        /// 曲の長さを得る
        /// </summary>
        public string GetLength()
        {
            return playingSet == null ? "-" : TimeToStr(playingSet.Stream.Length, playingSet.Stream.WaveFormat.SampleRate, playingSet.Stream.BytesPer1chSample);
        }

        public string LoopBegin()
        {
            return playingSet == null ? "-" : loopPoint[(int)playing] < 0 ? "-" : TimeToStr(playingSet.Stream.LoopBegin, playingSet.Stream.WaveFormat.SampleRate, playingSet.Stream.BytesPer1chSample);
        }
        public string LoopEnd()
        {
            return playingSet == null ? "-" : loopEnd[(int)playing] < 0 ? "-" : TimeToStr(playingSet.Stream.LoopEnd, playingSet.Stream.WaveFormat.SampleRate, playingSet.Stream.BytesPer1chSample);
        }

        string TimeToStr(long samples, int sampleRate, int bytesPer1chSample)
        {
            samples /= bytesPer1chSample;
            long miliseconds = samples * 1000 / sampleRate;
            return miliseconds / 60000 + ":" + (miliseconds / 1000 % 60).ToString("00") + "." + (miliseconds % 1000).ToString("000") + " (" + samples.ToString().PadLeft(10) + "samples)";
        }
        bool ChangeBGMVolume(int value)
        {
            if (playingSet == null) return false;
            playingSet.Channel.Volume = value * volumes[(int)playing] * 0.00001f;
            return true;
        }
        public class WaveStreamWithLoopPoint : WaveStream
        {
            WaveStream baseStream;
            public long LoopBegin { get; private set; }
            public long LoopEnd { get; private set; }
            public int BytesPer1chSample { get { return WaveFormat.BitsPerSample / 8 * WaveFormat.Channels; } }
            public WaveStreamWithLoopPoint(WaveStream baseStream, long begin, long end)
            {
                this.baseStream = baseStream;
                LoopBegin = begin * WaveFormat.SampleRate / 1000 * BytesPer1chSample;
                LoopEnd = end * WaveFormat.SampleRate / 1000 * BytesPer1chSample;
            }
            public override long Length { get { return baseStream.Length; } }
            public override long Position
            {
                get { return baseStream.Position; }
                set { baseStream.Position = value; }
            }
            public override WaveFormat WaveFormat
            {
                get { return baseStream.WaveFormat; }
            }
            public void ToBegin() { Position = 0; }
            public override int Read(byte[] buffer, int offset, int count)
            {
                int read = 0;
                int ptr = offset;
                while (count > 0)
                {
                    int bytes = count;
                    if (LoopEnd > 0) bytes = (int)Math.Min(count, LoopEnd - Position);
                    else bytes = (int)Math.Min(Length - Position, bytes);
                    bytes = baseStream.Read(buffer, ptr, bytes);
                    count -= bytes;
                    read += bytes;
                    ptr += bytes;
                    if (LoopBegin < 0) return read;
                    if (count > 0)
                        baseStream.Position = LoopBegin;
                }
                return read;
            }
        }
        /// <summary>
        /// 曲再生用のものをまとめたもの
        /// </summary>
        public class PlayerSet
        {
            public bool Enable { get; private set; }
            public readonly IWavePlayer Player;
            public readonly WaveStreamWithLoopPoint Stream;
            public readonly WaveChannel32 Channel;

            public PlayerSet(string fileName, long begin, long end)
            {
                Enable = true;
                Player = new WasapiOut(AudioClientShareMode.Shared, 1);
                try
                {
                    Channel = new WaveChannel32(new WaveFileReader(fileName));
                }
                catch
                {
                    Console.WriteLine("Player Set load error: " + fileName + " is not found.");
                    Console.WriteLine("Now inside: " + Directory.GetCurrentDirectory());
                }
                Stream = new WaveStreamWithLoopPoint(Channel, begin, end);
                Player.Init(Stream);
            }
            public void Close()
            {
                if (!Enable) return;
                Player.Stop();
                Stream.Dispose();
                Player.Dispose();
                Channel.Dispose();

                Enable = false;
            }
        }
    }
    static class WaveStreamExtension
    {
        public static int BytesPer1chSample(this WaveStream w) { return w.WaveFormat.BitsPerSample / 8 * w.WaveFormat.Channels; }
    }
}
