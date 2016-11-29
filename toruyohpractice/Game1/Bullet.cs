using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CommonPart
{
    class Bullet:Projection
    {
        public int life;
        public int score;
        public int sword;
        public bool lasered;
        public int atk = 1;

        /// <summary>
        /// 目標物体がある場合に使う
        /// </summary>
        /// <param name="_target">目標物体</param>
        /// <param name="_radian">初期角度、意味がない場合もある</param>
        public Bullet(double _x, double _y, MoveType _move_type, double _speed, double _acceleration, string _anime, Unit _target,
            double _radian, double _radius, int _sword, int _life = 1, int _score = 0, int _zoom_rate = 100)
            : base(_x, _y, _move_type, _anime, _target, _speed, _acceleration, _radian, _zoom_rate)
        {
            radius = _radius;
            life = _life;
            score = _score;
            sword = _sword;
            lasered = false;
        }
        /// <summary>
        /// 目標点だけある場合に使う
        /// </summary>
        /// <param name="_target_pos">使われる点</param>
        /// <param name="_pt">その点の使い方</param>
        /// <param name="_radian">初期角度、意味がない場合もある</param>
        public Bullet(double _x,double _y, MoveType _move_type,double _speed,double _acceleration,string _anime,Vector _target_pos,PointType _pt,
            double _radian,double _radius,int _sword, int _life=1,int _score=0, int _zoom_rate=100)
            :base(_x,_y,_move_type, _anime, _target_pos,_pt,_speed, _acceleration,_radian,_zoom_rate)
        {
            radius = _radius;
            life = _life;
            score = _score;
            sword = _sword;
            lasered = false;
        }
        /// <summary>
        /// 目標点も目標物体も使わない場合に使う。
        /// </summary>
        public Bullet(double _x, double _y, MoveType _move_type, double _speed, double _acceleration, string _anime, double _radian,
            double _radius,  int _sword, int _life=1, int _score=0, int _zoom_rate=100)
            : base(_x, _y, _move_type, _anime,_speed, _acceleration,_radian, _zoom_rate)
        {
            radius = _radius;
            life = _life;
            score = _score;
            sword = _sword;
            lasered = false;
        }

        public virtual void update(Player player,bool bulletMove=true)
        {
            base.update(bulletMove);
            if (x < Map.leftside - animation.X / 2 || x > Map.rightside + animation.X / 2
                || y > DataBase.WindowSlimSizeY + animation.Y / 2 || y < 0 - animation.Y / 2)
            {
                remove(Unit_state.out_of_window);
            }

            if (hit_jugde(player) == true)
            {
                player.damage(atk);
                remove(Unit_state.bulletDamagedPlayer);
            }
        }

        public override bool hit_jugde(Unit player)
        {
            return Function.hitcircle(x, y, radius, player.x, player.y, player.radius);
        }
        /// <summary>
        /// このbulletのx,yを原点として、このbulletのradius+p_radius半径内にpx,pyがあるかどうか
        /// </summary>
        /// <param name="px"></param>
        /// <param name="py"></param>
        /// <param name="p_radius"></param>
        /// <returns></returns>
        public override bool hit_jugde(double px, double py, double p_radius = 0)
        {
            return Function.hitcircle(x, y, radius, px, py, p_radius);
        }
        public virtual void damage(int d) {
            life -= d;
            if (life <= 0)  remove(Unit_state.dead);
        }
        protected virtual void dead()
        {
            delete = true;
            Map.make_chargePro(x, y, sword, Map.caculateBulletScore(sword));
        }
        protected virtual void killed()
        {
            Map.make_chargePro(x, y, sword, Map.caculateBulletScore(sword));
            dead();
        }
        public override void remove(Unit_state us=Unit_state.dead)
        {
            switch (us)
            {
                case Unit_state.exist_timeOut:
                    dead();
                    break;
                case Unit_state.dead: //弾丸がダメージを受けてなくなったか、発射したものが消えたからなくなった。
                    killed();
                    break;
                case Unit_state.bulletDamagedPlayer:
                    delete = true;
                    break;
                case Unit_state.fadeout:
                    delete = true;
                    break;
                case Unit_state.out_of_window:
                    delete = true;
                    break;
                default:
                    break;
            }
        }
    }
}
