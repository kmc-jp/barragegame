using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonPart
{
    class SkilledBullet : Bullet
    {
        Skill skill;
        Enemy myboss;

        /// <summary>
        /// 目標物体なし、目標点なしの場合に使える。
        /// </summary>
        /// <param name="_x"></param>
        /// <param name="_y"></param>
        /// <param name="_move_type"></param>
        /// <param name="_speed"></param>
        /// <param name="_acceleration"></param>
        /// <param name="_radian"></param>
        /// <param name="_anime"></param>
        /// <param name="_zoom_rate"></param>
        /// <param name="_radius"></param>
        /// <param name="_life"></param>
        /// <param name="_score"></param>
        /// <param name="_sword"></param>
        /// <param name="_skillName"></param>
        /// <param name="enemy"></param>
        public SkilledBullet(double _x, double _y, MoveType _move_type, double _speed, double _acceleration,string _anime, double _radian, double _radius, 
            string _skillName, Enemy enemy, int _sword,int _score=0,int _life=1,int _zoom_rate=100)
            : base(_x, _y, _move_type, _speed, _acceleration, _anime,_radian, _radius, _sword, _life, _score,_zoom_rate)
        {
            skill = new Skill(_skillName);
            myboss = enemy;
        }
        /// <summary>
        /// 目標がない場合に使う
        /// </summary>
        /// <param name="_radian">初期角度</param>
        /// <param name="_omega">レーザーが回転するならば、毎フレーム回転する度合い</param>
        /// <param name="_enemy">レーザーを使用したもの</param>
        /// <param name="_color">レーザーの色</param>
        public SkilledBullet(double _x, double _y, MoveType _move_type, double _speed, double _acceleration, string _anime, Unit _target,double _radian, double _radius,
             string _skillName, Enemy enemy,  int _sword, int _life = 1, int _score = 0, int _zoom_rate = 100)
            : base(_x, _y, _move_type, _speed, _acceleration, _anime,_target,_radian, _radius, _sword, _life, _score, _zoom_rate)
        {
            skill = new Skill(_skillName);
            myboss = enemy;
        }
        /// <summary>
        /// 目標点だけはある場合に使う
        /// </summary>
        /// <param name="_radian">初期角度</param>
        /// <param name="_omega">レーザーが回転するならば、毎フレーム回転する度合い</param>
        /// <param name="_enemy">レーザーを使用したもの</param>
        /// <param name="_color">レーザーの色</param>
        public SkilledBullet(double _x, double _y, MoveType _move_type, double _speed, double _acceleration, string _anime,Vector _target_pos, PointType _pt
        , double _radian, double _radius, string _skillName, Enemy enemy,  int _sword, int _life = 1, int _score = 0, int _zoom_rate = 100)
            : base(_x, _y, _move_type, _speed, _acceleration, _anime,_target_pos,_pt, _radian, _radius, _sword, _life, _score, _zoom_rate)
        {
            skill = new Skill(_skillName);
            myboss = enemy;
        }

        protected override void dead()
        {
            base.dead();
            shot(Map.player);
        }
        public override void update(Player player,bool bulletMove)
        {
            base.update(player,bulletMove);
            skill.update();
            shot(player);
        }
        
        public void shot()
        {

        }
    }
}
