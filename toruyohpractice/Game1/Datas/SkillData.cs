using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace CommonPart
{
    /// <summary>
    /// 大きいスキルの区分
    /// </summary>
    public enum SkillGenreL {none=0,generation=1,bullet_create }
    /// <summary>
    /// 細かいスキルの区分
    /// </summary>
    public enum SkillGenreS {none=0,shot,laser,circle,wayshot,zyuuzi,yanagi }

    abstract class SkillData
    {
        public int cooldownFps;
        public string skillName;
        public SkillGenreL sgl;
        public SkillGenreS sgs;

        public SkillData(string _skillName,SkillGenreL _sgl,SkillGenreS _sgs,int _cooldownFps)
        {
            skillName = _skillName;
            sgl = _sgl;
            sgs = _sgs;
            cooldownFps = _cooldownFps;
        }
    }//class SkillData end

    class SingleShotSkillData :SkillData
    {
        public double speed;
        public double acceleration;
        public double angle;
        public double radius;
        public int life;
        public int score;
        public int sword;
        public double space;
        public MoveType moveType;
        public string aniDName;
               
        public SingleShotSkillData(string _skillName,string _aniDName,int _cooldownFps,double _speed,double _acceleration,double _angle,double _radius,double _space,int _life,int _score,int _sword)
            :base(_skillName,SkillGenreL.generation,SkillGenreS.shot, _cooldownFps)
        {
            aniDName = _aniDName;
            speed = _speed;
            acceleration = _acceleration;
            angle = _angle;
            radius=_radius;
            space = _space;
            life = _life;
            score = _score;
            sword = _sword;
            moveType = MoveType.go_straight;
            switch (skillName)
            {
                case "circle":
                    sgs = SkillGenreS.circle;
                    break;
                case "wayshot":
                    sgs = SkillGenreS.wayshot;
                    break;
                case "zyuuzi":
                    sgs = SkillGenreS.zyuuzi;
                    break;
                case "yanagi":
                    sgs = SkillGenreS.yanagi;
                    break;
                default:
                    break;
            }
        }
    }//class singleshotskilldata end

    class WayShotSkillData : SingleShotSkillData
    {
        public int way;
        
        public WayShotSkillData(string _skillName,string _aniDName, int _cooldownFps, double _speed, double _acceleration, double _angle, double _radius, double _space, int _life, int _score, int _sword,int _way)
            : base(_skillName, _aniDName,_cooldownFps,_speed,_acceleration,_angle,_radius,_space,_life,_score,_sword)
        {
            way = _way;
            sgs = SkillGenreS.wayshot;
        }
    }//class WayShotSkillData end

    class LaserTopData :SkillData
    {
        public double speed;
        public double acceleration;
        public double angle;
        public double radius;
        public int life;
        public int score;
        public int sword;
        /// <summary>
        /// 角速度
        /// </summary>
        public double omega;
        public MoveType moveType;
        public Color color;
        public string aniDName;

        public LaserTopData(string _skillName, string _aniDName,int _cooldownFps,
            double _speed, double _acceleration, double _angle, double _radius, double _space, int _life, int _score, int _sword,double _omega,Color _color)
            : base(_skillName, SkillGenreL.generation, SkillGenreS.laser, _cooldownFps)
        {
            speed = _speed;
            acceleration = _acceleration;
            angle = _angle;
            radius = _radius;
            life = _life;
            score = _score;
            sword = _sword;
            omega = _omega;
            moveType = MoveType.chase_angle;
            color = _color;
            aniDName = _aniDName;
        }
    }

    class GenerateUnitSkillData : SingleShotSkillData
    {
        public string unitSkillName;

        public GenerateUnitSkillData(string _skillName,string _aniDName,int _cooldownFps,double _speed,double _acceleration,double _angle,double _radius,double _space,int _life,int _score,int _sword,
            string _unitSkillName)
            :base(_skillName,_aniDName, _cooldownFps, _speed, _acceleration, _angle, _radius,_space,_life, _score, _sword)
        {
            sgl = SkillGenreL.bullet_create;
            unitSkillName = _unitSkillName;
        }
    }
}
