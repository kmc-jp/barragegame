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
        public double x;
        public double y;
        public double speed_x;
        public double speed_y;
        public double radius;
        public int life;
        public int score;
        public int sword;
        public Texture2D texture;
        public SpriteBatch spriteBatch;

        public Bullet(double _x,double _y,double _speed_x,double _speed_y,double _radius, int _life,int _score, Texture2D _texture,SpriteBatch _spriteBatch)
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
            y = y - speed_y;    
        }

        public void draw(Texture2D texture)
        {
            spriteBatch.Draw(texture, new Vector2((float)(x - texture.Width / 2), (float)(y - texture.Height / 2)));
        }

        public void remove(List<Bullet> bullets)
        {
            bullets.Remove(this);
        }
    }
}
