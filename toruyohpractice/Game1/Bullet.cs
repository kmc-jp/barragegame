﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Game1
{
    class Bullet
    {
        public int x;
        public int y;
        public int speed_x;
        public int speed_y;
        public int life;
        public int radius;
        public Texture2D texture;
        public SpriteBatch spriteBatch;

        public Bullet(int _x,int _y,int _speed_x,int _speed_y,int _life,int _radius,Texture2D _texture,SpriteBatch _spriteBatch)
        {
            x = _x;
            y = _y;
            speed_x = _speed_x;
            speed_y = _speed_y;
            life = _life;
            radius = _radius;
            texture = _texture;
            spriteBatch = _spriteBatch;
        }

        public void move(int _bulletexist)
        {
            if (_bulletexist > 0)
            {
                x = x + speed_x;
                y = y - speed_y;
            }
            
        }

        public void draw()
        {
            spriteBatch.Draw(texture, new Vector2(x, y)); 
        }

        public void remove()
        {
            
        }
    }
}