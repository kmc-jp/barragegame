//#define SHOWTIME
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonPart {
    /// <summary>
    /// Sceneを管理するクラス
    /// </summary>
    class SceneManager {
        #region 変数
        /// <summary>
        /// 入力を扱うクラス　ゲーム内で基本的にはこれ一つ
        /// </summary>
        public readonly InputManager Input = new KeyManager();
        /// <summary>
        /// 現在のScene
        /// </summary>
        Scene nowScene { get { return scene.Last(); } }
        /// <summary>
        /// Sceneの一覧
        /// </summary>
        List<Scene> scene;
        /// <summary>
        /// 描画用クラス
        /// </summary>
        Drawing draw;

        /// <summary>
        /// Sceneをさかのぼって描画する数
        /// </summary>
        public int BackSceneNumber {
            get { return backSceneNumber; }
            set { backSceneNumber = Math.Max(0, value); }
        }
        int backSceneNumber;
        #endregion
        #region 関数
        public SceneManager(Drawing d) {
            draw = d;
            scene = new List<Scene>();
            new TitleScene(this);
        }

        double time;
        public bool Update() {
            //シーンがなかったら終わり
            if(scene.Count == 0) return false;
            //各種更新

            Input.Update();
            if (Input.IsKeyDownOld(KeyID.Select) == false && Input.IsKeyDown(KeyID.Select) == true)
            {

            }
            JoyPadManager.Update();
            draw.Update();
            Scene ns = nowScene;
            ns.Update();
            //Function.WatchStart();
            ns.SceneUpdate();
            //time = Function.WatchEnd();
            //現在（最後）のSceneが消去フラグが立ってる限り、シーンをリストから除外
            for(int i = 0; i < scene.Count; i++)
                if(scene.Count > 1 && scene[i].Delete) {
                    //nowScene.Changed();
                    BackSceneNumber--;
                    scene[i].Deleted();
                    scene.Remove(scene[i]);

                    i--;
                    if(i == scene.Count - 1)
                        scene[i].Focused();
                }
            while(scene.Count > 1 && nowScene.Delete2) {
                BackSceneNumber--;
                nowScene.Deleted();
                scene.Remove(nowScene);
                nowScene.Focused();
            }
            return scene.Count >= 1 && (scene.Count > 1 || !nowScene.Delete);
        }
        public void Draw() { 
            //Draw開始
            draw.Animate = true;
            //Sceneの描画

            for(int i = Math.Max(0, scene.Count - 1 - BackSceneNumber); i < scene.Count; i++) {
                if(i < scene.Count - 1 && scene[i].IgnoreDraw) continue;
                draw.SetDrawAbsolute();
                draw.DrawBegin();
                draw.Animate = i == scene.Count - 1;
                scene[i].Draw(draw);
                scene[i].SceneDraw(draw);
                draw.DrawEnd();
            }
            draw.DrawBegin();
#if DEBUG && SHOWTIME
            draw.SetDrawAbsolute();
            //new RichText(time.ToString("scene : 0.000ms")).Draw(draw, new Vector(380, 400), DepthID.Debug, 0.6f);
            string[] str = new string[] {
                "Position:", SoundManager.Music.GetNowTime(),
                "Length:", SoundManager.Music.GetLength(),
                "LoopBegin:", SoundManager.Music.LoopBegin(),
                "LoopEnd:", SoundManager.Music.LoopEnd()
            };
            StringBuilder bld = new StringBuilder(256);
            for(int i = 0; i < str.Length; ) {
                bld.Append(str[i++].PadRight(12)).Append(str[i++]).Append('\n');
            }
            new RichText(bld.ToString()).Draw(draw, new Vector(380, 420), DepthID.Debug, 0.6f);
            draw.SetDrawNormal();
#endif
            //Draw終わり
            draw.DrawEnd();
        }
        public void AddScene(Scene s) {
            //if(scene.Count != 0)
            //nowScene.Changed();
            scene.Add(s);
            //nowScene.Focused();
        }
        public bool IsTopScene(Scene s) {
            return s == nowScene;
        }
        public bool IsBottomScene(Scene s) {
            return s == scene.First();
        }
        /// <summary>
        /// 一気に特定のSceneまで戻る関数
        /// </summary>
        public void BackToScene(Type t) {
            int k = scene.FindLastIndex(x => t.Equals(x.GetType()));
            if(k >= 0) { BackSceneNumber -= scene.Count - k; scene = scene.GetRange(0, k + 1); nowScene.Focused(); }
        }
        public void Exit() { scene.Clear(); }
        #endregion
    }
}
