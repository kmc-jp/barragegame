using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonPart {
    /// <summary>
    /// マップ作成のシーンのクラス
    /// </summary>
    class MapEditorScene : Scene {
        static private List<List<Unit>> units = new List<List<Unit>>();
        static private List<List<Tile>> tiles = new List<List<Tile>>();
        static private List<Window> windows = new List<Window>();
        static public bool ready { get; private set; } = false;
        static public int w, h;


        public MapEditorScene(SceneManager s) : base(s)
        {
            setup_windows();
        }
        public void setup_windows() {
            windows.Add(new Window_WithColoum(20, 20, 200, 400));
            ((Window_WithColoum)windows[0]).AddColoum(new Coloum(0, 0, "", Command.apply_int));
        }
        public void initialize_Basic(int _w, int _h ) {
            w = _w;
            h = _h;
            for (int x = 0; x < _w; x++) {
                units[x] = new List<Unit>(h);
                for (int y = 0; y < _h; y++) {
                        
                }
            }
        }

        public override void SceneDraw(Drawing d) {
            
        }//SceneDraw

        public override void SceneUpdate() {
            base.SceneUpdate();


        }
    }


}
