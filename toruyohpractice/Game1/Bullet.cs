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

        public bool delete = false;
        public int atk = 1;

        public Bullet(double _x,double _y,MoveType _move_type,int _speed,Animation _anime,Vector _target_pos,int _zoom_rate
            ,double _radius, int _life,int _score,int _sword)
            :base(_x,_y,_move_type,_speed,_anime,_target_pos,_zoom_rate)
        {
            radius = _radius;
            life = _life;
            score = _score;
            sword = _sword;
        }

        public void update(Player player)
        {
            base.update();

            if (x < Map.leftside - animation.X / 2 || x > Map.rightside + animation.X / 2
                || y > DataBase.WindowSlimSizeY + animation.Y / 2 || y < 0 - animation.Y / 2)
            {
                remove();
            }

            if (Function.hitcircle(x, y, radius, player.x, player.y, player.radius) == true)
            {
                ////////
                life--;
                player.damage(atk);
            }
            if (life <= 0)
            {
                remove();
            }
        }

        public void remove()
        {
            delete = true;
        }
    }
}
