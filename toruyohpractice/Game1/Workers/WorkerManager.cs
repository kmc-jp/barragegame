using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonPart {
    /// <summary>
    /// Workerをまとめる…が今回あまり使わない（TurnManagerをかわりに使うため）
    /// </summary>
    class WorkerManager {
        #region 変数
        public readonly InputManager Input;
        /// <summary>
        /// Worker管理用
        /// </summary>
        List<Worker>[] workers;
        /// <summary>
        /// Workerの種類数
        /// </summary>
        public readonly int workerTypeNum = Enum.GetNames(typeof(WorkerType)).Length;

        List<Worker> addSameType;
        int updating = -1;
        #endregion
        #region 関数
        public WorkerManager(InputManager input) {
            Input = input;
            workers = new List<Worker>[workerTypeNum];
            for(int i = 0; i < workerTypeNum; i++)
                workers[i] = new List<Worker>();
        }
        public void Update() {
            //アップデート！
            for(updating = 0; updating < workerTypeNum; updating++) {
                addSameType = new List<Worker>();
                foreach(Worker w in workers[updating]) {
                    w.Update2();
                    w.Update();
                }
                workers[updating].AddRange(addSameType);
            }
            //削除フラグが経ったのを全部消す
            for(int i = 0; i < workerTypeNum; i++)
                workers[i].RemoveAll(w => w.Delete);
        }
        public void Draw(Drawing d) {
            for(int i = 0; i < workerTypeNum; i++)
                foreach(Worker w in workers[i]) w.Draw(d);
        }
        /// <summary>
        /// Workerを指定のTypeに追加する
        /// </summary>
        /// <param name="w">追加するWorker</param>
        public void AddWorker(Worker w) {
            if(updating == (int)w.Type)
                addSameType.Add(w);
            else
                workers[(int)w.Type].Add(w);
        }
        /// <summary>
        /// typeのWorker一覧を取得する
        /// </summary>
        /// <param name="type">取得するtype</param>
        /// <returns>Workerの一覧</returns>
        public List<Worker> GetWorkerList(WorkerType type) {
            return workers[(int)type];
        }
        #endregion
    }
}
