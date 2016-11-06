using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonPart {
    class Map {
        public int score = 0;

        public int stage;        protected StageData stagedata;

        public static bool boss_mode = false;

        public Vector score_pos = new Vector(1100, 180);
        #region player and Life Piece and chargeBar
        public string life_tex_name = "33x60バッテリーアイコン";
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
        public Vector life_pos = new Vector(1070, 105);
        #endregion
        #region player Projection
        public static List<Projection> pros = new List<Projection>();
        public static List<int> pro_swords = new List<int>();
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
        #region map BackGround variables
        static int total_BackGroundHeight = 0;
        protected static List<string> background_names = new List<string>();
        /// <summary>
        /// 背景画像の描画位置
        /// </summary>
        protected static List<Vector> v = new List<Vector>();
        #endregion

        #region step[] and others about Map
        /// <summary>
        /// 多種多様なMap上の状況判断に使える。整数変数配列である。今 0番目は時間を記録する
        /// </summary>
        public static List<int> step = new List<int>();

        public static Player player;
        public static List<Enemy> enemys = new List<Enemy>();
        public List<Enemy> enemys_inside_window = new List<Enemy>();
        /// <summary>
        /// 左側のバーの右端のx座標
        /// </summary>
        public static int leftside = 280;
        public static int rightside = 1000;
        public Vector camera = new Vector(leftside, 0);
        #endregion

        public Map(int _stage)//コンストラクタ
        {
            stage = _stage;
            stagedata = new Stage1Data("stage1");
            
            step.Add(0);
            scroll_speed = new Vector(defaultspeed_x, defaultspeed_y);
            player = new Player(DataBase.WindowDefaultSizeX/2, 500, 6, 10, 5*lifesPerPiece,"60 105-player");

            //set_change_scroll(600,20,120);

            chargeBar = new AnimationAdvanced(DataBase.getAniD("swordgauge"));
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <param name="tex"></param>
        /// <returns></returns>
        public bool inside_of_window(Vector v,Texture2D tex)
        {
            return camera.X < v.X + tex.Width || DataBase.WindowDefaultSizeX - camera.X > v.X || camera.Y < v.Y || camera.Y + DataBase.WindowSlimSizeY > v.Y + tex.Height;
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
            if (enemys.Count > 0)
            {
                for (int i = 0; i < enemys.Count; i++)
                {
                    enemys[i].update(player);
                }
            }
            for (int i = 0; i < enemys.Count; i++)
            {
                enemys[i].shot(player);
            }

            for (int i = 0; i < enemys_inside_window.Count; i++)
            {
                if (enemys_inside_window[i].delete == true)
                {
                    if (enemys_inside_window[i].fadeout == false)
                    {
                        score += enemys_inside_window[i].score();
                    }
                    enemys_inside_window[i].clear();
                    enemys.Remove(enemys_inside_window[i]);
                    enemys_inside_window.Remove(enemys_inside_window[i]);
                }
            }
        }
        public void update(InputManager input)
        {
            chargeBar.Update();

            stagedata.update();
            
            #region 生成
            /*
            Random cRandom = new System.Random();
            int iRandom = cRandom.Next(1280);

            if (step[0] % 100 == 0 && step[0] > -1) 
            {
                for (int i = 0; i < 1; i++)
                {
                    double random = Function.GetRandomDouble(900, 1000);
                    double random_y = Function.GetRandomDouble(250, 300);
                    if (step[0] % 200 == 0)
                    {
                        enemys.Add(new Boss1(random, random_y, "boss1"));
                    }else
                    {
                        enemys.Add(new Enemy(random, random_y, "enemy2"));
                    }
                }
            }
            */
            #endregion

            
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

            #region move

            camera += scroll_speed;//カメラupdate
            update_scroll_speed();

            //player.update(input,this);

            for(int i = 0;i < pros.Count; i++)
            {
                pros[i].update();
                if (pros[i].speed >= 40)
                {
                    pros[i].speed = 40;
                }
            }

            #endregion
            update_enemys();
            

            for (int i = 0; i < pros.Count; i++)
            {
                if (Function.hitcircle(pros[i].x, pros[i].y, 0, player.x, player.y, 20))
                {
                    player.sword += pro_swords[i];
                    pros.Remove(pros[i]);
                    pro_swords.Remove(pro_swords[i]);
                }
            }

            player.update(input, this);

            if (boss_mode == true)
            {
                barslide();
            }

            step[0]++;

        }//update end

        private void barslide()
        {
            if (leftside > 0)
            {
                leftside--;
                rightside++;
            }
        } 

        public static void make_chargePro(double x,double y,int sword,int score)
        {
            pros.Add(new Projection(x,y,MoveType.object_target, pro_speed, pro_acceleration, "heal1", player, 100));
            pro_swords.Add(sword);

            score += score;
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
        public static void create_enemy(double _x,double _y,string _unitType_name)
        {
            enemys.Add(new Enemy(leftside+_x, _y, _unitType_name));
        }
        public static void create_boss1(double _x, double _y, string _unitType_name)
        {
            enemys.Clear();
            enemys.Add(new Boss1(leftside + _x, _y, _unitType_name));
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
                if (inside_of_window(v[i], DataBase.getTex(background_names[i])) == true)
                { 
                    d.Draw(v[i], DataBase.getTex(background_names[i]), DepthID.BackGroundFloor);
                }
            }
            #endregion
            #region enemys draw()
            if (enemys.Count > 0)
            {
                for (int i = 0; i < enemys.Count; i++)
                {
                    if (enemys[i] != null)
                    {
                        enemys[i].draw(d);
                    }
                }
            }
            #endregion
            #region projections draw
            for (int i = 0; i < pros.Count; i++)
            {
                pros[i].draw(d);
            }
            #endregion

            player.draw(d);

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
            d.Draw(new Vector(leftside-DataBase.getTex("leftside1").Width, 0), DataBase.getTex("leftside1"), DepthID.StateFront);
            d.Draw(new Vector(rightside, 0), DataBase.getTex("rightside1"), DepthID.StateFront);
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
            
            chargeBar.Draw(d, new Vector(bar_pos.X-197, bar_pos.Y-chargeBar.Y/2-28), DepthID.Map);
            d.DrawLine(bar_pos, new Vector(bar_pos.X + player.sword * 3.8, bar_pos.Y), 17, Color.Violet, DepthID.Status);//剣ゲージ
            #endregion
        }//draw end

        public bool inside_window(Enemy enemy)
        {
            return enemy.animation.X > Map.leftside - enemy.animation.X / 2 ||
                    enemy.animation.X < Map.rightside + enemy.animation.X / 2 ||
                    enemy.animation.Y < DataBase.WindowSlimSizeY + enemy.animation.Y / 2 ||
                    enemy.animation.Y > 0 - enemy.animation.Y / 2;
        }

    }// class end

}// namespace end
