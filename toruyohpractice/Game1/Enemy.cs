using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Game1
{
    class Enemy
    {
        int x;
        int y;
        int speed_x;
        int speed_y;
        int life;
        Texture2D texture;
        SpriteBatch spriteBatch;
        
        public Enemy(int _x,int _y,int _speed_x,int _speed_y,int _life,Texture2D _texture,SpriteBatch _spriteBatch)
        {
            x = _x;
            y = _y;
            speed_x = _speed_x;
            speed_y = _speed_y;
            life = _life;
            texture = _texture;
            spriteBatch = _spriteBatch;
        }

        public void move()
        {
            x = x + speed_x;
            y = y + speed_y;
        }

        public void draw()
        {
            spriteBatch.Draw(texture, new Vector2(x, y));
        }
    }
}
