﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonPart
{
    class Projection:Unit
    {
        public double speed;
        public double speed_x, speed_y;
        public double acceleration;
        public double acceleration_x, acceleration_y;
        /// <summary>
        /// Math.PI/2は下を向いている。
        /// </summary>
        public double radian;
        /// <summary>
        /// x,yを持つenemyかplayer
        /// </summary>
        public Vector target_pos;
        public Unit target;
        public AnimationAdvanced animation = null;
        public MoveType move_type;
        /// <summary>
        /// target_posの意味。これは初期では.notused=-1である。
        /// </summary>
        public PointType point_type=PointType.notused;
        /// <summary>
        /// 角速度。もしくは角度差などを表す。
        /// </summary>
        public double omega;
        /// <summary>
        /// 自身のradianの値によって回転するかどうか
        /// </summary>
        public bool texRotate = false;
        public bool delete = false;

        #region around exist_times
        readonly int exist_times_length = Enum.GetNames(typeof(existTimesIndex)).Length;
        /// <summary>
        /// existTimesIndexを使用して:0:画像は見えない、動かない,1:画像は見えない、動く,2:画像は見える動かない,3:画像は見えて動く.
        /// </summary>
        public int[] exist_times;
        /// <summary>
        /// これがfalseの時、Projectionは必ず見える状態で存在し、動ける状態である。
        /// </summary>
        public bool useExistTime=false;
        
        #endregion


        #region constructor
        /// <summary>
        /// 基本のコンストラクタ、目的がない場合に使う。
        /// </summary>
        /// <param name="_x"></param>
        /// <param name="_y"></param>
        /// <param name="_move_type"></param>
        /// <param name="_anime"></param>
        /// <param name="_zoom_rate"></param>
        public Projection(double _x, double _y,MoveType _move_type, string _anime, double _speed=0,double _acceleration=0,double _radian=Math.PI/2,int _zoom_rate=100)
            :base(_x,_y)
        {
            x = _x;
            y = _y;
            target = null;
            move_type = _move_type;
            animation = new AnimationAdvanced(DataBase.getAniD(_anime));
            speed = _speed;      acceleration = _acceleration;      radian = _radian;
            zoom_rate = _zoom_rate;
            set_speed_and_acceleration_from_radian();
        }
        /// <summary>
        /// 点に向かって移動.MoveType,PointTypeとposから向きの角度がわかる場合_radianは意味を持ちません
        /// </summary>
        /// <param name="_pt">_target_posをどのように使用を決める</param>
        /// <param name="_target_pos">このMoveTypeに使用する点</param>
        public Projection(double _x, double _y, MoveType _move_type, string _anime, Vector _target_pos, PointType _pt,double _speed=0,double _acceleration=0, double _radian=0,int _zoom_rate=100)
            : this(_x, _y, _move_type, _anime,_speed,_acceleration,_radian, _zoom_rate)
        {
            point_type = _pt;
            target_pos = _target_pos;
            double e = Math.Sqrt(Function.distance(x, y, target_pos.X, target_pos.Y));
            acceleration_x = (x - target_pos.X) * acceleration / e;
            acceleration_y = (y - target_pos.Y) * acceleration / e;
            speed_x = (x - target_pos.X) * speed / e;
            speed_y = (y - target_pos.Y) * speed / e;
        }
        /// <summary>
        /// Unitに向かって移動する。MoveTypeによって、_radianは意味をなさないことがある
        /// </summary>
        /// <param name="_x">画面上生成位置x</param>
        /// <param name="_y">画面上生成位置y</param>
        /// <param name="_move_type">動きのパターン</param>
        /// <param name="_target">class Unitの目標</param>
        /// <param name="_acceleration">加速度</param>
        /// <param name="_radian">向き。MoveTypeによって、無意味になることもある</param>
        public Projection(double _x, double _y, MoveType _move_type, string _anime, Unit _target, double _speed,double _acceleration, double _radian = 0, int _zoom_rate = 100)
            : this(_x, _y, _move_type, _anime, _speed,_acceleration,_radian,_zoom_rate)
        {
            target = _target;
            switch (move_type)
            {
                case MoveType.chase_target:
                    radian = Math.Atan2(target.y - y, target.x - x);
                    break;
                default:
                    break;
            }
        }
        #endregion

        /// <summary>
        /// exist_timesを更新する.更新して 移動するとわかったと時はtrueを返す。移動できない時はfalseを返す。
        /// </summary>
        /// <returns></returns>
        protected virtual bool update_exist_times()
        {
            if (useExistTime)
            {
                int i;
                bool exist_end=true;// この瞬間に消えると仮定する
                for (i = 0; i < exist_times_length; i++)
                {
                    if (DataBase.timeEffective(exist_times[i]))
                    {
                        exist_end = false;//消えていないとわかる
                        if (exist_times[i] != DataBase.motion_inftyTime) { exist_times[i]--; }
                        break;
                    }
                }
                if (exist_end) { remove(Unit_state.exist_timeOut); }
                switch ((existTimesIndex)(i))
                {
                    case existTimesIndex.InvisibleStill:
                        return false;
                    case existTimesIndex.VisibleStill:
                        return false;
                }
            }
            return true; //移動できることを意味する。
        } 
        protected virtual bool visible() {
            return !useExistTime || ( !DataBase.timeEffective(exist_times[(int)existTimesIndex.InvisibleActive]) &&
                   !DataBase.timeEffective(exist_times[(int)existTimesIndex.InvisibleStill] )  );
        }
        public virtual void update(bool bulletMove=true)
        {
            if (delete) { return; }
            if (!update_exist_times())
            {
                return;
            }
            if (bulletMove)
            {
                #region switch move_type
                switch (move_type)
                {
                    case MoveType.noMotion:
                        break;
                    case MoveType.object_target:
                        if (!Function.hitcircle(x, y, radius, target.x, target.y, speed / 3))
                        {
                            speed += acceleration;
                            double eu = Math.Sqrt(Function.distance(x, y, target.x, target.y));
                            speed_x = (target.x - x) * speed / eu;
                            speed_y = (target.y - y) * speed / eu;
                            x += speed_x;
                            y += speed_y;
                        }
                        break;
                    case MoveType.screen_point_target:
                        if (!Function.hitcircle(x, y, radius, target_pos.X, target_pos.Y, speed / 3))
                        {
                            speed += acceleration;
                            double ep = Math.Sqrt(Function.distance(x, y, target_pos.X, target_pos.Y));
                            speed_x = speed * (target_pos.X - x) / ep;
                            speed_y = speed * (target_pos.Y - y) / ep;
                            x += speed_x;
                            y += speed_y;
                        }
                        break;
                    case MoveType.go_straight: //角度変化がない前提である。
                        speed += acceleration;
                        speed_x += acceleration_x;
                        speed_y += acceleration_y;
                        x += speed_x;
                        y += speed_y;
                        break;
                    default:
                        break;
                }
                #endregion
            }
            if (visible()) { animation.Update(); }
        }
        /// <summary>
        /// exist_timeを使用するかを決めて、使用する時渡された_etsの逆をexist_timesに入れる、ない部分は0で埋める. これをoverrideできます
        /// </summary>
        /// <param name="useSimple">trueの時は使用しない。falseで使用する</param>
        /// <param name="_ets">exist_times[0]=_ets[3],...,exist_times[3]=_ets[0]</param>
        public virtual void setup_exist_times(bool useSimple = true, int[] _ets=null)
        {
            if(useSimple || _ets==null) {
                useExistTime = false;
            }
            else
            {
                useExistTime = true;
                int i;
                exist_times = new int[exist_times_length];
                for (i=0 ; i < _ets.Length && i<exist_times_length; i++) { exist_times[exist_times_length-1-i] = _ets[i]; }
                while(i < exist_times_length) { exist_times[i] = 0;i++; }
            }
        }
        /// <summary>
        /// exist_timeを使用する上に、画面に見えてかつ動く時間だけを設置して、以外は0とする
        /// </summary>
        /// <param name="useSimple">trueの時は使用しない。falseで使用する</param>
        /// <param name="_ets">exist_times[0]=_ets[3],...,exist_times[3]=_ets[0]</param>
        public virtual void setup_exist_time(int _ets)
        {
            useExistTime = true;
            exist_times = new int[exist_times_length];
            for (int i = 1; i < exist_times_length; i++) { exist_times[exist_times_length - 1 - i] = 0; }
            exist_times[exist_times_length-1] =_ets;
        }

        /// <summary>
        /// radianが正しく記録されなければならない。
        /// </summary>
        protected virtual void set_speed_and_acceleration_from_radian()
        {
            if (speed != 0)
            {
                speed_x = speed * Math.Cos(radian);
                speed_y = speed * Math.Sin(radian);
            }
            if (acceleration != 0)
            {
                acceleration_x = acceleration * Math.Cos(radian);
                acceleration_y = acceleration * Math.Sin(radian);
            }
        }

        public virtual void remove(Unit_state us = Unit_state.dead)
        {
            switch (us)
            {
                case Unit_state.exist_timeOut:
                    delete = true;
                    break;
                case Unit_state.fadeout:
                    delete = true;
                    break;
                case Unit_state.out_of_window:
                    delete = true;
                    break;
                default:
                    break;
            }
        }


        public virtual bool hit_jugde(Unit u)
        {
            return false;
        }
        public virtual bool hit_jugde(double px, double py, double p_radius = 0)
        {
            return false;
        }

        public override void draw(Drawing d)
        {
            if (!visible()) return;
            if (!texRotate)
            {
                animation.Draw(d, new Vector((x - animation.X / 2), (y - animation.Y / 2)), DepthID.Enemy,zoom_rate/100);
            }
            else
            {
                float angle2 = (float)(radian);
                animation.Draw(d, new Vector((x + animation.Y / 2 * Math.Cos(angle2) + animation.X / 2 * Math.Sin(angle2)), (y + animation.Y / 2 * Math.Sin(angle2) - animation.X / 2 * Math.Cos(angle2))), DepthID.Enemy, 1, (float)(radian + Math.PI / 2));

            }
        }
    }

    class ChargeProjection:Projection
    {
        public int sword;
        /// <summary>
        /// 物体Unitに向かって移動
        /// </summary>
        public ChargeProjection(double _x, double _y, string _anime,int _sword, MoveType _move_type, double _speed, double _acceleration, Unit _target, int _zoom_rate)
            : base(_x, _y, _move_type, _anime, _target, _speed,_acceleration,_zoom_rate)
        { sword = _sword; }
        public ChargeProjection(double _x, double _y, string _anime, int _sword, double _speed, double _acceleration, Player _target)
            : this(_x, _y, _anime,_sword,MoveType.object_target, _speed, _acceleration, _target, 100)
        { }
        public override bool hit_jugde(double px, double py, double p_radius = 0)
        {
            return Function.hitcircle(x, y, speed, px, py, p_radius);
        }
        public override bool hit_jugde(Unit p)
        {
            return Function.hitcircle(x, y, speed/2, p.x,p.y,p.radius);
        }
    }
}
