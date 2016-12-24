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
        bool MapFulStop = false;
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
            window = null;
            gameOver = false; MapFulStop = false;
        }
        /// <summary>
        /// ゲーム画面の描画メソッド
        /// </summary>
        /// <param name="d"></param>
        public override void SceneDraw(Drawing d) {
            // マップの描画
            nMap.Draw(d);
            if (window!=null) window.draw(d); 
        }
        public override void SceneUpdate() {
            base.SceneUpdate();
            if (Input.IsKeyDown(KeyID.Escape)) { Delete = true; new StageSelectScene(scenem); }
            if (!MapFulStop && Map.mapState.Contains(Map.gameOver) && Map.stop_time == DataBase.motion_inftyTime && Map.readyToStop_time <= 0)
            {// gameOverに入ったので、準備をして、mapはもう更新しなくする
                #region gameOver starts as Map Scene. Create Window
                MapFulStop = true;
                gameOver = true;
                window = null;
                window = new Window_WithColoum(90, 220, 1100, 270);
                window.assignBackgroundImage("1100x270メッセージウィンドゥ");
                int nx = 520, ny = 100;
                window.AddColoum(new Button(nx, ny, "Retry", "", Command.buttonPressed1, false));
                ny += 60;
                window.AddColoum(new Button(nx, ny, "BackToTitle", "", Command.buttonPressed2, false));
                #endregion
                SoundManager.Music.PlayBGM(BGMID.None, true);
            } else if (!MapFulStop && Map.mapState.Contains(Map.backToStageSelection) && Map.stop_time == DataBase.motion_inftyTime && Map.readyToStop_time <= 0)
            {
                #region win 
                MapFulStop = true;
                window = null;
                window = new Window_WithColoum(90, 220, 1100, 270);
                window.assignBackgroundImage("1100x270メッセージウィンドゥ");
                int nx = 430, ny = 80;
                if ((stage == 6||stage==5)&&Game1.play_mode==-1)
                {
                    window.AddRichText("ALL CLEAR", new Vector(nx, ny));
                    nx = 0; ny = 0;
                    window.AddRichText("toal score : " + Map.score, new Vector(nx, ny));
                    nx = 430; ny = 180;
                    window.AddColoum(new Button(nx, ny, "THANK YOU FOR PLAYING!", "", Command.buttonPressed4, false));
                }
                else
                {
                    window.AddRichText(stage+"STAGE CLEAR", new Vector(nx, ny));
                    nx = 0; ny = 0;
                    window.AddRichText("toal score : " + Map.score, new Vector(nx, ny));
                    nx = 430; ny = 180;
                    window.AddColoum(new Button(nx, ny, "Go to next stage...Press Z key", "", Command.buttonPressed4, false));
                }
                #endregion
            }
            else if(!MapFulStop)
            {//gameOverに入っていないのでmapは更新する
                nMap.update(Input);
            }else if(window!=null)
            {// この時はwindowだけを操作する
                #region window update as GameIsOver
                window.update((KeyManager)Input, mouse);
                switch (window.commandForTop)
                {
                    case Command.buttonPressed1:
                        RetryThisMap();
                        break;
                    case Command.buttonPressed2:
                        Delete = true;
                        new TitleSceneWithWindows(scenem);
                        break;
                    case Command.buttonPressed3:
                        close();
                        if (Game1.play_mode == -1)
                        {
                            new MapScene(scenem, stage);
                        }
                        else if(Game1.play_mode==1)
                        {
                            new StageSelectScene(scenem);
                        }
                        SoundManager.Music.PlayBGM(BGMID.None, true);
                        break;
                    case Command.buttonPressed4:
                        close();
                        if (Game1.play_mode == -1)
                        {
                            if (stage < 3)
                            {
                                new MapScene(scenem, stage + 1);
                            }else if (stage == 3)//応急処置 stage4,5がないため
                            {
                                new MapScene(scenem, 6);
                            }else
                            {
                                new TitleSceneWithWindows(scenem);
                            }
                        }
                        else if(Game1.play_mode==1)
                        {
                            new StageSelectScene(scenem);
                        }
                        SoundManager.Music.PlayBGM(BGMID.None, true);
                        break;
                }
                #endregion
            }
            if (Map.step[0] < 0) { Delete = true; }
        }
        #endregion

        protected void close()
        {
            Delete = true;
        }
    }// class end
}// namespace end
