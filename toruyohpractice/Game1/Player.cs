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
    class Player:Unit
    {
        #region standard variables
        /// <summary>
        /// キャラクターの速度
        /// </summary>
        public double speed;
        public double speed_x;//後に計算されて代入される
        public double speed_y;

        /// <summary>
        /// (キャラクターの残機*MapでのlifesPerPiece) の値になります。
        /// </summary>
        public int life;
        /// <summary>
        /// 刀エネルギーの初期値
        /// </summary>
        public int sword = 80;
        /// <summary>
        /// かなたエネルギーの最大値
        /// </summary>
        public int sword_max = 160;
        #endregion
        /// <summary>
        /// 通常時の速度
        /// </summary>
        public int default_speed = 6;
        public int atk = 1000; //一度の回避によって切り付けるダメージ。またスキル使用時は3倍になる。
        #region about Animation and tex
        /// <summary>
        /// animationDataAdvancedのnameの前半を使用しています。使う時はaddOnと一緒に.
        /// </summary>
        public string animationDataKey;
        protected AnimationAdvanced ad=null;
        public float texW { get { if (ad == null) { return 0; } else { return ad.X; } }  }
        public float texH{get { if (ad == null) { return 0; } else { return ad.Y; } }}
        #endregion

        #region about skill
        /// <summary>
            /// スキル使用中かのbool
            /// </summary>
        public bool attack_mode = false;
        /// <summary>
        /// 追加攻撃しているかのbool
        /// </summary>
        public bool add_attack_mode = false;
        /// <summary>
        /// ターゲットとするenmey
        /// </summary>
        public Enemy enemyAsTarget;

        /// <summary>
        /// 攻撃スキルを使って敵を切った後そこにとどまる時間
        /// </summary>
        public int skill_stop_time = 14;
        public int skill_max_attckStandby = 15, skill_attackStandby=0;
        public int skill_speed = 15;
        /// <summary>
        /// 1回目のskill消費
        /// </summary>
        public int shouhi_sword { get { return sword_max / 4; } }
        /// <summary>
        /// 2回目以降のskill消費
        /// </summary>
        public int nextSwordSkill_Cost { get { return sword_max / 8; } }
        /// <summary>
        /// skillを使うに最低限のエネルギー
        /// </summary>
        public int sword_condition { get { return sword_max / 2; } }
        /// <summary>
        /// 敵のどれくらいしたまで移動するか
        /// </summary>
        public double enemy_below = 50;
        /// <summary>
        /// 刀エネルギーが最大になっている時
        /// </summary>
        public int bonusDamage = 500; 
        public int swordSkillDamage { get { return atk + 1500 * (2*sword-sword_max )/sword_max; } }
        //protected bool skill_attackBoss
        #endregion
        const int prosToBoss_dash_maximum=40;
        const int prosToBoss_slash_maximum = 18;
        protected int nowProsIndex = -1;
        protected double radianForPros=0;
        /// <summary>
        /// skillToBossに使われる切り付けのエフェクト。
        /// </summary>
        private Projection[] prosToBoss = new Projection[prosToBoss_dash_maximum+prosToBoss_slash_maximum];
        #region about evasion
        public bool avoid_mode = false;
        public bool avoid_InPlusAcceleration = true;
        //大体のフレーム数は (avoid_speed-default_speed)/avoid_acceleration *2 + avoid_stop_time
        public double avoid_speed = 10;
        public double avoid_acceleration = 1.0;
        /// <summary>
        /// 回避後静止する時間
        /// </summary>
        public int avoid_stop_time = 22;
        private SoundEffectID avoid_SEid =SoundEffectID.playerattack1;
        /// <summary>
        /// 回避時に敵弾を消せる半円の半径
        /// </summary>
        public int avoid_radius = 110;

        #endregion

        /// <summary>
        /// if this is over 0, the character cannot move/avoid
        /// </summary>
        public int stop_time;

        /// <summary>
        /// ダメージ受けてから無敵になる時間
        /// </summary>
        public int default_muteki_time = 90;
        /// <summary>
        /// ダメージを受けたか/強制移動中なのか
        /// </summary>
        public bool InForcedRoute = false;
        /// <summary>
        /// 無敵になっている時間を記録
        /// </summary>
        public int muteki_time=0;

        /// <summary>
        /// skillの1回目の使用である
        /// </summary>
        bool first = true;

        /// <summary>
        /// skill使用後の時間を記録
        /// </summary>
        public int attack_time = 0;

        // ###--  variables above  --------

//###----------------------------- Functions below    
        /// <summary>
        /// characterのコンストラクター
        /// </summary>
        /// <param name="_x"></param>
        /// <param name="_y"></param>
        /// <param name="_speed"></param>
        /// <param name="_radius"></param>
        /// <param name="_life">残機数*MapでのlifesPerPiece</param>
        /// <param name="a_n">これはAnimationDataAdvancedの名前の前半部分である。</param>
        public Player(double _x,double _y,double _speed,double _radius,int _life,string a_n):base(_x,_y)
        {
            speed = _speed;
            radius = _radius;
            life = _life;
            animationDataKey = a_n;
            
            ad = new AnimationAdvanced(DataBase.getAniD(animationDataKey, DataBase.defaultAnimationNameAddOn));
        }

        public void update(InputManager keymanager,Map map)
        {
            ad.Update();
            if (InForcedRoute == false)
            { // 強制移動中ではない。
                if (muteki_time > 0)  muteki_time--; //無敵時間を減らす。
                if (stop_time > 0) { stop_time--; }//硬直している ので硬直時間を減らす
                else{
                    #region move
                    if (attack_mode == false)
                    {
                        if (keymanager.IsKeyDown(KeyID.Up)) { y = y - speed; }
                        if (keymanager.IsKeyDown(KeyID.Down)) { y = y + speed; }
                        if (keymanager.IsKeyDown(KeyID.Right)) { x = x + speed; }
                        if (keymanager.IsKeyDown(KeyID.Left)) { x = x - speed; }
                        if (avoid_mode == false)
                        {//回避中に速度が小さくならないように
                            if (keymanager.IsKeyDown(KeyID.Slow) == true) { speed = default_speed/2; } else { speed = default_speed; }//テスト用数値
                        }
                    }
                    #endregion
                    if (Map.boss_mode && Map.step[0]%60==0) {
                        sword +=3;
                    }
                    //回避を使っているか
                    avoid(keymanager);
                }//if 硬直しているか end

                //スキルを使っているか
                #region use skill
                if (avoid_mode == false)
                {
                    if (Map.boss_mode)
                    {
                        skilltoBoss(keymanager);
                    }
                    else
                    {
                        skilltoEnemy(keymanager);
                    }
                }
                #endregion
            }
            else { // 強制移動,スクリーン上中央、y=500へ向かう
                #region In Forced Route. 
                double e = Math.Sqrt(Function.distance(x, y, DataBase.WindowDefaultSizeX / 2, 500));
                if (e > 0.1 || e<-0.1)
                {
                    speed_x = (DataBase.WindowDefaultSizeX / 2-x) * 2*speed / e;
                    speed_y = (500-y) * 2*speed / e;
                    x += speed_x;
                    y += speed_y;
                }
                if (Function.hitcircle(x, y, 0, DataBase.WindowDefaultSizeX / 2, 500, speed))
                {//強制移動終了
                    InForcedRoute = false;
                    muteki_time = default_muteki_time;
                }
                #endregion
            }
            #region Limit the character inside the window
            double percent = 0;
            if (x < Map.leftside+percent*texW/2) { x = Map.leftside+percent*texW / 2; }
            if (x > Map.rightside-percent*texW / 2) { x = Map.rightside-percent* texW / 2; }
            if (y > DataBase.WindowSlimSizeY- percent*texH / 2) { y = DataBase.WindowSlimSizeY- percent * texH / 2; }
            if (y < 0+ percent*texH / 2) { y = 0+ percent * texH / 2; }
            #endregion
            #region Limit Sword energy out of range
            if (sword > sword_max) { sword = sword_max; }
            if (sword < 0) { sword = 0; }
            #endregion
        }

        /// <summary>
        /// find the first selectable enemy in enemys_inside_window from 0 to .Count-1
        /// </summary>
        protected void search_OldestEnemy()
        {
            enemyAsTarget = null;
            if (Map.enemys_inside_window.Count > 0)
            {
                int j = 0;
                for (int i = 0; i < Map.enemys_inside_window.Count; i++)
                {
                    if (Map.enemys_inside_window[i].selectable() == true)
                    {
                        enemyAsTarget = Map.enemys_inside_window[i];
                        j = i;
                        break;
                    }
                }
            }
            if (enemyAsTarget != null)
            {
                double e2 = Math.Sqrt(Function.distance(x, y, enemyAsTarget.x, enemyAsTarget.y + enemy_below));
                 radianForPros=Math.Atan2( -(y - enemyAsTarget.y) / e2,-(x - enemyAsTarget.x) / e2);
            }
        }
        protected void search_boss()
        {
            enemyAsTarget = null;
            if (Map.enemys_inside_window.Count > 0)
            {
                int j = 0;
                for (int i = 0; i < Map.enemys_inside_window.Count; i++)
                {
                    if (Map.enemys_inside_window[i].label.Contains("boss")&&
                        Map.enemys_inside_window[i].selectable() == true)
                    {
                        enemyAsTarget = Map.enemys_inside_window[i];
                        j = i;
                        break;
                    }
                }

                for (int i = j+1; i < Map.enemys_inside_window.Count; i++)
                {
                    if (Map.enemys_inside_window[i].label.Contains("boss") &&
                        Map.enemys_inside_window[i].selectable() == true
                        && Function.distance(x, y, Map.enemys_inside_window[i].x, Map.enemys_inside_window[i].y) < Function.distance(x, y, enemyAsTarget.x, enemyAsTarget.y))
                    {
                        enemyAsTarget = Map.enemys_inside_window[i];
                    }
                }
            }
            if (enemyAsTarget != null)
            {
                double e2 = Math.Sqrt(Function.distance(x, y, enemyAsTarget.x, enemyAsTarget.y + enemy_below));
                radianForPros = Math.Atan2(-(y - enemyAsTarget.y) / e2, -(x - enemyAsTarget.x) / e2);
            }
        }

        #region characterAttackskills
        #region skillToEnemy
        /// <summary>
        /// 敵を見つけて、attack_mode=true,add_attack_mode=false
        /// </summary>
        /// <param name="map"></param>
        public void cast_skilltoEnemy()
        {
            search_OldestEnemy();
            if (enemyAsTarget != null)
            {
                attack_mode = true;
                add_attack_mode = false;
                skill_attackStandby = -1;
                playAnimation(DataBase.defaultAnimationNameAddOn);
            }
        }
        public void skilltoEnemy(InputManager input)
        {
            //skill開始からstop時間が終わるまで追加攻撃を予約したら、追加攻撃するが
            //stopが終わりそうでもadd_attack_mode==falseなら、InForcedRouteにして、マップの中心に移る
            if (attack_mode == true)
            {
                #region 使用中である. 敵に向かう、切る、stop_timeが切れるまでが含まれる。
                #region 敵目標関連 stop_time<=0
                if (stop_time <= 0)
                {
                    #region 敵が消えていたら、もう一回探してみる
                    if (enemyAsTarget == null || enemyAsTarget.selectable() == false)
                    {//敵がskill中にfadeout,もしくは消えた時に,もう一度見つける
                        search_OldestEnemy();
                    }
                    #endregion
                    if (enemyAsTarget != null && enemyAsTarget.selectable())
                    {
                        #region approaching the EnemyTarget
                        if (!Function.hitcircle(x, y, skill_speed / 2, enemyAsTarget.x, enemyAsTarget.y + enemy_below, enemyAsTarget.radius))
                        {
                            double e = Math.Sqrt(Function.distance(x, y, enemyAsTarget.x, enemyAsTarget.y + enemy_below));
                            double v = skill_speed / e;
                            x -= (x - enemyAsTarget.x) * v;
                            y -= (y - enemyAsTarget.y - enemy_below) * v;
                        }
                        #endregion
                        #region reached Enemy and attack
                        if (skill_attackStandby < 0)
                        {
                            if (Function.hitcircle(x, y, skill_speed / 2, enemyAsTarget.x, enemyAsTarget.y + enemy_below, enemyAsTarget.radius))
                            {
                                enemyAsTarget.stop_time = skill_max_attckStandby;
                                skill_attackStandby = skill_max_attckStandby;
                                playAnimation(DataBase.aniNameAddOn_spell);
                            }
                        }
                        else if (skill_attackStandby > 0) { skill_attackStandby--; }
                        else
                        {
                            enemyAsTarget.damage(swordSkillDamage);
                            enemyAsTarget = null;
                            if (first) { sword -= shouhi_sword; first = false; }
                            else sword -= nextSwordSkill_Cost; //2回目の使用である
                            stop_time = skill_stop_time + 2;
                        }
                        #endregion
                    }// if 目標は存在 then end
                    else
                    {//目標が消えた.強制終了
                        skilltoEnemyEnd();
                    }
                }//スキルで敵に向かっている途中である
                #endregion
                //追加攻撃を予約した
                if (input.IsKeyDownOnce(KeyID.Select) == true) add_attack_mode = true;
                if (stop_time > 0 && stop_time <= 2 && add_attack_mode)
                {//追加攻撃を実行する
                    if (canUseSkilltoEnemyForSecond())
                    {
                        cast_skilltoEnemy();
                        stop_time = 0;
                    }
                    else { add_attack_mode = false; }
                }
                if (stop_time > 0 && stop_time <= 2 && !add_attack_mode)
                {
                    skilltoEnemyEnd();
                    stop_time--;
                }
                #endregion
            }
            if (Map.enemys_inside_window.Count >= 1 && canUseSkilltoEnemyForFirst())
            {//1回目のskill使用可能な状態である
                if (input.IsKeyDownOnce(KeyID.Select) == true)
                {
                    cast_skilltoEnemy();
                }
            }
        }

        protected void skilltoEnemyEnd()
        {
            first = false;
            playAnimation(DataBase.defaultAnimationNameAddOn);
            InForcedRoute = true;
            attack_mode = false;
            add_attack_mode = false;
            enemyAsTarget = null;
            Map.numOfskillKilledEnemies = 0;
            Map.scoreOfskilltoEnemy = 0;
        }
        #endregion

        protected void skilltoBossEnd()
        {
            playAnimation(DataBase.defaultAnimationNameAddOn);
            InForcedRoute = true;
            attack_mode = false;
            add_attack_mode = false;
            enemyAsTarget = null;
            Map.numOfskillKilledEnemies = 0;
            Map.scoreOfskilltoEnemy = 0;
            for(int i = 0; i < prosToBoss.Length; i++) { prosToBoss[i] = null; }
            nowProsIndex = -1;
        }
        
        public void cast_skilltoBoss()
        {
            search_boss();
            if (enemyAsTarget != null && enemyAsTarget.selectable())
            {
                attack_mode = true;
                attack_time = 120;
                nowProsIndex = -1;
                skill_attackStandby = -1;
                Map.CutInTexture(DataBase.charaCutInTexName, -400, 100, 100, 100, 60, 10);
                Map.stopUpdating(50, 0);
            }
        }

        public void skilltoBoss(InputManager input)
        {
            #region skill is active
            if (attack_mode == true)
            {
                #region enemyAsTarget not null
                if (enemyAsTarget != null)
                {// target exists
                    if (enemyAsTarget.selectable() == false)
                    {//yet cannot be attacked. 
                        skilltoBossEnd();
                    }
                    else
                    {
                        #region projections 更新
                        if (nowProsIndex < prosToBoss_dash_maximum)
                        {
                            for (int j = 0; j <= nowProsIndex; j++)
                            {
                                prosToBoss[j].update();
                            }
                            nowProsIndex++;
                            prosToBoss[nowProsIndex] = new Projection(x, y, MoveType.noMotion, "swordSkilltoBossDash");
                            prosToBoss[nowProsIndex].texRotate = true;
                            prosToBoss[nowProsIndex].radian = radianForPros;
                        }
                        for (int i = 0; i < nowProsIndex; i++)
                        {
                            int nex = i + 1;
                            //Console.WriteLine(i+":"+Math.Sqrt(Function.distance(prosToBoss[nex].x, prosToBoss[nex].y, prosToBoss[i].x, prosToBoss[i].y)));
                            double s = Math.Sqrt(Function.distance(prosToBoss[i + 1].x, prosToBoss[nex].y, prosToBoss[i].x, prosToBoss[i].y)) - (prosToBoss[nex].animation.Y + prosToBoss[i].animation.Y )/2+2 ;
                            if (s > 0.01)
                            {
                                double e = Math.Sqrt(Function.distance(prosToBoss[i].x, prosToBoss[i].y, prosToBoss[nex].x, prosToBoss[nex].y));
                                double speed_x = (prosToBoss[nex].x - prosToBoss[i].x) * s / e;
                                double speed_y = (prosToBoss[nex].y - prosToBoss[i].y) * s / e;
                                prosToBoss[i].x += speed_x;
                                prosToBoss[i].y += speed_y;
                                //prosToBoss[i].radian = Math.Atan2(speed_y, speed_x);
                            }
                        }
                        int idF = nowProsIndex;
                        double s1 = Math.Sqrt(Function.distance(x, y, prosToBoss[idF].x, prosToBoss[idF].y)) - (prosToBoss[idF].animation.Y + prosToBoss[idF].animation.Y)/2+1  ;
                        if (s1 > 0.01)
                        {
                            double e = Math.Sqrt(Function.distance(prosToBoss[idF].x, prosToBoss[idF].y, x, y));
                            double speed_x = (x-enemy_below - prosToBoss[idF].x) * s1 / e;
                            double speed_y = (y-enemy_below - prosToBoss[idF].y) * s1 / e;
                            prosToBoss[idF].x += speed_x;
                            prosToBoss[idF].y += speed_y;
                            //prosToBoss[idF].radian = Math.Atan2(speed_y, speed_x);
                        }
                        #endregion
                        //----------above  prosToBoss[];        ----------------------------##  ##   ##
                        //----                             --------------------------------------###
                        #region move to boss
                        if (!Function.hitcircle(x, y, skill_speed / 2, enemyAsTarget.x, enemyAsTarget.y + enemy_below, enemyAsTarget.radius))
                        {
                            double e2 = Math.Sqrt(Function.distance(x, y, enemyAsTarget.x, enemyAsTarget.y + enemy_below));
                            double skill_speed = 15;
                            double v = skill_speed / e2;
                            x -= (x - enemyAsTarget.x) * v;
                            y -= (y - enemyAsTarget.y - enemy_below) * v;
                        }
                        #endregion
                        if (skill_attackStandby < 0)
                        {
                            if (Function.hitcircle(x, y, skill_speed /2 +1, enemyAsTarget.x, enemyAsTarget.y + enemy_below, enemyAsTarget.radius))
                            {
                                enemyAsTarget.stop_time = skill_max_attckStandby;
                                skill_attackStandby = 18;
                                playAnimation(DataBase.aniNameAddOn_spell);
                            }
                        }
                        else if (skill_attackStandby > 0) {
                            skill_attackStandby--;
                        }
                        else
                        {
                            enemyAsTarget.damage(swordSkillDamage);
                            if (sword == sword_max)
                            {
                                //SoundManager.PlaySE(SoundEffectID.player100gauge);
                                enemyAsTarget.damage(bonusDamage);
                            }else { //SoundManager.PlaySE(SoundEffectID.player50gauge); 
                            }
                            sword = 0;
                            enemyAsTarget = null;
                            stop_time = skill_stop_time + 2;
                        }
                    }
                }else { skilltoBossEnd(); }
                #endregion
                if (stop_time > 0 && stop_time <= 2)
                {
                    skilltoBossEnd();
                    stop_time--;
                }
            }
            #endregion
            #region try to cast a skill
            if (canUseSkilltoBoss())
            {
                if (input.IsKeyDown(KeyID.Select) == true)
                {
                    cast_skilltoBoss();
                }

            }
            #endregion
        }
        #endregion

        public void avoid(InputManager input)
        {
            #region cast evasion
            if (canEvade() && EvasionKeySetPressed(input)) //回避中でなく、かつ回避キーセットを押した。
            { //回避が確定しいる。
                avoid_mode = true;
                avoid_InPlusAcceleration = true;
                speed = 0;
                //SoundManager.PlaySE(avoid_SEid);

                #region old evasion
                /*
                #region　上下左右の回避によって、ダメージを受けるものたち
                for (int i = 0; i < Map.enemys_inside_window.Count; i++)
                {
                    #region its bullets
                    for (int j = 0; j < Map.enemys_inside_window[i].bullets.Count; j++)
                    {
                        if (Map.enemys_inside_window[i].bullets[j].hit_jugde(x, y, avoid_radius) &&
                         // すでに当たることが分かったものたちに対して、上下左右どっちに回避したかによって、消す。
                         ((input.IsKeyDown(KeyID.Up) == true && Map.enemys_inside_window[i].bullets[j].y <= y) ||
                               (input.IsKeyDown(KeyID.Down) == true && Map.enemys_inside_window[i].bullets[j].y >= y) ||
                               (input.IsKeyDown(KeyID.Left) == true && Map.enemys_inside_window[i].bullets[j].x <= x) ||
                               (input.IsKeyDown(KeyID.Right) == true && Map.enemys_inside_window[i].bullets[j].x >= x))
                        )
                        {
                            Map.enemys_inside_window[i].bullets[j].damage(atk);
                        }
                    }
                    #endregion
                    #region its bodys and bodys' bullets
                    if (Map.enemys_inside_window[i].label.Contains("boss"))
                    {
                        for(int j = 0; j < ((Boss)(Map.enemys_inside_window[i])).bodys.Length; j++)
                        {
                            for(int k=0; k < ((Boss)(Map.enemys_inside_window[i])).bodys[j].bullets.Count; k++)
                            {
                                if (((Boss)Map.enemys_inside_window[i]).bodys[j].bullets[k].hit_jugde(x, y, avoid_radius) &&
                                // すでに当たることが分かったものたちに対して、上下左右どっちに回避したかによって、消す。
                                ((input.IsKeyDown(KeyID.Up) == true && ((Boss)Map.enemys_inside_window[i]).bodys[j].bullets[k].y <= y) ||
                               (input.IsKeyDown(KeyID.Down) == true && ((Boss)Map.enemys_inside_window[i]).bodys[j].bullets[k].y >= y) ||
                               (input.IsKeyDown(KeyID.Left) == true && ((Boss)Map.enemys_inside_window[i]).bodys[j].bullets[k].x <= x) ||
                               (input.IsKeyDown(KeyID.Right) == true && ((Boss)Map.enemys_inside_window[i]).bodys[j].bullets[k].x >= x))
                                )
                                {
                                    ((Boss)Map.enemys_inside_window[i]).bodys[j].bullets[k].damage(atk);
                                }
                            }
                            if (((Boss)Map.enemys_inside_window[i]).bodys[j].hit_jugde(x, y, avoid_radius) &&
                            // すでに当たることが分かったものたちに対して、上下左右どっちに回避したかによって、消す。
                            ((input.IsKeyDown(KeyID.Up) == true && ((Boss)Map.enemys_inside_window[i]).bodys[j].y <= y) ||
                               (input.IsKeyDown(KeyID.Down) == true && ((Boss)Map.enemys_inside_window[i]).bodys[j].y >= y) ||
                               (input.IsKeyDown(KeyID.Left) == true && ((Boss)Map.enemys_inside_window[i]).bodys[j].x <= x) ||
                               (input.IsKeyDown(KeyID.Right) == true && ((Boss)Map.enemys_inside_window[i]).bodys[j].x >= x))
                        )
                            {
                                Map.enemys_inside_window[i].damage(atk/4);
                            }
                        }
                    }
                    #endregion
                    if (Map.enemys_inside_window[i].hit_jugde(x, y, avoid_radius) &&
                         // すでに当たることが分かったものたちに対して、上下左右どっちに回避したかによって、消す。
                         ((input.IsKeyDown(KeyID.Up) == true && Map.enemys_inside_window[i].y <= y) ||
                               (input.IsKeyDown(KeyID.Down) == true && Map.enemys_inside_window[i].y >= y) ||
                               (input.IsKeyDown(KeyID.Left) == true && Map.enemys_inside_window[i].x <= x) ||
                               (input.IsKeyDown(KeyID.Right) == true && Map.enemys_inside_window[i].x >= x))
                        )
                    {
                        Map.enemys_inside_window[i].damage(atk);
                    }
                }
                #endregion
                */
                #endregion
                Map.clearBullets(avoid_radius,avoid_stop_time,true,avoid_radius/2);
                if(input.IsKeyDown(KeyID.Up) || input.IsKeyDown(KeyID.Right) ) { playAnimation(DataBase.aniNameAddOn_evadeR); }
                else if (input.IsKeyDown(KeyID.Down) || input.IsKeyDown(KeyID.Left)) { playAnimation(DataBase.aniNameAddOn_evadeL); }
            }
            #endregion
            #region evasion motion
            if (avoid_mode == true)
            {
                if (avoid_InPlusAcceleration==true)
                {
                    speed +=avoid_acceleration ;
                    if (speed >= avoid_speed)
                    {
                        avoid_InPlusAcceleration = false;
                    }
                }
                else
                {
                    if (speed > default_speed)
                    {
                        speed -= (avoid_acceleration/2);
                    }
                    if (speed <= default_speed)
                    {
                        speed = default_speed;
                        avoid_InPlusAcceleration = true;
                        avoid_mode = false;
                        stop_time = avoid_stop_time;
                    }
                }
            }
            #endregion
        }// avoid end
        public void addEnergy(int value)
        {
            sword += value;
            if (sword < 0) sword = 0;
            else if (sword > sword_max) sword = sword_max;
        }
        public void damage(int atk)
        {
            if (life>0 && !Invincible()) 
            {
                SoundManager.PlaySE(SoundEffectID.playerdamage);
                life -= atk;
                InForcedRoute = true;
                Map.clearBullets(avoid_radius*4);
            }
            if (life>-5 && life <= 0) { life = -6; Map.game_over_start(); }
        }
        public override void draw(Drawing d)
        {
            for (int j = nowProsIndex; j >= 0; j--)
            { 
                prosToBoss[j].draw(d);
            }
            if (ad != null)
            {
                ad.Draw(d, new Vector(x - texW / 2, y - texH / 2), DepthID.Player);
            }
        }

        public void playAnimation(string addOn)
        {
            ad = new AnimationAdvanced(DataBase.getAniD(animationDataKey, addOn));
        }
        #region bool 各種状態を返す
        public bool Invincible() { return true||InForcedRoute || muteki_time > 0 || attack_mode || avoid_mode; }
        public bool canUseSkilltoBoss()
        {
            return stop_time <= 0 && !attack_mode && sword >= sword_condition;
        }
        public bool canUseSkilltoEnemyForFirst()
        {
            return stop_time<=0 && !attack_mode  && sword >= sword_condition ; 
        }
        public bool canUseSkilltoEnemyForSecond()
        {
            return  attack_mode && add_attack_mode && sword >= nextSwordSkill_Cost;
        }
        public bool canEvade() { return !InForcedRoute && !attack_mode && stop_time <= 0 && !avoid_mode; }
        protected bool AnyArrowKeyDown(InputManager input)
        {
            return input.IsKeyDown(KeyID.Up)  || input.IsKeyDown(KeyID.Down) || 
                 input.IsKeyDown(KeyID.Right) || input.IsKeyDown(KeyID.Left);
        }
        protected bool EvasionKeySetPressed(InputManager input) {
            return ( input.IsKeyDownOld(KeyID.Cancel) || input.IsKeyDown(KeyID.Cancel) ) && AnyArrowKeyDown(input);
        }

        public bool isAlive() { return life > 0; }
        #endregion
    }
}
