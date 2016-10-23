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
        public double angle;
        public int life = 1;
        public List<Bullet> bullets = new List<Bullet>();
        public List<Skill> skills = new List<Skill>();
        public string unitType_name;
        public ActiveAniSkiedUnitType unitType { get { return (ActiveAniSkiedUnitType)DataBase.getUnitType(unitType_name); } }
        public double radius { get { return unitType.radius; } }
        public bool delete = false;
        public bool exist = false;
        public bool fadeout = false;
        public AnimationAdvanced animation;
        protected int[] motion_index=new int[2];
        /// <summary>
        /// 0が今のルーチン、1が保存したルーチン
        /// </summary>
        public int[] times=new int[2];

                
        public Enemy(double _x,double _y,string _unitType_name)
        {
            x = _x;
            y = _y;
            unitType_name = _unitType_name;
            playAnimation(DataBase.defaultAnimationNameAddOn);
            motion_index[0] = 0;
            times[0] =0;
            setup_skill();
        }
        
        protected void setup_skill()
        {
            for(int i = 0; i < unitType.skillNames.Count; i++)
            {
                skills.Add(new Skill(unitType.skillNames[i]));
            }
        }

        public void update(Player player)
        {
            animation.Update();
            if (times[0] >=unitType.times[motion_index[0]])
            {
                if (motion_index[0] < unitType.moveTypes.Count-1) 
                {
                    motion_index[0]++;
                    Console.WriteLine("play animation!");
                }else { motion_index[0] = 0; }
                times[0] = 0;
            }
            times[0]++;
            switch (unitType.moveTypes[motion_index[0]])
            {
                case MoveType.mugen:
                    Vector displacement = MotionCalculation.mugenDisplacement(3, unitType.times[motion_index[0]], times[0]);
                    x -= displacement.X;
                    y -= displacement.Y;
                    break;
                case MoveType.go_straight:
                    Vector displacement1 = MotionCalculation.tousokuidouDisplacement(unitType.default_poses[motion_index[0]],unitType.times[motion_index[0]]);
                    x += displacement1.X;
                    y += displacement1.Y;
                    break;
                case MoveType.rightcircle:
                    Vector displacement2 = MotionCalculation.rightcircleDisplacement(unitType.speed * unitType.times[motion_index[0]], unitType.times[motion_index[0]], times[0]);
                    x += displacement2.X;
                    y += displacement2.Y;
                    break;
                case MoveType.leftcircle:
                    Vector displacement3 = MotionCalculation.leftcircleDisplacement(unitType.speed * unitType.times[motion_index[0]], unitType.times[motion_index[0]], times[0]);
                    x += displacement3.X;
                    y += displacement3.Y;
                    break;
                case MoveType.stop:
                    break;
                default:
                    break;
            }


            if (x < Map.leftside - animation.X / 2|| x > Map.rightside + animation.X / 2||y > DataBase.WindowSlimSizeY + animation.Y / 2||y < 0 - animation.Y / 2)
            {
                remove(Unit_state.out_of_window);
            }

            if (Function.hitcircle(x, y,unitType.radius, player.x, player.y, player.radius))
            {
                player.damage(1);
            }

            //bulletのupdate
            for (int i = 0; i < bullets.Count; i++)
            {
                bullets[i].update(player);
                
                if (bullets[i].delete == true) { bullets.Remove(bullets[i]); }
            }

            for (int i = 0; i < skills.Count; i++)
            {
                skills[i].update();
            }
        }

        public void draw(Drawing d)
        {
            animation.Draw(d, new Vector((x - animation.X / 2), (y - animation.Y / 2)), DepthID.Enemy);
            //d.Draw(new Vector((x- animation.X/2),(y- animation.Y/2)), DataBase.getTex(texture_name),DepthID.Enemy);
            for (int i = 0; i < bullets.Count; i++)
            {
                bullets[i].draw(d);
            }
        }

        public void shot(Player player)
        {
            for (int i = 0; i < skills.Count; i++)
            {
                if (skills[i].coolDown <= 0)
                {
                    SkillData sd=DataBase.SkillDatasDictionary[skills[i].skillName];
                    bool use = true;
                    switch (sd.sgl) {
                        case SkillGenreL.generation:
                            #region ジャンルの小さい分類
                            switch (sd.sgs)
                            {
                                case SkillGenreS.shot:
                                    SingleShotSkillData ss = (SingleShotSkillData)sd;
                                    bullets.Add(new Bullet(x, y, ss.moveType, ss.speed, ss.acceleration, null, new Vector(player.x, player.y), 100, ss.radius, ss.life, ss.score, ss.sword));
                                    break;
                                case SkillGenreS.circle:
                                    SingleShotSkillData ss1 = (SingleShotSkillData)sd;
                                    for (int j = 0; j < 2*Math.PI/ss1.angle; j++)
                                    {
                                        bullets.Add(new Bullet(x, y, ss1.moveType, ss1.speed, ss1.acceleration,-(Math.PI/2)+j*ss1.angle, null, 100, ss1.radius, ss1.life, ss1.score, ss1.sword));
                                    }
                                    break;
                                case SkillGenreS.laser:
                                    LaserTopData lt = (LaserTopData)sd;
                                    bullets.Add(new LaserTop(x, y, lt.moveType, lt.speed, lt.acceleration, 0,null, 100, lt.radius, lt.life, lt.score, lt.sword,lt.angle,lt.omega,this,lt.color));
                                    break;
                                case SkillGenreS.wayshot:
                                    WayShotSkillData ws = (WayShotSkillData)sd;
                                    double player_angle = Math.Atan2(player.y - y, player.x - x);
                                    if (ws.way % 2 == 1)
                                    {
                                        for (int j = 0; j < (ws.way + 1) / 2; j++) 
                                        {
                                            bullets.Add(new Bullet(x, y, ws.moveType, ws.speed, ws.acceleration, player_angle + j * ws.angle, null, 100, ws.radius, ws.life, ws.score, ws.sword));
                                            bullets.Add(new Bullet(x, y, ws.moveType, ws.speed, ws.acceleration, player_angle - j * ws.angle, null, 100, ws.radius, ws.life, ws.score, ws.sword));
                                        }
                                    }else
                                    {
                                        bullets.Add(new Bullet(x, y, ws.moveType, ws.speed, ws.acceleration, player_angle + ws.angle / 2, null, 100, ws.radius, ws.life, ws.score, ws.sword));
                                        bullets.Add(new Bullet(x, y, ws.moveType, ws.speed, ws.acceleration, player_angle - ws.angle / 2, null, 100, ws.radius, ws.life, ws.score, ws.sword));
                                        for (int j = 2; j < ws.way / 2; j++)
                                        {
                                            bullets.Add(new Bullet(x, y, ws.moveType, ws.speed, ws.acceleration, player_angle + j * ws.angle, null, 100, ws.radius, ws.life, ws.score, ws.sword));
                                            bullets.Add(new Bullet(x, y, ws.moveType, ws.speed, ws.acceleration, player_angle - j * ws.angle, null, 100, ws.radius, ws.life, ws.score, ws.sword));
                                        }
                                    }
                                    break;
                                case SkillGenreS.zyuuzi:
                                    SingleShotSkillData ss2 = (SingleShotSkillData)sd;
                                    for (int j = 0; j < 3; j++)
                                    {
                                        bullets.Add(new Bullet(x, y, ss2.moveType, ss2.speed, ss2.acceleration, j*Math.PI/2, null, 100, ss2.radius, ss2.life, ss2.score, ss2.sword));
                                    }
                                    break;
                                default:
                                    use = false;
                                    break;
                            }//switch sgs end
                            #endregion
                            break;
                        default:
                            use = false;
                            break;
                    }//switch sgl end
                    if (use == true)
                    {
                        skills[i].used();
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

        public void clear()
        {
            bullets.Clear();
            skills.Clear();
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

        public void playAnimation(string addOn)
        {
            animation = new AnimationAdvanced(DataBase.getAniD(unitType.animation_name, addOn));
        }

        public int score(Map map)
        {
            int total_score = 0;
            total_score += unitType.score;
            for (int j = 0; j < bullets.Count; j++)
            {
                total_score += bullets[j].score;

                map.pros.Add(new Projection(bullets[j].x, bullets[j].y,
                    MoveType.object_target, map.pro_speed, map.pro_acceleration, new Animation(new SingleTextureAnimationData(10, TextureID.Score, 3, 1)), map.player, 100));
                map.pro_swords.Add(bullets[j].sword);
            }

            return total_score;
        }
    }
}
