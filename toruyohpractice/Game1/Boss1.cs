using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonPart
{
    class Boss1:Enemy
    {
        const int body_max_index=12; 
        public Enemy[] bodys=new Enemy[body_max_index];

        public Boss1(double _x, double _y, string _unitType_name):base(_x,_y,_unitType_name)
        {
            for (int i = 0; i < body_max_index-1; i++)
            {
                bodys[i] = new Enemy(x,y-animation.Y*i,"boss1body"+i%3);
            }
            bodys[body_max_index] = new Enemy(x, y, "boss1tail");
        }

        public override void update(Player player)
        {
            base.update(player);

            if (moving())
            {
                for (int i = body_max_index; i > 0; i--)
                {
                    bodys[i].x = bodys[i - 1].x;
                    bodys[i].y = bodys[i - 1].y;
                }
                bodys[0].x = x;
                bodys[0].y = y;
            }

            shot(player);
            for (int i = 0; i <= body_max_index ; i++)
            {
                bodys[i].shot(player);
            }
        }

        private bool moving() { return true; }
    }
}
