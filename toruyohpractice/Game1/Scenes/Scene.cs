using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonPart;

namespace CommonPart {
    /// <summary>
    /// シーン（ゲームの場面）を扱う基底クラス
    /// </summary>
    abstract class Scene {
        #region 変数
        protected readonly SceneManager scenem;
        protected readonly WorkerManager manager;
        protected InputManager Input { get { return scenem.Input; } }
        /// <summary>
        /// 削除フラグ　trueで終了
        /// </summary>
        public bool Delete;
        /// <summary>
        /// 削除フラグ（旧仕様＝Topのみ消去）　trueで終了
        /// </summary>
        public bool Delete2;
        /// <summary>
        /// 巻き戻しDraw中に描画を無視する
        /// </summary>
        public bool IgnoreDraw;
        /// <summary>
        /// 発生からの時間
        /// </summary>
        public int Frame { get; private set; }
        #endregion
        #region 関数
        public Scene(SceneManager scene) {
            scenem = scene;
            manager = new WorkerManager(scene.Input);
            scene.AddScene(this);
        }
        public void Update() {
            manager.Update();
            Frame++;
        }
        public void Draw(Drawing d) {
            manager.Draw(d);
        }
        /// <summary>
        /// Scene自体のDraw　Drawの後に呼ばれる
        /// </summary>
        public virtual void SceneDraw(Drawing d) { }
        /// <summary>
        /// Scene自体の更新　Updateの後に呼ばれる
        /// </summary>
        public virtual void SceneUpdate() { }
        /// <summary>
        /// Sceneが他のものに移った時に呼ばれる
        /// </summary>
        //public virtual void Changed() { }
        /// <summary>
        /// Sceneが他のものから移った時に呼ばれる
        /// </summary>
        public virtual void Focused() { }
        /// <summary>
        /// Sceneが消されたときに呼ばれる
        /// </summary>
        public virtual void Deleted() { }
        #endregion
    }
    interface ICloseable {
        void Close();
    }
}
