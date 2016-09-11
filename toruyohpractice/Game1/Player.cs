using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonPart;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CommonPart
{
    class Player
    {
        public double x;
        public double y;
        public double speed;
        public double radius;
        public int life;
        public int life_piece;
        public int sword;
        public string texture_name;


        public Player(double _x,double _y,double _speed,double _radius,int _life,int _life_piece,int _sword,string t_n= "36 40-hex1.png")
        {
            x = _x;
            y = _y;
            speed = _speed;
            radius = _radius;
            life = _life;
            life_piece = _life_piece;
            sword = _sword;
            texture_name = t_n;
        }

        public void update(InputManager keymanager)
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

       
        public Bullet shot(string texture_name="18 20-tama1.png")
        {
            return new Bullet(x, y, 0, 10, 10,1,0, texture_name);

        }

        public void draw(Drawing d)
        {
            d.Draw(new Vector(x - DataBase.getTex(texture_name).Width / 2,y - DataBase.getTex(texture_name).Height / 2) , DataBase.getTex(texture_name),
                DepthID.Player);
        }
    }
}
