using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonPart
{
    class Projection
    {
        public double x, y;
        public int speed;
        public double speed_x, speed_y;
        public Vector target_pos;
        public Animation animation = null;
        public MoveType move_type;
        public int zoom_rate;


        public Projection(double _x, double _y, MoveType _move_type, Animation _anime, int _zoom_rate)
        {
            x = _x;
            y = _y;
            move_type = _move_type;
            animation = _anime;
            zoom_rate = _zoom_rate;

            if (animation == null)
            {
                animation = new Animation(new SingleTextureAnimationData(10,TextureID.Bullet,3,1));
            }
        }
        public Projection(double _x, double _y, MoveType _move_type, int _speed, Animation _anime, Vector _target_pos, int _zoom_rate)
            : this(_x, _y, _move_type, _anime, _zoom_rate)
        {
            speed = _speed;
            target_pos = _target_pos;
            double e = Math.Sqrt(Function.distance(x, y, target_pos.X, target_pos.Y));
            speed_x = (x - target_pos.X) * speed / e;
            speed_y = (y - target_pos.Y) * speed / e;
        }

        public virtual void update()
        {
            switch (move_type)
            {
                case MoveType.non_target:
                    break;
                case MoveType.object_target:
                    break;
                case MoveType.point_target:
                    double e = Math.Sqrt(Function.distance(x, y, target_pos.X, target_pos.Y));
                    x -= speed_x;
                    y -= speed_y;
                    break;
                default:
                    break;
            }

            animation.Update();
        }

        public virtual void draw(Drawing d)
        {
            animation.Draw(d,new Vector(x,y),DepthID.Effect,zoom_rate/100);
        }
    }
}
