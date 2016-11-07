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
        public double speed;
        public double speed_x, speed_y;
        public double acceleration;
        public double acceleration_x, acceleration_y;
        public double radian;
        public Vector target_pos;
        public Player target;
        public AnimationAdvanced animation = null;
        public MoveType move_type;
        public int zoom_rate;

        public bool delete = false;

        //timeをつくろう
        /// <summary>
        /// コンストラクタ(位置不変)
        /// </summary>
        /// <param name="_x"></param>
        /// <param name="_y"></param>
        /// <param name="_move_type"></param>
        /// <param name="_anime"></param>
        /// <param name="_zoom_rate"></param>
        public Projection(double _x, double _y,MoveType _move_type, string _anime, int _zoom_rate)
        {
            x = _x;
            y = _y;
            target = null;
            move_type = _move_type;
            animation = new AnimationAdvanced(DataBase.getAniD(_anime));
            zoom_rate = _zoom_rate;
        }
        /// <summary>
        /// コンストラクタ(速度、加速度あり)
        /// </summary>
        /// <param name="_x"></param>
        /// <param name="_y"></param>
        /// <param name="_move_type"></param>
        /// <param name="_speed"></param>
        /// <param name="_acceleration"></param>
        /// <param name="_anime"></param>
        /// <param name="_zoom_rate"></param>
        public Projection(double _x,double _y,MoveType _move_type,double _speed,double _acceleration,double _radian, string _anime,int _zoom_rate)
        {
            x = _x;
            y = _y;
            target = null;
            speed = _speed;
            acceleration = _acceleration;
            radian = _radian;
            move_type = _move_type;
            animation = new AnimationAdvanced(DataBase.getAniD(_anime));
            zoom_rate = _zoom_rate;

            speed_x = speed * Math.Cos(radian);
            speed_y = speed * Math.Sin(radian);
            acceleration_x = acceleration * Math.Cos(radian);
            acceleration_y = acceleration * Math.Sin(radian);
        }
        /// <summary>
        /// 点に向かって移動
        /// </summary>
        /// <param name="_x"></param>
        /// <param name="_y"></param>
        /// <param name="_move_type"></param>
        /// <param name="_speed"></param>
        /// <param name="_anime"></param>
        /// <param name="_target_pos"></param>
        /// <param name="_zoom_rate"></param>
        public Projection(double _x, double _y, MoveType _move_type, double _speed,double _acceleration, string _anime, Vector _target_pos, int _zoom_rate)
            : this(_x, _y, _move_type, _anime, _zoom_rate)
        {
            speed = _speed;
            acceleration = _acceleration;
            target_pos = _target_pos;
            double e = Math.Sqrt(Function.distance(x, y, target_pos.X, target_pos.Y));
            acceleration_x = (x - target_pos.X) * acceleration / e;
            acceleration_y = (y - target_pos.Y) * acceleration / e;
            speed_x = (x - target_pos.X) * speed / e;
            speed_y = (y - target_pos.Y) * speed / e;
        }
        /// <summary>
        /// 物体に向かって移動
        /// </summary>
        /// <param name="_x"></param>
        /// <param name="_y"></param>
        /// <param name="_move_type"></param>
        /// <param name="_speed"></param>
        /// <param name="_anime"></param>
        /// <param name="_target"></param>
        /// <param name="_zoom_rate"></param>
        public Projection(double _x, double _y, MoveType _move_type, double _speed,double _acceleration, string _anime, Player _target, int _zoom_rate)
            : this(_x, _y, _move_type, _anime, _zoom_rate)
        {
            speed = _speed;
            acceleration = _acceleration;
            target = _target;
            double e = Math.Sqrt(Function.distance(x, y, target.x, target.y));
            speed_x =  (x - target.x) * speed / e;
            speed_y = (y - target.y) * speed / e;
        }

        public virtual void update()
        {
            if (delete) { return; }
            switch (move_type)
            {
                case MoveType.non_target:
                    break;
                case MoveType.object_target:
                    speed += acceleration;
                    double e = Math.Sqrt(Function.distance(x, y, target.x, target.y));
                    speed_x = (x - target.x) * speed / e;
                    speed_y = (y - target.y) * speed / e;
                    x -= speed_x;
                    y -= speed_y;
                    break;
                case MoveType.point_target:
                    speed_x += acceleration_x;
                    speed_y += acceleration_y;
                    //double e = Math.Sqrt(Function.distance(x, y, target_pos.X, target_pos.Y));
                    x -= speed_x;
                    y -= speed_y;
                    break;
                case MoveType.go_straight:
                    speed_x += acceleration_x;
                    speed_y += acceleration_y;
                    x -= speed_x;
                    y -= speed_y;
                    break;
                default:
                    break;
            }
            animation.Update();
        }

        public virtual bool hit_jugde(Player player)
        {
            return false;
        }
        public virtual bool hit_jugde(double px,double py, double p_radius=0)
        {
            return false;
        }

        public virtual void draw(Drawing d)
        {
            animation.Draw(d,new Vector(x,y),DepthID.Enemy,zoom_rate/100);
        }
    }

    class ChargeProjection:Projection
    {
        public int sword;
        /// <summary>
        /// 物体playerに向かって移動
        /// </summary>
        /// <param name="_x"></param>
        /// <param name="_y"></param>
        /// <param name="_move_type"></param>
        /// <param name="_speed"></param>
        /// <param name="_anime"></param>
        /// <param name="_target"></param>
        /// <param name="_zoom_rate"></param>
        public ChargeProjection(double _x, double _y, string _anime,int _sword, MoveType _move_type, double _speed, double _acceleration, Player _target, int _zoom_rate)
            : base(_x, _y, _move_type, _speed,_acceleration,_anime, _target,_zoom_rate)
        { sword = _sword; }
        public ChargeProjection(double _x, double _y, string _anime, int _sword, double _speed, double _acceleration, Player _target)
            : this(_x, _y, _anime,_sword,MoveType.object_target, _speed, _acceleration, _target, 100)
        { }
        public override bool hit_jugde(double px, double py, double p_radius = 0)
        {
            return Function.hitcircle(x, y, speed, px, py, p_radius);
        }
        public override bool hit_jugde(Player p)
        {
            return Function.hitcircle(x, y, speed/2, p.x,p.y,p.radius);
        }
    }
}
