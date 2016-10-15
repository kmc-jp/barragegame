using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace CommonPart {
    /// <summary>
    /// メッセージ履歴・表示を管理します
    /// </summary>
    class MessageManager {
        /// <summary>
        /// 履歴の長さ
        /// </summary>
        internal const int MesLength = 500;

        public int Length { get; private set; }
        public int LengthT { get; private set; }
        public int LengthS { get; private set; }
        
        /// <summary>
        /// メッセージ履歴
        /// </summary>
        string[] messages = new string[MesLength];
        /// <summary>
        /// 会話のみ寄せ集め
        /// </summary>
        string[] messagesTalk = new string[MesLength];
        /// <summary>
        /// システムメッセージのみ寄せ集め
        /// </summary>
        string[] messagesSystem = new string[MesLength];
        /// <summary>
        /// 表示位置
        /// </summary>
        int dispIndex = -1;
        int dispIndexT = -1;
        int dispIndexS = -1;
        /// <summary>
        /// 書き込み位置
        /// </summary>
        int index = -1;
        int indexT = -1;
        int indexS = -1;
        /// <summary>
        /// 表示時間
        /// </summary>
        int count;

        //ただの省略
        int _P(int x) { return (x + MesLength) % MesLength; }

        public MessageManager() :base() { }
        public void Update() {
            count--;
        }
        /// <summary>
        /// メッセージを追加する
        /// </summary>
        /// <param name="s">メッセージ</param>
        public void Add(string s, bool isTalk = false) {
            if(s.IndexOf('\n') >= 0) {
                string[] str = s.Split('\n');
                foreach(string st in str)
                    if(st.Trim() != "")
                        Add(st, isTalk);
                return;
            }
            index++;
            dispIndex++;
            messages[_P(index)] = s;
            if(Length < MesLength) Length++;
            count = 300;
            if(isTalk) {
                indexT++;
                dispIndexT++;
                messagesTalk[_P(indexT)] = s;
                if(LengthT < MesLength) LengthT++;
            } else {
                indexS++;
                dispIndexS++;
                messagesSystem[_P(indexS)] = s;
                if(LengthS < MesLength) LengthS++;
            }
        }
        public void Reset() {
            Length = 0;
            LengthT = 0;
            LengthS = 0;
            messages = new string[MesLength];
            messagesTalk = new string[MesLength];
            messagesSystem = new string[MesLength];
            dispIndex = -1;
            dispIndexT = -1;
            dispIndexS = -1;
            index = -1;
            indexT = -1;
            indexS = -1;
        }
        public string[] GetLastMessages(int length) {
            string[] array = new string[length];
            for(int i = 0; i < length; i++)
                array[length - 1 - i] = messagesSystem[_P(indexS - i)] ?? "";
            return array;
        }
        public void Draw(Drawing d, int x, int last, int length) {
            if(true) {
                d.SetDrawAbsolute();
                for(int i = Math.Max(-length + 1, last - MesLength + 1); i <= 0; i++)
                    new RichText(messages[_P(i + dispIndex - last)] ?? "", FontID.Medium, Color.White * Math.Min(1,(i + length) * 0.3f)).Draw(d, new Vector(x, 450 + i * 14), DepthID.Message, 0.7f);

                d.SetDrawNormal();
            }
        }
        public void DrawTalk(Drawing d, int x, int last, int length) {
            d.SetDrawAbsolute();
            for(int i = Math.Max(-length + 1, last - MesLength + 1); i <= 0; i++)
                new RichText(messagesTalk[_P(i + dispIndexT - last)] ?? "", FontID.Medium, Color.White * Math.Min(1, (i + length) * 0.3f)).Draw(d, new Vector(x, 450 + i * 14), DepthID.Message, 0.7f);
            d.SetDrawNormal();
        }
        public void DrawSystem(Drawing d, int x, int last, int length) {
            d.SetDrawAbsolute();
            for(int i = Math.Max(-length + 1, last - MesLength + 1); i <= 0; i++)
                new RichText(messagesSystem[_P(i + dispIndexS - last)] ?? "", FontID.Medium, Color.White * Math.Min(1, (i + length) * 0.3f)).Draw(d, new Vector(x, 450 + i * 14), DepthID.Message, 0.7f);
            d.SetDrawNormal();
        }
        #region Save
        public void SaveOrLoad(SaveManager s) {
            int ver = 1;
            s.ReadOrWrite(ref ver);
            switch(ver) {
                case 1: SaveOrLoad1(s); break;
                default: throw new SaveLoadException_Inner("Messageのバージョンが異常です");
            }
        }
        public void SaveOrLoad1(SaveManager s) {
            int l = MesLength;
            s.ReadOrWrite(ref l);
            s.ReadOrWrite(ref dispIndex);
            s.ReadOrWrite(ref dispIndexT);
            s.ReadOrWrite(ref dispIndexS);
            s.ReadOrWrite(ref index);
            s.ReadOrWrite(ref indexT);
            s.ReadOrWrite(ref indexS);
            if(s.IsReadMode) {
                int x = 0; ;
                s.ReadOrWrite(ref x); Length = x;
                s.ReadOrWrite(ref x); LengthT = x;
                s.ReadOrWrite(ref x); LengthS = x;
                for(int i = 0; i < l; i++) {
                    s.ReadOrWrite(ref messages[i]);
                    s.ReadOrWrite(ref messagesSystem[i]);
                    s.ReadOrWrite(ref messagesTalk[i]);
                }
            } else {
                s.Write(Length);
                s.Write(LengthT);
                s.Write(LengthS);
                for(int i = 0; i < MesLength; i++) {
                    s.Write(messages[i] ?? "");
                    s.Write(messagesSystem[i] ?? "");
                    s.Write(messagesTalk[i] ?? "");
                }
            }
        }
        #endregion
    }
}
