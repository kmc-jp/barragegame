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
        #region standard variables
        /// <summary>
        /// キャラクターのスクリーン上の座標
        /// </summary>
        public double x, y;
        /// <summary>
        /// キャラクターの速度
        /// </summary>
        public double speed;
        public double speed_x;//後に計算されて代入される
        public double speed_y;
        /// <summary>
        /// 当たり判定用の半径
        /// </summary>
        public double radius;
        /// <summary>
        /// (キャラクターの残機*MapでのlifesPerPiece) の値になります。
        /// </summary>
        public int life;
        /// <summary>
        /// 刀エネルギーの初期値
        /// </summary>
        public int sword = 50;
        /// <summary>
        /// かなたエネルギーの最大値
        /// </summary>
        public int sword_max = 100;
        #endregion
        public int atk = 500; //一度の回避によって切り付けるダメージ。またスキル使用時は3倍になる。
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
        public int skill_stop_time = 6;
        public int skill_speed = 15;
        /// <summary>
        /// 1回目のskill消費
        /// </summary>
        public int shouhi_sword = 20;
        /// <summary>
        /// 2回目以降のskill消費
        /// </summary>
        public int nextSwordSkill_Cost = 10;
        /// <summary>
        /// skillを使うに最低限のエネルギー
        /// </summary>
        public int sword_condition = 20;
        public int default_speed = 6;
        /// <summary>
        /// 敵のどれくらいしたまで移動するか
        /// </summary>
        public double enemy_below = 50;
        /// <summary>
        /// 刀エネルギーが最大になっている時
        /// </summary>
        public int bonusDamage = 1000; 
        public int swordSkillDamage { get { return atk*3 + 30 * (sword - 50); } }
        //protected bool skill_attackBoss
        #endregion

        #region about evasion
        public bool avoid_mode = false;
        public bool avoid_InPlusAcceleration = true;
        public int default_avoid_time = 6;
        public int avoid_speed = 13;
        public int avoid_acceleration = 3;
        public int avoid_stop_time = 20;
        /// <summary>
        /// 回避時に敵弾を消せる半円の半径
        /// </summary>
        public int avoid_radius = 100;

        #endregion

        /// <summary>
        /// if this is over 0, the character cannot move/avoid
        /// </summary>
        public int stop_time;

        /// <summary>
        /// ダメージ受けてから無敵になる時間
        /// </summary>
        public int default_muteki_time = 30;
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
        public Player(double _x,double _y,double _speed,double _radius,int _life,string a_n)
        {
            x = _x;y = _y;
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
                        if (keymanager.IsKeyDown(KeyID.Up) == true) { y = y - speed; }
                        if (keymanager.IsKeyDown(KeyID.Down) == true) { y = y + speed; }
                        if (keymanager.IsKeyDown(KeyID.Right) == true) { x = x + speed; }
                        if (keymanager.IsKeyDown(KeyID.Left) == true) { x = x - speed; }
                        if (avoid_mode == false)
                        {//回避中に速度が小さくならないように
                            if (keymanager.IsKeyDown(KeyID.Slow) == true) { speed = 2; } else { speed = default_speed; }//テスト用数値
                        }
                    }
                    #endregion
                    //回避を使っているか
                    avoid(keymanager, map);
                }//if 硬直しているか end

                //スキルを使っているか
                #region use skill
                if (avoid_mode == false)
                {
                    if (Map.boss_mode)
                    {
                        skilltoBoss(keymanager, map);
                    }
                    else
                    {
                        skilltoEnemy(keymanager, map);
                    }
                }
                #endregion
            }
            else { // 強制移動中である
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
            if (sword >= sword_max) { sword = sword_max; }
            if (sword <= 0) { sword = 0; }
            #endregion
        }

        public void search_enemy(Map map)
        {
            enemyAsTarget = null;
            if (map.enemys_inside_window.Count > 0)
            {
                int j = 0;
                for (int i = 0; i < map.enemys_inside_window.Count; i++)
                {
                    if (map.enemys_inside_window[i].selectable() == true)
                    {
                        enemyAsTarget = map.enemys_inside_window[i];
                        j = i;
                        break;
                    }
                }

                for (int i = j; i < map.enemys_inside_window.Count; i++)
                {
                    if (map.enemys_inside_window[i].selectable()==true
                        && Function.distance(x, y, map.enemys_inside_window[i].x, map.enemys_inside_window[i].y) < Function.distance(x, y, enemyAsTarget.x, enemyAsTarget.y))
                    {
                        enemyAsTarget = map.enemys_inside_window[i];
                    }
                }
            }
        }
        #region characterAttackskills
        /// <summary>
        /// 敵を見つけて、attack_mode=true,add_attack_mode=false
        /// </summary>
        /// <param name="map"></param>
        public void cast_skilltoEnemy(Map map)
        {
            search_enemy(map);
            attack_mode = true;
            add_attack_mode = false;
        }
        protected void skilltoEnemyEnd()
        {
            InForcedRoute = true;
            attack_mode = false;
            add_attack_mode = false;
            enemyAsTarget = null;
            Map.numOfskillKilledEnemies = 0;
            Map.scoreOfskilltoEnemy = 0;
        }
        public void skilltoEnemy(InputManager input,Map map)
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
                        search_enemy(map);
                    }
                    #endregion
                    if (enemyAsTarget != null && enemyAsTarget.selectable())
                    {
                        #region approaching the EnemyTarget
                        double e = Math.Sqrt(Function.distance(x, y, enemyAsTarget.x, enemyAsTarget.y + enemy_below));
                        double v = skill_speed / e;
                        x -= (x - enemyAsTarget.x) * v;
                        y -= (y - enemyAsTarget.y - enemy_below) * v;
                        #endregion
                        #region reached Enemy and attack
                        if (Function.hitcircle(x, y, skill_speed / 2, enemyAsTarget.x, enemyAsTarget.y + enemy_below, enemyAsTarget.radius))
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
                if (stop_time>0 &&stop_time <= 2 && add_attack_mode)
                {//追加攻撃を実行する
                    if (canUseSkilltoEnemyForSecond())
                    {
                        cast_skilltoEnemy(map);
                        stop_time = 0;
                    }
                    else { add_attack_mode = false;}
                }
                if (stop_time > 0 && stop_time <= 2 && !add_attack_mode) {
                    skilltoEnemyEnd();
                    stop_time--;
                }
                #endregion
            }
            if (map.enemys_inside_window.Count>=1 && canUseSkilltoEnemyForFirst())
            {//1回目のskill使用可能な状態である
                if (input.IsKeyDownOnce(KeyID.Select) == true)
                {
                    cast_skilltoEnemy(map);
                }
            }
        }

        public void cast_skilltoBoss(Map map)
        {
            search_enemy(map);
            attack_mode = true;
            attack_time = 120;
            shouhi_sword = sword;
        }

        public void skilltoBoss(InputManager input,Map map)
        {
            if (attack_mode == false)
            {
                if (input.IsKeyDown(KeyID.Select) == true && sword >= 0)
                {
                    cast_skilltoBoss(map);
                }
                
            }

            if (attack_mode == true)
            {
                if (enemyAsTarget != null)
                {
                    if (enemyAsTarget.selectable() == false)
                    {
                        search_enemy(map);
                    }
                    else
                    {
                        double e = Math.Sqrt(Function.distance(x, y, enemyAsTarget.x, enemyAsTarget.y + enemy_below));
                        double skill_speed = 15;
                        double v = skill_speed / e;
                        x -= (x - enemyAsTarget.x) * v;
                        y -= (y - enemyAsTarget.y - enemy_below) * v;

                        if (Function.hitcircle(x, y, 2, enemyAsTarget.x, enemyAsTarget.y + enemy_below, 6))
                        {
                            sword -= shouhi_sword;
                            shouhi_sword = 100000;//テスト用
                            enemyAsTarget.damage(swordSkillDamage);
                            if (shouhi_sword == sword_max)
                            {
                                enemyAsTarget.damage(bonusDamage);
                            }
                            enemyAsTarget = null;
                        }
                    }
                }

                if (enemyAsTarget==null)
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
        #endregion

        public void avoid(InputManager input,Map map)
        {
            #region cast evasion
            if (canEvade() && EvasionKeySetPressed(input)) //回避中でなく、かつ回避キーセットを押した。
            { //回避が確定しいる。
                avoid_mode = true;
                avoid_InPlusAcceleration = true;
                speed = 0;
                #region　上下左右の回避によって、ダメージを受けるものたち
                for (int i = 0; i < map.enemys_inside_window.Count; i++)
                {
                    for (int j = 0; j < map.enemys_inside_window[i].bullets.Count; j++)
                    {
                        if (map.enemys_inside_window[i].bullets[j].hit_jugde(x, y, avoid_radius) &&
                         // すでに当たることが分かったものたちに対して、上下左右どっちに回避したかによって、消す。
                         ((input.IsKeyDown(KeyID.Up) == true && map.enemys_inside_window[i].bullets[j].y <= y) ||
                               (input.IsKeyDown(KeyID.Down) == true && map.enemys_inside_window[i].bullets[j].y >= y) ||
                               (input.IsKeyDown(KeyID.Left) == true && map.enemys_inside_window[i].bullets[j].x <= x) ||
                               (input.IsKeyDown(KeyID.Right) == true && map.enemys_inside_window[i].bullets[j].x >= x))
                        )
                        {
                            /*Map.make_chargePro(map.enemys_inside_window[i].bullets[j].x, map.enemys_inside_window[i].bullets[j].y,
                                map.enemys_inside_window[i].bullets[j].sword, map.enemys_inside_window[i].bullets[j].score);
                            map.score += map.enemys_inside_window[i].bullets[j].score;*/
                            map.enemys_inside_window[i].bullets[j].damage(atk);
                        }
                    }

                    if (map.enemys_inside_window[i].hit_jugde(x, y, avoid_radius) &&
                         // すでに当たることが分かったものたちに対して、上下左右どっちに回避したかによって、消す。
                         ((input.IsKeyDown(KeyID.Up) == true && map.enemys_inside_window[i].y <= y) ||
                               (input.IsKeyDown(KeyID.Down) == true && map.enemys_inside_window[i].y >= y) ||
                               (input.IsKeyDown(KeyID.Left) == true && map.enemys_inside_window[i].x <= x) ||
                               (input.IsKeyDown(KeyID.Right) == true && map.enemys_inside_window[i].x >= x))
                        )
                    {
                        map.enemys_inside_window[i].damage(atk);
                    }
                }
                #endregion
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
                        speed -= (avoid_acceleration/2+1);
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

        public void damage(int atk)
        {
            if (!Invincible()) 
            {
                life -= atk;
                InForcedRoute = true;
            }
            if (life < 0) { life = 0; }
        }
        public void draw(Drawing d)
        {
            if (ad != null)
            {
                ad.Draw(d, new Vector(x - texW / 2, y - texH / 2), DepthID.Player);
            }
        }

        #region bool 各種状態を返す
        public bool Invincible() { return InForcedRoute || muteki_time > 0 || attack_mode || avoid_mode; }
        public bool canUseSkilltoEnemyForFirst()
        {
            return stop_time<=0 && !attack_mode  && sword >= sword_condition ; 
        }
        public bool canUseSkilltoEnemyForSecond()
        {
            return  attack_mode && add_attack_mode && sword >= nextSwordSkill_Cost;
        }
        public bool canEvade() { return !InForcedRoute && !attack_mode && stop_time <= 0 && !avoid_mode; }
        protected bool EvasionKeySetPressed(InputManager input) {
            return input.GetKeyPressed(KeyID.Cancel) == true && (input.IsKeyDown(KeyID.Up) == true || input.IsKeyDown(KeyID.Down) == true
                    || input.IsKeyDown(KeyID.Right) == true || input.IsKeyDown(KeyID.Left) == true);
        }
        #endregion
    }
}
