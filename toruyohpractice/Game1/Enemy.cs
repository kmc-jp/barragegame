using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CommonPart;

namespace CommonPart
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
        public int sword;
        public string texture_name;
        public List<Bullet> bullets = new List<Bullet>();
        public int total_score = 0;
        public List<Skill> skills=new List<Skill>();

        public double bullet_speed = 3;

        public bool delete = false;
        public bool exist = false;
        public bool fadeout = false;

        
        public Enemy(double _x,double _y,double _speed_x,double _speed_y,double _radius, int _life,int _score,int _sword,string _texture_name="36 40-enemy1.png")
        {
            x = _x;
            y = _y;
            speed_x = _speed_x;
            speed_y = _speed_y;
            radius = _radius;
            life = _life;
            score = _score;
            sword = _sword;
            texture_name = _texture_name;

            skills.Add(new Skill("circle"));
        }

        public void update(Player player)
        {
            x = x + speed_x;
            y = y + speed_y;

            if (x < Map.leftside - DataBase.getTex(texture_name).Width / 2|| x > Map.rightside + DataBase.getTex(texture_name).Width / 2||y > DataBase.WindowSlimSizeY + DataBase.getTex(texture_name).Height / 2||y < 0 - DataBase.getTex(texture_name).Height / 2)
            {
                remove(Unit_state.out_of_window);
            }

            if (Function.hitcircle(x, y, radius, player.x, player.y, player.radius))
            {
                player.damage(1);
            }

            //bulletのupdate
            for (int i = 0; i < bullets.Count; i++)
            {
                bullets[i].update(player);
                Console.Write(bullets[i].speed_x);
                if (x < Map.leftside - DataBase.getTex(texture_name).Width / 2 || x > Map.rightside + DataBase.getTex(texture_name).Width / 2 || y > DataBase.WindowSlimSizeY + DataBase.getTex(texture_name).Height / 2 || y < 0 - DataBase.getTex(texture_name).Height / 2)
                {
                    bullets[i].remove();
                }
                if (bullets[i].delete == true) { bullets.Remove(bullets[i]); }
            }

            for (int i = 0; i < skills.Count; i++)
            {
                skills[i].update();
            }
        }

        public void draw(Drawing d)
        {
            d.Draw(new Vector((x- DataBase.getTex(texture_name).Width/2),(y- DataBase.getTex(texture_name).Height/2)), DataBase.getTex(texture_name),
                DepthID.Enemy);
            for (int i = 0; i < bullets.Count; i++)
            {
                bullets[i].draw(d);
            }
        }

        public void shot1(Player player)//自機狙い
        {
            bullets.Add(new Bullet(x, y,MoveType.point_target,bullet_speed,0,null,new Vector(player.x,player.y),100,10,1,1,5));
        }

        public void shot(Player player)
        {
            for (int i = 0; i < skills.Count; i++)
            {
                if (skills[i].coolDown <= 0)
                {
                    SkillData sd=DataBase.SkillDatasDictionary[skills[i].skillName];
                    switch (sd.sgl) {
                        case SkillGenreL.generation:
                            switch (sd.sgs)
                            {
                                case SkillGenreS.shot:
                                    SingleShotSkillData ss = (SingleShotSkillData)sd;
                                    //引数は全てskilldataに持たせよう
                                    bullets.Add(new Bullet(x, y, ss.moveType, ss.speed, ss.acceleration, null, new Vector(player.x, player.y), 100, ss.radius, ss.life, ss.score, ss.sword));
                                    break;
                                case SkillGenreS.circle:
                                    SingleShotSkillData sss = (SingleShotSkillData)sd;
                                    for (int j = 0; j < 2*Math.PI/sss.angle; j++)
                                    {
                                        bullets.Add(new Bullet(x, y, sss.moveType, sss.speed, sss.acceleration,-(Math.PI/2)+j*sss.angle, null, 100, sss.radius, sss.life, sss.score, sss.sword));
                                    }
                                    break;
                                case SkillGenreS.laser:
                                    break;
                                default:
                                    break;
                            }
                            break;
                        default:
                            break;
                    }

                }
            }
        }

        public void damage(int atk)
        {
            life -= atk;
            if (life <= 0)
            {
                remove(Unit_state.dead);
            }
        }

        public void remove(Unit_state unitstate)
        {
            switch (unitstate)
            {
                case Unit_state.dead:
                    delete = true;
                    break;
                case Unit_state.out_of_window:
                    delete = true;
                    fadeout = true;
                    break;
                case Unit_state.fadeout:
                    //
                    break;
                default:
                    break;
            }
        }

        public bool selectable()
        {
            if (delete == true)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
