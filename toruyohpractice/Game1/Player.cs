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
        public int life_piece = 0;
        public string texture_name;
        public bool attack_mode = false;
        public bool add_attack_mode = false;
        public Enemy closest_enemy;
        public bool attacked = false;
        public bool avoid_mode = false;
        public int avoid_time;
        public int stop_time;
        public int sword = 100;
        public bool acceleration_mode = true;
        public bool dead_mode = false;
        public int dead_time;
        public int muteki_time=0;
        /// <summary>
        /// skillの1度目の入力を無視
        /// </summary>
        bool first = false;

        public double speed_x;
        public double speed_y;

        public int default_x;
        public int default_y;
        public int skill_stop = 6;
        public int skill_speed = 15;
        public int shouhi_sword = 10;
        public int default_speed=6;
        public int atk = 100;
        public double enemy_below = 50;
        public int sword_max = 100;
        public int attack_time = 2;
        public int damage_percent = 2;
        public int bonus_damage = 1000;
        public int default_avoid_time = 6;
        public int avoid_speed=13;
        public int acceleration=3;
        public int avoid_stop=20;
        /// <summary>
        /// 回避時に敵弾を消せる半円の半径
        /// </summary>
        public int avoid_radius=100;
        public int default_dead_time = 60;
        public int default_muteki_time = 30;
        

        public Player(double _x,double _y,double _speed,double _radius,int _life,string t_n= "36 40-hex1")
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
            if (dead_mode == false)
            {
                if (muteki_time > 0)
                {
                    muteki_time--;
                }

                if (stop_time <= 0)
                {
                    if (attack_mode == false)
                    {
                        if (keymanager.IsKeyDown(KeyID.Up) == true) { y = y - speed; }
                        if (keymanager.IsKeyDown(KeyID.Down) == true) { y = y + speed; }
                        if (keymanager.IsKeyDown(KeyID.Right) == true) { x = x + speed; }
                        if (keymanager.IsKeyDown(KeyID.Left) == true) { x = x - speed; }
                        if (avoid_mode == false)
                        {
                            if (keymanager.IsKeyDown(KeyID.Slow) == true) { speed = 2; } else { speed = default_speed; }//テスト用数値
                        }
                        avoid(keymanager, map);
                    }

                    if (avoid_mode == false)
                    {
                        skill(keymanager, map);
                    }
                }
                else
                {
                    stop_time--;
                }
            }else
            {
                /*dead_time--;
                if (dead_time <= 0)
                {
                    x = DataBase.WindowDefaultSizeX / 2;
                    y = 500;
                    dead_mode = false;
                    dead_time = default_dead_time;
                    muteki_time = default_muteki_time;
                }*/
                double e = Math.Sqrt(Function.distance(x, y, DataBase.WindowDefaultSizeX / 2, 500));
                if (e != 0)
                {
                    speed_x = (x - DataBase.WindowDefaultSizeX / 2) * speed / e;
                    speed_y = (y - 500) * speed / e;
                    x -= speed_x;
                    y -= speed_y;
                }
                else
                {
                    dead_mode = false;
                    muteki_time = default_muteki_time;
                }
                if (Function.hitcircle(x, y, 0, DataBase.WindowDefaultSizeX / 2, 500, speed/2))
                {
                    dead_mode = false;
                    muteki_time = default_muteki_time;
                }
            }

            if (x < Map.leftside+DataBase.getTex(texture_name).Width/2) { x = Map.leftside+DataBase.getTex(texture_name).Width / 2; }
            if (x > Map.rightside-DataBase.getTex(texture_name).Width / 2) { x = Map.rightside- DataBase.getTex(texture_name).Width / 2; }
            if (y > DataBase.WindowSlimSizeY- DataBase.getTex(texture_name).Height / 2) { y = DataBase.WindowSlimSizeY- DataBase.getTex(texture_name).Height / 2; }
            if (y < 0+ DataBase.getTex(texture_name).Height / 2) { y = 0+ DataBase.getTex(texture_name).Height / 2; }
            if (sword >= sword_max) { sword = sword_max; }
            if (sword <= 0) { sword = 0; }
        }

        public void search_enemy(Map map)
        {
            closest_enemy = null;
            if (map.enemys_inside_window.Count > 0)
            {
                int j = 0;
                for (int i = 0; i < map.enemys_inside_window.Count; i++)
                {
                    if (map.enemys_inside_window[i].selectable() == true)
                    {
                        closest_enemy = map.enemys_inside_window[i];
                        j = i;
                        break;
                    }
                }

                for (int i = j; i < map.enemys_inside_window.Count; i++)
                {
                    if (map.enemys_inside_window[i].selectable()==true
                        && Function.distance(x, y, map.enemys_inside_window[i].x, map.enemys_inside_window[i].y) < Function.distance(x, y, closest_enemy.x, closest_enemy.y))
                    {
                        closest_enemy = map.enemys_inside_window[i];
                    }
                }
            }
        }

        public void cast_skill(Map map)
        {
            search_enemy(map);
            attack_mode = true;
            attack_time = 120;
            //sword -= shouhi_sword;
            add_attack_mode = false;
            first = true;
        }

        public void skill(InputManager input,Map map)
        {
            if (attack_mode == false)
            {
                if (input.IsKeyDownOld(KeyID.Select) == false && input.IsKeyDown(KeyID.Select) == true && sword >= 20) 
                {
                    cast_skill(map);
                }
            }

            if (attack_mode == true)
            {
                if (first == false)
                {
                    if (closest_enemy != null && closest_enemy.selectable() == false)
                    {
                        search_enemy(map);
                    }
                    else
                    if (closest_enemy != null)
                    {
                        double e = Math.Sqrt(Function.distance(x, y, closest_enemy.x, closest_enemy.y + enemy_below));
                        double v = skill_speed / e;
                        x -= (x - closest_enemy.x) * v;
                        y -= (y - closest_enemy.y - enemy_below) * v;

                        if (input.GetKeyPressed(KeyID.Select) == true)
                        {
                            add_attack_mode = true;
                        }

                        if (Function.hitcircle(x, y, 2, closest_enemy.x, closest_enemy.y + enemy_below, 6))
                        {
                            closest_enemy.damage(atk);
                            closest_enemy = null;
                            sword -= shouhi_sword;
                            stop_time = skill_stop;
                        }
                    }
                    else
                    {
                        attack_time--;
                        if ((input.GetKeyPressed(KeyID.Select) == true || add_attack_mode == true) && sword >= 10)
                        {
                            cast_skill(map);
                            add_attack_mode = false;
                        }
                        if (input.IsKeyDown(KeyID.Up) == true || input.IsKeyDown(KeyID.Down) == true
                            || input.IsKeyDown(KeyID.Right) == true || input.IsKeyDown(KeyID.Left) == true)
                        {
                            attack_time = 0;
                        }
                    }

                    if (attack_time <= 0 || map.enemys_inside_window.Count <= 0)
                    {
                        attack_mode = false;
                        add_attack_mode = false;
                    }
                }else
                {
                    first = false;
                }
            }
    }

        public void cast_skill2(Map map)
        {
            search_enemy(map);
            attack_mode = true;
            attack_time = 120;
            shouhi_sword = sword;
        }

        public void skill_2(InputManager input,Map map)
        {
            if (attack_mode == false)
            {
                if (input.IsKeyDown(KeyID.Select) == true && sword >= 0)
                {
                    cast_skill2(map);
                }
                
            }

            if (attack_mode == true)
            {
                if (closest_enemy != null)
                {
                    if (closest_enemy.selectable() == false)
                    {
                        search_enemy(map);
                    }
                    else
                    {
                        double e = Math.Sqrt(Function.distance(x, y, closest_enemy.x, closest_enemy.y + enemy_below));
                        double skill_speed = 15;
                        double v = skill_speed / e;
                        x -= (x - closest_enemy.x) * v;
                        y -= (y - closest_enemy.y - enemy_below) * v;

                        if (Function.hitcircle(x, y, 2, closest_enemy.x, closest_enemy.y + enemy_below, 6))
                        {
                            sword -= shouhi_sword;
                            shouhi_sword = 100000;//テスト用
                            closest_enemy.damage(shouhi_sword * damage_percent);
                            if (shouhi_sword == sword_max)
                            {
                                closest_enemy.damage(bonus_damage);
                            }
                            closest_enemy = null;
                        }
                    }
                }

                if (closest_enemy==null)
                {
                    double e = Math.Sqrt(Function.distance(x, y, DataBase.WindowDefaultSizeX / 2, 500));
                    double skill_speed = 15;
                    double v = skill_speed / e;
                    x -= (x - DataBase.WindowDefaultSizeX/2) * v;
                    y -= (y - 500) * v;

                    if (Function.hitcircle(x, y, 0, DataBase.WindowDefaultSizeX/2,500, 8))
                    {
                        attack_mode = false;
                    }
                }
            }
        }

        public void avoid(InputManager input,Map map)
        {
            if (avoid_mode == false && input.GetKeyPressed(KeyID.Cancel) == true && (input.IsKeyDown(KeyID.Up) == true || input.IsKeyDown(KeyID.Down) == true
                || input.IsKeyDown(KeyID.Right) == true || input.IsKeyDown(KeyID.Left) == true)) 
            {
                #region　上下左右の回避
                if(input.IsKeyDown(KeyID.Up) == true)
                {
                    for (int i = 0; i < map.enemys_inside_window.Count; i++)
                    {
                        for (int j = 0; j < map.enemys_inside_window[i].bullets.Count; j++)
                        {
                            if (Function.hitcircle(x, y, avoid_radius, map.enemys_inside_window[i].bullets[j].x, map.enemys_inside_window[i].bullets[j].y, map.enemys_inside_window[i].bullets[j].radius)
                            && map.enemys_inside_window[i].bullets[j].y <= y)
                            {
                                map.pros.Add(new Projection(map.enemys_inside_window[i].bullets[j].x, map.enemys_inside_window[i].bullets[j].y,
                                    MoveType.object_target, map.pro_speed,map.pro_acceleration, new Animation(new SingleTextureAnimationData(10, TextureID.Score, 3, 1)), this, 100));
                                map.pro_swords.Add(map.enemys_inside_window[i].bullets[j].sword);

                                map.score += map.enemys_inside_window[i].bullets[j].score;
                                map.enemys_inside_window[i].bullets[j].remove();
                            }
                        }

                        if (Function.hitcircle(x, y, avoid_radius, map.enemys_inside_window[i].x, map.enemys_inside_window[i].y, map.enemys_inside_window[i].radius)
                            && map.enemys_inside_window[i].y <= y)
                        {
                            sword += map.enemys_inside_window[i].unitType.sword;
                            map.enemys_inside_window[i].remove(Unit_state.dead);
                        }
                    }    
                }
                else if (input.IsKeyDown(KeyID.Down) == true)
                {
                    for (int i = 0; i < map.enemys_inside_window.Count; i++)
                    {
                        for (int j = 0; j < map.enemys_inside_window[i].bullets.Count; j++)
                        {
                            if (Function.hitcircle(x, y, avoid_radius, map.enemys_inside_window[i].bullets[j].x, map.enemys_inside_window[i].bullets[j].y, map.enemys_inside_window[i].bullets[j].radius)
                            && map.enemys_inside_window[i].bullets[j].y >= y)
                            {

                                map.pros.Add(new Projection(map.enemys_inside_window[i].bullets[j].x, map.enemys_inside_window[i].bullets[j].y,
                                    MoveType.object_target, map.pro_speed,map.pro_acceleration ,new Animation(new SingleTextureAnimationData(10, TextureID.Score, 3, 1)), this, 100));
                                map.pro_swords.Add(map.enemys_inside_window[i].bullets[j].sword);
                                map.score += map.enemys_inside_window[i].bullets[j].score;
                                map.enemys_inside_window[i].bullets[j].remove();
                            }
                        }

                        if (Function.hitcircle(x, y, avoid_radius, map.enemys_inside_window[i].x, map.enemys_inside_window[i].y, map.enemys_inside_window[i].radius)
                            && map.enemys_inside_window[i].y >= y)
                        {
                            sword += map.enemys_inside_window[i].unitType.sword;
                            map.enemys_inside_window[i].remove(Unit_state.dead);
                        }
                    }
                }
                else if (input.IsKeyDown(KeyID.Right) == true)
                {
                    for (int i = 0; i < map.enemys_inside_window.Count; i++)
                    {
                        for (int j = 0; j < map.enemys_inside_window[i].bullets.Count; j++)
                        {
                            if (Function.hitcircle(x, y, avoid_radius, map.enemys_inside_window[i].bullets[j].x, map.enemys_inside_window[i].bullets[j].y, map.enemys_inside_window[i].bullets[j].radius)
                            && map.enemys_inside_window[i].bullets[j].x >= x)
                            {
                                map.pros.Add(new Projection(map.enemys_inside_window[i].bullets[j].x, map.enemys_inside_window[i].bullets[j].y,
                                    MoveType.object_target, map.pro_speed,map.pro_acceleration, new Animation(new SingleTextureAnimationData(10, TextureID.Score, 3, 1)), this, 100));
                                map.pro_swords.Add(map.enemys_inside_window[i].bullets[j].sword);
                                map.score += map.enemys_inside_window[i].bullets[j].score;
                                map.enemys_inside_window[i].bullets[j].remove();
                            }
                        }

                        if (Function.hitcircle(x, y, avoid_radius, map.enemys_inside_window[i].x, map.enemys_inside_window[i].y, map.enemys_inside_window[i].radius)
                            && map.enemys_inside_window[i].x >= x)
                        {
                            sword += map.enemys_inside_window[i].unitType.sword;
                            map.enemys_inside_window[i].remove(Unit_state.dead);
                        }
                    }
                }
                else if (input.IsKeyDown(KeyID.Left) == true)
                {
                    for (int i = 0; i < map.enemys_inside_window.Count; i++)
                    {
                        for (int j = 0; j < map.enemys_inside_window[i].bullets.Count; j++)
                        {
                            if (Function.hitcircle(x, y, avoid_radius, map.enemys_inside_window[i].bullets[j].x, map.enemys_inside_window[i].bullets[j].y, map.enemys_inside_window[i].bullets[j].radius)
                            && map.enemys_inside_window[i].bullets[j].x <= x)
                            {
                                map.pros.Add(new Projection(map.enemys_inside_window[i].bullets[j].x, map.enemys_inside_window[i].bullets[j].y,
                                     MoveType.object_target, map.pro_speed,map.pro_acceleration, new Animation(new SingleTextureAnimationData(10, TextureID.Score, 3, 1)), this, 100));
                                map.pro_swords.Add(map.enemys_inside_window[i].bullets[j].sword);
                                map.score += map.enemys_inside_window[i].bullets[j].score;
                                map.enemys_inside_window[i].bullets[j].remove();
                            }
                        }

                        if (Function.hitcircle(x, y, avoid_radius, map.enemys_inside_window[i].x, map.enemys_inside_window[i].y, map.enemys_inside_window[i].radius)
                            && map.enemys_inside_window[i].x <= x)
                        {
                            sword += map.enemys_inside_window[i].unitType.sword;
                            map.enemys_inside_window[i].remove(Unit_state.dead);
                        }
                    }
                }
                #endregion
                avoid_mode = true;
                acceleration_mode = true;
                speed = 0;
            }

            if (avoid_mode == true)
            {
                if (acceleration_mode==true)
                {
                    speed +=acceleration ;
                    if (speed >= avoid_speed)
                    {
                        acceleration_mode = false;
                    }
                }
                else
                {
                    if (speed > default_speed)
                    {
                        speed -= (acceleration/2+1);
                    }
                    if (speed <= default_speed)
                    {
                        speed = default_speed;
                        acceleration_mode = true;
                        avoid_mode = false;
                        stop_time = avoid_stop;
                    }
                }
            }
        }

        public void damage(int atk)
        {
            if (attack_mode == false && avoid_mode == false && dead_mode == false && muteki_time <= 0) 
            {
                life -= atk;
                dead_mode = true;
                dead_time=default_dead_time;
            }
        }

        public void draw(Drawing d)
        {
            d.Draw(new Vector(x - DataBase.getTex(texture_name).Width / 2,y - DataBase.getTex(texture_name).Height / 2) , DataBase.getTex(texture_name),
                DepthID.Player);
        }
    }
}
