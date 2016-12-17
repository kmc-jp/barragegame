using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonPart
{
    class Motion
    {
        public MoveType mt;
        public PointType pt;
        public Vector pos;

        public double speed, acceleration;
        public double angle, omega;

        public int alltime;

        public Motion(MoveType _mt,PointType _pt,Vector _pos,double _speed,double _accelaration,
            int _T,double _omega=0,double _angle = Math.PI/2)
        {
            mt = _mt;
            pt = _pt;
            pos = _pos;
            speed = _speed;
            alltime = _T;
            angle = _angle;
            omega = _omega;
        }



        public static bool Has_a_Object(MoveType _mt) {
            switch (_mt)
            {
                case MoveType.chase_player_target:
                case MoveType.player_target:
                    return true;
                default:
                    return false;
            }
        }

        public static double getAngleFromPointType(PointType _pt,double _angle,double px,double sx=0,double sy=0,double self_angle=0)
        {
            if (_angle == DataBase.AngleToPlayer)
                _angle = Math.Atan2(Map.player.y - sy, Map.player.x - sx);
            else if (_angle == DataBase.SelfAngle)
                _angle = self_angle;
            switch (_pt)
            {
                case PointType.Direction:
                    return _angle;
                case PointType.randomDirection:
                    return _angle + Function.GetRandomDouble(px * 2) - px;
                case PointType.player_pos:
                    return Math.Atan2(Map.player.y - sy, Map.player.x - sx);// +_angle;
                default:
                    return _angle;
            }
        }

        #region Functions About PointType
        /// <summary>
        /// PointTypeが"ベクトルが点なのか"を判定する、
        /// </summary>
        /// <param name="_pt"></param>
        /// <returns></returns>
        public static bool Is_a_Point(PointType _pt)
        {
            switch (_pt)
            {
                case PointType.player_pos:
                case PointType.pos_on_screen:
                    return true;
                default:
                    return false;
            }
        }
        /// <summary>
        /// PointTypeが"ベクトルが方向を意味するか"を判定する
        /// </summary>
        /// <param name="_pt"></param>
        /// <returns></returns>
        public static bool Is_a_Direction(PointType _pt)
        {
            switch (_pt)
            {
                case PointType.Direction:
                case PointType.randomDirection:
                    return true;
                default:
                    return false;
            }
        }
        
        public static bool Is_a_Displacement(PointType _pt)
        {
            switch (_pt)
            {
                case PointType.displacement:
                case PointType.randomRange:
                    return true;
                default:
                    return false;
            }
        }


        /// <summary>
        /// 渡された(x,y)とPointTypeで適切なyを返す.player_posの時player.yを返す.この時点でrandomRangeが計算されるが、directionを使うものはすでに計算したangleを渡すべき。
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="_pt"></param>
        /// <param name="_angle">directionを使うものはすでに計算したangleを渡す</param>
        public static double from_PointType_getPosY(double px, double py, PointType _pt, int t = 1, double _speed = 1, double _angle = Math.PI / 2, MoveType _mt = MoveType.noMotion)
        {
            switch (_pt)
            {
                case PointType.notused:
                    return 0;
                case PointType.player_pos:
                    return Map.player.y;
                case PointType.randomRange:
                    return (Function.GetRandomDouble(2.0) - 1) * py;
                case PointType.displacement:
                    return py;
                case PointType.randomDirection:
                case PointType.Direction:
                    return _speed * Math.Sin(_angle);
                default:
                    return py;
            }
        }
        /// <summary>
        /// 渡された(x,y)とPointTypeで適切なxを返す.player_posはこの時点のplayer.xを返す。この時点でrandomRangeが計算されるが、directionを使うものはすでに計算したangleを渡すべき。
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="_pt"></param>
        /// <param name="_angle">directionを使うものはすでに計算したangleを渡す</param>
        public static double from_PointType_getPosX(double px, double py, PointType _pt, int t = 1, double _speed=1, double _angle=Math.PI/2, MoveType _mt = MoveType.noMotion)
        {
            switch (_pt)
            {
                case PointType.notused:
                    return 0;
                case PointType.player_pos:
                    return Map.player.x;
                case PointType.randomRange:
                    return (Function.GetRandomDouble(2.0) - 1) * px;
                case PointType.displacement:
                    /*
                    if (t > 0) return px / t;
                    else
                    {
                        double dis = Math.Sqrt(px * px + py * py);
                        return px *= _speed / dis;
                    }*/
                    return px;
                case PointType.pos_on_screen:
                    return px + Map.leftside;
                case PointType.randomDirection:
                case PointType.Direction:
                    return _speed * Math.Cos(_angle);
                default:
                    return px;
            }
        }
        #endregion

    }
}
