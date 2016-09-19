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
    class Bullet
    {
        public double x;
        public double y;
        public double speed_x;
        public double speed_y;
        public double radius;
        public int life;
        public int score;
        public int sword;
        public string texture_name;

        public bool delete = false;
        public int atk = 1;

        public Bullet(double _x,double _y,double _speed_x,double _speed_y,double _radius, int _life,int _score, string _texture_name="18 20-tama1.png")
        {
            x = _x;
            y = _y;
            speed_x = _speed_x;
            speed_y = _speed_y;
            radius = _radius;
            life = _life;
            score = _score;
            texture_name = _texture_name;
        }

        public void update(Player player)
        {
            x = x + speed_x;
            y = y - speed_y;

            if (x < Map.leftside - DataBase.getTex(texture_name).Width / 2 || x > Map.rightside + DataBase.getTex(texture_name).Width / 2 || y > DataBase.WindowSlimSizeY + DataBase.getTex(texture_name).Height / 2 || y < 0 - DataBase.getTex(texture_name).Height / 2)
            {
                remove();
            }

            if (Function.hitcircle(x, y, radius, player.x, player.y, player.radius) == true)
            {
                life--;
                player.damage(atk);
            }
            if (life <= 0)
            {
                remove();
            }
        }


        public void draw(Drawing d)
        {
            d.Draw(new Vector((x - DataBase.getTex(texture_name).Width / 2),(y - DataBase.getTex(texture_name).Height / 2)),DataBase.getTex(texture_name),
                DepthID.Effect);
        }

        public void remove(List<Bullet> bullets)
        {
            bullets.Remove(this);
        }
        public void remove()
        {
            delete = true;
        }
    }
}
