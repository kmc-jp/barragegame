using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonPart
{
    class Window_AniDList : Window_WithColoum
    {
        protected Scroll aniDscroll;
        protected const int white_space_size = 40;
        #region constructor
        public Window_AniDList(int _x, int _y, int _w, int _h) : base(_x, _y, _w, _h)
        {
            setup_AniDscroll();
            mouse_dragable = true;
        }
        #endregion
        public string getAniScrollContent_str() { return aniDscroll.content; }
        protected void setup_AniDscroll()
        {
            int nx = 10, ny = 10;int dy = 30;
            aniDscroll = new Scroll(nx, ny, "AnimationDatas", dy, 10);
            nx = 16; ny = 0;int dx = 0;
            foreach (AnimationDataAdvanced adAd in DataBase.AnimationAdDataDictionary.Values)
            {
                aniDscroll.addColoum(new Button(nx, ny, "", adAd.animationDataName, Command.selectInScroll, false));
                nx += dx;ny += dy;
            }
            coloums.Add(aniDscroll);
        }
        public override void draw(Drawing d)
        {
            base.draw(d);
            aniDscroll.draw(d, x, y);
        }
        public void reloadAniDscroll()
        {
            aniDscroll.clear();
            aniDscroll = null;
            setup_AniDscroll();
        }
    }
}
