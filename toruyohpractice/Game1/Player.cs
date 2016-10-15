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
        public int sword = 1;
        public string texture_name;
        public bool attack_mode = false;
        public bool add_attack_mode = false;
        public int shouhi_sword = 0;
        public Enemy closest_enemy;

        public int atk = 100;
        public double enemy_below = 50;
        public int sword_max = 100;
        public int attack_time = 60;
        


        public Player(double _x,double _y,double _speed,double _radius,int _life,string t_n= "36 40-hex1.png")
        {
            x = _x;
            y = _y;
            speed = _speed;
            radius = _radius;
            life = _life;
            texture_name = t_n;
        }

        public void update(InputManager keymanager,Map map)
        {
            keymanager.Update();
            if (attack_mode == false)
            {
                if (keymanager.IsKeyDown(KeyID.Up) == true) { y = y - speed; }
                if (keymanager.IsKeyDown(KeyID.Down) == true) { y = y + speed; }
                if (keymanager.IsKeyDown(KeyID.Right) == true) { x = x + speed; }
                if (keymanager.IsKeyDown(KeyID.Left) == true) { x = x - speed; }
                if (keymanager.IsKeyDown(KeyID.Slow) == true) { speed = 2; } else { speed = 6; }//テスト用数値
            }

            skill(keymanager, map);

            if (x < Map.leftside+DataBase.getTex(texture_name).Width/2) { x = Map.leftside+DataBase.getTex(texture_name).Width / 2; }
            if (x > Map.rightside-DataBase.getTex(texture_name).Width / 2) { x = Map.rightside- DataBase.getTex(texture_name).Width / 2; }
            if (y > DataBase.WindowSlimSizeY- DataBase.getTex(texture_name).Height / 2) { y = DataBase.WindowSlimSizeY- DataBase.getTex(texture_name).Height / 2; }
            if (y < 0+ DataBase.getTex(texture_name).Height / 2) { y = 0+ DataBase.getTex(texture_name).Height / 2; }
            if (sword >= sword_max) { sword = sword_max; }
        }

        public void search_enemy(Map map)
        {
            if (map.enemys_inside_window.Count > 0)
            {
                Console.Write(map.enemys_inside_window.Count);
                closest_enemy = map.enemys_inside_window[0];
                for (int i = 0; i < map.enemys_inside_window.Count; i++)
                {
                    if (Function.distance(x, y, map.enemys_inside_window[i].x, map.enemys_inside_window[i].y) < Function.distance(x, y, closest_enemy.x, closest_enemy.y))
                    {
                        closest_enemy = map.enemys_inside_window[i];
                    }
                }
            }
            
        }
        double minForText = 99999;
        public void skill(InputManager input,Map map)
        {
            if (attack_mode == false)
            {
                if (input.IsKeyDown(KeyID.Select) == true && sword > 0)
                {
                    Console.Write("a");
                    attack_mode = true;
                    attack_time = 120;
                    sword -= shouhi_sword;
                    search_enemy(map);
                }
            }

            if (attack_mode == true)
            {
                if (closest_enemy != null)
                {
                    double e = Math.Sqrt(Function.distance(x, y, closest_enemy.x, closest_enemy.y + enemy_below));
                    double skill_speed = 15;
                    double v = skill_speed / e;
                    x -= (x - closest_enemy.x) * v;
                    y -= (y - closest_enemy.y-enemy_below) * v;
                    if (Function.hitcircle(x, y, 2, closest_enemy.x, closest_enemy.y + enemy_below, 6))
                    {
                        Console.Write("ddd");
                        closest_enemy.damage(atk);
                        closest_enemy = null;
                        sword -= shouhi_sword;
                    }
                }

                if (closest_enemy == null)
                {
                    attack_time--;
                    if (input.IsKeyDown(KeyID.Select) == true)
                    {
                        Console.Write("ad");
                        attack_time = 120;
                        search_enemy(map);
                        add_attack_mode = true;
                    }
                }
                if (attack_time <= 0||map.enemys_inside_window.Count<=0)
                {
                    
                    attack_mode = false;
                    add_attack_mode = false;
                }
            }
    }

        public void damage(int atk)
        {
            life -= atk;
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
