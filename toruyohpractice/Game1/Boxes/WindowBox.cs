using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonPart;

namespace CommonPart {
    /// <summary>
    /// プレイ画面上のバーなどのクラス
    /// </summary>
    class WindowBox {

        #region Variable
        // 描画する位置（左上）の座標
        public Vector windowPosition;
        // 横と縦のマス目(16x16)の数
        int width, height;
        
        #endregion

        #region Method

        public WindowBox(Vector _pos, int _w, int _h) {
            windowPosition = _pos;
            width = _w;
            height = _h;
        }

        public void Draw(Drawing d) {
            // ボックスの背景を表示している
            // 左上と右上
            d.Draw(windowPosition,DataBase.box_flame[0],DepthID.Message);
            d.Draw(windowPosition + new Vector((width - 1) * 16d, 0d), DataBase.box_flame[2], DepthID.Message);
            // 上下の中央
            for (int i = 1; i < width - 1;i++) {
                d.Draw(windowPosition + new Vector(i * 16d, 0d), DataBase.box_flame[1], DepthID.Message);
                d.Draw(windowPosition + new Vector(i * 16d, (height - 1) * 16d), DataBase.box_flame[7], DepthID.Message);
            }
            // 左下と右下
            d.Draw(windowPosition + new Vector(0d, (height - 1) * 16d), DataBase.box_flame[6], DepthID.Message);
            d.Draw(windowPosition + new Vector((width - 1) * 16d, (height - 1) * 16d), DataBase.box_flame[8], DepthID.Message);
            // 左右の中央
            for (int i = 1; i < height - 1; i++) {
                d.Draw(windowPosition + new Vector(0, i * 16), DataBase.box_flame[3], DepthID.Message);
                d.Draw(windowPosition + new Vector((width - 1) * 16d, i * 16d), DataBase.box_flame[5], DepthID.Message);
            }
            // 真ん中
            for (int i = 1; i < width - 1; i++) {
                for (int j = 1; j < height - 1; j++) {
                    d.Draw(windowPosition + new Vector(i * 16d, j * 16d), DataBase.box_flame[4], DepthID.Message);
                }
            }
        }

        #endregion
    }// class end
}// namespace end
