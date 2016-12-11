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
    public enum SkillGenreL {none=0,generation=1,UseSkilledBullet }
    /// <summary>
    /// 細かいスキルの区分
    /// </summary>
    public enum SkillGenreS {none=0,laser,circle,wayshot,yanagi }

    abstract class SkillData
    {
        public int cooldownFps;
        public string skillName;
        public SkillGenreL sgl;
        public SkillGenreS sgs;

        /// <summary>
        /// 発動条件などを記すに使っている。他のもあり
        /// </summary>
        public string Label;

        public SkillData(string _skillName,SkillGenreL _sgl,SkillGenreS _sgs,int _cooldownFps,string _label=null)
        {
            skillName = _skillName;
            sgl = _sgl;
            sgs = _sgs;
            cooldownFps = _cooldownFps;
            Label = _label;
        }
    }//class SkillData end

    class BarrageUsedSkillData:SkillData
    {
        #region standard variables
        public string aniDName;
        public double radius;
        public int life;
        public int score;
        public int sword;
        #endregion
        #region about motion
        public double speed;
        public double acceleration;
        public int motion_time;
        public MoveType moveType;
        public PointType pointType;
        /// <summary>
        /// PointTypeによって、使い方が変わる
        /// </summary>
        public Vector vec;
        /// <summary>
        /// 角速度
        /// </summary>
        public double omega;
        #endregion
        /// <summary>
        /// レーザーにのみ意味を持つ
        /// </summary>
        public Color color;
        public int duration;
        public double angle;
        public double space;
        /// <summary>
        /// 恐らく最も詳しいスキルデータを一つ構成する.これで作ったものには　初期化していないものがないようにするべき
        /// </summary>
        /// <param name="_skillName">スキルの名前，唯一であることを確認してください</param>
        /// <param name="_label">スキルの使用条件やその他に使われる</param>
        /// <param name="_sgs">このスキルが生成する弾丸の種類</param>
        /// <param name="mt">弾丸の動きパターン</param>
        /// <param name="pt">動くに使われるベクトルの使い方</param>
        /// <param name="v">使われるベクトル</param>
        /// <param name="_motion_fps">動きの周期、必要なら設ける</param>
        /// <param name="_aniDName">弾丸のアニメーション</param>
        /// <param name="_cooldownFps">スキルの使用間隔</param>
        /// <param name="_speed">弾丸の速度</param>
        /// <param name="_acceleration">弾丸の加速度</param>
        /// <param name="_angle">弾丸の初期角度、pointTypeによって使い方が変わる?</param>
        /// <param name="_omega">弾丸の動きの角速度、必要な時にのみ使われる</param>
        /// <param name="_radius">弾丸の半径</param>
        /// <param name="_c">レーザーに限り、レーザーの直線の色を指定する</param>
        /// <param name="_duration">弾丸が見えるかつ動ける状態の持続時間</param>
        /// <param name="_space">弾丸と弾丸の差、使い方はまだ定まらない</param>
        /// <param name="_sword">弾丸が吸収される時回復するエネルギー</param>
        /// <param name="_score">弾丸が吸収される時の点数</param>
        /// <param name="_life">弾丸のhp</param>
        public BarrageUsedSkillData(string _skillName, string _label,SkillGenreS _sgs, string _aniDName, int _cooldownFps, MoveType mt, PointType pt,Vector v, int _motion_fps,double _speed, double _acceleration, double _angle, double _omega, 
            double _radius,Color _c,int _duration=DataBase.motion_inftyTime, double _space = 0,int _sword = 1,int _score=10, int _life = 1)
            :base(_skillName,SkillGenreL.generation,_sgs,_cooldownFps,_label)
        {
            #region standard variables
            aniDName=_aniDName;
            radius=_radius;
            life=_life;
            score=_score;
            sword=_sword;
            #endregion
            #region about motion
            speed=_speed;
            acceleration=_acceleration;
            motion_time=_motion_fps;
            moveType=mt;
            pointType=pt;
            vec=v;
            omega=_omega;
            #endregion
            angle = _angle;
            color = _c;
            duration = _duration;
            space = _space;
        }

    }

    class WayShotSkillData : BarrageUsedSkillData
    {
        public int way;
        /// <summary>
        /// skill付きのbulletを生成するか
        /// </summary>
        public bool UseSkilledBullet = false;
        public List<string> skillNames;

        public WayShotSkillData(string _skillName,SkillGenreS _sgs, MoveType mt,string _aniDName, int _cooldownFps, double _speed, 
            double _acceleration, double _angle, double _radius, int _way, int _duration=DataBase.motion_inftyTime,double _space=0, int _sword=1,int _score=10,int _life=1)
            : base(_skillName,_sgs, mt,_aniDName,_cooldownFps,_speed,_acceleration,_angle,_radius,_duration,_space,_sword,_score,_life)
        {
            way = _way;
        }
    }//class WayShotSkillData end

    class WaySkilledBulletsData: WayShotSkillData
    {
        public List<string> skillNames;
        public WaySkilledBulletsData(string _skillName, SkillGenreS _sgs, MoveType mt, string _aniDName, int _cooldownFps, double _speed,
            double _acceleration, double _angle, double _radius, string _unitSkillName, int _duration = DataBase.motion_inftyTime, double _space = 0, int _sword = 1, int _score = 10, int _life = 1)
            :base(_skillName,_sgs,mt,_aniDName, _cooldownFps, _speed, _acceleration, _angle, _radius,_duration,_space, _sword,_score,_life)
        {
            sgl = SkillGenreL.UseSkilledBullet;
            unitSkillName = _unitSkillName;
        }
    }

    class LaserTopData :BarrageUsedSkillData
    {
        public LaserTopData(string _skillName, MoveType mt, string _aniDName,int _cooldownFps,
            double _speed, double _acceleration, double _angle, double _radius, double _omega, Color _color, int _duration=DataBase.motion_inftyTime,
            double _space=0, int _sword=1,int _score=10, int _life=1)
            : base(_skillName,SkillGenreS.laser,mt,_aniDName, _cooldownFps,_speed,_acceleration,_angle,_radius,_duration,_space,_sword,_score,_life)
        {
            color = _color;
            omega = _omega;
        }
    }

}
