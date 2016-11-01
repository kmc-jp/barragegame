﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonPart
{
    /// <summary>
    /// default_posesの意味
    /// </summary>
    public enum PointType { default_pos = 0, displacement, }

    class ActiveAniSkiedUnitType:SkilledAnimatedUnitType
    {
        public List<MoveType> moveTypes;
        public double speed;
        public double acceleration;
        public double radius;
        public double angle;
        public int score;
        public int sword;
        public List<Vector> default_poses;
        public List<int> times;
        public PointType pointType;

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
        }


        public void add_MoveTypeDataSet(MoveType m, int t,Vector v)
        {

            moveTypes.Add(m);
            set_ith_MTDataSet(moveTypes.Count - 1, t, v);
        }
        public void set_ith_MTDataSet(int id,int t,Vector v)
        {
            for(int i = times.Count; i < id-1; i++)
            {
                times.Add(0);
                
            }
            for (int j = default_poses.Count; j < id - 1; j++)
            {
                default_poses.Add(new Vector(0,0));

            }
            if (times.Capacity > id) { times[id] = t; }else { times.Add(t); }
            if (default_poses.Capacity > id) { default_poses.Add(v); } else { default_poses.Add(v); }
        }

        public void Initialize(List<MoveType> _moveTypes, List<Vector> _default_poses, List<int> _times, double _speed, double _acceleration, double _radius,double _angle, int _score, int _sword)
        {
            setup_moveType(_moveTypes);
            setup_default_pos(_default_poses);
            setup_time(_times);
            setup_standard(_speed, _acceleration, _radius,_angle, _score, _sword);
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
        public void setup_standard(double _speed,double _acceleration,double _radius,double _angle,int _score,int _sword)
        {
            speed = _speed;
            acceleration = _acceleration;
            radius = _radius;
            angle = _angle;
            score = _score;
            sword = _sword;
        }


    }
}