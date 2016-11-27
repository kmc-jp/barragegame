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
        public double angle=0;
        protected bool texRotate { get { if (unitType == null) { return false; } else { return unitType.textureTurn; } } }
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

        private bool once = false;
        private Vector displacement4; 
                
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

        public virtual void update(Player player)
        {
            animation.Update();
            if (times[0] >=unitType.times[motion_index[0]])
            {
                if (motion_index[0] < unitType.moveTypes.Count-1) 
                {
                    motion_index[0]++;
                    Console.WriteLine("play animation!");
                    once = false;
                }else { motion_index[0] = 0; }
                times[0] = 0;
            }
            times[0]++;
            switch (unitType.moveTypes[motion_index[0]])
            {
                #region 動作
                case MoveType.mugen:
                    Vector displacement = MotionCalculation.mugenDisplacement(3, unitType.times[motion_index[0]], times[0]);
                    x -= displacement.X;
                    y -= displacement.Y;
                    if (texRotate) { angle = Math.Atan2(-displacement.Y, -displacement.X); } //- Math.PI / 2; }
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
                case MoveType.screen_point_target:
                    if (once == false)
                    {
                        Vector goal = unitType.default_poses[motion_index[0]];
                        displacement4 = new Vector(goal.X - x, goal.Y - y);
                        once = true;
                    }
                    x += displacement4.X/unitType.times[motion_index[0]];
                    y += displacement4.Y/unitType.times[motion_index[0]];
                    if (texRotate) { angle = Math.Atan2(displacement4.Y, displacement4.X); }// - Math.PI / 2; }
                    break;
                case MoveType.stop:
                    times[0] = 10;
                    break;
                default:
                    break;
                    #endregion
            }


            if (unitType.label.Contains("fadeout")&&(x < Map.leftside - animation.X / 2|| x > Map.rightside + animation.X / 2||y > DataBase.WindowSlimSizeY + animation.Y / 2||y < 0 - animation.Y / 2))
            {
                remove(Unit_state.out_of_window);
            }
            
            if (Function.hitcircle(x, y,unitType.radius, player.x, player.y, player.radius))
            {
                player.damage(1);
            }

            //bulletのupdate
            for (int i = 0; i < bullets.Count; i++)//update 専用
            {
                bullets[i].update(player);
                
            }
            for (int i = 0; i < bullets.Count; i++)//消す専用
            {
                if (bullets[i].delete == true) { bullets.Remove(bullets[i]); }

            }
            for (int i = 0; i < skills.Count; i++)
            {
                skills[i].update();
            }
        }

        public virtual void draw(Drawing d)
        {
            if (!texRotate)
            {
                animation.Draw(d, new Vector((x - animation.X / 2), (y - animation.Y / 2)), DepthID.Enemy);
            }else
            {
                float angle2 = (float)(angle ) ;
                animation.Draw(d, new Vector((x + animation.Y / 2*Math.Cos(angle2)+animation.X/2*Math.Sin(angle2)), (y +animation.Y/2*Math.Sin(angle2) - animation.X/ 2*Math.Cos(angle2))), DepthID.Enemy, 1, (float)(angle+Math.PI/2));
               
            }
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
                    BarrageUsedSkillData sd= (BarrageUsedSkillData)DataBase.SkillDatasDictionary[skills[i].skillName];
                    bool use = true;
                    switch (sd.sgl) {
                        case SkillGenreL.generation:
                            #region ジャンルの小さい分類
                            switch (sd.sgs)
                            {
                                case SkillGenreS.shot:
                                    SingleShotSkillData ss = (SingleShotSkillData)sd;
                                    bullets.Add(new Bullet(x, y, ss.moveType, ss.speed, ss.acceleration, ss.aniDName, new Vector(player.x, player.y), 100, ss.radius, ss.life, ss.score, ss.sword));
                                    break;
                                case SkillGenreS.circle:
                                    SingleShotSkillData ss1 = (SingleShotSkillData)sd;
                                    for (int j = 0; j < 2*Math.PI/ss1.angle; j++)
                                    {
                                        bullets.Add(new Bullet(x, y, ss1.moveType, ss1.speed, ss1.acceleration,-(Math.PI/2)+j*ss1.angle, ss1.aniDName, 100, ss1.radius, ss1.life, ss1.score, ss1.sword));
                                    }
                                    break;
                                case SkillGenreS.laser:
                                    LaserTopData lt = (LaserTopData)sd;
                                    bullets.Add(new LaserTop(x, y, lt.moveType, lt.speed, lt.acceleration, lt.angle,lt.aniDName, 100, lt.radius, lt.life, lt.score, lt.sword,lt.omega,this,lt.color));
                                    break;
                                case SkillGenreS.wayshot:
                                    WayShotSkillData ws = (WayShotSkillData)sd;
                                    double player_angle = Math.Atan2(player.y - y, player.x - x);
                                    if (ws.way % 2 == 1)
                                    {
                                        bullets.Add(new Bullet(x, y, ws.moveType, -ws.speed, ws.acceleration, player_angle, ws.aniDName, 100, ws.radius, ws.life, ws.score, ws.sword));
                                        for (int j = 1; j < (ws.way + 1) / 2; j++) 
                                        {
                                            bullets.Add(new Bullet(x, y, ws.moveType, -ws.speed, ws.acceleration, player_angle + j * ws.angle, ws.aniDName, 100, ws.radius, ws.life, ws.score, ws.sword));
                                            bullets.Add(new Bullet(x, y, ws.moveType, -ws.speed, ws.acceleration, player_angle - j * ws.angle, ws.aniDName, 100, ws.radius, ws.life, ws.score, ws.sword));
                                        }
                                    }else
                                    {
                                        for (int j = 0; j < ws.way / 2; j++)
                                        {
                                            bullets.Add(new Bullet(x, y, ws.moveType, -ws.speed, ws.acceleration, player_angle + j * ws.angle+ ws.angle / 2, ws.aniDName, 100, ws.radius, ws.life, ws.score, ws.sword));
                                            bullets.Add(new Bullet(x, y, ws.moveType, -ws.speed, ws.acceleration, player_angle - j * ws.angle- ws.angle / 2, ws.aniDName, 100, ws.radius, ws.life, ws.score, ws.sword));
                                        }
                                    }
                                    break;
                                case SkillGenreS.zyuzi:
                                    SingleShotSkillData ss2 = (SingleShotSkillData)sd;
                                    for (int j = 0; j < 3; j++)
                                    {
                                        bullets.Add(new Bullet(x, y, ss2.moveType, ss2.speed, ss2.acceleration, j*Math.PI/2, ss2.aniDName, 100, ss2.radius, ss2.life, ss2.score, ss2.sword));
                                    }
                                    break;
                                case SkillGenreS.yanagi:
                                    SingleShotSkillData ss3 = (SingleShotSkillData)sd;
                                    for(int j = 1; j < 5; j++)
                                    {

                                        Bullet bullet1 = new Bullet(x + ss3.space * j, y - ss3.space * j, ss3.moveType, ss3.speed, ss3.acceleration, ss3.angle, ss3.aniDName, 100, ss3.radius, ss3.life, ss3.score, ss3.sword);
                                        bullet1.speed_x = -Math.Pow(bullet1.speed * ss3.space * j,2) * 2;
                                        bullet1.speed_y = bullet1.speed;
                                        bullet1.acceleration_x = 0;
                                        bullet1.acceleration_y = -ss3.acceleration;
                                        bullets.Add(bullet1);
                                        Bullet bullet2 = new Bullet(x - ss3.space * j, y - ss3.space * j, ss3.moveType, ss3.speed, ss3.acceleration, ss3.angle, ss3.aniDName, 100, ss3.radius, ss3.life, ss3.score, ss3.sword);
                                        bullet2.speed_x = Math.Pow(bullet2.speed * ss3.space * j,2) * 2;
                                        bullet2.speed_y = bullet2.speed;
                                        bullet2.acceleration_x = 0;
                                        bullet2.acceleration_y = -ss3.acceleration;
                                        bullets.Add(bullet2);
                                    }
                                    break;
                                default:
                                    use = false;
                                    break;
                            }//switch sgs end
                            #endregion
                            break;
                        case SkillGenreL.bullet_create:
                            #region ジャンルの小さい分類
                            switch (sd.sgs)
                            {
                                case SkillGenreS.shot:
                                    GenerateUnitSkillData ss = (GenerateUnitSkillData)sd;
                                    bullets.Add(new SkilledBullet(x, y, ss.moveType, ss.speed, ss.acceleration,ss.angle, ss.aniDName, 100, ss.radius, ss.life, ss.score, ss.sword,ss.unitSkillName,this));
                                    break;
                                case SkillGenreS.circle:
                                    GenerateUnitSkillData ss1 = (GenerateUnitSkillData)sd;
                                    for (int j = 0; j < 2 * Math.PI / ss1.angle; j++)
                                    {
                                        bullets.Add(new SkilledBullet(x, y, ss1.moveType, ss1.speed, ss1.acceleration, -(Math.PI / 2) + j * ss1.angle, ss1.aniDName, 100, ss1.radius, ss1.life, ss1.score, ss1.sword,ss1.unitSkillName,this));
                                    }
                                    break;
                                case SkillGenreS.zyuzi:
                                    GenerateUnitSkillData ss2 = (GenerateUnitSkillData)sd;
                                    for (int j = 0; j < 3; j++)
                                    {
                                        bullets.Add(new SkilledBullet(x, y, ss2.moveType, ss2.speed, ss2.acceleration, j * Math.PI / 2, ss2.aniDName, 100, ss2.radius, ss2.life, ss2.score, ss2.sword,ss2.unitSkillName,this));
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

        public int score()
        {
            int total_score = 0;
            total_score += unitType.score;
            for (int j = 0; j < bullets.Count; j++)
            {
                total_score += bullets[j].score;

                Map.pros.Add(new Projection(bullets[j].x, bullets[j].y,
                    MoveType.object_target, Map.pro_speed, Map.pro_acceleration, "heal1", Map.player, 100));
                Map.pro_swords.Add(bullets[j].sword);
            }

            return total_score;
        }
    }
}
