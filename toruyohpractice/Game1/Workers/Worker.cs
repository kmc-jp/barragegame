using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CommonPart {
    abstract class Worker {
        #region 変数
        /// <summary>
        /// マネージャー
        /// </summary>
        protected WorkerManager manager;
        /// <summary>
        /// 出現してからの時間
        /// </summary>
        public int Frame { get; private set; }
        /// <summary>
        /// 消去フラグ（trueにすると消える）
        /// </summary>
        public bool Delete;
        /// <summary>
        /// WorkerのType（overrideして使う、処理順に関係）
        /// </summary>
        public abstract WorkerType Type { get; }
        #endregion
        #region 関数
        protected Worker(WorkerManager w) {
            manager = w;
            //Workerのリストに加える
            w.AddWorker(this);
        }
        /// <summary>
        /// 更新　毎フレーム行われる
        /// </summary>
        public abstract void Update();
        /// <summary>
        /// フレームのカウントを増やすだけ
        /// </summary>
        public virtual void Update2() {
            Frame++;
        }
        /// <summary>
        /// 描画　スペックによっては毎フレーム行われるとは限らない
        /// </summary>
        /// <param name="d"></param>
        public abstract void Draw(Drawing d);
        #endregion
    }
    abstract class WorkerWithPos : Worker {
        /// <summary>
        /// 位置
        /// </summary>
        public Vector Pos;
        /// <summary>
        /// 画面外自然消滅の距離
        /// </summary>
        protected double AutoDelete = -1;
        protected WorkerWithPos(WorkerManager w, Vector pos)
            : base(w) {
            Pos = pos;
        }
        /// <summary>
        /// AutoDelete + フレームのカウントを増やすだけ
        /// </summary>
        public override void Update2() {
            if(AutoDelete >= 0 && !InScreen(AutoDelete)) Delete = true;
            base.Update2();
        }
        public bool InScreen(double span) {
            return !(Pos.X < -span || Pos.Y < -span || Pos.Y > Game1.WindowSizeX + span || Pos.X > Game1.WindowSizeY + span);
        }
    }
    /// <summary>
    /// Workerの種類（この順番が処理順）
    /// </summary>
    enum WorkerType {
        Manager,
        Picture,
    }
}
