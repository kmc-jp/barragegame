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
    class Enemy
    {
        int x;
        int y;
        int speed_x;
        int speed_y;
        int life;
        int radius;
        Texture2D texture;
        SpriteBatch spriteBatch;

        
        public Enemy(int _x,int _y,int _speed_x,int _speed_y,int _life,int _radius,Texture2D _texture,SpriteBatch _spriteBatch)
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

        public void move()
        {
            x = x + speed_x;
            y = y + speed_y;
        }

        public void draw()
        {
            spriteBatch.Draw(texture, new Vector2(x, y));
        }

        public void remove(Player _player,List<Enemy> enemys)
        {
            if (Math.Sqrt((x - _player.x) ^ 2 + (y - _player.y) ^ 2) < radius + _player.radius || x > 1280 || x < 0 || y > 720 || y < 0)
            {
                enemys.Remove(this);
            }
        }
    }
}
