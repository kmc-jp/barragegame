using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonPart
{
    class SkilledBullet : Bullet
    {
        Skill skill;
        Enemy myboss;

        public SkilledBullet(double _x, double _y, MoveType _move_type, double _speed, double _acceleration, double _radian, string _anime, int _zoom_rate, double _radius, int _life, int _score, int _sword,
            string _skillName,Enemy enemy)
            : base(_x, _y, _move_type, _speed, _acceleration, _radian, _anime, _zoom_rate, _radius, _life, _score, _sword)
        {
            skill = new Skill(_skillName);
            myboss = enemy;
        }

        public override void update(Player player)
        {
            base.update(player);
            skill.update();
            shot(player,myboss);
        }
        public void shot(Player player,Enemy enemy)
        {
            if (skill.coolDown <= 0)
            {
                SkillData sd = DataBase.SkillDatasDictionary[skill.skillName];
                bool use = true;
                switch (sd.sgl)
                {
                    case SkillGenreL.generation:
                        #region ジャンルの小さい分類
                        switch (sd.sgs)
                        {
                            case SkillGenreS.shot:
                                SingleShotSkillData ss = (SingleShotSkillData)sd;
                                myboss.bullets.Add(new Bullet(x, y, ss.moveType, ss.speed, ss.acceleration, ss.aniDName, new Vector(player.x, player.y), 100, ss.radius, ss.life, ss.score, ss.sword));
                                break;
                            case SkillGenreS.circle:
                                SingleShotSkillData ss1 = (SingleShotSkillData)sd;
                                for (int j = 0; j < 2 * Math.PI / ss1.angle; j++)
                                {
                                    myboss.bullets.Add(new Bullet(x, y, ss1.moveType, ss1.speed, ss1.acceleration, -(Math.PI / 2) + j * ss1.angle, ss1.aniDName, 100, ss1.radius, ss1.life, ss1.score, ss1.sword));
                                }
                                break;
                            case SkillGenreS.laser:
                                LaserTopData lt = (LaserTopData)sd;
                                myboss.bullets.Add(new LaserTop(x, y, lt.moveType, lt.speed, lt.acceleration, 0, lt.aniDName, 100, lt.radius, lt.life, lt.score, lt.sword, lt.angle, lt.omega, myboss, lt.color));
                                break;
                            case SkillGenreS.wayshot:
                                WayShotSkillData ws = (WayShotSkillData)sd;
                                double player_angle = Math.Atan2(player.y - y, player.x - x);
                                if (ws.way % 2 == 1)
                                {
                                    myboss.bullets.Add(new Bullet(x, y, ws.moveType, ws.speed, ws.acceleration, player_angle, ws.aniDName, 100, ws.radius, ws.life, ws.score, ws.sword));
                                    for (int j = 1; j < (ws.way + 1) / 2; j++)
                                    {
                                        myboss.bullets.Add(new Bullet(x, y, ws.moveType, ws.speed, ws.acceleration, player_angle + j * ws.angle, ws.aniDName, 100, ws.radius, ws.life, ws.score, ws.sword));
                                        myboss.bullets.Add(new Bullet(x, y, ws.moveType, ws.speed, ws.acceleration, player_angle - j * ws.angle, ws.aniDName, 100, ws.radius, ws.life, ws.score, ws.sword));
                                    }
                                }
                                else
                                {
                                    for (int j = 0; j < ws.way / 2; j++)
                                    {
                                        myboss.bullets.Add(new Bullet(x, y, ws.moveType, ws.speed, ws.acceleration, player_angle + j * ws.angle+ ws.angle / 2, ws.aniDName, 100, ws.radius, ws.life, ws.score, ws.sword));
                                        myboss.bullets.Add(new Bullet(x, y, ws.moveType, ws.speed, ws.acceleration, player_angle - j * ws.angle- ws.angle / 2, ws.aniDName, 100, ws.radius, ws.life, ws.score, ws.sword));
                                    }
                                }
                                break;
                            case SkillGenreS.zyuuzi:
                                SingleShotSkillData ss2 = (SingleShotSkillData)sd;
                                for (int j = 0; j < 3; j++)
                                {
                                    myboss.bullets.Add(new Bullet(x, y, ss2.moveType, ss2.speed, ss2.acceleration, j * Math.PI / 2, ss2.aniDName, 100, ss2.radius, ss2.life, ss2.score, ss2.sword));
                                }
                                break;
                            case SkillGenreS.yanagi:

                                SingleShotSkillData ss3 = (SingleShotSkillData)sd;
                                for (int j = 1; j < 5; j++)
                                {
                                    Bullet bullet1 = new Bullet(x + ss3.space * j, y - ss3.space * j* j*2+4+ animation.Y, ss3.moveType, ss3.speed, ss3.acceleration, ss3.angle, ss3.aniDName, 100, ss3.radius, ss3.life, ss3.score, ss3.sword);
                                    bullet1.speed_x = -ss3.speed * ss3.space*j*j*0.15 ;
                                    bullet1.speed_y = ss3.speed+0.5*(ss3.space * j);
                                    bullet1.acceleration_x = -ss3.acceleration*j*j/120;
                                    bullet1.acceleration_y =-ss3.acceleration;
                                    myboss.bullets.Add(bullet1);
                                    Bullet bullet2 = new Bullet(x - ss3.space * j, y - ss3.space *j* j*2+4+animation.Y, ss3.moveType, ss3.speed, ss3.acceleration, ss3.angle, ss3.aniDName, 100, ss3.radius, ss3.life, ss3.score, ss3.sword);
                                    bullet2.speed_x = ss3.speed * ss3.space*j*j*0.15;
                                    bullet2.speed_y = ss3.speed+0.5*(ss3.space * j);
                                    bullet2.acceleration_x = ss3.acceleration*j*j / 120;
                                    bullet2.acceleration_y = -ss3.acceleration;
                                    myboss.bullets.Add(bullet2);
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
                    skill.used();
                }
            }
        }
    }
}
