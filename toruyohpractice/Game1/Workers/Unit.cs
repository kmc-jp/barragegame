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
        #region basic variables
        /// <summary>
        /// スクリーン上の座標
        /// </summary>
        public double x,y;
        /// <summary>
        /// マップ上の座標
        /// </summary>
        public int x_index,y_index;
        //true Width and Heigh can be obtained with Texture.Width,Height
        /// <summary>
        /// usually it is 100, which is 100 % rate
        /// </summary>
        public int zoom_rate;
        public UnitType unitType { get { return DataBase.getUnitType(unitType_name); } }
        public string unitType_name;
        /// <summary>
        /// このUnitの名前になる.UnitTypeの名前とは無関係である。
        /// </summary>
        public string name;
        /// <summary>
        /// このUnitの様々な状態を表すに使えるもの. Replaceで文字列を消せる。
        /// </summary>
        public string label;
        #endregion
        public List<Skill> skills = new List<Skill>();
        #endregion

        #region protected
        protected int frame_now;
        #endregion

        #region constructor
        public Unit(int _x_index, int _y_index, string _unitTypeName, string _label,string _name=null)
        {
            x_index = _x_index;
            y_index = _y_index;
            unitType_name = _unitTypeName;
            if (_name != null)
            {
                name = _name;
            }else { name = unitType.typename; }
            label = _label;
        }
        public Unit(List<int> ins, List<string> strs):this(ins[0],ins[1],strs[0],strs[1],strs[2])
        {        }
        #endregion
        #region get property in int[] + string[]
        public List<int> getListIntData()
        {
            List<int> c = new List<int> { x_index,y_index};
            return c;

        }
        public string[] getStringData()
        {
            return new string[] {
                unitType_name,
                label, 
                name,
                //any other int variables should be added here
            };
        }
        #endregion
        #region method
        public void add_skill(string skillName)
        {
            if (DataBase.existSkillDataName(skillName))
            {
                skills.Add(new Skill(skillName));
            }else { Console.WriteLine("add_skill: Does not exist"+skillName); }
        }

        /// <summary>
        /// スキルのクールダウンをそのデータでのクールダウン*reduceByPercentまで増やす
        /// </summary>
        /// <param name="_skillName"></param>
        /// <param name="reduceByPercent"></param>
        public void add_skill_coolDown(string _skillName,float reduceByPercent)
        {
            foreach (Skill sk in skills)
            {
                if (sk.skillName == _skillName)
                {
                    sk.coolDown +=(int)(reduceByPercent*DataBase.getSkillData(_skillName).cooldownFps);
                }
            }
        }
        /// <summary>
        /// スキルのクールダウンをそのデータでのクールダウン*reduceByPercentまで増やす
        /// </summary>
        public void add_skill_coolDown(int index, float reduceByPercent)
        {
            skills[index].coolDown += (int)(reduceByPercent * DataBase.getSkillData(skills[index].skillName).cooldownFps);
        }
        public void set_skill_coolDown(string _skillName,int cd)
        {
            foreach(Skill sk in skills)
            {
                if (sk.skillName == _skillName)
                {
                    sk.coolDown = cd;
                }
            }
        }
        /// <summary>
        /// skillsの配列のiインデックスのスキルのクールダウンを設定する
        /// </summary>
        /// <param name="index">インデックスは常に0から始まている</param>
        /// <param name="cd"></param>
        public void set_skill_coolDown(int index, int cd)
        {
            if (index >= skills.Count) { return; }
            else { skills[index].coolDown = cd; }
        }


        public void change_unit_type(string unit_type2name)
        {
            unitType_name = unit_type2name;
        }
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
                DataBase.getTex(unitType.texture_name), DataBase.getRectFromTextureNameAndIndex(unitType.texture_name, frame_now), DepthID.Enemy, (float)(zoom_rate)/100);

        }
    }// Unit end
}// namespace CommonPart End
