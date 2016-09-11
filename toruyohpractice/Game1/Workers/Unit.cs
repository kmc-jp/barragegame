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
        public int real_w;// true,origin, absolute w,h
        public int real_h;
        public int zoom_rate; // usually it is 100, which is 100 % rate
        public int hp;
        public UnitType unit_type;
        public int[] skillIDs;
        public Effect[] effects;
        public string name;
        #endregion

        #region private
        private int frame_now;
        #endregion

        #region constructor
        public Unit(int _x_index, int _y_index, UnitType _unit_type, int _hp)
        {
            x_index = _x_index;
            y_index = _y_index;
            unit_type = _unit_type;
            hp = _hp;
            name = unit_type.getTypename();
        }
        public Unit(int x_index, int y_index, UnitType unit_type) :this(x_index, y_index,unit_type, unit_type.maxhp)
        {        }
        #endregion
        #region get property in int[] + string[]
        public List<int> getListIntData()
        {
            //return 
            int[] a = {
                x_index, //0th
                y_index,
                real_w,
                real_h,
                zoom_rate,
                hp,
            //any other int variables should be added here
            };
            List<int> b = new List<int>(a);
            b.AddRange(skillIDs);

            return b;

        }
        public string[] getStringData()
        {
            return new string[] {
                name, //0th
                unit_type.getTypename(), 

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

        public UnitType getUnitype()
        {
            return unit_type;
        }
        #endregion

    }// Unit end
}// namespace CommonPart End
