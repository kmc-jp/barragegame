using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CommonPart
{
    class LaserTop:Bullet
    {
        public double length = 0;
        protected Color color;
        protected Unit enemy;

        /// <summary>
        /// 目標がない場合に使う
        /// </summary>
        /// <param name="_radian">初期角度</param>
        /// <param name="_omega">レーザーが回転するならば、毎フレーム回転する度合い</param>
        /// <param name="_enemy">レーザーを使用したもの</param>
        /// <param name="_color">レーザーの色</param>
        public LaserTop(double _x, double _y, MoveType _move_type, double _speed, double _acceleration, double _radian, string _anime,double _radius, 
            double _omega,Unit _enemy,Color _color, int _sword, int _life=1, int _score=0,  int _zoom_rate=100)
            : base(_x, _y, _move_type, _speed, _acceleration, _anime,_radian, _omega,_radius, _sword, _life, _score, _zoom_rate)
        {
            omega = _omega;
            enemy = _enemy;
            color = _color;

        }
        /// <summary>
        /// 目標点だけはある場合に使う
        /// </summary>
        /// <param name="_radian">初期角度</param>
        /// <param name="_omega">レーザーが回転するならば、毎フレーム回転する度合い</param>
        /// <param name="_enemy">レーザーを使用したもの</param>
        /// <param name="_color">レーザーの色</param>
        public LaserTop(double _x, double _y, MoveType _move_type, double _speed, double _acceleration, Vector _target_pos, PointType _pt,int _motion_time,
  string _anime, double _radian, double _radius, double _omega, Unit _enemy, Color _color, int _sword, int _life = 1, int _score = 0, int _zoom_rate = 100)
            : base(_x, _y, _move_type, _speed, _acceleration, _anime,_target_pos,_pt, _motion_time,_radian,_omega, _radius, _sword, _life, _score, _zoom_rate)
        {
            omega = _omega;
            enemy = _enemy;
            color = _color;
            
        }

        /// <summary>
        /// 目標物体がある場合に使う
        /// </summary>
        /// <param name="_radian">初期角度</param>
        /// <param name="_omega">レーザーが回転するならば、毎フレーム回転する度合い</param>
        /// <param name="_enemy">レーザーを使用したもの</param>
        /// <param name="_color">レーザーの色</param>
        public LaserTop(double _x, double _y, MoveType _move_type, double _speed, double _acceleration, Unit _target, string _anime
            , double _radian, double _radius, double _omega, Unit _enemy, Color _color, int _sword, int _life = 1, int _score = 0, int _zoom_rate = 100)
            : base(_x, _y, _move_type, _speed, _acceleration, _anime,_target, _radian, _omega,_radius, _sword, _life, _score, _zoom_rate)
        {
            omega = _omega;
            enemy = _enemy;
            color = _color;

        }


        public override void update(Player player,bool bulletMove,bool skillsUpdate=false)
        {
            update(bulletMove);
            #region Laser Motion: length is not changed here!
            switch (move_type)
            {
                /*case MoveType.chase_player_target:
                    speed += acceleration;
                    radian = Function.towardValue(radian, Math.Atan2(target.y-enemy.y,target.x-enemy.x), omega);
                    
                    x = enemy.x+length * Math.Cos(radian);
                    y = enemy.y+length * Math.Sin(radian);
                    break;
                case MoveType.go_straight: //これはBulletのgo_straightを上書きする
                    speed += acceleration;
                    radian += omega;
                    //x = enemy.x+ length * Math.Cos(radian);
                    //y = enemy.y+ length * Math.Sin(radian);
                    break;
                    */
            }
            #endregion
            if (bulletMove)
            {
                #region Laser cannot getout of Window;  changes length and limit it
                if ((x < Map.leftside + animation.X / 2) || (x > Map.rightside - animation.X / 2) || (y > DataBase.WindowSlimSizeY - animation.Y / 2)
                    || (y < 0 + animation.Y / 2))
                { /* もしレーザーの先頭がすでに画面外に出ているなら、lengthを増やさない*/  }
                else
                {
                    length += speed;
                }
                if (length <= 0) { length = 0; }
                #endregion
            }
            if (hit_jugde(player) == true)
            {
                if (!player.avoid_mode)
                {
                    player.damage(atk);
                    //レーザーはキャラクターにダメージを与えても消えない。
                }else { //レーザーが回避中のプレイヤーに当たる
                    damage(player.atk);
                }
            }
        }
        public override void damage(int d)
        {
            length = 0;
            x = enemy.x; y = enemy.y;
            Map.make_chargePro(x, y, sword, Map.caculateBulletScore(sword));
            //laserはダメージを受けない
        }
        public override bool hit_jugde(double px, double py, double p_radius = 0)
        {
            bool close;
            bool hit;
            double k;
            double d;
            if (Math.Abs(enemy.x - x) > 0.05)
            {
                bool px_inside =  ((enemy.x <= x && (enemy.x < px && px < x)) || (enemy.x >= x && x < px && px < enemy.x)) ;
                bool py_inside =  ((enemy.y <= y && (enemy.y < py && py < y)) || (enemy.y >= y && y < py && py < enemy.y));
                close = px_inside && py_inside;

                k = (enemy.y - y) / (enemy.x - x);
                d = (Math.Abs(k * px - py - k * x + y)) / Math.Sqrt(k * k + 1);
                hit = d < (p_radius + radius);
            }
            else
            {
                close = (py > enemy.y && py < y) || (py < enemy.y && py > y);
                hit = Math.Abs(x - px) < (p_radius + radius);
            }
            return (hit && close);
        }
        public override bool hit_jugde(Unit player)
        {
            return hit_jugde(player.x, player.y, player.radius);
            /*
            bool close;
            bool hit;
            double k;
            double d;
            if (Math.Abs(enemy.x - x) > 0.05)
            {
                if (enemy.x <= x)
                {
                    if (enemy.y <= y)
                    {
                        close = (player.x > enemy.x && player.x < x) && (player.y > enemy.y && player.y < y);
                    }
                    else
                    {
                        close = (player.x > enemy.x && player.x < x) && (player.y < enemy.y && player.y > y);
                    }
                }
                else
                {
                    if (enemy.y <= y)
                    {
                        close = (player.x < enemy.x && player.x > x) && (player.y > enemy.y && player.y < y);
                    }
                    else
                    {
                        close = (player.x < enemy.x && player.x > x) && (player.y < enemy.y && player.y > y);
                    }
                }
                k = (enemy.y - y) / (enemy.x - x);
                d = (Math.Abs(k * player.x - player.y - k * x + y)) / Math.Sqrt(k * k + 1);
                hit = d < (player.radius + radius);
            }
            else
            {
                close = (player.y > enemy.y && player.y < y) || (player.y < enemy.y && player.y > y);
                hit = Math.Abs(x - player.x) < (player.radius + radius);
            }
            
            if(hit && close)
            {
                if (false)//(player.avoid_mode == true)
                {
                    length = 0;

                    Map.make_chargePro(x, y,sword,Map.caculateBulletScore(score));
                    return false;
                }
                return true;
            }else { return false; }
            */
        }

        public override void draw(Drawing d)
        {
            if (!visible()) return;
            double dx, dy;
            dx = (radius) * Math.Cos(radian - Math.PI / 2)/2;
            dy = (radius) * Math.Sin(radian - Math.PI / 2) / 2;
            d.DrawLine(new Vector(enemy.x + dx, enemy.y + dy), new Vector(x + dx, y+dy), (float)radius, new Color(color,(int)(color.A*0.3)), DepthID.Player);
            dx = (radius*2/3) * Math.Cos(radian - Math.PI / 2) / 2;
            dy = (radius*2 /3) * Math.Sin(radian - Math.PI / 2) / 2;

            d.DrawLine(new Vector(enemy.x+dx, enemy.y+dy), new Vector(x+dx, y+dy), (float)radius*2/3, new Color(color, (int)(color.A * 0.6)), DepthID.Player);
            dx = (radius / 4) * Math.Cos(radian - Math.PI / 2) / 2;
            dy = (radius / 4) * Math.Sin(radian - Math.PI / 2) / 2;
            d.DrawLine(new Vector(enemy.x+dx, enemy.y+dy), new Vector(x+dx, y+dy), (float)radius/4, color, DepthID.Player);
        }
    }

    class SkilledLaserTop : LaserTop {
        public List<Skill> skills=new List<Skill>();

        /// <summary>
        /// 目標がない場合に使う
        /// </summary>
        /// <param name="_radian">初期角度</param>
        /// <param name="_omega">レーザーが回転するならば、毎フレーム回転する度合い</param>
        /// <param name="_enemy">レーザーを使用したもの</param>
        /// <param name="_color">レーザーの色</param>
        public SkilledLaserTop(double _x, double _y, MoveType _move_type, double _speed, double _acceleration, string _anime, double _radian, double _omega,
            double _radius, string[] _skillNames,Unit _enemy, Color _color, int _sword, int _life = 1, int _score = 0, int _zoom_rate = 100)
            : base(_x, _y, _move_type, _speed, _acceleration, _radian, _anime, _radius,_omega,_enemy,_color, _sword, _life, _score, _zoom_rate)
        {
            addSkills(_skillNames);
        }
        /// <summary>
        /// 目標点だけはある場合に使う
        /// </summary>
        /// <param name="_radian">初期角度</param>
        /// <param name="_omega">レーザーが回転するならば、毎フレーム回転する度合い</param>
        /// <param name="_enemy">レーザーを使用したもの</param>
        /// <param name="_color">レーザーの色</param>
        public SkilledLaserTop(double _x, double _y, MoveType _move_type, double _speed, double _acceleration, Vector _target_pos, PointType _pt,
  string _anime, double _radian, double _omega, double _radius, int _motion_time, string[] _skillNames,Unit _enemy, Color _color, int _sword, int _life = 1, int _score = 0, int _zoom_rate = 100)
            : base(_x, _y, _move_type, _speed, _acceleration, _target_pos,_pt,_motion_time, _anime, _radian, _radius,_omega,_enemy,_color, _sword, _life, _score, _zoom_rate)
        {
            addSkills(_skillNames);
        }

        /// <summary>
        /// 目標物体がある場合に使う
        /// </summary>
        /// <param name="_radian">初期角度</param>
        /// <param name="_omega">レーザーが回転するならば、毎フレーム回転する度合い</param>
        /// <param name="_enemy">レーザーを使用したもの</param>
        /// <param name="_color">レーザーの色</param>
        public SkilledLaserTop(double _x, double _y, MoveType _move_type, double _speed, double _acceleration, Unit _target, string _anime
            , double _radian, double _omega, double _radius, string[] _skillNames,Unit _enemy, Color _color, int _sword, int _life = 1, int _score = 0, int _zoom_rate = 100)
            : base(_x, _y, _move_type, _speed, _acceleration, _target, _anime, _radian, _radius,_omega,_enemy,_color, _sword, _life, _score, _zoom_rate)
        {
            addSkills(_skillNames);
        }

        public void addSkills(string[] _skillNames)
        {
            foreach (string _skillName in _skillNames)
            {
                skills.Add(new Skill(_skillName));
            }
        }

        public override void update(Player player, bool bulletMove, bool skillsUpdate = false)
        {
            base.update(player, bulletMove, false);
            if (skillsUpdate)
            {
                for (int i = 0; i < skills.Count; i++)
                {
                    skills[i].update();
                }
            }
        }
    }
}
