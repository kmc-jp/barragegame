using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonPart
{
    /// <summary>
    /// BGMID,BGMの名前,BGMのファイルパス,ループ情報などを一つにまとめたものである
    /// </summary>
    class BGMdata
    {
        public long millisecond_loopStart;
        public long millisecond_loopEnd;
        public int volume;
        public string BGMname; // bgmがプログラムの中での名前。
        public BGMID bgmId; // bgmを放送するには、MusicPlayer2を使用しますが、その時に使われるのがBGMIDである。
        public string filePath;// このbgmファイルを取得するためのファイルへのパス

        /// <summary>
        /// bgmのパス,bgmが使うBGMID,ループ起点のミリ秒,ループ終点のミリ秒,bgmの名前
        /// </summary>
        /// <param name="_filePath">ファイルのパス</param>
        /// <param name="id"></param>
        /// <param name="_voloume"></param>
        /// <param name="_bgmName">ゲーム中での名前</param>
        /// <param name="_loopStartMillisecond">ループしないなら-1を入れる</param>
        /// <param name="_loopEndMillisecond">ループしないなら-1を入れる</param>
        public BGMdata(string _filePath, BGMID id, int _voloume, string _bgmName, long _loopStartMillisecond = -1, long _loopEndMillisecond = -1)
        {
            filePath = _filePath;
            if (_bgmName != null)
            {
                BGMname = _bgmName;
            }
            bgmId = id;
            volume = _voloume;
            millisecond_loopStart = _loopStartMillisecond;
            millisecond_loopEnd = _loopEndMillisecond;
        }
        /// <summary>
        /// 自動的にfilePathからこのbgmのファイル名を獲得する。
        /// </summary>
        /// <param name="_filePath"></param>
        /// <param name="id"></param>
        /// <param name="_loopStartMillisecond"></param>
        /// <param name="_loopEndMillisecond"></param>
        public BGMdata(string _filePath, BGMID id, int _voloume, long _loopStartMillisecond = -1, long _loopEndMillisecond = -1)
            : this(_filePath, id, _voloume, null, _loopStartMillisecond, _loopEndMillisecond)
        {
            BGMname = getFileNameFromFilePath(_filePath);
        }

        protected string getFileNameFromFilePath(string filePath, char da = '/', char db = '.')
        {
            if (filePath == null)
            { //Console.WriteLine("getFileName: filePath is null.");
                return filePath;
            }
            int ia = 0, ib = 0;
            for (int r = 0; r < filePath.Length; r++)
            {
                if (filePath[r] == da)
                {
                    ia = r;
                    //Console.WriteLine("Found / " + r.ToString() + " ");
                }
                else if (filePath[r] == db)
                {
                    ib = r;
                    //Console.WriteLine("Found . " + r.ToString() + " ");
                }
            }
            ia++;
            //if (ia <= 1) { Console.WriteLine("getFileName:" + filePath + " ia not Found?or the First. da is " + da); }
            //if (ib <= ia) { Console.WriteLine("getFileName:" + filePath + " ib<=ia; da is " + da + " db is " + db); }
            if (ib == 0)
            { //Console.WriteLine("getFileName:" + filePath + " not Found " + db); 
                ib = ia + 1;
            }
            //if (ib >= filePath.Length) { Console.WriteLine("getFileName:" + filePath + " ib>=length (ia is the last?)"); }
            return filePath.Substring(ia, ib - ia);
        }
    }
}
