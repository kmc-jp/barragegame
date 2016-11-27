using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonPart;

namespace CommonPart
{
    abstract class Unit
    {
        #region basic variables
        /// <summary>
        /// スクリーン上の座標
        /// </summary>
        public double x, y;
        /// <summary>
        /// マップ上の座標
        /// </summary>
        public int x_index,y_index;
        /// <summary>
        /// 当たり判定用の半径
        /// </summary>
        public virtual double radius { get; set; } = 0;
        /// <summary>
        /// usually it is 100, which is 100 % rate
        /// </summary>
        public int zoom_rate;

        /// <summary>
        /// スクリーン上のこのUnitの中心座標x
        /// </summary>
        public virtual double cx_s { get { return x; } }
        /// <summary>
        /// スクリーン上のこのUnitの中心座標y
        /// </summary>
        public virtual double cy_s { get { return y; } }
        #endregion

        #region constructor
        /// <summary>
        /// only assign x,y. do nothing else
        /// </summary>
        /// <param name="_x"></param>
        /// <param name="_y"></param>
        public Unit(double _x,double _y)
        {    x = _x; y = _y;    }
        #endregion

        #region method
        //Functions around skill are all added to Enemy 
        public void moveToScreenPos_now(double _x,double _y) { x = _x; y = _y; }
        /// <summary>
        /// マップ上のこの座標へ移動させる
        /// </summary>
        /// <param name="x_index2"></param>
        /// <param name="y_index2"></param>
        public void movetoMapPos_now(int x_index2,int y_index2)
        {
            x_index = x_index2;
            y_index = y_index2;
        }
        #endregion
/*
        /// <summary>
        /// Unitはマップ上の座標を持つので、それをスクリーン上の座標に変える。マップでは左下0,0. screenは左上が0,0
        /// </summary>
        /// <param name="Xrate">マップのx座標の1 がスクリーンのXrateとなる</param>
        /// <param name="Yrate">map y座標の1 がscreen Yrateとなる</param>
        /// <param name="ltx">スクリーン上に見えるマップの左上のマップのx座標</param>
        /// <param name="lty">スクリーン上のマップの左上のマップのy座標。</param>
        /// <param name="leftsideX">スクリーン上の(マップの左上)のスクリーンのx座標。</param>
        /// <param name="topsideY">スクリーン上の(マップの左上)のスクリーンのy座標。</param>
        /// <returns>スクリーン上の座標である。そのままdrawに使えるかな</returns>
        public virtual Vector getPosInScreen(double Xrate,double Yrate,double ltx,double lty,double leftsideX,double topsideY)
        {
            //x_index はmapの上の座標である。 x_index*Xrateはあくまでmapをscreenと同じ縮尺にした際のmap上の座標
            //(x_index-ltx)とすると、今スクリーン上に見えるマップの最左辺からの相対距離が求まる
            return new Vector((x_index-ltx) * Xrate -leftsideX, (lty - y_index ) * Yrate-topsideY);
        }
*/
        public abstract void draw(Drawing d);
    }// Unit end
}// namespace CommonPart End
