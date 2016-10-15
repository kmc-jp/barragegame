using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonPart {
    /// <summary>
    /// 文字列を表示するバーのクラス
    /// </summary>
    class StringBox : WindowBox {
        #region Variable
        // 表示する文字列とその位置のリスト
        List<RichText> texts;
        List<Vector> textPositions;
        #endregion
        #region Method
        // コンストラクタ
        public StringBox(Vector _pos, int _w, int _h)
            : base(_pos, _w, _h) {
            texts = new List<RichText>();
            textPositions = new List<Vector>();
        }
        public StringBox(Vector _pos, int _w, int _h, List<RichText> _texts, List<Vector> _stringPositions)
            : base(_pos, _w, _h) {
            if (_texts.Count != _stringPositions.Count) ;         // 例外処理
            texts = new List<RichText>();
            textPositions = new List<Vector>();
            texts.AddRange(_texts);
            textPositions = _stringPositions;
        }
        public StringBox(Vector _pos, int _w, int _h, List<string> _strings, List<Vector> _stringPositions)
            : base(_pos, _w, _h) {
            if (_strings.Count != _stringPositions.Count) ;         // 例外処理
            texts = new List<RichText>();
            textPositions = new List<Vector>();
            foreach (string st in _strings) texts.Add(new RichText(st));
            textPositions = _stringPositions;
        }

        // 表示する文字列を追加するメソッド
        public void AddString(string _str, Vector _pos) {
            texts.Add(new RichText(_str));
            textPositions.Add(_pos);
        }
        public void AddString(RichText _text, Vector _pos) {
            texts.Add(_text);
            textPositions.Add(_pos);
        }
        public void AddStrings(List<string> _strs, List<Vector> _poss) {
            foreach(string st in _strs) texts.Add(new RichText(st));
            textPositions.AddRange(_poss);
        }
        public void AddStrings(List<RichText> _texts, List<Vector> _poss) {
            texts.AddRange(_texts);
            textPositions.AddRange(_poss);
        }
        // 描画メソッドの上書き
        public new void Draw(Drawing d) {
            base.Draw(d);
            for(int i = 0; i < texts.Count; i++)
                texts[i].Draw(d, textPositions[i], DepthID.Status);
        }
        #endregion
    }// class end
}// namespace end
