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
    class Enemy:Unit
    {
        #region const labels
        public const string unitLabel_FadeOut = "fadeout";
        #endregion

        #region basic undeclared variables
        public double x;
        public double y;
        public List<Bullet> bullets = new List<Bullet>();
        public List<Skill> skills = new List<Skill>();
        public string unitType_name;
        public string label;
        public AnimationAdvanced animation;
        #endregion
        public double angle=0;
        #region property
        protected bool texRotate { get { if (unitType == null) { return false; } else { return unitType.textureTurn; } } }
        public ActiveAniSkiedUnitType unitType { get { return (ActiveAniSkiedUnitType)DataBase.getUnitType(unitType_name); } }
        public double radius { get { return unitType.radius; } }
        #endregion
        public int life = 1;
        public int maxLife;
        #region status variables
        public bool delete = false;
        public bool exist = false;
        public bool fadeout = false;
        #endregion
        #region about Motion
        protected int[] motion_index=new int[2];
        protected MoveType mt;
        protected Vector default_pos;
        protected int alltime;
        protected PointType pt;
        /// <summary>
        /// 0が今のルーチン、1が保存したルーチン
        /// </summary>
        public int[] times=new int[2];

        public int stop_time=0;

        private bool once = false;
        private Vector displacement4;

        /// <summary>
        /// これがfalseでは、motionのループはそもそも考えない
        /// </summary>
        protected bool motionLooped = false;
        protected int motionLoopIndex;
        /// <summary>
        /// これらはmotionLoopSetUp()が呼ばれた時に作られる.ループの始点と終点を意味する.
        /// </summary>
        protected List<int> motionLoopsStart, motionLoopsEnd;
        #endregion

        public Enemy(double _x,double _y,string _unitType_name):base(_x,_y)
        {
            unitType_name = _unitType_name;
            label = unitType.label;
            playAnimation(DataBase.defaultAnimationNameAddOn);
            motion_index[0] = 0;
            motion_index[1] = -1;
            times[0] =0;
            setup_skill();
        }
        public virtual void update(Player player)
        {
            animation.Update();
            if (stop_time <= 0)
            {//敵が行動不能ではない
                #region about motion
                if (times[0] >= unitType.times[motion_index[0]])
                {
                    if (motion_index[0] < unitType.moveTypes.Count - 1)
                    {
                        motion_index[0]++;
                        //Console.WriteLine("play animation!");
                        once = false;
                    }
                    else { motion_index[0] = 0; }
                    times[0] = 0;
                }
                times[0]++;
                switch (unitType.moveTypes[motion_index[0]])
                {
                    #region 動作
                    case MoveType.mugen:
                        Vector displacement = MotionCalculation.mugenDisplacement(unitType.speed, unitType.times[motion_index[0]], times[0]);
                        x -= displacement.X;
                        y -= displacement.Y;
                        if (texRotate) { angle = Math.Atan2(-displacement.Y, -displacement.X); } //- Math.PI / 2; }
                        break;
                    case MoveType.go_straight:
                        Vector displacement1 = MotionCalculation.tousokuidouDisplacement(unitType.default_poses[motion_index[0]], unitType.times[motion_index[0]]);
                        x += displacement1.X;
                        y += displacement1.Y;

                        break;
                    case MoveType.rightcircle:
                        Vector displacement2 = MotionCalculation.rightcircleDisplacement(unitType.speed, unitType.times[motion_index[0]], times[0]);
                        x += displacement2.X;
                        y += displacement2.Y;

                        break;
                    case MoveType.leftcircle:
                        Vector displacement3 = MotionCalculation.leftcircleDisplacement(unitType.speed, unitType.times[motion_index[0]], times[0]);
                        x += displacement3.X;
                        y += displacement3.Y;

                        break;
                    case MoveType.screen_point_target:
                        if (once == false)
                        {
                            Vector goal = new Vector(unitType.default_poses[motion_index[0]].X + Map.leftside, unitType.default_poses[motion_index[0]].Y);
                            displacement4 = new Vector(goal.X - x, goal.Y - y);
                            once = true;
                        }
                        x += displacement4.X / unitType.times[motion_index[0]];
                        y += displacement4.Y / unitType.times[motion_index[0]];
                        if (texRotate) { angle = Math.Atan2(displacement4.Y, displacement4.X); }// - Math.PI / 2; }
                        break;
                    case MoveType.stop:
                        times[0] = 10;
                        break;
                    default:
                        break;
                        #endregion
                }


                if (label.Contains(unitLabel_FadeOut) && (x < Map.leftside - animation.X / 2 || x > Map.rightside + animation.X / 2 || y > DataBase.WindowSlimSizeY + animation.Y / 2 || y < 0 - animation.Y / 2))
                {
                    remove(Unit_state.out_of_window);
                }
                #endregion
                if (Function.hitcircle(x, y, unitType.radius, player.x, player.y, player.radius))
                {
                    player.damage(1);
                }

                #region bulletのupdate
                for (int i = 0; i < bullets.Count; i++)//update 専用
                {
                    bullets[i].update(player);
                }
                for (int i = 0; i < bullets.Count; i++)//消す専用
                {
                    if (bullets[i].delete == true) { bullets.Remove(bullets[i]); }
                }
                #endregion
                update_skills();
                shot(player);
            }else { stop_time--; return; }
        }

        public override void draw(Drawing d)
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


        #region Functions about skills
        protected void setup_skill()
        {
            for (int i = 0; i < unitType.skillNames.Count; i++)
            {
                skills.Add(new Skill(unitType.skillNames[i]));
            }
        }

        protected void update_skills() {
            for (int i = 0; i < skills.Count; i++)
            {
                skills[i].update();
            }
        }

        public void add_skill(string skillName)
        {
            if (DataBase.existSkillDataName(skillName))
            {
                skills.Add(new Skill(skillName));
            }
            else { Console.WriteLine("add_skill: Does not exist" + skillName); }
        }
        /// <summary>
        /// スキルのクールダウンをそのデータでのクールダウン*reduceByPercentまで増やす
        /// </summary>
        /// <param name="_skillName"></param>
        /// <param name="reduceByPercent"></param>
        public void add_skill_coolDown(string _skillName, float reduceByPercent)
        {
            foreach (Skill sk in skills)
            {
                if (sk.skillName == _skillName)
                {
                    sk.coolDown += (int)(reduceByPercent * DataBase.getSkillData(_skillName).cooldownFps);
                }
            }
        }
        /// <summary>
        /// スキルのクールダウンをそのデータでのクールダウン*reduceByPercentまで増やす
        /// </summary>
        public void add_skill_coolDown(int index, float reduceByPercent)
        {
            skills[index].coolDown += (int)(reduceByPercent * DataBase.getSkillData(skills[index].skillName).cooldownFps);
        }
        public void set_skill_coolDown(string _skillName, int cd)
        {
            foreach (Skill sk in skills)
            {
                if (sk.skillName == _skillName)
                {
                    sk.coolDown = cd;
                }
            }
        }
        /// <summary>
        /// skillsの配列のiインデックスのスキルのクールダウンを設定する
        /// </summary>
        /// <param name="index">インデックスは常に0から始まている</param>
        /// <param name="cd"></param>
        public void set_skill_coolDown(int index, int cd)
        {
            if (index >= skills.Count) { return; }
            else { skills[index].coolDown = cd; }
        }
        #endregion

        #region Functions About PointType
        /// <summary>
        /// 渡された(x,y)とPointTypeで適切なyを返す
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="_pt"></param>
        protected double from_PointType_getPosY(double x, double y, PointType _pt)
        {
            switch (_pt)
            {
                case PointType.notused:
                    return 0;
                default:
                    return y;
            }
        }
        /// <summary>
        /// 渡された(x,y)とPointTypeで適切なxを返す
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="_pt"></param>
        protected double from_PointType_getPosX(double x, double y, PointType _pt)
        {
            switch (_pt)
            {
                case PointType.notused:
                    return 0;
                default:
                    return x;
            }
        }
        #endregion

        #region Funtions about Motion
        /// <summary>
        /// 例外のmotionを設置する
        /// </summary>
        /// <param name="_mt">今してほしいMoveType</param>
        /// <param name="_pt"></param>
        /// <param name="_alltime">その動きにかかる時間、不要なら埋めなくてよい</param>
        /// <param name="_pos">その動きに必要な点、不要なら適当に</param>
        protected void setup_extra_motion(MoveType _mt,PointType _pt,Vector _pos, int _alltime=DataBase.motion_inftyTime)
        {
            backup_Motion_into_1();
            alltime = _alltime;
            times[0] = 0;
            mt = _mt;
            pt = _pt;
            setup_default_pos(_pos);
        }
        /// <summary>
        /// times[0]は変更されず、motion_index[0],mt,alltime,default_posを設置する
        /// </summary>
        /// <param name="i"></param>
        protected void setup_motion(int i) {
            if (i < unitType.moveTypes.Count) {
                motion_index[0] = i; mt =unitType.moveTypes[i]; alltime = unitType.times[i];
                setup_default_pos(i);
            }
            else { Console.Write("setup_motion:Invaild Motion-" + unitType_name + "- i is" + i); }
        }
        protected void update_motion_index() {
            if (motion_index[1] >= 0)
            {//例外が設置された。
                if (times[0] >= alltime)
                {//例外のmotionから脱出して、元に戻る。
                    get_Motion_from_1();
                    setup_motion(motion_index[0]);
                    clear_extra_motion_1();
                }
            }
            else
            {
                if (times[0] >= unitType.times[motion_index[0]])
                {
                    if (motion_index[0] < unitType.moveTypes.Count - 1)
                    {

                        motion_index[0]++;

                        once = false;
                    }
                    else { motion_index[0] = 0; }
                    times[0] = 0;
                }
            }
        }
        /// <summary>
        /// motion_index[0]とtimes[0]を[1]の値にする
        /// </summary>
        protected void get_Motion_from_1() {
            motion_index[0] = motion_index[1]; times[0] = times[1];
        }
        protected void clear_extra_motion_1()
        {
            motion_index[1] = -1;
        }
        /// <summary>
        /// 現在のmotionIndex[0]をmotionIndex[1]に入れる.time[0]はtime[1]に入れる.
        /// </summary>
        protected void backup_Motion_into_1()
        {
            motion_index[1] = motion_index[0];  times[1] = times[0];
        }
        
        /// <summary>
        /// インデックスiのmotionのPointTypeに対応したdefault_posを求める
        /// </summary>
        /// <param name="i"></param>
        protected void setup_default_pos(int i)
        {
            default_pos.X = from_PointType_getPosX(default_pos.X, default_pos.Y, unitType.pointTypes[i]);
            default_pos.Y = from_PointType_getPosY(default_pos.X, default_pos.Y, unitType.pointTypes[i]);
        }

        protected void setup_default_pos(Vector pos)
        {
            default_pos.X = from_PointType_getPosX(pos.X, pos.Y, pt);
            default_pos.Y = from_PointType_getPosY(pos.X, pos.Y, pt);
        }

        #endregion

        public void FadeOut()
        {
            label += unitLabel_FadeOut;
            backup_Motion_into_1();
            if (x < Map.rightside / 2 + Map.leftside / 2)
            {

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
                                    if (ss.moveType == MoveType.object_target)
                                    {
                                        bullets.Add(new Bullet(x, y, ss.moveType, ss.speed, ss.acceleration, ss.aniDName, new Vector(player.x, player.y), 100, ss.radius, ss.life, ss.score, ss.sword));
                                    }else
                                    {
                                        bullets.Add(new Bullet(x, y, ss.moveType, ss.speed, ss.acceleration,ss.angle, ss.aniDName, 100, ss.radius, ss.life, ss.score, ss.sword));
                                    }

                                    break;
                                case SkillGenreS.circle:
                                    SingleShotSkillData ss1 = (SingleShotSkillData)sd;
                                    for (int j = 0; j < 2*Math.PI/ss1.angle; j++)
                                    {
                                        bullets.Add(new Bullet(x, y, ss1.moveType, ss1.speed, ss1.acceleration,(Math.PI/2)+j*ss1.angle, ss1.aniDName, 100, ss1.radius, ss1.life, ss1.score, ss1.sword));
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
                                        bullets.Add(new Bullet(x, y, ws.moveType, ws.speed, ws.acceleration, player_angle, ws.aniDName, 100, ws.radius, ws.life, ws.score, ws.sword));
                                        for (int j = 1; j < (ws.way + 1) / 2; j++) 
                                        {
                                            bullets.Add(new Bullet(x, y, ws.moveType, ws.speed, ws.acceleration, player_angle + j * ws.angle, ws.aniDName, 100, ws.radius, ws.life, ws.score, ws.sword));
                                            bullets.Add(new Bullet(x, y, ws.moveType, ws.speed, ws.acceleration, player_angle - j * ws.angle, ws.aniDName, 100, ws.radius, ws.life, ws.score, ws.sword));
                                        }
                                    }else
                                    {
                                        for (int j = 0; j < ws.way / 2; j++)
                                        {
                                            bullets.Add(new Bullet(x, y, ws.moveType, ws.speed, ws.acceleration, player_angle + j * ws.angle+ ws.angle / 2, ws.aniDName, 100, ws.radius, ws.life, ws.score, ws.sword));
                                            bullets.Add(new Bullet(x, y, ws.moveType, ws.speed, ws.acceleration, player_angle - j * ws.angle- ws.angle / 2, ws.aniDName, 100, ws.radius, ws.life, ws.score, ws.sword));
                                        }
                                    }
                                    break;
                                case SkillGenreS.zyuzi:
                                    SingleShotSkillData ss2 = (SingleShotSkillData)sd;
                                    for (int j = 0; j < 4; j++)
                                    {
                                        bullets.Add(new Bullet(x, y, ss2.moveType, ss2.speed, ss2.acceleration, j*Math.PI/2, ss2.aniDName, 100, ss2.radius, ss2.life, ss2.score, ss2.sword));
                                    }
                                    break;
                                case SkillGenreS.yanagi:
                                    SingleShotSkillData ss3 = (SingleShotSkillData)sd;
                                    for (int j = 1; j < 5; j++)
                                    {
                                        Bullet bullet1 = new Bullet(x + ss3.space * j+ss3.radius, y - ss3.space * j * j *2 + 4 + animation.Y/2, ss3.moveType, ss3.speed, ss3.acceleration, ss3.angle, ss3.aniDName, 100, ss3.radius, ss3.life, ss3.score, ss3.sword);
                                        bullet1.speed_x = ss3.speed * (j-1) * 0.25;
                                        bullet1.speed_y = -ss3.speed + 0.1 * (ss3.space * j);
                                        bullet1.acceleration_x = +ss3.acceleration * j * j / 120;
                                        bullet1.acceleration_y = ss3.acceleration;
                                        bullets.Add(bullet1);
                                        Bullet bullet2 = new Bullet(x - ss3.space * j-ss3.radius, y - ss3.space * j * j *2 + 4 + animation.Y/2, ss3.moveType, ss3.speed, ss3.acceleration, ss3.angle, ss3.aniDName, 100, ss3.radius, ss3.life, ss3.score, ss3.sword);
                                        bullet2.speed_x = -ss3.speed * (j-1) * 0.25;
                                        bullet2.speed_y = -ss3.speed + 0.1 * (ss3.space * j);
                                        bullet2.acceleration_x = -ss3.acceleration * j * j / 120;
                                        bullet2.acceleration_y = ss3.acceleration;
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
                                        bullets.Add(new SkilledBullet(x, y, ss1.moveType, ss1.speed, ss1.acceleration, (Math.PI / 2) + j * ss1.angle, ss1.aniDName, 100, ss1.radius, ss1.life, ss1.score, ss1.sword,ss1.unitSkillName,this));
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

        /// <summary>
        /// このenemyのx,yを原点として、このbulletのradius+p_radius半径内にpx,pyがあるかどうか
        /// </summary>
        /// <param name="px"></param>
        /// <param name="py"></param>
        /// <param name="p_radius"></param>
        /// <returns></returns>
        public bool hit_jugde(double px, double py, double p_radius = 0)
        {
            return Function.hitcircle(x, y, radius, px, py, p_radius);
        }
        public virtual void damage(int atk)
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
            if (delete || fadeout||!Map.inside_window(this))
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

        /// <summary>
        /// 現在のplayerの使用スキルに応じて,この敵のスコアと、発した弾丸のスコアを返す。また発した弾丸はすべてremove()が呼ばれる。
        /// </summary>
        /// <returns></returns>
        public virtual void score()
        {
            int total_score = 0;
            total_score += Map.caculateEnemyScore(unitType.score);
            for (int j = 0; j < bullets.Count; j++)
            {
                bullets[j].remove(Unit_state.dead);//remove()を見て、どうなっているかを確認するように
            }

            Map.make_chargePro(x,y,unitType.sword,total_score);
        }
    }
}
