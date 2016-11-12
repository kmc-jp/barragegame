using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

namespace CommonPart {
    /// <summary>
    /// ゲーム開始後の処理を書いたクラス
    /// </summary>
    class MapScene : Scene {
        #region Variable
        Map nMap;

        #endregion
        #region Method
        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="stage">indexではない、1がステージ1のデータを指定している。</param>
        public MapScene(SceneManager s,int stage)
            : base(s) {
            nMap = new Map(stage);
        }

        /// <summary>
        /// ゲーム画面の描画メソッド
        /// </summary>
        /// <param name="d"></param>
        public override void SceneDraw(Drawing d) {
            // マップの描画
            nMap.Draw(d);
            
        }
        public override void SceneUpdate() {
            base.SceneUpdate();
            
            nMap.update(Input);
            if (Map.step[0] < 0) { Delete = true; }
        }
        #endregion
    }// class end
}// namespace end
