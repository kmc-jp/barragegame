using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonPart {
    class Tile {
        #region 変数
        public int x_index;
        public int y_index;
        public int tile_type;
        public int w;
        public int h;
        public int real_w;
        public int real_h;
        public int zoom_rate;
        public int now_texture_id;
        private int frame_now;
        #endregion
        #region 関数
        public Tile(int X_index, int Y_index, int Tile_type, int Real_w, int Real_h, int Zoom_rate = 100) {
            x_index = X_index;
            y_index = Y_index;
            tile_type = Tile_type;
            real_w = Real_w;
            real_h = Real_h;
            zoom_rate = Zoom_rate;
        }
        public void update() {

        }
        public void draw() {

        }
        public bool passable() {
            return true;
        }
        #endregion
    }
}
