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
        public double radius;
        public int life;
        public int score;
        public int sword;
        public bool lasered;
        public int atk = 1;

        public Bullet(double _x,double _y, MoveType _move_type,double _speed,double _acceleration,string _anime,Vector _target_pos,int _zoom_rate
            ,double _radius, int _life,int _score,int _sword)
            :base(_x,_y,_move_type,_speed,_acceleration,_anime,_target_pos,_zoom_rate)
        {
            radius = _radius;
            life = _life;
            score = _score;
            sword = _sword;
            lasered = false;
        }
        public Bullet(double _x, double _y, MoveType _move_type, double _speed, double _acceleration, double _radian, string _anime, int _zoom_rate,
            double _radius, int _life, int _score, int _sword)
            : base(_x, _y, _move_type, _speed, _acceleration,_radian, _anime, _zoom_rate)
        {
            radius = _radius;
            life = _life;
            score = _score;
            sword = _sword;
            lasered = false;
        }

        public virtual void update(Player player)
        {
            base.update();
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

        public override bool hit_jugde(Player player)
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
        public void damage(int d) {
            life -= d;
            if (life <= 0)  remove(Unit_state.dead);
        }
        public void remove(Unit_state us=Unit_state.dead)
        {
            switch (us)
            {
                case Unit_state.dead: //弾丸がダメージを受けてなくなったか、発射したものが消えたからなくなった。
                    delete = true;
                    Map.make_chargePro(x, y, sword, Map.caculateBulletScore(sword));
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
