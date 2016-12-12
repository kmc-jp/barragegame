using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonPart
{

    class ActiveAniSkiedUnitType:SkilledAnimatedUnitType
    {
        public List<MoveType> moveTypes;
        public double speed;
        public double acceleration;
        public double radius;
        public double angle;
        public double omega;
        public int score;
        public int sword;
        public List<Vector> default_poses;
        public List<int> times;
        public List<PointType> pointTypes;
        public bool textureTurn;

        public ActiveAniSkiedUnitType(string _typename, string _texture_name, string _label,List<MoveType> _moveTypes,List<Vector> _default_poses,List<int> _times) : this(_typename, _texture_name, _label,_moveTypes)
        {
            setup_default_pos(_default_poses);
            setup_time(_times);
        }
        public ActiveAniSkiedUnitType(string _typename, string _texture_name, string _label, List<MoveType> _moveTypes): this(_typename, _texture_name, _label)
        {
            setup_moveType(_moveTypes);
        }
        public ActiveAniSkiedUnitType(string _typename, string _texture_name, string _label) : base(_typename, _texture_name, _label)
        {
            genre = (int)Unit_Genre.animated + (int)Unit_Genre.skilled + (int)Unit_Genre.active;
            moveTypes = new List<MoveType>();
            default_poses = new List<Vector>();
            times = new List<int>();
            pointTypes = new List<PointType>();
        }

        /// <summary>
        /// MoveTypeがscreen_pointの時はpointTypeはnotUsedだとscreen_pointになる。go_straightの時はdisplacementになる
        /// </summary>
        /// <param name="m">MoveTypeの設定、これによって、PointTypeがnotUsedでも一番使われるものになる</param>
        /// <param name="t">持続時間</param>
        /// <param name="v">使うベクトル</param>
        /// <param name="p">上のベクトルの使い方</param>
        public void add_MoveTypeDataSet(MoveType m, int t, Vector v, PointType p = PointType.notused)
        {
            moveTypes.Add(m);
            switch (m)
            {
                case MoveType.screen_point_target:
                    if (p == PointType.notused) { p = PointType.pos_on_screen; }
                    break;
                case MoveType.go_straight:
                    if (p == PointType.notused) { p = PointType.displacement; }
                    break;
                default:
                    break;
            }
            set_ith_MTDataSet(moveTypes.Count - 1, t, v, p);
        }
        public void set_ith_MTDataSet(int id, int t, Vector v, PointType p= PointType.notused)
        {
            for(int i = times.Count; i < id-1; i++)
            {
                times.Add(0);
                
            }
            for (int j = default_poses.Count; j < id - 1; j++)
            {
                default_poses.Add(new Vector(0,0));

            }
            for (int i = pointTypes.Count; i < id - 1; i++)
            {
                pointTypes.Add(PointType.notused);

            }
            if (times.Count > id) { times[id] = t; }else { times.Add(t); }
            if (default_poses.Count > id) { default_poses.Add(v); } else { default_poses.Add(v); }
            if (pointTypes.Count > id) { pointTypes[id] = p; } else { pointTypes.Add(p); }
        }

        public void Initialize(List<MoveType> _moveTypes, List<Vector> _default_poses, List<int> _times, double _speed, double _acceleration, double _radius,double _angle,double _omega, int _score, int _sword)
        {
            setup_moveType(_moveTypes);
            setup_default_pos(_default_poses);
            setup_time(_times);
            setup_standard(_speed, _acceleration, _radius,_angle,_omega, _score, _sword);
        }

        public void setup_moveType(List<MoveType> _moveTypes)
        {
            for (int i = 0; i < _moveTypes.Count; i++)
            {
                moveTypes.Add(_moveTypes[i]);
            }
        }
        public void setup_default_pos(List<Vector> _default_poses)
        {
            for (int i = 0; i < _default_poses.Count; i++)
            {
                default_poses.Add(_default_poses[i]);
            }
        }
        public void setup_time(List<int> _times)
        {
            for (int i = 0; i < _times.Count; i++)
            {
                times.Add(_times[i]);
            }
        }
        public void setup_standard(double _speed,double _acceleration,double _radius,double _angle,double _omega=Math.PI/60,int _score=10,int _sword=1)
        {
            speed = _speed;
            acceleration = _acceleration;
            radius = _radius;
            angle = _angle;
            omega = _omega;
            score = _score;
            sword = _sword;
        }


    }
}
