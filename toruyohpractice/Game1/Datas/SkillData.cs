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
    public enum SkillGenreS {none=0,laser,wayshot,yanagi }

    abstract class SkillData
    {
        public int cooldownFps;
        public string skillName;
        public SkillGenreL sgl;
        public SkillGenreS sgs;

        /// <summary>
        /// 発動条件などを記すに使っている。他のもあり
        /// </summary>
        public string conditions;

        public SkillData(string _skillName,SkillGenreL _sgl,SkillGenreS _sgs,int _cooldownFps,string _conditions=null)
        {
            skillName = _skillName;
            sgl = _sgl;
            sgs = _sgs;
            cooldownFps = _cooldownFps;
            conditions = _conditions;
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
        /// <param name="_conditions">スキルの使用条件やその他に使われる</param>
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
        /// <param name="_space">弾丸と弾丸の差、場合によって角度差、距離差</param>
        /// <param name="_sword">弾丸が吸収される時回復するエネルギー</param>
        /// <param name="_score">弾丸が吸収される時の点数</param>
        /// <param name="_life">弾丸のhp</param>
        public BarrageUsedSkillData(string _skillName, string _conditions,SkillGenreL _sgL,SkillGenreS _sgs, string _aniDName, int _cooldownFps, MoveType mt, PointType pt,Vector v, int _motion_fps,double _speed, double _acceleration, double _angle, double _omega, 
            double _radius,Color _c,int _duration=DataBase.motion_inftyTime, double _space = 0,int _sword = 1,int _score=10, int _life = 1)
            :base(_skillName,_sgL,_sgs,_cooldownFps,_conditions)
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
            if (_sgs == SkillGenreS.laser) color = _c;
            else Console.WriteLine(_skillName+":not a laser but has a color.");
            duration = _duration;
            space = _space;
        }
        
        /// <summary>
        /// 恐らく2番目に詳しいスキルデータを一つ構成する.レーザーの色を扱わない
        /// </summary>
        /// <param name="_skillName">スキルの名前，唯一であることを確認してください</param>
        /// <param name="_conditions">スキルの使用条件やその他に使われる</param>
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
        /// <param name="_duration">弾丸が見えるかつ動ける状態の持続時間</param>
        /// <param name="_space">弾丸と弾丸の差、場合によって角度差、距離差</param>
        /// <param name="_sword">弾丸が吸収される時回復するエネルギー</param>
        /// <param name="_score">弾丸が吸収される時の点数</param>
        /// <param name="_life">弾丸のhp</param>
        public BarrageUsedSkillData(string _skillName, string _conditions, SkillGenreL _sgL, SkillGenreS _sgs, string _aniDName, int _cooldownFps, MoveType mt, PointType pt, Vector v, int _motion_fps, double _speed, double _acceleration, double _angle, double _omega,
            double _radius, int _duration = DataBase.motion_inftyTime, double _space = 0, int _sword = 1, int _score = 10, int _life = 1)
            : base(_skillName, _sgL, _sgs, _cooldownFps, _conditions)
        {
            #region standard variables
            aniDName = _aniDName;
            radius = _radius;
            life = _life;
            score = _score;
            sword = _sword;
            #endregion
            #region about motion
            speed = _speed;
            acceleration = _acceleration;
            motion_time = _motion_fps;
            moveType = mt;
            pointType = pt;
            vec = v;
            omega = _omega;
            #endregion
            angle = _angle;
            duration = _duration;
            space = _space;
            if (_sgs == SkillGenreS.laser)
                Console.WriteLine(_skillName + ": laser with default Color!");
        }

        /// <summary>
        /// 恐らく最も詳しいスキルデータを一つ構成する.これで作ったものには　初期化していないものがないようにするべき
        /// </summary>
        /// <param name="_skillName">スキルの名前，唯一であることを確認してください</param>
        /// <param name="_conditions">スキルの使用条件やその他に使われる</param>
        /// <param name="_sgL">このスキルがどのように解釈されるかを決める</param>
        /// <param name="_sgs">このスキルが生成する弾丸の種類</param>
        /// <param name="motion">Motionを使って、省略する</param>
        /// <param name="_aniDName">弾丸のアニメーション</param>
        /// <param name="_cooldownFps">スキルの使用間隔</param>
        /// <param name="_radius">弾丸の半径</param>
        /// <param name="_c">レーザーに限り、レーザーの直線の色を指定する</param>
        /// <param name="_duration">弾丸が見えるかつ動ける状態の持続時間</param>
        /// <param name="_space">弾丸と弾丸の差、場合によって角度差、距離差</param>
        /// <param name="_sword">弾丸が吸収される時回復するエネルギー</param>
        /// <param name="_score">弾丸が吸収される時の点数</param>
        /// <param name="_life">弾丸のhp</param>
        public BarrageUsedSkillData(string _skillName, string _conditions, SkillGenreL _sgL, SkillGenreS _sgs, string _aniDName, int _cooldownFps, Motion motion,
            double _radius, Color _c, int _duration = DataBase.motion_inftyTime, double _space = 0, int _sword = 1, int _score = 10, int _life = 1)
            : base(_skillName, _sgL, _sgs, _cooldownFps, _conditions)
        {
            #region standard variables
            aniDName = _aniDName;
            radius = _radius;
            life = _life;
            score = _score;
            sword = _sword;
            #endregion
            #region about motion
            speed = motion.speed;
            acceleration = motion.acceleration;
            motion_time = motion.alltime;
            moveType = motion.mt;
            pointType = motion.pt;
            vec = motion.pos;
            omega = motion.omega;
            #endregion
            angle = motion.angle;
            if (_sgs == SkillGenreS.laser) color = _c;
            else Console.WriteLine(_skillName + ":not a laser but has a color.");
            duration = _duration;
            space = _space;
        }

        /// <summary>
        /// 恐らく最も詳しいスキルデータを一つ構成する.これで作ったものには　初期化していないものがないようにするべき
        /// </summary>
        /// <param name="_skillName">スキルの名前，唯一であることを確認してください</param>
        /// <param name="_conditions">スキルの使用条件やその他に使われる</param>
        /// <param name="_sgL">このスキルがどのように解釈されるかを決める</param>
        /// <param name="_sgs">このスキルが生成する弾丸の種類</param>
        /// <param name="motion">Motionを使って、省略する</param>
        /// <param name="_aniDName">弾丸のアニメーション</param>
        /// <param name="_cooldownFps">スキルの使用間隔</param>
        /// <param name="_radius">弾丸の半径</param>
        /// <param name="_duration">弾丸が見えるかつ動ける状態の持続時間</param>
        /// <param name="_space">弾丸と弾丸の差、場合によって角度差、距離差</param>
        /// <param name="_sword">弾丸が吸収される時回復するエネルギー</param>
        /// <param name="_score">弾丸が吸収される時の点数</param>
        /// <param name="_life">弾丸のhp</param>
        public BarrageUsedSkillData(string _skillName, string _conditions, SkillGenreL _sgL, SkillGenreS _sgs, string _aniDName, int _cooldownFps, Motion motion,
            double _radius, int _duration = DataBase.motion_inftyTime, double _space = 0, int _sword = 1, int _score = 10, int _life = 1)
            : base(_skillName, _sgL, _sgs, _cooldownFps, _conditions)
        {
            #region standard variables
            aniDName = _aniDName;
            radius = _radius;
            life = _life;
            score = _score;
            sword = _sword;
            #endregion
            #region about motion
            speed = motion.speed;
            acceleration = motion.acceleration;
            motion_time = motion.alltime;
            moveType = motion.mt;
            pointType = motion.pt;
            vec = motion.pos;
            omega = motion.omega;
            #endregion
            angle = motion.angle;
            if (_sgs == SkillGenreS.laser)
                Console.WriteLine(_skillName + ": laser with default Color!");
            duration = _duration;
            space = _space;
        }
        /// <summary>
        /// 恐らく最も詳しいスキルデータを一つ構成する.これで作ったものには　初期化していないものがないようにするべき
        /// </summary>
        /// <param name="_skillName">スキルの名前，唯一であることを確認してください</param>
        /// <param name="_conditions">スキルの使用条件やその他に使われる</param>
        /// <param name="_sgL">このスキルがどのように解釈されるかを決める</param>
        /// <param name="_sgs">このスキルが生成する弾丸の種類</param>
        /// <param name="motion">Motionを使って、省略する</param>
        /// <param name="_aniDName">弾丸のアニメーション</param>
        /// <param name="_cooldownFps">スキルの使用間隔</param>
        /// <param name="_radius">弾丸の半径</param>
        /// <param name="_c">レーザーに限り、レーザーの直線の色を指定する</param>
        /// <param name="_duration">弾丸が見えるかつ動ける状態の持続時間</param>
        /// <param name="_space">弾丸と弾丸の差、場合によって角度差、距離差</param>
        /// <param name="_sword">弾丸が吸収される時回復するエネルギー</param>
        /// <param name="_score">弾丸が吸収される時の点数</param>
        /// <param name="_life">弾丸のhp</param>
        public BarrageUsedSkillData(string _skillName, string _conditions, SkillGenreL _sgL, SkillGenreS _sgs, string _aniDName, int _cooldownFps, Motion motion,
            double _speed, double _acceleration, double _angle, double _omega,int _motion_time,
            double _radius, Color _c, int _duration = DataBase.motion_inftyTime, double _space = 0, int _sword = 1, int _score = 10, int _life = 1)
            : base(_skillName, _sgL, _sgs, _cooldownFps, _conditions)
        {
            #region standard variables
            aniDName = _aniDName;
            radius = _radius;
            life = _life;
            score = _score;
            sword = _sword;
            #endregion
            #region about motion
            speed = _speed;
            acceleration = _acceleration;
            motion_time = _motion_time;
            moveType = motion.mt;
            pointType = motion.pt;
            vec = motion.pos;
            omega = _omega;
            #endregion
            angle = _angle;
            if (_sgs == SkillGenreS.laser) color = _c;
            else Console.WriteLine(_skillName + ":not a laser but has a color.");
            duration = _duration;
            space = _space;
        }

        /// <summary>
        /// 恐らく最も詳しいスキルデータを一つ構成する.これで作ったものには　初期化していないものがないようにするべき
        /// </summary>
        /// <param name="_skillName">スキルの名前，唯一であることを確認してください</param>
        /// <param name="_conditions">スキルの使用条件やその他に使われる</param>
        /// <param name="_sgL">このスキルがどのように解釈されるかを決める</param>
        /// <param name="_sgs">このスキルが生成する弾丸の種類</param>
        /// <param name="motion">Motionを使って、省略する</param>
        /// <param name="_aniDName">弾丸のアニメーション</param>
        /// <param name="_cooldownFps">スキルの使用間隔</param>
        /// <param name="_radius">弾丸の半径</param>
        /// <param name="_duration">弾丸が見えるかつ動ける状態の持続時間</param>
        /// <param name="_space">弾丸と弾丸の差、場合によって角度差、距離差</param>
        /// <param name="_sword">弾丸が吸収される時回復するエネルギー</param>
        /// <param name="_score">弾丸が吸収される時の点数</param>
        /// <param name="_life">弾丸のhp</param>
        public BarrageUsedSkillData(string _skillName, string _conditions, SkillGenreL _sgL, SkillGenreS _sgs, string _aniDName, int _cooldownFps, Motion motion,
            double _speed, double _acceleration, double _angle, double _omega, int _motion_time,
            double _radius, int _duration = DataBase.motion_inftyTime, double _space = 0, int _sword = 1, int _score = 10, int _life = 1)
            : base(_skillName, _sgL, _sgs, _cooldownFps, _conditions)
        {
            #region standard variables
            aniDName = _aniDName;
            radius = _radius;
            life = _life;
            score = _score;
            sword = _sword;
            #endregion
            #region about motion
            speed = _speed;
            acceleration = _acceleration;
            motion_time = _motion_time;
            moveType = motion.mt;
            pointType = motion.pt;
            vec = motion.pos;
            omega = _omega;
            #endregion
            angle = motion.angle;
            if (_sgs == SkillGenreS.laser)
                Console.WriteLine(_skillName + ": laser with default Color!");
            duration = _duration;
            space = _space;
        }
    }

    class WayShotSkillData : BarrageUsedSkillData
    {
        public int way;

        /// <summary>
        /// SkilledBulletを使わない、同時に_way個の弾丸を発するスキル、レーザーは作れない
        /// </summary>
        /// <param name="_skillName">スキルの名前，唯一であることを確認してください</param>
        /// <param name="_way">同時に発する弾丸の数</param>
        /// <param name="_conditions">スキルの使用条件やその他に使われる</param>
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
        /// <param name="_space">弾丸と弾丸の差、場合によって角度差、距離差</param>
        /// <param name="_sword">弾丸が吸収される時回復するエネルギー</param>
        /// <param name="_score">弾丸が吸収される時の点数</param>
        /// <param name="_life">弾丸のhp</param>
        public WayShotSkillData(string _skillName, string _conditions,SkillGenreS _sgs, string _aniDName, int _cooldownFps, MoveType mt, PointType pt, Vector v, int _motion_fps, double _speed, double _acceleration, double _angle, double _omega,
            double _radius,int _way=1, double _space = 0, int _duration = DataBase.motion_inftyTime, int _sword = 1, int _score = 10, int _life = 1)
            :base(_skillName,_conditions,SkillGenreL.generation,_sgs,_aniDName,_cooldownFps,mt,pt,v,_motion_fps,_speed,_acceleration,_angle,_omega,_radius,_duration,_space,_sword,_score,_life)
        {
            way = _way;
        }
        
        /// <summary>
        /// SkilledBulletを使わない、同時に_way個の弾丸を発するスキル、レーザーもこれで作成する
        /// </summary>
        /// <param name="_skillName">スキルの名前，唯一であることを確認してください</param>
        /// <param name="_way">同時に発する弾丸の数</param>
        /// <param name="_conditions">スキルの使用条件やその他に使われる</param>
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
        /// <param name="_duration">弾丸が見えるかつ動ける状態の持続時間</param>
        /// <param name="_space">弾丸と弾丸の差、場合によって角度差、距離差</param>
        /// <param name="_sword">弾丸が吸収される時回復するエネルギー</param>
        /// <param name="_score">弾丸が吸収される時の点数</param>
        /// <param name="_life">弾丸のhp</param>
        public WayShotSkillData(string _skillName, string _conditions, SkillGenreS _sgs, string _aniDName, int _cooldownFps, MoveType mt, PointType pt, Vector v, int _motion_fps, double _speed, double _acceleration, double _angle, double _omega,
            double _radius, Color _c, int _way = 1, double _space = 0, int _duration = DataBase.motion_inftyTime, int _sword = 1, int _score = 10, int _life = 1)
            : base(_skillName, _conditions, SkillGenreL.generation, _sgs, _aniDName, _cooldownFps, mt, pt, v, _motion_fps, _speed, _acceleration, _angle, _omega, _radius, _c, _duration, _space, _sword, _score, _life)
        {
            way = _way;
        }
        /// <summary>
        /// これは使わないでください。
        /// </summary>
        public WayShotSkillData(string _skillName,string _conditions,SkillGenreS _sgs, MoveType mt,string _aniDName, int _cooldownFps, double _speed, 
            double _acceleration, double _space, double _radius, int _way=1, double _angle=DataBase.AngleToPlayer, int _duration = DataBase.motion_inftyTime, int _sword=1,int _score=10,int _life=1)
            : base(_skillName,_conditions,SkillGenreL.generation,_sgs,_aniDName,_cooldownFps, mt, PointType.Direction, new Vector(),0,_speed, _acceleration,_angle,0,_radius,_duration,_space,_sword,_score,_life)

        {
            way = _way;
        }

        /// <summary>
        /// SkilledBulletは使わない、Motionを使って書き方を簡略化する
        /// </summary>
        /// <param name="_skillName">スキルの名前，唯一であることを確認してください</param>
        /// <param name="_conditions">スキルの使用条件やその他に使われる</param>
        /// <param name="_sgL">このスキルがどのように解釈されるかを決める</param>
        /// <param name="_sgs">このスキルが生成する弾丸の種類</param>
        /// <param name="motion">Motionを使って、省略する</param>
        /// <param name="_aniDName">弾丸のアニメーション</param>
        /// <param name="_cooldownFps">スキルの使用間隔</param>
        /// <param name="_radius">弾丸の半径</param>
        /// <param name="_way">同時に発する弾丸の個数</param>
        /// <param name="_c">レーザーに限り、レーザーの直線の色を指定する</param>
        /// <param name="_duration">弾丸が見えるかつ動ける状態の持続時間</param>
        /// <param name="_space">弾丸と弾丸の差、場合によって角度差、距離差</param>
        /// <param name="_sword">弾丸が吸収される時回復するエネルギー</param>
        /// <param name="_score">弾丸が吸収される時の点数</param>
        /// <param name="_life">弾丸のhp</param>
        public WayShotSkillData(string _skillName, string _conditions, SkillGenreL _sgL,SkillGenreS _sgs, string _aniDName, int _cooldownFps, Motion motion,
            double _radius, Color _c, int _way=1, double _space = 0, int _duration = DataBase.motion_inftyTime, int _sword = 1, int _score = 10, int _life = 1)
            : base(_skillName, _conditions, _sgL, _sgs, _aniDName, _cooldownFps, motion, _radius, _c, _duration, _space, _sword, _score, _life)
        {
            way = _way;
        }

        /// <summary>
        /// SkilledBulletは使わない、レーザー作らない、Motionを使って書き方を簡略化する
        /// </summary>
        /// <param name="_skillName">スキルの名前，唯一であることを確認してください</param>
        /// <param name="_conditions">スキルの使用条件やその他に使われる</param>
        /// <param name="_sgL">このスキルがどのように解釈されるかを決める</param>
        /// <param name="_sgs">このスキルが生成する弾丸の種類</param>
        /// <param name="motion">Motionを使って、省略する</param>
        /// <param name="_aniDName">弾丸のアニメーション</param>
        /// <param name="_cooldownFps">スキルの使用間隔</param>
        /// <param name="_radius">弾丸の半径</param>
        /// <param name="_way">同時に発する弾丸の個数</param>
        /// <param name="_duration">弾丸が見えるかつ動ける状態の持続時間</param>
        /// <param name="_space">弾丸と弾丸の差、場合によって角度差、距離差</param>
        /// <param name="_sword">弾丸が吸収される時回復するエネルギー</param>
        /// <param name="_score">弾丸が吸収される時の点数</param>
        /// <param name="_life">弾丸のhp</param>
        public WayShotSkillData(string _skillName, string _conditions, SkillGenreL _sgL, SkillGenreS _sgs, string _aniDName, int _cooldownFps, Motion motion,
            double _radius, int _way=1, double _space = 0, int _duration = DataBase.motion_inftyTime, int _sword = 1, int _score = 10, int _life = 1)
            : base(_skillName, _conditions, _sgL, _sgs, _aniDName, _cooldownFps, motion, _radius, _duration, _space, _sword, _score, _life)
        {
            way = _way;
        }
        /// <summary>
        /// SkilledBulletは使わない、レーザー作らない、Motionを使って書き方を簡略化する
        /// </summary>
        /// <param name="_skillName">スキルの名前，唯一であることを確認してください</param>
        /// <param name="_conditions">スキルの使用条件やその他に使われる</param>
        /// <param name="_sgs">このスキルが生成する弾丸の種類</param>
        /// <param name="motion">Motionを使って、省略する</param>
        /// <param name="_aniDName">弾丸のアニメーション</param>
        /// <param name="_cooldownFps">スキルの使用間隔</param>
        /// <param name="_radius">弾丸の半径</param>
        /// <param name="_way">同時に発する弾丸の個数</param>
        /// <param name="_duration">弾丸が見えるかつ動ける状態の持続時間</param>
        /// <param name="_space">弾丸と弾丸の差、場合によって角度差、距離差</param>
        /// <param name="_sword">弾丸が吸収される時回復するエネルギー</param>
        /// <param name="_score">弾丸が吸収される時の点数</param>
        /// <param name="_life">弾丸のhp</param>
        public WayShotSkillData(string _skillName, string _conditions, SkillGenreS _sgs, string _aniDName, int _cooldownFps, Motion motion,
            double _radius, int _way = 1, double _space = 0, int _duration = DataBase.motion_inftyTime, int _sword = 1, int _score = 10, int _life = 1)
            : base(_skillName, _conditions, SkillGenreL.generation, _sgs, _aniDName, _cooldownFps, motion, _radius, _duration, _space, _sword, _score, _life)
        {
            way = _way;
        }

        /// <summary>
        /// SkilledBulletは使わない、Motionを使って書き方を簡略化する
        /// </summary>
        /// <param name="_skillName">スキルの名前，唯一であることを確認してください</param>
        /// <param name="_conditions">スキルの使用条件やその他に使われる</param>
        /// <param name="_sgs">このスキルが生成する弾丸の種類</param>
        /// <param name="motion">Motionを使って、省略する</param>
        /// <param name="_aniDName">弾丸のアニメーション</param>
        /// <param name="_cooldownFps">スキルの使用間隔</param>
        /// <param name="_radius">弾丸の半径</param>
        /// <param name="_way">同時に発する弾丸の個数</param>
        /// <param name="_c">レーザーに限り、レーザーの直線の色を指定する</param>
        /// <param name="_duration">弾丸が見えるかつ動ける状態の持続時間</param>
        /// <param name="_space">弾丸と弾丸の差、場合によって角度差、距離差</param>
        /// <param name="_sword">弾丸が吸収される時回復するエネルギー</param>
        /// <param name="_score">弾丸が吸収される時の点数</param>
        /// <param name="_life">弾丸のhp</param>
        public WayShotSkillData(string _skillName, string _conditions, SkillGenreS _sgs, string _aniDName, int _cooldownFps, Motion motion,
            double _speed, double _acceleration,double _angle,double _omega,int _motion_time,
            double _radius, Color _c, int _way = 1, double _space = 0, int _duration = DataBase.motion_inftyTime, int _sword = 1, int _score = 10, int _life = 1)
            : base(_skillName, _conditions, SkillGenreL.generation, _sgs, _aniDName, _cooldownFps, motion,_speed,_acceleration,_angle,_omega,_motion_time, _radius, _c, _duration, _space, _sword, _score, _life)
        {
            way = _way;
        }

        /// <summary>
        /// SkilledBulletは使わない、レーザー作らない、Motionを使って書き方を簡略化する
        /// </summary>
        /// <param name="_skillName">スキルの名前，唯一であることを確認してください</param>
        /// <param name="_conditions">スキルの使用条件やその他に使われる</param>
        /// <param name="_sgs">このスキルが生成する弾丸の種類</param>
        /// <param name="motion">Motionを使って、省略する</param>
        /// <param name="_aniDName">弾丸のアニメーション</param>
        /// <param name="_cooldownFps">スキルの使用間隔</param>
        /// <param name="_radius">弾丸の半径</param>
        /// <param name="_way">同時に発する弾丸の個数</param>
        /// <param name="_duration">弾丸が見えるかつ動ける状態の持続時間</param>
        /// <param name="_space">弾丸と弾丸の差、場合によって角度差、距離差</param>
        /// <param name="_sword">弾丸が吸収される時回復するエネルギー</param>
        /// <param name="_score">弾丸が吸収される時の点数</param>
        /// <param name="_life">弾丸のhp</param>
        public WayShotSkillData(string _skillName, string _conditions, SkillGenreS _sgs, string _aniDName, int _cooldownFps, Motion motion,
            double _speed, double _acceleration, double _angle, double _omega, int _motion_time,
            double _radius, int _way = 1, double _space = 0, int _duration = DataBase.motion_inftyTime, int _sword = 1, int _score = 10, int _life = 1)
            : base(_skillName, _conditions, SkillGenreL.generation, _sgs, _aniDName, _cooldownFps, motion,_speed,_acceleration,_angle,_omega,_motion_time, _radius, _duration, _space, _sword, _score, _life)
        {
            way = _way;
        }
        /// <summary>
        /// SkilledBulletは使わない、Motionを使って書き方を簡略化する
        /// </summary>
        /// <param name="_skillName">スキルの名前，唯一であることを確認してください</param>
        /// <param name="_conditions">スキルの使用条件やその他に使われる</param>
        /// <param name="_sgL">このスキルがどのように解釈されるかを決める</param>
        /// <param name="_sgs">このスキルが生成する弾丸の種類</param>
        /// <param name="motion">Motionを使って、省略する</param>
        /// <param name="_aniDName">弾丸のアニメーション</param>
        /// <param name="_cooldownFps">スキルの使用間隔</param>
        /// <param name="_radius">弾丸の半径</param>
        /// <param name="_way">同時に発する弾丸の個数</param>
        /// <param name="_c">レーザーに限り、レーザーの直線の色を指定する</param>
        /// <param name="_duration">弾丸が見えるかつ動ける状態の持続時間</param>
        /// <param name="_space">弾丸と弾丸の差、場合によって角度差、距離差</param>
        /// <param name="_sword">弾丸が吸収される時回復するエネルギー</param>
        /// <param name="_score">弾丸が吸収される時の点数</param>
        /// <param name="_life">弾丸のhp</param>
        public WayShotSkillData(string _skillName, string _conditions, SkillGenreL _sgl,SkillGenreS _sgs, string _aniDName, int _cooldownFps, Motion motion,
            double _speed, double _acceleration, double _angle, double _omega, int _motion_time,
            double _radius, Color _c, int _way = 1, double _space = 0, int _duration = DataBase.motion_inftyTime, int _sword = 1, int _score = 10, int _life = 1)
            : base(_skillName, _conditions, _sgl, _sgs, _aniDName, _cooldownFps, motion, _speed, _acceleration, _angle, _omega, _motion_time, _radius, _c, _duration, _space, _sword, _score, _life)
        {
            way = _way;
        }

        /// <summary>
        /// SkilledBulletは使わない、レーザー作らない、Motionを使って書き方を簡略化する
        /// </summary>
        /// <param name="_skillName">スキルの名前，唯一であることを確認してください</param>
        /// <param name="_conditions">スキルの使用条件やその他に使われる</param>
        /// <param name="_sgL">このスキルがどのように解釈されるかを決める</param>
        /// <param name="_sgs">このスキルが生成する弾丸の種類</param>
        /// <param name="motion">Motionを使って、省略する</param>
        /// <param name="_aniDName">弾丸のアニメーション</param>
        /// <param name="_cooldownFps">スキルの使用間隔</param>
        /// <param name="_radius">弾丸の半径</param>
        /// <param name="_way">同時に発する弾丸の個数</param>
        /// <param name="_duration">弾丸が見えるかつ動ける状態の持続時間</param>
        /// <param name="_space">弾丸と弾丸の差、場合によって角度差、距離差</param>
        /// <param name="_sword">弾丸が吸収される時回復するエネルギー</param>
        /// <param name="_score">弾丸が吸収される時の点数</param>
        /// <param name="_life">弾丸のhp</param>
        public WayShotSkillData(string _skillName, string _conditions, SkillGenreL _sgl,SkillGenreS _sgs, string _aniDName, int _cooldownFps, Motion motion,
            double _speed, double _acceleration, double _angle, double _omega, int _motion_time,
            double _radius, int _way = 1, double _space = 0, int _duration = DataBase.motion_inftyTime, int _sword = 1, int _score = 10, int _life = 1)
            : base(_skillName, _conditions, _sgl, _sgs, _aniDName, _cooldownFps, motion, _speed, _acceleration, _angle, _omega, _motion_time, _radius, _duration, _space, _sword, _score, _life)
        {
            way = _way;
        }
    }//class WayShotSkillData end

    class WaySkilledBulletsData: WayShotSkillData
    {
        public string[] BulletSkillNames;

        /// <summary>
        /// SkilledBulletは使わない、Motionを使って書き方を簡略化する
        /// </summary>
        /// <param name="_skillName">スキルの名前，唯一であることを確認してください</param>
        /// <param name="_conditions">スキルの使用条件やその他に使われる</param>
        /// <param name="_sgL">このスキルがどのように解釈されるかを決める</param>
        /// <param name="_sgs">このスキルが生成する弾丸の種類</param>
        /// <param name="motion">Motionを使って、省略する</param>
        /// <param name="_aniDName">弾丸のアニメーション</param>
        /// <param name="_cooldownFps">スキルの使用間隔</param>
        /// <param name="_radius">弾丸の半径</param>
        /// <param name="_way">同時に発する弾丸の個数</param>
        /// <param name="_c">レーザーに限り、レーザーの直線の色を指定する</param>
        /// <param name="_duration">弾丸が見えるかつ動ける状態の持続時間</param>
        /// <param name="_space">弾丸と弾丸の差、場合によって角度差、距離差</param>
        /// <param name="_sword">弾丸が吸収される時回復するエネルギー</param>
        /// <param name="_score">弾丸が吸収される時の点数</param>
        /// <param name="_life">弾丸のhp</param>
        public WaySkilledBulletsData(string _skillName, string _conditions, SkillGenreS _sgs, string _aniDName, int _cooldownFps, Motion motion,
            double _radius, Color _c, string _SkillName,int _way=1, int _duration = DataBase.motion_inftyTime, double _space = 0, int _sword = 1, int _score = 10, int _life = 1)
            : base(_skillName, _conditions, SkillGenreL.UseSkilledBullet, _sgs, _aniDName, _cooldownFps, motion, _radius, _c, _way, _space, _duration, _sword, _score, _life)
        {
            BulletSkillNames= new string[] { _SkillName };
        }

        /// <summary>
        /// SkilledBulletを使い、レーザーは作れない、Motionを使って書き方を簡略化する
        /// </summary>
        /// <param name="_skillName">スキルの名前，唯一であることを確認してください</param>
        /// <param name="_conditions">スキルの使用条件やその他に使われる</param>
        /// <param name="_sgL">このスキルがどのように解釈されるかを決める</param>
        /// <param name="_sgs">このスキルが生成する弾丸の種類</param>
        /// <param name="motion">Motionを使って、省略する</param>
        /// <param name="_aniDName">弾丸のアニメーション</param>
        /// <param name="_cooldownFps">スキルの使用間隔</param>
        /// <param name="_radius">弾丸の半径</param>
        /// <param name="_way">同時に発する弾丸の個数</param>
        /// <param name="_duration">弾丸が見えるかつ動ける状態の持続時間</param>
        /// <param name="_space">弾丸と弾丸の差、場合によって角度差、距離差</param>
        /// <param name="_sword">弾丸が吸収される時回復するエネルギー</param>
        /// <param name="_score">弾丸が吸収される時の点数</param>
        /// <param name="_life">弾丸のhp</param>
        public WaySkilledBulletsData(string _skillName, string _conditions, SkillGenreS _sgs, string _aniDName, int _cooldownFps, Motion motion,
            double _radius, string _SkillName, int _way=1, int _duration = DataBase.motion_inftyTime, double _space = 0, int _sword = 1, int _score = 10, int _life = 1)
            : base(_skillName, _conditions, SkillGenreL.UseSkilledBullet, _sgs, _aniDName, _cooldownFps, motion, _radius, _way, _space, _duration, _sword, _score, _life)
        {
            BulletSkillNames = new string[] { _SkillName };
        }

        /// <summary>
        /// SkilledBulletは使わない、Motionを使って書き方を簡略化する
        /// </summary>
        /// <param name="_skillName">スキルの名前，唯一であることを確認してください</param>
        /// <param name="_conditions">スキルの使用条件やその他に使われる</param>
        /// <param name="_sgL">このスキルがどのように解釈されるかを決める</param>
        /// <param name="_sgs">このスキルが生成する弾丸の種類</param>
        /// <param name="motion">Motionを使って、省略する</param>
        /// <param name="_aniDName">弾丸のアニメーション</param>
        /// <param name="_cooldownFps">スキルの使用間隔</param>
        /// <param name="_radius">弾丸の半径</param>
        /// <param name="_way">同時に発する弾丸の個数</param>
        /// <param name="_c">レーザーに限り、レーザーの直線の色を指定する</param>
        /// <param name="_duration">弾丸が見えるかつ動ける状態の持続時間</param>
        /// <param name="_space">弾丸と弾丸の差、場合によって角度差、距離差</param>
        /// <param name="_sword">弾丸が吸収される時回復するエネルギー</param>
        /// <param name="_score">弾丸が吸収される時の点数</param>
        /// <param name="_life">弾丸のhp</param>
        public WaySkilledBulletsData(string _skillName, string _conditions, SkillGenreS _sgs, string _aniDName, int _cooldownFps, Motion motion,
            double _radius, Color _c, string[] _SkillNames, int _way = 1, int _duration = DataBase.motion_inftyTime, double _space = 0, int _sword = 1, int _score = 10, int _life = 1)
            : base(_skillName, _conditions, SkillGenreL.UseSkilledBullet, _sgs, _aniDName, _cooldownFps, motion, _radius, _c, _way, _space, _duration, _sword, _score, _life)
        {
            BulletSkillNames = _SkillNames;
        }

        /// <summary>
        /// SkilledBulletは使わない、レーザーは作れない、Motionを使って書き方を簡略化する
        /// </summary>
        /// <param name="_skillName">スキルの名前，唯一であることを確認してください</param>
        /// <param name="_conditions">スキルの使用条件やその他に使われる</param>
        /// <param name="_sgL">このスキルがどのように解釈されるかを決める</param>
        /// <param name="_sgs">このスキルが生成する弾丸の種類</param>
        /// <param name="motion">Motionを使って、省略する</param>
        /// <param name="_aniDName">弾丸のアニメーション</param>
        /// <param name="_cooldownFps">スキルの使用間隔</param>
        /// <param name="_radius">弾丸の半径</param>
        /// <param name="_way">同時に発する弾丸の個数</param>
        /// <param name="_duration">弾丸が見えるかつ動ける状態の持続時間</param>
        /// <param name="_space">弾丸と弾丸の差、場合によって角度差、距離差</param>
        /// <param name="_sword">弾丸が吸収される時回復するエネルギー</param>
        /// <param name="_score">弾丸が吸収される時の点数</param>
        /// <param name="_life">弾丸のhp</param>
        public WaySkilledBulletsData(string _skillName, string _conditions, SkillGenreS _sgs, string _aniDName, int _cooldownFps, Motion motion,
            double _radius, string[] _SkillNames, int _way = 1, int _duration = DataBase.motion_inftyTime, double _space = 0, int _sword = 1, int _score = 10, int _life = 1)
            : base(_skillName, _conditions, SkillGenreL.UseSkilledBullet, _sgs, _aniDName, _cooldownFps, motion, _radius, _way, _space, _duration, _sword, _score, _life)
        {
            BulletSkillNames = _SkillNames;
        }

        /// <summary>
        /// SkilledBulletは使わない、Motionを使って書き方を簡略化する
        /// </summary>
        /// <param name="_skillName">スキルの名前，唯一であることを確認してください</param>
        /// <param name="_conditions">スキルの使用条件やその他に使われる</param>
        /// <param name="_sgL">このスキルがどのように解釈されるかを決める</param>
        /// <param name="_sgs">このスキルが生成する弾丸の種類</param>
        /// <param name="motion">Motionを使って、省略する</param>
        /// <param name="_aniDName">弾丸のアニメーション</param>
        /// <param name="_cooldownFps">スキルの使用間隔</param>
        /// <param name="_radius">弾丸の半径</param>
        /// <param name="_way">同時に発する弾丸の個数</param>
        /// <param name="_c">レーザーに限り、レーザーの直線の色を指定する</param>
        /// <param name="_duration">弾丸が見えるかつ動ける状態の持続時間</param>
        /// <param name="_space">弾丸と弾丸の差、場合によって角度差、距離差</param>
        /// <param name="_sword">弾丸が吸収される時回復するエネルギー</param>
        /// <param name="_score">弾丸が吸収される時の点数</param>
        /// <param name="_life">弾丸のhp</param>
        public WaySkilledBulletsData(string _skillName, string _conditions, SkillGenreS _sgs, string _aniDName, int _cooldownFps, Motion motion,
            double _speed, double _acceleration, double _angle, double _omega, int _motion_time,
            double _radius, Color _c, string[] _SkillNames, int _way = 1, int _duration = DataBase.motion_inftyTime, double _space = 0, int _sword = 1, int _score = 10, int _life = 1)
            : base(_skillName, _conditions, SkillGenreL.UseSkilledBullet, _sgs, _aniDName, _cooldownFps, motion, _speed, _acceleration, _angle, _omega, _motion_time, _radius, _c, _way, _space, _duration, _sword, _score, _life)
        {
            BulletSkillNames = _SkillNames;
        }

        /// <summary>
        /// SkilledBulletは使わない、レーザーは作れない、Motionを使って書き方を簡略化する
        /// </summary>
        /// <param name="_skillName">スキルの名前，唯一であることを確認してください</param>
        /// <param name="_conditions">スキルの使用条件やその他に使われる</param>
        /// <param name="_sgL">このスキルがどのように解釈されるかを決める</param>
        /// <param name="_sgs">このスキルが生成する弾丸の種類</param>
        /// <param name="motion">Motionを使って、省略する</param>
        /// <param name="_aniDName">弾丸のアニメーション</param>
        /// <param name="_cooldownFps">スキルの使用間隔</param>
        /// <param name="_radius">弾丸の半径</param>
        /// <param name="_way">同時に発する弾丸の個数</param>
        /// <param name="_duration">弾丸が見えるかつ動ける状態の持続時間</param>
        /// <param name="_space">弾丸と弾丸の差、場合によって角度差、距離差</param>
        /// <param name="_sword">弾丸が吸収される時回復するエネルギー</param>
        /// <param name="_score">弾丸が吸収される時の点数</param>
        /// <param name="_life">弾丸のhp</param>
        public WaySkilledBulletsData(string _skillName, string _conditions, SkillGenreS _sgs, string _aniDName, int _cooldownFps, Motion motion,
            double _speed, double _acceleration, double _angle, double _omega, int _motion_time,
            double _radius, string[] _SkillNames, int _way = 1, int _duration = DataBase.motion_inftyTime, double _space = 0, int _sword = 1, int _score = 10, int _life = 1)
            : base(_skillName, _conditions, SkillGenreL.UseSkilledBullet, _sgs, _aniDName, _cooldownFps, motion,_speed,_acceleration,_angle,_omega,_motion_time, _radius, _way, _space, _duration, _sword, _score, _life)
        {
            BulletSkillNames = _SkillNames;
        }
    }

}
