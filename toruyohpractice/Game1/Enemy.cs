﻿using System;
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
        const int MaximumOfBullets = 400;
        #region const labels
        public const string unitLabel_FadeOut = "fadeout";
        #endregion

        #region basic variables as an Enemy
        public bool bulletsMove = true,skillsUpdate=true;
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
        public override double radius { get { return unitType.radius; } }
        #endregion
        public int life = 1;
        public int maxLife=1;
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
        protected double speed;
        protected double omega;
        protected PointType pt;
        /// <summary>
        /// 0が今のルーチン、1が保存したルーチン
        /// </summary>
        public int[] times=new int[2];

        public int stop_time=0;

        /// <summary>
        /// これがfalseでは、motionのループはそもそも考えない
        /// </summary>
        protected bool motionLooped = false;
        public int motionLoopIndex;
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
            motionLoopIndex = 0;
            times[0] =0;
            speed = unitType.speed;
            omega = unitType.omega;
            setup_motion(0);
            setup_skill();
        }
        public virtual void update(Player player)
        {
            animation.Update();
            if (stop_time <= 0)
            {//敵が行動不能ではない
                #region about motion
                times[0]++;
                update_motion_index();
                switch (mt)
                {
                    #region 動作
                    case MoveType.mugen:
                        Vector displacement = MotionCalculation.mugenDisplacement(unitType.speed, unitType.times[motion_index[0]], times[0]);
                        x -= displacement.X;
                        y -= displacement.Y;
                        if (texRotate) { angle = Math.Atan2(-displacement.Y, -displacement.X); } //- Math.PI / 2; }
                        break;
                    case MoveType.go_straight:
                        x += default_pos.X;
                        y += default_pos.Y;
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
                    case MoveType.chase_player_target:
                        if (!Function.hitcircle(x, y, 0, player.x, player.y, speed / 2))
                        {
                            speed += unitType.acceleration;
                            angle = Function.towardValue(angle, Math.Atan2(player.y - y, player.x - x), omega);
                            x += speed*Math.Cos(angle);
                            y += speed*Math.Sin(angle);
                        }
                        else { if (alltime == DataBase.motion_inftyTime) times[0] = alltime; }
                        break;
                    case MoveType.player_target:
                        if (!Function.hitcircle(x, y, 0, player.x, player.y, speed / 2))
                        {
                            speed += unitType.acceleration;
                            double ep = Math.Sqrt(Function.distance(x, y, player.x, player.y));
                            double speed_x = speed * (player.x - x) / ep;
                            double speed_y = speed * (player.y - y) / ep;
                            x += speed_x;
                            y += speed_y;
                            if (texRotate) { angle = Math.Atan2(speed_y, speed_x); }
                        }
                        else { if (alltime == DataBase.motion_inftyTime) times[0] = alltime; }
                        break;
                    case MoveType.rotateAndGo:
                        speed += unitType.acceleration;
                        angle += omega;
                        x += speed * Math.Cos(angle);
                        y += speed * Math.Sin(angle);
                        break;
                    case MoveType.screen_point_target://スクリーン上の点に移動し、十分近ければその点に移りMoveTypeを次に移す
                        if (!Function.hitcircle(x, y, 0, default_pos.X, default_pos.Y, unitType.speed/2))
                        {
                            speed += unitType.acceleration;
                            double ep = Math.Sqrt(Function.distance(x, y, default_pos.X, default_pos.Y));
                            double speed_x = speed * (default_pos.X - x) / ep;
                            double speed_y = speed * (default_pos.Y - y) / ep;
                            x += speed_x;
                            y += speed_y;
                            if (texRotate) { angle = Math.Atan2(speed_y, speed_x); }
                        }else { if (alltime == DataBase.motion_inftyTime) times[0] = alltime; }
                        break;
                    case MoveType.stop:
                        //times[0]フレームだけ停止する
                        break;
                    case MoveType.noMotion:
                        times[0] = -1;
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
                    player.damage(3);
                }

                #region bulletのupdate
                for (int i = bullets.Count-1; i >= 0; i--)//update 専用
                {
                    bullets[i].update(player,bulletsMove,skillsUpdate);
                    #region chase enemyのみ
                    if (bullets[i].move_type == MoveType.chase_enemy_target)
                    {
                        bullets[i].x = x;
                        bullets[i].y = y;
                        bullets[i].radian += bullets[i].omega;
                    }
                    #endregion
                }
                for (int i = bullets.Count - 1; i >= 0; i--)//update 専用
                {
                    if (bullets[i].delete)
                    {
                        bullets.Remove(bullets[i]);
                        
                        //Console.Write(i+" ");
                    }
                }
                #endregion
                //Console.WriteLine(bullets.Count);
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
                skills[i].update(skillsUpdate);
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
        public void set_skill_coolDown(string _skillName, int cd,bool absoluteFrame=false)
        {
            foreach (Skill sk in skills)
            {
                if (sk.skillName == _skillName)
                {
                    if (!absoluteFrame)
                        sk.coolDown = cd;
                    else
                        sk.coolDown =(int)(cd * Game1.enemySkills_update_fps / DataBase.basicFramePerSecond);
                }
            }
        }
        /// <summary>
        /// skillsの配列のiインデックスのスキルのクールダウンを設定する
        /// </summary>
        /// <param name="index">インデックスは常に0から始まている</param>
        /// <param name="cd"></param>
        public void set_skill_coolDown(int index, int cd, bool absoluteFrame=false)
        {
            if (index >= skills.Count) { return; }
            else {
                if (!absoluteFrame)
                    skills[index].coolDown = cd;
                else
                    skills[index].coolDown = (int)(cd * Game1.enemySkills_update_fps / DataBase.basicFramePerSecond);
            }
        }
        /// <summary>
        /// 指定したスキルの現在CDを取得する、
        /// </summary>
        /// <param name="index"></param>
        /// <param name="absoluteFrame">true:絶対frame数=skillUpdate_framesで換算したCDを取得する。黙認ではfalse; falseでそのままの数値</param>
        /// <returns></returns>
        public int skillcd(int index, bool absoluteFrame=false) {
            if (index >= skills.Count) { Console.Write("skillcd: Do not have "+index+"th skill."); return 0; }
            else
            {
                if (absoluteFrame)
                    return (int)(skills[index].coolDown * Game1.enemySkills_update_fps / DataBase.basicFramePerSecond);
                else
                    return skills[index].coolDown;
            }
        }
        /// <summary>
        /// 指定したスキルのCDを{すでに経過したframes}を使用して、あるCD後に使われるようにする. 
        /// </summary>
        /// <param name="index">スキルを指定に使う</param>
        /// <param name="maxCD">指定したCD. {今から使われるまでの時間ではない}</param>
        /// <param name="frames_passed">指定したCDに対して、実はこのframe数だけCDは経過した。</param>
        /// <returns></returns>
        public void sync_skill_coolDown(int index, int maxCD, int frames_passed)
        {
            if (index >= skills.Count) { Console.Write("skillcd: Do not have " + index + "th skill."); return; }
            else
            {
                skills[index].coolDown = maxCD - (int)(frames_passed * Game1.enemySkills_update_fps / DataBase.basicFramePerSecond)%maxCD;
            }
        }

        public void delete_all_skills()
        {
            skills.Clear();
            
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
        public virtual void setup_extra_motion(MoveType _mt,PointType _pt,Vector _pos, int _alltime=DataBase.motion_inftyTime)
        {
            backup_Motion_into_1();
            alltime = _alltime;
            times[0] = 0;
            mt = _mt;
            pt = _pt;
            setup_default_pos(_pos);
            set_from_moveTypeAndPointType_2();
        }
        /// <summary>
        /// times[0]は変更されず、motion_index[0],mt,alltime,default_posを設置する
        /// </summary>
        /// <param name="i"></param>
        public virtual void setup_motion(int i,int _time=-2) {
            if (i < unitType.moveTypes.Count) {
                motion_index[0] = i; mt =unitType.moveTypes[i];
                alltime = unitType.times[i];
                pt = unitType.pointTypes[i];
                default_pos = unitType.default_poses[i];
                setup_default_pos(i);
                speed = unitType.speed;
                set_from_moveTypeAndPointType_2();
                if (_time != -2)
                    times[0] = _time;
                //Console.WriteLine(default_pos+" "+mt+" t:"+alltime);
            }
            else { Console.Write("setup_motion:Invaild Motion-" + unitType_name + "- i is" + i); }
        }
        /// <summary>
        /// setup_default_pos()の後にあり、moveTypeなどによって、速度の大きさなどを調整する
        /// </summary>
        protected void set_from_moveTypeAndPointType_2() {
            #region screen_point_target
            if (mt == MoveType.screen_point_target && alltime > 0)
            {
                if (Motion.Is_a_Point(pt))
                {
                    speed = Math.Sqrt((default_pos.X - x) * (default_pos.X - x) + (default_pos.Y - y) * (default_pos.Y - y)) / alltime;
                }
                else if (Motion.Is_a_Direction(pt))
                {
                    speed = Math.Sqrt(default_pos.X * default_pos.X + default_pos.Y * default_pos.Y) / alltime;
                }
            }
            #endregion
            #region go_straight
            else if (mt == MoveType.go_straight)
            {
                #region is a point
                if (Motion.Is_a_Point(pt))
                {
                    if (alltime > 0)
                    {
                        default_pos.X = (default_pos.X - x) / alltime;
                        default_pos.Y = (default_pos.Y - y) / alltime;
                    }
                    else
                    {
                        double distance = Math.Sqrt(Function.distance(default_pos.X, default_pos.Y, x, y));
                        default_pos.X = (default_pos.X - x) * speed / distance;
                        default_pos.Y = (default_pos.Y - y) * speed / distance;
                    }
                }
                #endregion
                #region is a displacement
                else if (Motion.Is_a_Displacement(pt))
                {
                    if (alltime > 0)
                    {
                        default_pos.X /= alltime;
                        default_pos.Y /= alltime;
                    }
                    else
                    {
                        double distance = default_pos.GetLength();
                        default_pos.X = default_pos.X * speed / distance;
                        default_pos.Y = default_pos.Y * speed / distance;
                    }
                }
                #endregion
                // is a direction ではspeedはそのまま
            }
            #endregion
        }
        protected void update_motion_index() {
            if (motion_index[1] >= 0)
            {//例外が設置された。
                if (DataBase.timeExceedMaxDuration(times[0], alltime))
                {//例外のmotionから脱出して、元に戻る。
                    get_Motion_from_1();
                    setup_motion(motion_index[0]);
                    clear_extra_motion_1();
                }
            }
            else
            {
                if (DataBase.timeExceedMaxDuration(times[0], alltime))
                {
                    // motion loops
                    if (motionLooped &&  
                            motion_index[0] == motionLoopsEnd[motionLoopIndex])
                    {
                        motion_index[0] = motionLoopsStart[motionLoopIndex];
                        
                    }
                    // not yet motion looped 
                    else if (motion_index[0] < unitType.moveTypes.Count - 1){
                        motion_index[0]++;
                    }
                    else { motion_index[0] = 0; }
                    times[0] = 0;//times[0]を0にする,下の関数にはこれは実行していない。
                    setup_motion(motion_index[0]);// motion_index[0]に従ってmotionを設置するが、これだけだとtimes[0]が0にならない
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
            default_pos.X = Motion.from_PointType_getPosX(default_pos.X, default_pos.Y, unitType.pointTypes[i],unitType.times[i], speed, angle, unitType.moveTypes[i]);
            default_pos.Y = Motion.from_PointType_getPosY(default_pos.X, default_pos.Y, unitType.pointTypes[i],unitType.times[i], speed, angle, unitType.moveTypes[i]);
        }
        /// <summary>
        /// 現在のPointType, times[0], MoveTypeを使って、default_posを更新する
        /// </summary>
        /// <param name="pos"></param>
        protected void setup_default_pos(Vector pos)
        {
            default_pos.X = Motion.from_PointType_getPosX(pos.X, pos.Y, pt, alltime, speed, angle,mt);
            default_pos.Y = Motion.from_PointType_getPosY(pos.X, pos.Y, pt,alltime, speed, angle,mt);
        }
        public virtual void setup_LoopSet(int _loopStart, int _loopEnd, int _loopIndex = -1)
        {
            if (!motionLooped)
            {
                motionLooped = true;
                motionLoopsEnd = new List<int>();
                motionLoopsStart = new List<int>();
                motionLoopIndex = 0;
            }
            if (_loopIndex >= 0) // index 指定
            {
                if (motionLoopsStart.Count <= _loopIndex)
                {
                    for (int i = 0; i < _loopIndex + 1 - motionLoopsStart.Count; i++)
                        motionLoopsStart.Add(0);
                    Console.Write("motionLoopsStart: 0 added due to outOfIndex");
                }
                if (motionLoopsEnd.Count <= _loopIndex)
                {
                    for (int i = 0; i < _loopIndex + 1 - motionLoopsEnd.Count; i++)
                        motionLoopsEnd.Add(0);
                    Console.Write("motionLoopsEnd: 0 added due to outOfIndex");
                }
                motionLoopsStart[_loopIndex] = _loopStart;
                motionLoopsEnd[_loopIndex] = _loopEnd;

            }
            else
            {
                motionLoopsStart.Add(_loopStart);
                motionLoopsEnd.Add(_loopEnd);
            }
        }
        #endregion

        public void FadeOut()
        {
            label += unitLabel_FadeOut;
            //backup_Motion_into_1();
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
                    
                    if (!skills[i].used(alltime, motionLoopIndex, life, maxLife)) {  continue; }
                    switch (sd.sgl) {
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
                                        by = y - sd.space * j * j*Math.Pow(0.99,j)/2 + animation.Y / 2;
                                        double bdx = sd.space * j*j + sd.radius;
                                        if (ws.sgl == SkillGenreL.UseSkilledBullet)
                                        {
                                            WaySkilledBulletsData wSs = (WaySkilledBulletsData)ws;
                                            bullet1 = new SkilledBullet(x + bdx, by, sd.moveType,
                                                sd.speed, sd.acceleration, sd.aniDName, sd.angle, sd.radius, wSs.BulletSkillNames, this, sd.sword, sd.life, sd.score);
                                            bullet2 = new SkilledBullet(x - bdx, by, sd.moveType,
                                                sd.speed, sd.acceleration, sd.aniDName, sd.angle, sd.radius, wSs.BulletSkillNames, this, sd.sword, sd.life, sd.score);
                                        }
                                        else
                                        {
                                            bullet2 = new Bullet(x - bdx, by, sd.moveType,
                                            sd.speed, sd.acceleration, sd.aniDName, sd.angle, sd.radius, sd.sword, sd.life, sd.score);
                                            bullet1 = new Bullet(x + bdx, by, sd.moveType,
                                                sd.speed, sd.acceleration, sd.aniDName, sd.angle, sd.radius, sd.sword, sd.life, sd.score);
                                        }
                                        double bspeedX = sd.speed * (j - 1) * 0.01;
                                        double bspeedY = -sd.speed;// + 0.01 * (sd.space * j); -by+y+animation.Y/2
                                        bullet1.target_pos.X = bspeedX;
                                        bullet1.target_pos.Y = bspeedY;
                                        bullet1.acceleration_x = +sd.acceleration * j * j / 120;
                                        bullet1.acceleration_y = sd.acceleration;
                                        bullets.Add(bullet1);
                                        bullet2.target_pos.X = -bspeedX;
                                        bullet2.target_pos.Y = bspeedY;
                                        bullet2.acceleration_x = -sd.acceleration * j * j / 120;
                                        bullet2.acceleration_y = sd.acceleration;
                                        bullets.Add(bullet2);
                                    }
                                    #endregion
                                    break;
                                #endregion
                                default:
                                    #region bulletsAdd
                                    double bx = x; by = y;
                                    double _angle = Motion.getAngleFromPointType(ws.pointType, ws.angle, ws.vec.X,x,y,angle);
                                    //Console.WriteLine("in Enemy: " + ws.skillName + " ws.angle:" + ws.angle +" angle:"+_angle+" way:"+ws.way+ "#");

                                    if (ws.way % 2 == 1)
                                    {
                                        bulletsAdd(x, y, _angle, ws);
                                        for (int j = 1; j < (ws.way + 1) / 2; j++)
                                        {
                                            bulletsAdd(bx, by,_angle + j * sd.space,ws);
                                            bulletsAdd(bx, by,_angle - j * sd.space,ws);
                                        }
                                    }else{
                                        for (int j = 0; j < ws.way / 2; j++)
                                        {
                                            bulletsAdd(bx, by, _angle + (j+0.5) * sd.space, ws);
                                            bulletsAdd(bx, by, _angle - (j+0.5) * sd.space, ws);
                                        }
                                    }
                                    if (sd.duration > 0) { for (int kk = 0; kk < ws.way; kk++) { bullets[bullets.Count - 1 - kk].setup_exist_time(ws.duration); } }
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

        public void bulletsAdd(double _x, double _y, double _angle, WayShotSkillData _ws, double __speed=-999, double __acceleration=-999)
        {
            if (bullets.Count > MaximumOfBullets)
            {
                //Console.WriteLine(bullets.Count);
                return;
            }
            double _speed, _acceleration;
            if (__speed == -999 && __acceleration == -999)
            { _speed = _ws.speed; _acceleration = _ws.acceleration; }
            else { _speed = __speed; _acceleration = __acceleration; }
            switch (_ws.sgl)
            {
                #region creat Non-Skilled 
                case SkillGenreL.generation:
                    switch (_ws.sgs)
                    {
                        case SkillGenreS.laser:
                            if (Motion.Has_a_Object(_ws.moveType))
                            {
                                //物体目標のスキル
                                bullets.Add(new LaserTop(_x, _y, _ws.moveType, _speed, _acceleration,Map.player, _ws.aniDName,_angle, _ws.radius, _ws.omega, this, _ws.color, _ws.sword, _ws.life, _ws.score));
                            }else
                            {
                                //点目標のスキル
                                bullets.Add(new LaserTop(_x, _y, _ws.moveType, _speed, _acceleration, _ws.vec,_ws.pointType,_ws.motion_time, _ws.aniDName,_angle, _ws.radius, _ws.omega, this, _ws.color, _ws.sword, _ws.life, _ws.score));
                            }
                            break;
                        default:
                            if (Motion.Has_a_Object(_ws.moveType))
                            {
                                //物体目標のスキル
                                bullets.Add(new Bullet(_x, _y, _ws.moveType, _speed, _acceleration, _ws.aniDName, Map.player, _angle, _ws.omega, _ws.radius, _ws.sword, _ws.life, _ws.score));
                            }
                            else
                            {
                                //点目標のスキル
                                bullets.Add(new Bullet(_x, _y, _ws.moveType, _speed, _acceleration, _ws.aniDName, _ws.vec, _ws.pointType, _ws.motion_time, _angle, _ws.omega, _ws.radius, _ws.sword, _ws.life, _ws.score));
                            }
                            break;
                    }
                    break;
                #endregion
                #region create Skilled
                case SkillGenreL.UseSkilledBullet:
                    WaySkilledBulletsData wSs = (WaySkilledBulletsData)_ws;
                    switch (_ws.sgs)
                    {
                        case SkillGenreS.laser:
                            if (Motion.Has_a_Object(_ws.moveType))
                            {
                                //物体目標のスキル
                                bullets.Add(new SkilledLaserTop(_x, _y, _ws.moveType, _speed, _acceleration, Map.player, _ws.aniDName, _angle, _ws.omega, _ws.radius,wSs.BulletSkillNames, this, _ws.color, _ws.sword, _ws.life, _ws.score));
                            }
                            else
                            {
                                //点目標のスキル
                                bullets.Add(new SkilledLaserTop(_x, _y, _ws.moveType, _speed, _acceleration, _ws.vec, _ws.pointType, _ws.aniDName, _angle, _ws.omega, _ws.radius, _ws.motion_time,wSs.BulletSkillNames, this, _ws.color, _ws.sword, _ws.life, _ws.score));
                            }
                            break;
                        default:
                            if (Motion.Has_a_Object(_ws.moveType))
                            {
                                //物体目標のスキル
                                bullets.Add(new SkilledBullet(_x, _y, _ws.moveType, _speed, _acceleration, _ws.aniDName, Map.player,_ws.motion_time, _angle, _ws.omega, _ws.radius,wSs.BulletSkillNames,this, _ws.sword, _ws.life, _ws.score));
                            }
                            else
                            {
                                //点目標のスキル
                                bullets.Add(new SkilledBullet(_x, _y, _ws.moveType, _speed, _acceleration, _ws.aniDName, _ws.vec, _ws.pointType, _ws.motion_time, _angle, _ws.omega, _ws.radius,wSs.BulletSkillNames ,this,_ws.sword, _ws.life, _ws.score));
                            }
                            break;
                    }
                    break;
                    #endregion
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
            if(!animation.dataIsNull())
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

        public virtual bool selectable()
        {
            if (delete || fadeout||animation.dataIsNull()||!Map.inside_window(this))
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
