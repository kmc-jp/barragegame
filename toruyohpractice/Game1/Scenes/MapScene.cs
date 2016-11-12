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
        int stage;
        Window_WithColoum window;
        bool gameOver=false;
        #endregion
        #region Method
        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="stage">indexではない、1がステージ1のデータを指定している。</param>
        public MapScene(SceneManager s,int _stage)
            : base(s) {
            stage = _stage;
            nMap = new Map(stage);
        }
        private void RetryThisMap()
        {
            nMap = null;
            nMap=new Map(stage);
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
            if (!gameOver && Map.mapState.Contains(Map.gameOver) && Map.stop_time == DataBase.motion_inftyTime && Map.readyToStop_time <= 0)
            {
                gameOver = true;
                window = null;
                window = new Window_WithColoum(90,220,1100,270);
                window.assignBackgroundImage("1100x270メッセージウィンドゥ");
                int nx = 600, ny = 40;
                window.AddColoum(new Button(nx, ny,"Retry","",Command.buttonPressed1,false));
                ny +=270/2;
                window.AddColoum(new Button(600, 45, "BackToTitle", "", Command.buttonPressed1, false));
            }
            else if(!gameOver)
            {
                nMap.update(Input);
            }else
            {
                window.update((KeyManager)Input, mouse);
                switch (window.commandForTop)
                {
                    case Command.buttonPressed1:
                        RetryThisMap();
                        break;
                    case Command.buttonPressed2:
                        Delete = true;
                        break;
                }
            }
            if (Map.step[0] < 0) { Delete = true; }
        }
        #endregion
    }// class end
}// namespace end
