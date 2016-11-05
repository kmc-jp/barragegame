using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonPart {
    class Map {
        public static Player player;
        public static List<Enemy> enemys = new List<Enemy>();
        public List<Enemy> enemys_inside_window = new List<Enemy>();
        public int score = 0;
        public Vector scroll_speed;
        public int stage;
        protected AnimationAdvanced chargeBar;
        protected string chargeBar_animation_name;
        //いる？
        protected StageData stagedata;

        public static List<Projection> pros = new List<Projection>();
        public static List<int> pro_swords = new List<int>();
        public static double pro_speed = -2;
        public static double pro_acceleration = 1;
        public static bool boss_mode=false;

        public Vector bar_pos = new Vector(450,650);
        public Vector life_pos = new Vector(1050, 95);
        public Vector score_pos = new Vector(1100, 180);

        int total_height = 0;
        public int scroll_start;
        public int scroll_time;
        public double changed_scroll_speed;

        public double defaultspeed_x=0;
        public double defaultspeed_y=1;


        public static List<string> background_names = new List<string>();
        public static List<BGMID> bgmIDs = new List<BGMID>();
        public static List<int> step = new List<int>();

        /// <summary>
        /// 背景画像の描画位置
        /// </summary>
        public List<Vector> v = new List<Vector>();

        /// <summary>
        /// 左側のバーの右端のx座標
        /// </summary>
        public static int leftside = 280;
        public static int rightside = 1000;

        public string life_tex_name = "testlife";

        public Vector camera = new Vector(leftside, 0);

        public Map(string _background_name,int _stage)//コンストラクタ
        {
            stage = _stage;
            stagedata = new Stage1Data(BGMID.stage1,"stage1");

            background_names.Add(_background_name);
            background_names.Add(_background_name);
            step.Add(0);
            v.Add(new Vector(leftside, DataBase.WindowSlimSizeY - DataBase.getTex(background_names[0]).Height));
            v.Add(new Vector(280, v[0].Y - DataBase.getTex(background_names[0]).Height));
           
            scroll_speed = new Vector(defaultspeed_x, defaultspeed_y);
            player = new Player(DataBase.WindowDefaultSizeX/2, 500, 6, 10, 5,"60 105-player");

            set_change_scroll(600,20,120);

            for (int i = 0; i < background_names.Count; i++)
            {
                total_height += DataBase.getTex(background_names[i]).Height;
            }

            leftside = 280;
            rightside = 1000;

            SoundManager.Music.PlayBGM(BGMID.stage2, true);
            chargeBar = new AnimationAdvanced(DataBase.getAniD("swordgauge"));
        }
        public Map(string[] bns)
        {
            for(int i = 0; i < bns.Length; i++)
            {
                background_names.Add(bns[i]);
            }
            step.Add(0);
            v.Add(new Vector(leftside, DataBase.WindowSlimSizeY - DataBase.getTex(background_names[0]).Height));
            for (int i = 1; i < background_names.Count; i++)
            {
                v.Add(new Vector(leftside, v[i - 1].Y - DataBase.getTex(background_names[i]).Height));

            }
            /*for (int i = 0; i < background_names.Count; i++)
            {
                v.Add(new Vector(leftside, v[v.Count - 1].Y - DataBase.getTex(background_names[i]).Height));
            }*/
            scroll_speed = new Vector(defaultspeed_x, defaultspeed_y);
            player = new Player(DataBase.WindowDefaultSizeX / 2, 500, 6, 10, 5);

            set_change_scroll(600, 20, 120);
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
                    v[i]= new Vector(v[i].X, v[i].Y-total_height);
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
                    enemys[i].clear();
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
        public static void set_BGM(BGMID id)
        {
            bgmIDs.Add(id);
        }
        public static void PlayBGM(BGMID id)
        {
            SoundManager.Music.PlayBGM(id, true);
        }
        public static void set_backgroundName(string _background_name)
        {
            background_names.Add(_background_name);
        }
        public static void create_enemy(double _x,double _y,string _unitType_name)
        {
            enemys.Add(new Enemy(leftside+_x, _y, _unitType_name));
        }
        public static void create_boss1(double _x, double _y, string _unitType_name)
        {
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

            for (int i = 0; i < v.Count; i++)
            {
                if (inside_of_window(v[i], DataBase.getTex(background_names[i])) == true)
                { 
                    d.Draw(v[i], DataBase.getTex(background_names[i]), DepthID.BackGroundFloor);
                }
            }

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

            for (int i = 0; i < pros.Count; i++)
            {
                pros[i].draw(d);
            }

            //if (player.dead_mode == false)
            //{
                player.draw(d);
            //}

            for(int i = 0; i < player.life; i++)
            {
                d.Draw(new Vector(life_pos.X + DataBase.getTex(life_tex_name).Width * i , life_pos.Y), DataBase.getTex(life_tex_name), DepthID.Status);
            }

            RichText scoreboard=new RichText(score.ToString(), FontID.Medium,Color.White);
            scoreboard.Draw(d, score_pos, DepthID.Status);

            d.Draw(new Vector(leftside-DataBase.getTex("leftside1").Width, 0), DataBase.getTex("leftside1"), DepthID.StateFront);
            d.Draw(new Vector(rightside, 0), DataBase.getTex("rightside1"), DepthID.StateFront);

            
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
