using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonPart {
    class TileType　{
        #region 変数
        private string typename;
        private string label;
        public string texture_name { get; private set; } 
        public int texture_max_id { get; private set; }
        public int texture_min_id { get; private set; }
        public int passable_type { get; private set; }
        #endregion
        #region get property in int[] + string[]
        public int[] getIntData() {
            return new int[] {
                texture_max_id, //0th
                texture_min_id, //1st
                passable_type   //2nd

                //any other int variables should be added here
            };
        }
        public string[] getStringData()
        {
            return new string[] {
                texture_name,
                typename,
                label,

                //any other int variables should be added here
            };
        }
        #endregion
        #region 関数
        public TileType(string Type_name, string Label, int Texture_max_id, int Texture_min_id, int Passable_type) {
            typename = Type_name;
            label = Label;
            texture_max_id = Texture_max_id;
            texture_min_id = Texture_min_id;
            passable_type = Passable_type;
        }
        public bool passable() {
            return true;
        }
        public string getLabel() { return label; }
        #endregion
    }
}
