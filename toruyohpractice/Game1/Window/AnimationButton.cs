using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonPart
{
    class AnimationButton:AnimationColoum
    {
        public AnimationButton(int _x, int _y, string _str, AnimationDataAdvanced _content, Command _reply, int _dx = 0, int _dy = default_distance)
            : base(_x, _y, _str, _content,_reply,_dx,_dy)
        { updated = false; }

        public override Command is_left()
        {
            //Console.WriteLine("AnimationButton:is_left()");
            /*if (selected)
            {
                stopAndGoToStart();
            }*/
            stopAndGoToStart();
            return base.is_left();
        }
        public override Command update_with_mouse_manager(MouseManager m)
        {
            play();
            return base.update_with_mouse_manager(m);
        }
        public override void is_selected()
        {
            Console.WriteLine(content);
            play();
            base.is_selected();
        }
    }
}
