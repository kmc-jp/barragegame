using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonPart {
    class Map {
        static public int score = 0;

        public int stage;        protected StageData stagedata;

        public Vector score_pos = new Vector(1100, 220);

        public static Window window=null;

        #region player and Life Piece and chargeBar
        public string life_tex_name = "24x44バッテリーアイコン";
        /// <summary>
        /// プレイヤーの1残機をlifePerPiece等分する。基本ダメージは1残機削るが、得たスコアーによって残機が(このint)分の1で回復する。
        /// </summary>
        public int lifesPerPiece = 3;

        protected AnimationAdvanced chargeBar;
        protected string chargeBar_animation_name;

        /// <summary>
        /// チャージバーの左上の座標
        /// </summary>
        public Vector bar_pos = new Vector(450, 650);
        /// <summary>
        /// 残機表示の左上の座標
        /// </summary>
        public Vector life_pos = new Vector(1070, 120);
        /// <summary>
        /// 扇の描画位置
        /// </summary>
        public Vector ougi_pos = new Vector(950,80);
        #endregion
        #region player- clean bullets
        public static bool damageEnemys;
        public static int maxRadiusOfCleaningBullets;
        public static int speed_radiusOfCleaningBullets =8;
        public static int now_radiusOfCleaningBullets = -1;
        public static int frames_CleaningBullets = 80;
        #endregion
        /// <summary>
        /// playerがskilltoEnemyで倒した敵の個体数を記録
        /// </summary>
        public static int numOfskillKilledEnemies=0;
        public static int scoreOfskilltoEnemy = 0;
        #region player ChargeProjections
        public static List<ChargeProjection> pros = new List<ChargeProjection>();
        public static double pro_speed = -2;
        public static double pro_acceleration = 1;
        #endregion
        #region map scroll variable
        Vector scroll_speed;
        int scroll_start;
        int scroll_time;
        double changed_scroll_speed;
        /// <summary>
        /// デフォルトのmapのスクロールの速度
        /// </summary>
        const double defaultspeed_x = 0,defaultspeed_y = 1;
        #endregion
        #region map BackGround/textures variables
        static int total_BackGroundHeight = 0;
        protected static List<string> background_names = new List<string>();
        protected static List<string> textureNames = new List<string>();
        /// <summary>
        /// 背景画像の描画位置
        /// </summary>
        protected static List<Vector> v = new List<Vector>();
        #endregion
        #region about boss
        public static bool boss_mode = false;
        /// <summary>
        /// animation nameとして扱う. 
        /// </summary>
        static protected string bossLifeBarTextureName;
        static protected AnimationAdvanced bossLifeBarAnime = null;
        protected Vector bossLifeBarAnimationLeftTopPos = new Vector(0, 0);
        protected Vector bossLifeGaugeLeftTopDisplacement = new Vector(102, 24);
        protected Vector bossLifeGaugeLeftTopPos { get { return bossLifeBarAnimationLeftTopPos + bossLifeGaugeLeftTopDisplacement; } }
        /// <summary>
        /// .Yは一定だと思って使う.
        /// </summary>
        protected Vector bossLifeGaugeSizeMaximum = new Vector(1077, 37), bossLifeGaugeSize=new Vector(0,37);
        protected Color bossLifeGaugeColor = Color.Crimson;
        protected Enemy BOSS { get { if (boss_mode) { if (enemys.Count > enemysIndexOfBoss) { return enemys[enemysIndexOfBoss]; } else { return null; } } else { return null; } } }
        #endregion
        #region about Map State
        /// <summary>
        /// ゲーム中のmapでどのような動作を行う/行っているか　の定義を示す。
        /// </summary>
        public const string updated = "-updated", fullStopped = "-fulStop", playerStopped = "-plaStop", enemysStopped = "-eneStop",
             bothSideMove = "-boSidMove", gameOver = "-gAMeOVer", backToStageSelection = "-backToStageSelection" ;
        static public string mapState = ""; //初期は空として、その都度変えていく 
        /// <summary>
        /// 両サイドが0,1280まで広がり、最後のboss戦画面になる
        /// </summary>
        static public void EngagingTrueBoss()
        {
            boss_mode = true;
            leftsideTo = 0; rightsideTo = 1280;
            mapState += bothSideMove;
        }
        #endregion
        #region map cutin texture and about stop
        /// <summary>
        /// マップの更新が止まるまでの時間.
        /// </summary>
        public static int readyToStop_time = 0;
        /// <summary>
        /// mapが更新を止める時間
        /// </summary>
        public static int stop_time=0;
        /// <summary>
        /// cut in するtextureの名前
        /// </summary>
        public static string cutIn_texName=null;
        public static int cutIn_texPosNowX,cutIn_texPosNowY;
        public static int cutIn_texPosToX,cutIn_texPosToY;
        public static int cutIn_speed;
        public static int cutIn_texTime =-1;
        #endregion
        #region step[] and others about Map
        /// <summary>
        /// 多種多様なMap上の状況判断に使える。整数変数配列である。今 0番目は時間を記録する
        /// </summary>
        public static List<int> step = new List<int>();

        public static Player player;
        public static int enemysIndexOfBoss=0;
        public static List<Enemy> enemys = new List<Enemy>();
        public static List<Enemy> enemys_inside_window = new List<Enemy>();      

        /// <summary>
        /// 左側のバーの右端のx座標
        /// </summary>
        public static int leftside = 280;
        public static int rightside = 1000;
        public static int topside = 0;
        static protected int leftsideTo = 280; // bothSideMoveの時、左辺がどこに行くのかを決める
        static protected int rightsideTo = 1000; // bothSideMoveの時、右辺がどこに行くのかを決める
        static protected int sideMoveSpeed = 10; // 両サイドが動くときの速度
        #endregion


//##############     variables above
 
//###############   functions below
        /// <summary>
        /// constructor コンストラクター
        /// </summary>
        /// <param name="_stage"></param>
        public Map(int _stage)
        {
            setMapAsNewlyCreated();
            stage = _stage;
            #region switch stage 
            switch (stage)
            {
                case 1:
                    stagedata = new Stage1Data("stage1");
                    break;
                case 2:
                    stagedata = new Stage2Data("stage2");
                    break;
                case 4:
                    stagedata = new Stage4Data("stage4");
                    break;
                case 3:
                    stagedata = new Stage3Data("stage3");
                    break;
                case 5:
                    stagedata = new Stage6Data("stage6");
                    break;
                case 6:
                    stagedata = new Stage6Data("stage6");
                    break;
                case -1:
                    stagedata = new stageData_forTestMap("test");
                    stage = 1;
                    break;
                default:
                    stage = 1;
                    stagedata = new Stage1Data("stage1");
                    break;
            }
            #endregion
            stagedata.update();
            scroll_speed = new Vector(defaultspeed_x, defaultspeed_y);
            Map.player = new Player(DataBase.WindowDefaultSizeX/2, 500, 6, 10, 3*lifesPerPiece,DataBase.charaName);

            bossLifeGaugeSize.X=0;
            leftside = 280;
            rightside = 1000;

            chargeBar = new AnimationAdvanced(DataBase.getAniD("swordgauge"));
        }
        
        private void setMapAsNewlyCreated()
        {
            Map.mapState = "";
            stagedata = null;
            Map.player = null;
            chargeBar = null;
            Map.bossLifeBarAnime = null;
            Map.step.Clear();
            Map.step.Add(0);
            Map.leftside = 280; Map.rightside = 1000;
            Map.leftsideTo = 280; Map.rightsideTo = 1000;
            Map.stop_time = Map.readyToStop_time = 0;
            Map.cutIn_texName = null;
            Map.cutIn_texTime = 0;
            #region enemys clear
            Map.enemys.Clear(); Map.enemys_inside_window.Clear();
            enemys = null; enemys_inside_window = null;
            enemys = new List<Enemy>(); enemys_inside_window = new List<Enemy>();
            #endregion
            #region about background 
            v.Clear(); background_names.Clear(); total_BackGroundHeight = 0;
            #endregion
            textureNames.Clear();
            Map.pros.Clear();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <param name="tex"></param>
        /// <returns></returns>
        public bool inside_of_window(Vector v,int w, int h)
        {
            return leftside < v.X + w || DataBase.WindowDefaultSizeX - leftside > v.X || topside < v.Y || topside + DataBase.WindowSlimSizeY > v.Y + h;
        }

        public void set_change_scroll(int _scroll_time, double _changed_scroll_speed,int _scroll_start=-1)
        {
            scroll_start = _scroll_start;
            scroll_time = _scroll_time;
            changed_scroll_speed = _changed_scroll_speed;

        }

        public void update_scroll_speed()
        {
            for (int i = 0; i < v.Count; i++)
            {
                v[i] += scroll_speed;
            }

            for (int i = 0; i < v.Count; i++)
            {
                if (v[i].Y >= DataBase.WindowSlimSizeY)
                {
                    v[i]= new Vector(v[i].X, v[i].Y-total_BackGroundHeight);
                }
            }

            scroll_time--;
            if (step[0] > scroll_start && scroll_time > 0)
            {
                scroll_speed.Y = changed_scroll_speed;
                scroll_time--; 
            }
            else
            {
                scroll_speed.Y = defaultspeed_y;
            }
        }
        private void update_enemys()
        {
            bool bulletsMove = ((int)(step[0] * Game1.enemyBullets_update_fps / Game1.fps)
                - (int)((step[0] - 1) * Game1.enemyBullets_update_fps / Game1.fps)) >= 1;
            bool skillsUpdate = ((int)(step[0] * Game1.enemySkills_update_fps / Game1.fps)
                - (int)((step[0] - 1) * Game1.enemySkills_update_fps / Game1.fps)) >= 1;
            if (enemys.Count > 0)
            {
                for (int i = 0; i < enemys.Count; i++)
                {
                    enemys[i].bulletsMove = bulletsMove;
                    enemys[i].skillsUpdate = skillsUpdate;
                    enemys[i].update(player);
                }
            }

            for (int i = 0; i < enemys_inside_window.Count; i++)
            {
                if (enemys_inside_window[i].delete == true)
                {
                    if (enemys_inside_window[i].fadeout == false)
                    {
                        enemys_inside_window[i].score();
                    }
                    enemys_inside_window[i].clear();
                    enemys.Remove(enemys_inside_window[i]);
                    enemys_inside_window.Remove(enemys_inside_window[i]);
                    i--;
                }
            }
            
        }
        private void update_chargeProjections()
        {
            for (int i = 0; i < pros.Count; i++)
            {
                if (!pros[i].delete)
                {
                    pros[i].update();
                    if (pros[i].speed >= 40)
                    {
                        pros[i].speed = 40;
                    }
                    if (Function.hitcircle(pros[i].x, pros[i].y, 0, player.x, player.y, 20))
                    {
                        player.addEnergy(pros[i].sword);
                        pros[i].delete = true;
                    }
                }
            }
            for (int j = 0; j < pros.Count; j++)
            {
                if (pros[j].delete)
                {
                    pros.Remove(pros[j]);
                    j--;
                }
            }
        }

        public void update(InputManager input)
        {
            #region update cutIn
            if (Map.cutIn_texTime > 0)
            {
                Map.cutIn_texTime--;
            } 
            else if(Map.cutIn_texTime <=0 && Map.cutIn_texTime != DataBase.motion_inftyTime)
            {
                Map.cutIn_texName = null;
            }
            Map.cutIn_texPosNowX = Function.towardValue(Map.cutIn_texPosNowX, Map.cutIn_texPosToX, Map.cutIn_speed);
            Map.cutIn_texPosNowY = Function.towardValue(Map.cutIn_texPosNowY, Map.cutIn_texPosToY, Map.cutIn_speed);
            #endregion
            #region about stop time
            if (!DataBase.timeEffective(Map.readyToStop_time) )
            {
                if (DataBase.timeEffective(Map.stop_time))
                {
                    if (Map.stop_time != DataBase.motion_inftyTime)
                    {
                        Map.stop_time--;
                    }
                    return;
                }
            }else if(Map.readyToStop_time!=DataBase.motion_inftyTime){ Map.readyToStop_time--; }
            #endregion
            chargeBar.Update();
            stagedata.update();
            #region 画面上に見える敵を"見える敵たち"に入れる
            for (int i = 0; i < enemys.Count; i++)
            {
                if (inside_window(enemys[i]))
                {
                    if (enemys[i].exist == false)
                    {
                        enemys_inside_window.Add(enemys[i]);
                        enemys[i].exist = true;
                    }
                }
            }
            #endregion
            #region map scroll
            update_scroll_speed();
            #endregion
            update_enemys();
            update_chargeProjections();

            #region player update
            if (player.isAlive())
            {
                player.update(input, this);
                #region cleaning bullets enemys 
                if (now_radiusOfCleaningBullets != -1)
                {
                    now_radiusOfCleaningBullets = Function.towardValue(now_radiusOfCleaningBullets,
                        maxRadiusOfCleaningBullets, speed_radiusOfCleaningBullets);
                    if (now_radiusOfCleaningBullets >= maxRadiusOfCleaningBullets && maxRadiusOfCleaningBullets != -1)
                        maxRadiusOfCleaningBullets = -1;
                    #region cleaning bullets
                    for (int jj = 0; jj < enemys.Count; jj++)
                    {
                        #region its bullets
                        for (int j = 0; j < Map.enemys[jj].bullets.Count; j++)
                        {
                            if (Map.enemys[jj].bullets[j].hit_jugde(player.x, player.y, now_radiusOfCleaningBullets))
                            {
                                Map.enemys[jj].bullets[j].damage(player.atk/frames_CleaningBullets);
                            }
                        }
                        #endregion
                        if (Map.enemys[jj].label.Contains("boss"))
                        {
                            for (int j = 0; j < ((Boss)(Map.enemys[jj])).bodys.Length; j++)
                            {
                                #region its bodys' bullets
                                for (int k = 0; k < ((Boss)(Map.enemys[jj])).bodys[j].bullets.Count; k++)
                                {
                                    if (((Boss)Map.enemys[jj]).bodys[j].bullets[k].hit_jugde(player.x, player.y, now_radiusOfCleaningBullets))
                                    {
                                        ((Boss)Map.enemys[jj]).bodys[j].bullets[k].damage(player.atk/frames_CleaningBullets);
                                    }
                                }
                                #endregion
                                #region its bodys
                                if (damageEnemys)
                                    if (((Boss)Map.enemys_inside_window[jj]).bodys[j].hit_jugde(player.x, player.y, now_radiusOfCleaningBullets))
                                    {
                                        Map.enemys_inside_window[jj].damage(player.atk / (4*frames_CleaningBullets));
                                    }
                                #endregion
                            }
                        }
                        if(damageEnemys)
                            if (Map.enemys[jj].hit_jugde(player.x, player.y, now_radiusOfCleaningBullets))
                            {
                                Map.enemys[jj].damage(player.atk / frames_CleaningBullets);
                            }
                    }
                    #endregion
                }
                #endregion
                #region player attack skill - translate point into life
                if (player.attack_mode)
                {
                    if (Map.scoreOfskilltoEnemy / 10000 >= 1)
                    {
                        player.life += Map.scoreOfskilltoEnemy / 10000;
                        Map.scoreOfskilltoEnemy %= 10000;
                    }
                }
                #endregion
            }
            #endregion
            else { maxRadiusOfCleaningBullets = now_radiusOfCleaningBullets = -1; }
            if (boss_mode == true)
            {
                barslide();
                if (BOSS!=null && BOSS.label.Contains("boss")&&BOSS.life <= 0 && !mapState.Contains(backToStageSelection) )
                {
                    game_win_start();
                }
            }
            step[0]++;
        }//update end

        public static int caculateBulletScore(int sw)
        {//sw is may be sword that clearing a bullet gains the character
            // swはキャラクターが弾丸を消した時に得られる刀チャージの量 かもしれない
            //Console.Write("sw:" + sw);
            if (player.attack_mode)
            {
                int s = sw * 20 + 40;
                scoreOfskilltoEnemy += s;
                //Console.Write(" to " + s.ToString() + "\n");
                return s;
            }
            //Console.Write(" to " + (sw*100).ToString() + "\n");
            return sw*100;
        }
        public static int caculateEnemyScore(int _score)
        {
            //Console.Write("score:" + _score);
            if (player.attack_mode) {
                numOfskillKilledEnemies++;
                int s = _score * 100 + (numOfskillKilledEnemies - 1) * _score * 40;
                scoreOfskilltoEnemy += s;
                //Console.Write(" to " + s.ToString() + "\n");
                return s;
            }
            //Console.Write(" to " + (_score).ToString()+"\n");
            return _score;
        }
        #region Map Advanced Function
        public static void DialougeWindow(string[] text,Vector[] vecs) {

        }
        /// <summary>
        /// マップ上のすべての物体の更新を止める。
        /// </summary>
        /// <param name="time"></param>
        public static void stopUpdating(int _stop_time,int _ready_to_stop_time)
        {
            readyToStop_time = _ready_to_stop_time;
            stop_time = _stop_time;
        }
        public static void CutInTexture(string texName,int x,int y,int toX,int toY,int texTime,int _cutInSpeed)
        {
            Map.cutIn_speed = _cutInSpeed;
            Map.cutIn_texName = texName;
            Map.cutIn_texPosNowX = x;
            Map.cutIn_texPosNowY = y;
            Map.cutIn_texPosToX = toX;
            Map.cutIn_texPosToY = toY;
            Map.cutIn_texTime = texTime;
        }
        public static void game_over_start()
        {
            Console.Write("gameOver");
            stopUpdating(DataBase.motion_inftyTime, 180);
            CutInTexture("1280x2000背景用グレー画像", 0,-2000,0,0,DataBase.motion_inftyTime,30);
            mapState +=gameOver;
        }
        public static void game_win_start()
        {
            stopUpdating(DataBase.motion_inftyTime, 180);
            CutInTexture("1280x2000背景用グレー画像", 0, -2000, 0, 0, DataBase.motion_inftyTime, 30);
            mapState += backToStageSelection;
        }
        #endregion
        #region Create / Make   Map Function
        /// <summary>
        /// すでに正しく計算された刀チャージ量と点数を引数で渡し、点数はそのまま総点数に加算し、キャラクターに向かうChargeProjectionを作る
        /// </summary>
        /// <param name="x">生成位置x</param>
        /// <param name="y">生成位置x</param>
        /// <param name="sword">すでに計算された刀チャージ量</param>
        /// <param name="_score">すでに計算されたscore</param>
        public static void make_chargePro(double x,double y,int sword,int _score)
        {
            pros.Add(new ChargeProjection(x,y, "heal1",sword, pro_speed, pro_acceleration, player));
            Map.score += _score;
            //Console.Write(score.ToString() + " " + _score.ToString()+" ");
        }
        public static void create_enemy(double _x, double _y, string _unitType_name)
        {
            enemys.Add(new Enemy(leftside + _x, _y, _unitType_name));
        }
        public static void create_boss1(double _x, double _y, string _unitType_name, string _bossLifeBarName = DataBase.bossLifeBar_default_aniName)
        {
            enemys.Clear();
            enemys_inside_window.Clear();
            if (enemys.Count <= enemysIndexOfBoss)
            {
                enemys.Add(new Boss1(leftside + _x, _y, _unitType_name));
            }
            else
            {
                enemys.Insert(enemysIndexOfBoss, new Boss1(leftside + _x, _y, _unitType_name));
            }
            bossLifeBarTextureName = _bossLifeBarName;
            bossLifeBarAnime = new AnimationAdvanced(DataBase.getAniD(bossLifeBarTextureName + DataBase.defaultAnimationNameAddOn));
        }
        public static void create_boss2(double _x, double _y, string _unitType_name, string _bossLifeBarName = DataBase.bossLifeBar_default_aniName)
        {
            enemys.Clear();
            enemys_inside_window.Clear();
            if (enemys.Count <= enemysIndexOfBoss)
            {
                enemys.Add(new Boss2(leftside + _x, _y, _unitType_name));
            }
            else
            {
                enemys.Insert(enemysIndexOfBoss, new Boss2(leftside + _x, _y, _unitType_name));
            }
            bossLifeBarTextureName = _bossLifeBarName;
            bossLifeBarAnime = new AnimationAdvanced(DataBase.getAniD(bossLifeBarTextureName + DataBase.defaultAnimationNameAddOn));
        }
        public static void create_boss3(double _x, double _y, string _unitType_name, string _bossLifeBarName = DataBase.bossLifeBar_default_aniName)
        {
            enemys.Clear();
            enemys_inside_window.Clear();
            if (enemys.Count <= enemysIndexOfBoss)
            {
                enemys.Add(new Boss3(leftside + _x, _y, _unitType_name));
            }
            else
            {
                enemys.Insert(enemysIndexOfBoss, new Boss3(leftside + _x, _y, _unitType_name));
            }
            bossLifeBarTextureName = _bossLifeBarName;
            bossLifeBarAnime = new AnimationAdvanced(DataBase.getAniD(bossLifeBarTextureName + DataBase.defaultAnimationNameAddOn));
        }
        public static void bossDamaged()
        {
            bossLifeBarAnime = null;
            bossLifeBarAnime = new AnimationAdvanced(DataBase.getAniD(bossLifeBarTextureName+DataBase.aniNameAddOn_spell));
            
        }
        #endregion
        #region about Player   damaged
        public static void clearBullets(int _maxRadius=440,int _frames=80,bool _cleanEnemys=false, int _nowRadius=10)
        {
            if (player.isAlive())
            {
                now_radiusOfCleaningBullets = _nowRadius;
                frames_CleaningBullets = _frames;
                maxRadiusOfCleaningBullets = _maxRadius;
                speed_radiusOfCleaningBullets = (_maxRadius * 2 - now_radiusOfCleaningBullets)/frames_CleaningBullets;
                damageEnemys = _cleanEnemys;
            }
        }
        #endregion
        #region standard Map Functions
        private void barslide()
        {
            leftside = Function.towardValue(leftside, leftsideTo, sideMoveSpeed);
            rightside = Function.towardValue(rightside, rightsideTo, sideMoveSpeed);
            if (leftside==leftsideTo && rightside == rightsideTo)
            {
                mapState.Replace(bothSideMove, "");
            }
        }
        public static void PlayBGM(BGMID id)
        {
            SoundManager.Music.PlayBGM(id, true);
        }
        public static void setup_backgroundNamesFromNames(string[] bns)
        {
            if (bns.Length <= 0) { Console.WriteLine("setup Map Background:bns 0 length!"); }
            else if (bns.Length <= 1)
            {
                background_names.Add(bns[0]);
                background_names.Add(bns[0]);
            }
            else{
                for (int i = 0; i < bns.Length; i++)
                {
                    background_names.Add(bns[i]);
                }
            }
            v.Add(new Vector(0, DataBase.WindowSlimSizeY - DataBase.getTex(background_names[0]).Height));
            for (int i = 1; i < background_names.Count; i++)
            {
                v.Add(new Vector(v[i-1].X, v[i - 1].Y - DataBase.getTex(background_names[i]).Height));
            }
            for (int i = 0; i < background_names.Count; i++)
            {
                total_BackGroundHeight += DataBase.getTex(background_names[i]).Height;
            }
        }
        public static void setup_textureNames(List<string> _textureNames)
        {
            foreach(string _textureName in _textureNames)
            {
                textureNames.Add(_textureName);
            }
            Console.WriteLine("ssd"+textureNames.Count);
        }
        
        private void chargeBarChange(string name,string addOn=null) {
            if (addOn == null) { chargeBar_animation_name = name; }
            else { chargeBar_animation_name = name+addOn; }
            chargeBar = new AnimationAdvanced(DataBase.getAniD(chargeBar_animation_name));
        }

        public void Draw(Drawing d)
        {
            d.SetDrawAbsolute();

            #region drawBackGrounds
            for (int i = 0; i < v.Count; i++)
            {
                if (inside_of_window(v[i], DataBase.getTexD(background_names[i]).w_single, DataBase.getTexD(background_names[i]).h_single) )
                { 
                    d.Draw(v[i], DataBase.getTex(background_names[i]), DepthID.BackGroundFloor);
                }
            }
            #endregion
            #region enemys draw()
            if (enemys_inside_window.Count > 0)
            {
                for (int i = 0; i < enemys_inside_window.Count; i++)
                {
                    if (enemys_inside_window[i] != null)
                    {
                        enemys_inside_window[i].draw(d);
                    }
                }
            }
            #endregion
            if (bossLifeBarAnime != null&& BOSS!=null)
            {
                bossLifeGaugeSize.X = Function.towardValue(bossLifeGaugeSize.X,(bossLifeGaugeSizeMaximum.X * BOSS.life*1.0/BOSS.maxLife),100);
                d.DrawBox(bossLifeGaugeLeftTopPos, bossLifeGaugeSize, bossLifeGaugeColor, DepthID.Status);
                bossLifeBarAnime.Draw(d, bossLifeBarAnimationLeftTopPos, DepthID.Status);
            }
            #region projections draw
            for (int i = 0; i < pros.Count; i++)
            {
                pros[i].draw(d);
            }
            #endregion

            player.draw(d);
            #region cleaning bullets draw
            if (now_radiusOfCleaningBullets != -1 && player.isAlive())
            {
                d.DrawCircle(new Vector(player.x, player.y), now_radiusOfCleaningBullets, 4, 20, Color.Purple, DepthID.Player);
                d.DrawCircle(new Vector(player.x, player.y), now_radiusOfCleaningBullets-4, 2, 20, Color.PeachPuff, DepthID.Player);
            }
            #endregion
            #region life piece draw
            int ii;
            for(ii = 0; ii < player.life/lifesPerPiece; ii++)
            {
                d.Draw(new Vector(life_pos.X + DataBase.getTexD(life_tex_name).w_single * ii, life_pos.Y), DataBase.getTex(life_tex_name),
                DataBase.getRectFromTextureNameAndIndex(life_tex_name,lifesPerPiece-1), DepthID.Status);
            }
            if (player.life%lifesPerPiece!=0) {
                d.Draw(new Vector(life_pos.X + DataBase.getTexD(life_tex_name).w_single * ii, life_pos.Y), DataBase.getTex(life_tex_name),
                DataBase.getRectFromTextureNameAndIndex(life_tex_name, player.life % lifesPerPiece -1), DepthID.Status);
            }
            #endregion

            RichText scoreboard=new RichText(score.ToString(), FontID.Medium,Color.White);
            scoreboard.Draw(d, score_pos, DepthID.Status);

            #region sidebar draw
            if (textureNames.Count >= 2)
            {
                if (inside_of_window(new Vector(leftside - DataBase.getTex(textureNames[0]).Width, 0), DataBase.getTexD(textureNames[0]).w_single, DataBase.getTexD(textureNames[0]).h_single))
                {
                    d.Draw(new Vector(leftside - DataBase.getTex(textureNames[0]).Width, 0), DataBase.getTex(textureNames[0]), DepthID.StateFront);

                }
                if (inside_of_window(new Vector(rightside, 0), DataBase.getTexD(textureNames[1]).w_single, DataBase.getTexD(textureNames[1]).h_single))
                {
                    d.Draw(new Vector(rightside, 0), DataBase.getTex(textureNames[1]), DepthID.StateFront);
                }
            }
            #endregion
            #region ougi draw
            if (boss_mode == true)
            {
                d.Draw(ougi_pos, DataBase.getTex("ougi"), DepthID.StateFront);
            }
            #endregion
            #region draw sword gauge
            if (player.sword < player.sword_max / 2)
            {
                if (chargeBar_animation_name != "swordgauge")
                {
                    chargeBarChange("swordgauge");
                }
            } else
            {
                if (player.sword < player.sword_max)
                {
                    if (chargeBar_animation_name != "swordgaugehigh")
                    {
                        chargeBarChange("swordgauge", "high");
                    }
                }
                else
                {
                    if (chargeBar_animation_name != "swordgaugemax")
                    {
                        chargeBarChange("swordgauge", "max");
                    }
                }
            }


            d.DrawLine(bar_pos, new Vector(bar_pos.X + 380 * player.sword / player.sword_max, bar_pos.Y), 17, Color.Violet, DepthID.StateBack);//剣ゲージ
            chargeBar.Draw(d, new Vector(bar_pos.X-197, bar_pos.Y-chargeBar.Y/2-28), DepthID.StateBack);
            #endregion

            #region draw CutIns
            if (DataBase.timeEffective(Map.cutIn_texTime) && Map.cutIn_texName !=null && Map.cutIn_texName != "")
            {
                d.Draw(new Vector(Map.cutIn_texPosNowX, Map.cutIn_texPosNowY),DataBase.getTex(Map.cutIn_texName),DepthID.Message);
            }
            #endregion
        }//draw end

        static public bool inside_window(Enemy enemy)
        {
            return enemy.x > leftside - enemy.animation.X / 2 &&
                    enemy.x < rightside + enemy.animation.X / 2 &&
                    enemy.y < DataBase.WindowSlimSizeY + enemy.animation.Y / 2 &&
                    enemy.y > 0 - enemy.animation.Y / 2;
        }
        #endregion
    }// class end

}// namespace end
