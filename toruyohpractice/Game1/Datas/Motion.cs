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
                case MoveType.chase_target:
                case MoveType.object_target:
                    return true;
                default:
                    return false;
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
        /// 渡された(x,y)とPointTypeで適切なyを返す.player_posの時player.yを返す
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="_pt"></param>
        public static double from_PointType_getPosY(double px, double py, PointType _pt, int t = 1, MoveType _mt = MoveType.noMotion)
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
                default:
                    return py;
            }
        }
        /// <summary>
        /// 渡された(x,y)とPointTypeで適切なxを返す.player_posはこの時点のplayer.xを返す。
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="_pt"></param>
        public static double from_PointType_getPosX(double px, double py, PointType _pt, int t = 1, MoveType _mt = MoveType.noMotion)
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
                    return px;
                case PointType.pos_on_screen:
                    return px + Map.leftside;
                default:
                    return px;
            }
        }
        #endregion

        public static 
    }
}
