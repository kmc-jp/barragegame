using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonPart
{
    class SkilledBullet : Bullet
    {
        public List<Skill> skills=new List<Skill> ();
        Enemy myboss;

        /// <summary>
        /// 目標物体なし、目標点なしの場合に使える。
        /// </summary>
        /// <param name="_x"></param>
        /// <param name="_y"></param>
        /// <param name="_move_type"></param>
        /// <param name="_speed"></param>
        /// <param name="_acceleration"></param>
        /// <param name="_radian"></param>
        /// <param name="_anime"></param>
        /// <param name="_zoom_rate"></param>
        /// <param name="_radius"></param>
        /// <param name="_life"></param>
        /// <param name="_score"></param>
        /// <param name="_sword"></param>
        /// <param name="_skillName"></param>
        /// <param name="enemy"></param>
        public SkilledBullet(double _x, double _y, MoveType _move_type, double _speed, double _acceleration,string _anime, double _radian, double _radius, 
            string[] _skillNames, Enemy enemy, int _sword,int _score=0,int _life=1,int _zoom_rate=100)
            : base(_x, _y, _move_type, _speed, _acceleration, _anime,_radian, _radius, _sword, _life, _score,_zoom_rate)
        {
            addSkills(_skillNames);
            myboss = enemy;
        }
        /// <summary>
        /// 目標がない場合に使う
        /// </summary>
        /// <param name="_radian">初期角度</param>
        /// <param name="_omega">レーザーが回転するならば、毎フレーム回転する度合い</param>
        /// <param name="_enemy">レーザーを使用したもの</param>
        /// <param name="_color">レーザーの色</param>
        public SkilledBullet(double _x, double _y, MoveType _move_type, double _speed, double _acceleration, string _anime, Unit _target,double _radian, double _radius,
             string[] _skillNames, Enemy enemy,  int _sword, int _life = 1, int _score = 0, int _zoom_rate = 100)
            : base(_x, _y, _move_type, _speed, _acceleration, _anime,_target,_radian, _radius, _sword, _life, _score, _zoom_rate)
        {
            addSkills(_skillNames);
            myboss = enemy;
        }
        /// <summary>
        /// 目標点だけはある場合に使う
        /// </summary>
        /// <param name="_radian">初期角度</param>
        /// <param name="_omega">レーザーが回転するならば、毎フレーム回転する度合い</param>
        /// <param name="_enemy">レーザーを使用したもの</param>
        /// <param name="_color">レーザーの色</param>
        public SkilledBullet(double _x, double _y, MoveType _move_type, double _speed, double _acceleration, string _anime,Vector _target_pos, PointType _pt
        , double _radian, double _radius, string[] _skillNames, Enemy enemy,  int _sword, int _life = 1, int _score = 0, int _zoom_rate = 100)
            : base(_x, _y, _move_type, _speed, _acceleration, _anime,_target_pos,_pt, _radian, _radius, _sword, _life, _score, _zoom_rate)
        {
            addSkills(_skillNames);
            myboss = enemy;
        }

        public void addSkills(string[] _skillNames)
        {
            foreach (string _skillName in _skillNames)
            {
                skills.Add(new Skill(_skillName));
            }
        }

        protected override void dead()
        {
            base.dead();
            shot(Map.player);
        }
        public override void update(Player player,bool bulletMove)
        {
            base.update(player,bulletMove);
            for (int i=0; i < skills.Count; i++)
            {
                skills[i].update();
            }
            shot(player);
        }
<<<<<<< HEAD
        
        public void shot()
        {

=======
        public void shot(Unit player, bool afterDeath = false)
        {
            for (int i = 0; i < skills.Count; i++)
            {
                if (skills[i].coolDown <= 0 && (afterDeath || !skills[i].skillName.Contains(DataBase.skillUsedAfterDeath)))
                {
                    BarrageUsedSkillData sd = (BarrageUsedSkillData)DataBase.SkillDatasDictionary[skills[i].skillName];
                    bool use = true;
                    switch (sd.sgl)
                    {
                        case SkillGenreL.generation:
                            #region ジャンルの小さい分類
                            switch (sd.sgs)
                            {
                                #region genre small. shot
                                case SkillGenreS.shot:
                                    SingleShotSkillData ss = (SingleShotSkillData)sd;
                                    if (ss.moveType == MoveType.object_target)//これは常に敵を追うパターンである。
                                    {
                                        myboss.bullets.Add(new Bullet(x, y, sd.moveType, sd.speed, sd.acceleration, sd.aniDName, player, sd.angle, sd.radius, sd.sword, sd.life, sd.score));
                                    }
                                    else if (sd.moveType == MoveType.screen_point_target)
                                    {
                                        myboss.bullets.Add(new Bullet(x, y, sd.moveType, sd.speed, sd.acceleration, sd.aniDName, new Vector(player.x, player.y), PointType.pos_on_screen, sd.angle, sd.radius, sd.sword, sd.life, sd.score));
                                    }
                                    else { myboss.bullets.Add(new Bullet(x, y, sd.moveType, sd.speed, sd.acceleration, sd.aniDName, sd.angle, sd.radius, sd.sword, sd.life, sd.score)); }
                                    break;
                                #endregion
                                #region genre small. circle
                                case SkillGenreS.circle:
                                    SingleShotSkillData ss1 = (SingleShotSkillData)sd;
                                    for (int j = 0; j < 2 * Math.PI / sd.angle; j++)
                                    {
                                        myboss.bullets.Add(new Bullet(x, y, sd.moveType, sd.speed, sd.acceleration, sd.aniDName, (Math.PI / 2) + j * sd.angle, sd.radius, sd.sword, sd.life, sd.score));
                                        if (sd.duration > 0) { myboss.bullets[myboss.bullets.Count - 1].setup_exist_time(sd.duration); }
                                    }
                                    break;
                                #endregion
                                #region genre small. laser
                                case SkillGenreS.laser:
                                    LaserTopData lt = (LaserTopData)sd;
                                    if (lt.moveType == MoveType.chase_target)
                                    {
                                        myboss.bullets.Add(new LaserTop(x, y, lt.moveType, lt.speed, lt.acceleration, player, lt.aniDName, lt.angle, lt.radius, lt.omega, this, lt.color, lt.sword, lt.life, lt.score));
                                    }
                                    else
                                    {
                                        myboss.bullets.Add(new LaserTop(x, y, lt.moveType, lt.speed, lt.acceleration, lt.angle, lt.aniDName, lt.radius, lt.omega, this, lt.color, lt.sword, lt.life, lt.score));
                                    }
                                    if (lt.duration > 0) { myboss.bullets[myboss.bullets.Count - 1].setup_exist_time(lt.duration); }
                                    break;
                                #endregion
                                #region genre small. wayshot
                                case SkillGenreS.wayshot:
                                    WayShotSkillData ws = (WayShotSkillData)sd;
                                    double player_angle = Math.Atan2(player.y - y, player.x - x);
                                    if (ws.way % 2 == 1)
                                    {
                                        myboss.bullets.Add(new Bullet(x, y, sd.moveType, sd.speed, sd.acceleration, sd.aniDName, player_angle, sd.radius, sd.sword, sd.life, sd.score));
                                        for (int j = 1; j < (ws.way + 1) / 2; j++)
                                        {
                                            myboss.bullets.Add(new Bullet(x, y, sd.moveType, sd.speed, sd.acceleration, sd.aniDName, player_angle + j * sd.angle, sd.radius, sd.sword, sd.life, sd.score));
                                            myboss.bullets.Add(new Bullet(x, y, sd.moveType, sd.speed, sd.acceleration, sd.aniDName, player_angle - j * sd.angle, sd.radius, sd.sword, sd.life, sd.score));
                                        }
                                    }
                                    else
                                    {
                                        for (int j = 0; j < ws.way / 2; j++)
                                        {
                                            myboss.bullets.Add(new Bullet(x, y, sd.moveType, sd.speed, sd.acceleration, sd.aniDName, player_angle + j * sd.angle + sd.angle / 2, sd.radius, sd.sword, sd.life, sd.score));
                                            myboss.bullets.Add(new Bullet(x, y, sd.moveType, sd.speed, sd.acceleration, sd.aniDName, player_angle - j * sd.angle - sd.angle / 2, sd.radius, sd.sword, sd.life, sd.score));
                                        }
                                    }
                                    if (sd.duration > 0) { for (int kk = 0; kk < ws.way; kk++) { myboss.bullets[myboss.bullets.Count - 1 - kk].setup_exist_time(ws.duration); } }
                                    break;
                                #endregion
                                #region genre small yanagi
                                case SkillGenreS.yanagi:
                                    WayShotSkillData ws2 = (WayShotSkillData)sd;
                                    #region yanagi setting
                                    Console.Write(sd.aniDName + DataBase.existsAniD(sd.aniDName, null));
                                    for (int j = 1; j < ws2.way + 1; j++)
                                    {
                                        Bullet bullet1 = new Bullet(x + sd.space * j + sd.radius, y - sd.space * j * j * 2 + 4 + animation.Y / 2, sd.moveType,
                                            sd.speed, sd.acceleration, sd.aniDName, sd.angle, sd.radius, sd.sword, sd.life, sd.score);
                                        bullet1.speed_x = sd.speed * (j - 1) * 0.25;
                                        bullet1.speed_y = -sd.speed + 0.1 * (sd.space * j);
                                        bullet1.acceleration_x = +sd.acceleration * j * j / 120;
                                        bullet1.acceleration_y = sd.acceleration;
                                        myboss.bullets.Add(bullet1);
                                        Bullet bullet2 = new Bullet(x - sd.space * j - sd.radius, y - sd.space * j * j * 2 + 4 + animation.Y / 2, sd.moveType,
                                            sd.speed, sd.acceleration, sd.aniDName, sd.angle, sd.radius, sd.sword, sd.life, sd.score);
                                        bullet2.speed_x = -sd.speed * (j - 1) * 0.25;
                                        bullet2.speed_y = -sd.speed + 0.1 * (sd.space * j);
                                        bullet2.acceleration_x = -sd.acceleration * j * j / 120;
                                        bullet2.acceleration_y = sd.acceleration;
                                        myboss.bullets.Add(bullet2);
                                    }
                                    Console.Write(myboss.bullets.Count);
                                    #endregion
                                    break;
                                #endregion
                                default:
                                    use = false;
                                    break;
                            }//switch sgs end
                            #endregion
                            break;
                        case SkillGenreL.UseSkilledBullet:
                            GenerateSkilledBulletData gsb = (GenerateSkilledBulletData)sd;
                            #region ジャンルの小さい分類
                            switch (sd.sgs)
                            {
                                #region genre small. shot:
                                case SkillGenreS.shot:
                                    if (gsb.moveType == MoveType.object_target)//これは常に敵を追うパターンである。
                                    {
                                        myboss.bullets.Add(new SkilledBullet(x, y, sd.moveType, sd.speed, sd.acceleration, sd.aniDName, player, sd.angle, sd.radius,
                                            gsb.unitSkillName, myboss, sd.sword, sd.life, sd.score));
                                    }
                                    else if (sd.moveType == MoveType.screen_point_target)
                                    {
                                        myboss.bullets.Add(new SkilledBullet(x, y, sd.moveType, sd.speed, sd.acceleration, sd.aniDName, new Vector(player.x, player.y), PointType.pos_on_screen,
                                            sd.angle, sd.radius, gsb.unitSkillName, myboss, sd.sword, sd.life, sd.score));
                                    }
                                    else
                                    {
                                        myboss.bullets.Add(new SkilledBullet(x, y, sd.moveType, sd.speed, sd.acceleration, sd.aniDName, sd.angle, sd.radius,
                                     gsb.unitSkillName, myboss, sd.sword, sd.life, sd.score));
                                    }
                                    break;
                                #endregion
                                #region genre small circle
                                case SkillGenreS.circle:
                                    SingleShotSkillData ss1 = (SingleShotSkillData)sd;
                                    for (int j = 0; j < 2 * Math.PI / sd.angle; j++)
                                    {
                                        myboss.bullets.Add(new SkilledBullet(x, y, sd.moveType, sd.speed, sd.acceleration, sd.aniDName, (Math.PI / 2) + j * sd.angle, sd.radius,
                                            gsb.unitSkillName, myboss, sd.sword, sd.life, sd.score));
                                        if (sd.duration > 0) { myboss.bullets[myboss.bullets.Count - 1].setup_exist_time(sd.duration); }
                                    }
                                    break;
                                #endregion
                                #region genre small wayshot
                                case SkillGenreS.wayshot:
                                    WayShotSkillData ws = (WayShotSkillData)sd;
                                    double player_angle = Math.Atan2(player.y - y, player.x - x);
                                    if (ws.way % 2 == 1)
                                    {
                                        myboss.bullets.Add(new SkilledBullet(x, y, sd.moveType, sd.speed, sd.acceleration, sd.aniDName, player_angle, sd.radius,
                                            gsb.unitSkillName, myboss, sd.sword, sd.life, sd.score));
                                        for (int j = 1; j < (ws.way + 1) / 2; j++)
                                        {
                                            myboss.bullets.Add(new SkilledBullet(x, y, sd.moveType, sd.speed, sd.acceleration, sd.aniDName, player_angle + j * sd.angle, sd.radius,
                                                gsb.unitSkillName, myboss, sd.sword, sd.life, sd.score));
                                            myboss.bullets.Add(new SkilledBullet(x, y, sd.moveType, sd.speed, sd.acceleration, sd.aniDName, player_angle - j * sd.angle, sd.radius,
                                                gsb.unitSkillName, myboss, sd.sword, sd.life, sd.score));
                                        }
                                    }
                                    else
                                    {
                                        for (int j = 0; j < ws.way / 2; j++)
                                        {
                                            myboss.bullets.Add(new SkilledBullet(x, y, sd.moveType, sd.speed, sd.acceleration, sd.aniDName, player_angle + j * sd.angle + sd.angle / 2, sd.radius,
                                                gsb.unitSkillName, myboss, sd.sword, sd.life, sd.score));
                                            myboss.bullets.Add(new SkilledBullet(x, y, sd.moveType, sd.speed, sd.acceleration, sd.aniDName, player_angle - j * sd.angle - sd.angle / 2, sd.radius,
                                                gsb.unitSkillName, myboss, sd.sword, sd.life, sd.score));
                                        }
                                    }
                                    if (sd.duration > 0) { for (int kk = 0; kk < ws.way; kk++) { myboss.bullets[myboss.bullets.Count - 1 - kk].setup_exist_time(sd.duration); } }
                                    break;
                                #endregion
                                #region yanagi
                                case SkillGenreS.yanagi:
                                    WayShotSkillData ws2 = (WayShotSkillData)sd;
                                    #region yanagi setting
                                    for (int j = 1; j < ws2.way + 1; j++)
                                    {
                                        Bullet bullet1 = new SkilledBullet(x + sd.space * j + sd.radius, y - sd.space * j * j * 2 + 4 + animation.Y / 2, sd.moveType,
                                            sd.speed, sd.acceleration, sd.aniDName, sd.angle, sd.radius,
                                            gsb.unitSkillName, myboss, sd.sword, sd.life, sd.score);
                                        bullet1.speed_x = sd.speed * (j - 1) * 0.25;
                                        bullet1.speed_y = -sd.speed + 0.1 * (sd.space * j);
                                        bullet1.acceleration_x = +sd.acceleration * j * j / 120;
                                        bullet1.acceleration_y = sd.acceleration;
                                        myboss.bullets.Add(bullet1);
                                        Bullet bullet2 = new SkilledBullet(x - sd.space * j - sd.radius, y - sd.space * j * j * 2 + 4 + animation.Y / 2, sd.moveType,
                                            sd.speed, sd.acceleration, sd.aniDName, sd.angle, sd.radius,
                                            gsb.unitSkillName, myboss, sd.sword, sd.life, sd.score);
                                        bullet2.speed_x = -sd.speed * (j - 1) * 0.25;
                                        bullet2.speed_y = -sd.speed + 0.1 * (sd.space * j);
                                        bullet2.acceleration_x = -sd.acceleration * j * j / 120;
                                        bullet2.acceleration_y = sd.acceleration;
                                        myboss.bullets.Add(bullet2);
                                    }
                                    #endregion
                                    break;
                                #endregion
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
>>>>>>> refs/remotes/origin/master
        }
    }
}
