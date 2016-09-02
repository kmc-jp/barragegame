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
        public double x;
        public double y;
        public double speed;
        public double radius;
        public int life;
        public int sword;
        public Texture2D texture;
        public SpriteBatch spriteBatch;


        public Player(double _x,double _y,double _speed,double _radius,int _life,int _sword, Texture2D _texture,SpriteBatch _spriteBatch)
        {
            x = _x;
            y = _y;
            speed = _speed;
            radius = _radius;
            life = _life;
            sword = _sword;
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
            if (keymanager.IsKeyDown(KeyID.Slow) == true) { speed = 2; }else { speed = 6; }//テスト用数値

            if (x < 280) { x = 280; }
            if (x > 1000) { x = 1000; }
            if (y > 720) { y = 720; }
            if (y < 0) { y = 0; }
        }

       
        public Bullet shot(Texture2D _texture)
        {
            return new Bullet(x, y, 0, 10, 10,1,0, _texture, spriteBatch);

        }

        public void draw(Texture2D texture)
        {
            spriteBatch.Draw(texture, new Vector2((float)(x - texture.Width / 2), (float)(y - texture.Height / 2)));//良くなさそう
        }
    }
}
