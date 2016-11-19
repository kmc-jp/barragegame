﻿using System;
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
    public enum SkillGenreS {none=0,shot,laser,circle,wayshot,zyuzi,yanagi }

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

    class BarrageUsedSkillData:SkillData
    {
        public int duration;
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
        public BarrageUsedSkillData(string _skillName, SkillGenreS _sgs,MoveType mt, string _aniDName, int _cooldownFps, double _speed, double _acceleration, double _angle, double _radius,
            int _duration=DataBase.motion_inftyTime, double _space = 0,int _sword = 1,int _score=10, int _life = 1)
            :base(_skillName,SkillGenreL.generation,_sgs,_cooldownFps)
        {
            aniDName = _aniDName;
            speed = _speed;
            acceleration = _acceleration;
            angle = _angle;
            radius = _radius;
            duration = _duration;
            space = _space;
            life = _life;
            score = _score;
            sword = _sword;
            moveType = mt;
        }
    }

    class SingleShotSkillData :BarrageUsedSkillData
    {
        public SingleShotSkillData(string _skillName,SkillGenreS _sgs,MoveType mt,string _aniDName,int _cooldownFps,double _speed,
            double _acceleration,double _angle,double _radius,int _duration=DataBase.motion_inftyTime,double _space=0,int _sword=1,int _score=10,int _life=1)
            :base(_skillName,_sgs,mt,_aniDName, _cooldownFps,_speed,_acceleration,_angle,_radius,_duration,_space,_sword,_score,_life)
        {
           
        }
    }//class singleshotskilldata end

    class WayShotSkillData : BarrageUsedSkillData
    {
        public int way;
        
        public WayShotSkillData(string _skillName,SkillGenreS _sgs, MoveType mt,string _aniDName, int _cooldownFps, double _speed, 
            double _acceleration, double _angle, double _radius, int _way, int _duration=DataBase.motion_inftyTime,double _space=0, int _sword=1,int _score=10,int _life=1)
            : base(_skillName,_sgs, mt,_aniDName,_cooldownFps,_speed,_acceleration,_angle,_radius,_duration,_space,_sword,_score,_life)
        {
            way = _way;
        }
    }//class WayShotSkillData end

    class LaserTopData :BarrageUsedSkillData
    {
        /// <summary>
        /// 角速度
        /// </summary>
        public double omega;
        public Color color;

        public LaserTopData(string _skillName, MoveType mt, string _aniDName,int _cooldownFps,
            double _speed, double _acceleration, double _angle, double _radius, double _omega, Color _color, int _duration=DataBase.motion_inftyTime,
            double _space=0, int _sword=1,int _score=10, int _life=1)
            : base(_skillName,SkillGenreS.laser,mt,_aniDName, _cooldownFps,_speed,_acceleration,_angle,_radius,_duration,_space,_sword,_score,_life)
        {
            color = _color;
            omega = _omega;
        }
    }

    class GenerateSkilledBulletData : BarrageUsedSkillData
    {
        public string unitSkillName;

        public GenerateSkilledBulletData(string _skillName,SkillGenreS _sgs, MoveType mt,string _aniDName,int _cooldownFps,double _speed,
            double _acceleration,double _angle,double _radius, string _unitSkillName, int _duration=DataBase.motion_inftyTime,double _space=0,int _sword=1,int _score=10,int _life=1)
            :base(_skillName,_sgs,mt,_aniDName, _cooldownFps, _speed, _acceleration, _angle, _radius,_duration,_space, _sword,_score,_life)
        {
            sgl = SkillGenreL.bullet_create;
            unitSkillName = _unitSkillName;
        }
    }
}
