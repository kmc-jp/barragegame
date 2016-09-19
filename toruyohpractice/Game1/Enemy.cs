using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CommonPart;

namespace CommonPart
{
    class Enemy
    {
        public double x;
        public double y;
        public double speed_x;
        public double speed_y;
        public double radius;
        public int life;
        public int score;
        public string texture_name;
        public List<Bullet> bullets = new List<Bullet>();
        public bool delete = false; 


        
        public Enemy(double _x,double _y,double _speed_x,double _speed_y,double _radius, int _life,int _score,string _texture_name="36 40-enemy1.png")
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
            y = y + speed_y;

            if (x < Map.leftside - DataBase.getTex(texture_name).Width / 2|| x > Map.rightside + DataBase.getTex(texture_name).Width / 2||y > DataBase.WindowSlimSizeY + DataBase.getTex(texture_name).Height / 2||y < 0 - DataBase.getTex(texture_name).Height / 2)
            {
                remove(Unit_state.out_of_window);
            }

            if (Function.hitcircle(x, y, radius, player.x, player.y, player.radius))
            {
                player.damage(1);
            }
            
            //bulletのupdate
            for (int i = 0; i < bullets.Count; i++)
            {
                bullets[i].update(player);
                if (x < Map.leftside - DataBase.getTex(texture_name).Width / 2 || x > Map.rightside + DataBase.getTex(texture_name).Width / 2 || y > DataBase.WindowSlimSizeY + DataBase.getTex(texture_name).Height / 2 || y < 0 - DataBase.getTex(texture_name).Height / 2)
                {
                    bullets[i].remove();
                }
                if (bullets[i].delete == true) { bullets.Remove(bullets[i]); }
            }
        }

        public void draw(Drawing d)
        {
            d.Draw(new Vector((x- DataBase.getTex(texture_name).Width/2),(y- DataBase.getTex(texture_name).Height/2)), DataBase.getTex(texture_name),
                DepthID.Enemy);
            for (int i = 0; i < bullets.Count; i++)
            {
                bullets[i].draw(d);
            }
        }

        public void shot1(Player player)//自機狙い
        {
            double e = Math.Sqrt(Function.distance(player.x,player.y,x,y));
            double speed = 6;
            double v = speed / e;
            bullets.Add(new Bullet(x, y,(player.x-x)*v,-(player.y-y)*v,10,1,1));
        }

        public void damage(int atk)
        {
            life -= atk;
            if (life <= 0)
            {
                remove(Unit_state.dead);
            }
        }

        public void remove(Unit_state unitstate)
        {
            switch (unitstate)
            {
                case Unit_state.dead:
                    delete = true;
                    break;
                case Unit_state.out_of_window:
                    delete = true;
                    break;
                case Unit_state.fadeout:
                    //
                    break;
                default:
                    break;
            }
        }
    }
}
