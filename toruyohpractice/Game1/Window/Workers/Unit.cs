using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonPart;

namespace CommonPart
{
    class Unit
    {
        #region public
        public int x_index;
        public int y_index;
        public int w;// in game screen w,h
        public int h;
        //true Width and Heigh can be obtained with Texture.Width,Height
        /// <summary>
        /// usually it is 100, which is 100 % rate
        /// </summary>
        public int zoom_rate;
        public int hp;
        public UnitType unit_type { get; protected set; }

        public string name;


        #endregion

        #region protected
        protected int frame_now;
        #endregion

        #region constructor
        public Unit(int _x_index, int _y_index, UnitType _unit_type, string _name)
        {
            x_index = _x_index;
            y_index = _y_index;
            unit_type = _unit_type;
            name = _name;
        }
        public Unit(int _x_index, int _y_index, UnitType _unit_type) :this(_x_index, _y_index,_unit_type, _unit_type.typename)
        {        }
        public Unit(List<int> ins, List<string> strs):this(ins[0],ins[1],DataBase.getUnitType(strs[0]),strs[1])
        {        }
        #endregion
        #region get property in int[] + string[]
        public List<int> getListIntData()
        {
            //return 
            int[] a = {
                x_index, //0th
                y_index,
            //any other int variables should be added here
            };
            List<int> b = new List<int>(a);

            return b;

        }
        public string[] getStringData()
        {
            return new string[] {
                unit_type.typename,
                name, 
                //any other int variables should be added here
            };
        }
        #endregion
        #region method
        public void add_skill()
        {

        }
        
        public void change_unit_type(UnitType unit_type2)
        {
            unit_type = unit_type2;
        }

        public void moveto_now(int x_index2,int y_index2)
        {
            x_index = x_index2;
            y_index = y_index2;

        }
        #endregion

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
        public virtual void update() { }

        public virtual void draw(Drawing d)
        {
            ///MapEditorSceneはこれらをstaticで所持しているので、こう書けてしまう
            d.Draw(getPosInScreen(MapDataSave.Xrate, MapDataSave.Yrate,
                MapDataSave.ltx, MapDataSave.lty, MapDataSave.leftsideX, MapDataSave.topsideY),//ここまでが座標
                DataBase.getTex(unit_type.texture_name), DataBase.getRectFromTextureNameAndIndex(unit_type.texture_name, frame_now), DepthID.Enemy, (float)(zoom_rate)/100);

        }
    }// Unit end
}// namespace CommonPart End
