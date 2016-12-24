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
        /// 目標物体がある場合に使う
        /// </summary>
        /// <param name="_radian">初期角度</param>
        /// <param name="_omega">レーザーが回転するならば、毎フレーム回転する度合い</param>
        /// <param name="_enemy">レーザーを使用したもの</param>
        /// <param name="_color">レーザーの色</param>
        public SkilledBullet(double _x, double _y, MoveType _move_type, double _speed, double _acceleration, string _anime, Unit _target,int _motion_time,double _radian, double _omega,double _radius,
             string[] _skillNames, Enemy enemy,  int _sword, int _life = 1, int _score = 0, int _zoom_rate = 100)
            : base(_x, _y, _move_type, _speed, _acceleration, _anime,_target,_radian,_omega, _radius, _sword, _life, _score, _zoom_rate)
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
        public SkilledBullet(double _x, double _y, MoveType _move_type, double _speed, double _acceleration, string _anime,Vector _target_pos, PointType _pt,int _motion_time,
          double _radian,double _omega, double _radius, string[] _skillNames, Enemy enemy,  int _sword, int _life = 1, int _score = 0, int _zoom_rate = 100)
            : base(_x, _y, _move_type, _speed, _acceleration, _anime,_target_pos,_pt,_motion_time, _radian,_omega, _radius, _sword, _life, _score, _zoom_rate)
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
        public override void update(Player player,bool bulletMove,bool skillsUpdate)
        {
            base.update(player, animation.dataIsNull() || bulletMove); // アニメーションのないskill bulletはbulletsMoveに拘束されない
            if (skillsUpdate)
            {
                for (int i = 0; i < skills.Count; i++)
                {
                    skills[i].update();
                }
            }
            if(!delete)
                shot(player);
        }

        public void shot(Unit player)
        {
            for (int i = 0; i < skills.Count; i++)
            {
                if (skills[i].coolDown<=0 )
                {
                    BarrageUsedSkillData sd = (BarrageUsedSkillData)DataBase.SkillDatasDictionary[skills[i].skillName];
                    if (!skills[i].used(nowMotionTime,-1, life, maxLife)) { continue; }
                    switch (sd.sgl)
                    {
                        case SkillGenreL.generation:
                        case SkillGenreL.UseSkilledBullet:
                            WayShotSkillData ws = (WayShotSkillData)sd;
                            double by;
                            #region ジャンルの小さい分類
                            switch (sd.sgs)
                            {
                                #region genre small yanagi
                                case SkillGenreS.yanagi:
                                    #region yanagi setting
                                    for (int j = 1; j <= ws.way/2 + 1; j++)
                                    {
                                        Bullet bullet1, bullet2;
                                        by = y - sd.space * j * j + animation.Y / 2;
                                        double bdx = sd.space * j*j/2 + sd.radius;
                                        if (ws.sgl == SkillGenreL.UseSkilledBullet)
                                        {
                                            WaySkilledBulletsData wSs = (WaySkilledBulletsData)ws;
                                            bullet1 = new SkilledBullet(x + bdx, by, sd.moveType,
                                                sd.speed, sd.acceleration, sd.aniDName, sd.angle, sd.radius, wSs.BulletSkillNames, myboss, sd.sword, sd.life, sd.score);
                                            bullet2 = new SkilledBullet(x - bdx, by, sd.moveType,
                                                sd.speed, sd.acceleration, sd.aniDName, sd.angle, sd.radius, wSs.BulletSkillNames, myboss, sd.sword, sd.life, sd.score);
                                        }
                                        else
                                        {
                                            bullet2 = new Bullet(x - bdx, by, sd.moveType,
                                            sd.speed, sd.acceleration, sd.aniDName, sd.angle, sd.radius, sd.sword, sd.life, sd.score);
                                            bullet1 = new Bullet(x + bdx, by, sd.moveType,
                                                sd.speed, sd.acceleration, sd.aniDName, sd.angle, sd.radius, sd.sword, sd.life, sd.score);
                                        }
                                        double bspeedX = sd.speed * (j - 1) * 0.01;
                                        double bspeedY = -sd.speed;// + 0.1 * (sd.space * j);
                                        bullet1.target_pos.X = bspeedX;
                                        bullet1.target_pos.Y = bspeedY;
                                        bullet1.acceleration_x = +sd.acceleration * j * j / 120;
                                        bullet1.acceleration_y = sd.acceleration;
                                        myboss.bullets.Add(bullet1);
                                        bullet2.target_pos.X = -bspeedX;
                                        bullet2.target_pos.Y = bspeedY;
                                        bullet2.acceleration_x = -sd.acceleration * j * j / 120;
                                        bullet2.acceleration_y = sd.acceleration;
                                        myboss.bullets.Add(bullet2);
                                    }
                                    #endregion
                                    break;
                                #endregion
                                default:
                                    #region bulletsAdd
                                    double bx = x; by = y;
                                    //Console.WriteLine(ws.skillName + " is " + ws.way+" "+ws.angle);
                                    double _angle = Motion.getAngleFromPointType(ws.pointType, ws.angle, ws.vec.X ,x,y,radian);
                                    if (ws.way % 2 == 1)
                                    {
                                        //Console.WriteLine("single");
                                        myboss.bulletsAdd(x, y, _angle, ws);
                                        for (int j = 1; j < (ws.way + 1) / 2; j++)
                                        {
                                            myboss.bulletsAdd(bx, by, _angle + j * sd.space, ws);
                                            myboss.bulletsAdd(bx, by, _angle - j * sd.space, ws);
                                        }
                                    }
                                    else
                                    {
                                        //Console.WriteLine("sfsf"+ws.way/2);
                                        for (int j = 0; j < ws.way / 2; j++)
                                        {
                                            myboss.bulletsAdd(bx, by, _angle + (j+0.5) * sd.space, ws);
                                            myboss.bulletsAdd(bx, by, _angle - (j+0.5) * sd.space, ws);
                                        }
                                    }
                                    if (sd.duration > 0) { for (int kk = 0; kk < ws.way; kk++) { myboss.bullets[myboss.bullets.Count - 1 - kk].setup_exist_time(ws.duration); } }
                                    #endregion
                                    break;
                            }//switch sgs end
                            #endregion
                            break;
                        default:
                            break;
                    }//switch sgl end
                }
            }
        }
    }
}
