using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cellgame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Game1
{
    class Player
    {
        public int x;
        public int y;
        public int speed;
        public int radius;
        public Texture2D texture;
        public SpriteBatch spriteBatch;


        public Player(int _x,int _y,int _speed,int _radius,Texture2D _texture,SpriteBatch _spriteBatch)
        {
            x = _x;
            y = _y;
            speed = _speed;
            radius = _radius;
            texture = _texture;
            spriteBatch = _spriteBatch;
        }

        KeyManager keymanager = new KeyManager();
        public void move()
        {
            keymanager.Update();
            if (keymanager.IsKeyDown(KeyID.Up) == true) { y = y - speed; }
            if (keymanager.IsKeyDown(KeyID.Down) == true) { y = y + speed; }
            if (keymanager.IsKeyDown(KeyID.Right) == true) { x = x + speed; }
            if (keymanager.IsKeyDown(KeyID.Left) == true) { x = x - speed; }
            if (keymanager.IsKeyDown(KeyID.Slow) == true) { speed = 2; }else { speed = 5; }//テスト用数値

            if (x > 1280) { x = 1280; }
            if (x < 0) { x = 0; }
            if (y > 720) { y = 720; }
            if (y < 0) { y = 0; }

            if (bulletexist > 0)
            {
                for (int i = 0; i < bullets.Count; i++)
                {
                    bullets[i].move(bulletexist);
                }
            }//else { x = 100;y = 100; }
        }

        List<Bullet> bullets=new List<Bullet>();
        public int bulletexist;
        public void shot(Texture2D _texture)
        {
            keymanager.Update();
            if (keymanager.IsKeyDown(KeyID.Select) == true)
            {
                bullets.Add( new Bullet(x, y, 0, 1, 1,1, _texture, spriteBatch));
                bulletexist++;
            }

        }

        public void draw()
        {
            if(bulletexist > 0) { for (int i = 0; i < bullets.Count; i++) { bullets[i].draw();} }
            spriteBatch.Draw(texture, new Vector2(x, y));
        }
    }
}
