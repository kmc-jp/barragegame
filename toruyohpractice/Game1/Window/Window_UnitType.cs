using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonPart
{ 
    class Window_UnitType : Window_WithColoum
    {
        public UnitType ut;
        public List<int> utIntList = new List<int>();
        public List<string> utStringList = new List<string>();

        public Window_UnitType(UnitType _ut, int _x, int _y, int _w, int _h) : base(_x, _y, _w, _h)
        {
            ut = _ut;
            setup_unitType_window(ut);
        }

        /// <summary>
        /// これはUnitTypeの設定に応じて書き換えが必要あるでしょう。
        /// </summary>
        public void setup_unitType_window(UnitType _ut)
        {
            if (_ut != null) { ut = _ut; } else { Console.WriteLine("Window: null UnitType"); }
            switch (ut.genre)
            {
                case (int)Unit_Genre.textured:
                    setup_textured_unitType(ut);
                    break;
                case (int)Unit_Genre.animated:
                    setup_animated_unitType((AnimatedUnitType)ut);
                    break;
                default:
                    Console.WriteLine("Unknown Genre of UnitType! " + ut.genre);
                    break;
            }

        }

        public void setup_animated_unitType(AnimatedUnitType ut)
        {
            int dy = 20;
            int ny = y;
            if (ut == null) { Console.WriteLine("Window: null UnitType"); }
            clear_old_data_and_put_in_now_data();
            int n = 0;
            /*genre, //0th         
            passableType,   
            */
            coloums.Add(create_blank(Command.apply_int, x, ny, "genre", utIntList[n].ToString()));
            n++; ny += dy;
            coloums.Add(create_blank(Command.apply_int, x, ny, "passableType", utIntList[n].ToString()));
            n++; ny += dy;

            /* texture_name,
               typename,
               label,
            */
            n = 0;
            coloums.Add(create_blank(Command.apply_string, x, ny, "texture_name", utStringList[n]));
            n++; ny += dy;
            coloums.Add(create_blank(Command.apply_string, x, ny, "typename", utStringList[n]));
            n++; ny += dy;
            coloums.Add(create_blank(Command.apply_string, x, ny, "label", utStringList[n]));
            n++; ny += dy;
        }
        public void setup_textured_unitType(UnitType ut)
        {
            int dy = 20;
            int ny = y;
            if (ut == null) { Console.WriteLine("Window: null UnitType"); }
            clear_old_data_and_put_in_now_data();
            int n = 0;
            /* genre // 0th
            texture_max_id,
            texture_min_id,         
            passableType,   
            */
            coloums.Add(create_blank(Command.apply_int, x, ny, "genre", utIntList[n].ToString()));
            n++; ny += dy;
            coloums.Add(create_blank(Command.apply_int, x, ny, "texture_max_id", utIntList[n].ToString()));
            n++; ny += dy;
            coloums.Add(create_blank(Command.apply_int, x, ny, "texture_min_id", utIntList[n].ToString()));
            n++; ny += dy;
            coloums.Add(create_blank(Command.apply_int, x, ny, "passableType", utIntList[n].ToString()));
            n++; ny += dy;

            /* texture_name,
               typename,
               label,
            */
            n = 0;
            coloums.Add(create_blank(Command.apply_string, x, ny, "texture_name", utStringList[n]));
            n++; ny += dy;
            coloums.Add(create_blank(Command.apply_string, x, ny, "typename", utStringList[n]));
            n++; ny += dy;
            coloums.Add(create_blank(Command.apply_string, x, ny, "label", utStringList[n]));
            n++; ny += dy;
        }

        public void clear_old_data_and_put_in_now_data()
        {
            utIntList.Clear();
            utStringList.Clear();
            utIntList.AddRange(ut.getIntData());
            utStringList.AddRange(ut.getStringData());
        }


    }//class Window_UnitType end
}
