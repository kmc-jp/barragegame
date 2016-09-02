using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using cellgame;

namespace Game1
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
        public Texture2D texture;
        public SpriteBatch spriteBatch;
        public List<Bullet> bullets = new List<Bullet>();

        
        public Enemy(double _x,double _y,double _speed_x,double _speed_y,double _radius, int _life,int _score, Texture2D _texture,SpriteBatch _spriteBatch)
        {
            x = _x;
            y = _y;
            speed_x = _speed_x;
            speed_y = _speed_y;
            radius = _radius;
            life = _life;
            score = _score;
            texture = _texture;
            spriteBatch = _spriteBatch;
        }

        public void move()
        {
            x = x + speed_x;
            y = y + speed_y;
        }

        public void draw(Texture2D texture)
        {
            spriteBatch.Draw(texture, new Vector2((float)(x-texture.Width/2), (float)(y-texture.Height/2)));
        }

        public void shot1(Player player,Texture2D _texture)//自機狙い
        {
            double e = Math.Sqrt(Function.distance(player.x,player.y,x,y));
            double speed = 6;
            double v = speed / e;
            bullets.Add(new Bullet(x, y,(player.x-x)*v,-(player.y-y)*v,10,1,1,_texture,spriteBatch));
        }

        public void remove(Player _player, List<Enemy> enemys)
        {
            if (Function.hitcircle(x, y, radius, _player.x, _player.y, _player.radius) || x > 1280 || x < 0 || y > 720 || y < 0)
            {
                if (y < 720)
                {
                    /*
                    Console.Write(x);
                    Console.Write(",");
                    Console.Write(y);
                    Console.Write(",");
                    Console.Write(Function.hitcircle(x, y, radius, _player.x, _player.y, _player.radius));
                    Console.Write(",\n");
                    */
                }
                enemys.Remove(this);
            }
        }
        public void remove(List<Enemy> enemys)
        {
            enemys.Remove(this);
        }
    }
}
