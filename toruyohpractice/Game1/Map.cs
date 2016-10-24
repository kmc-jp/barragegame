using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonPart {
    class Map {
        public Player player;
        public List<Enemy> enemys = new List<Enemy>();
        public List<Enemy> enemys_inside_window = new List<Enemy>();
        public int score = 0;
        public int enemyexist = 0;
        public Vector scroll_speed;
        public int stage;
        protected Animation sword_animation;

        public List<Projection> pros = new List<Projection>();
        public List<int> pro_swords = new List<int>();
        public double pro_speed = -2;
        public double pro_acceleration = 1;

        public Vector bar_pos = new Vector(300,600);
        public Vector life_pos = new Vector(1050, 95);
        public Vector score_pos = new Vector(1100, 180);

        int total_height = 0;
        public int scroll_start;
        public int scroll_time;
        public double changed_scroll_speed;

        public double defaultspeed_x=0;
        public double defaultspeed_y=1;


        public List<string> background_names = new List<string>();
        public List<int> step = new List<int>();

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

            background_names.Add(_background_name);
            background_names.Add(_background_name);
            step.Add(0);
            v.Add(new Vector(leftside, DataBase.WindowSlimSizeY - DataBase.getTex(background_names[0]).Height));
            v.Add(new Vector(leftside, v[0].Y - DataBase.getTex(background_names[0]).Height));
           
            scroll_speed = new Vector(defaultspeed_x, defaultspeed_y);
            player = new Player(DataBase.WindowDefaultSizeX/2, 500, 6, 10, 5,"60 105-player");

            set_change_scroll(600,20,120);

            for (int i = 0; i < background_names.Count; i++)
            {
                total_height += DataBase.getTex(background_names[i]).Height;
            }

            SoundManager.Music.PlayBGM(BGMID.stage2, true);
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

        public void update(InputManager input)
        {
            int bullets = 0;
            for (int i = 0; i < enemys.Count; i++)
            {
                bullets += enemys[i].bullets.Count;
            }
           
            #region 生成
            Random cRandom = new System.Random();
            int iRandom = cRandom.Next(1280);

            if (step[0] % 10000 == 0 && step[0] > -1) 
            {
                for (int i = 0; i < 1; i++)
                {
                    double random = Function.GetRandomDouble(500, 800);
                    double random_y = Function.GetRandomDouble(150, 200);
                    if (step[0] % 200 == 1)
                    {
                        enemys.Add(new Enemy(random, random_y, "boss1"));
                    }else
                    {
                        enemys.Add(new Enemy(random, random_y, "enemy2"));
                    }
                    enemyexist++;
                }
            }
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

            player.update(input,this);

            if (enemyexist > 0)
            {
                for (int i = 0; i < enemys.Count; i++)
                {
                    enemys[i].update(player);
                }
            }

            for(int i = 0;i < pros.Count; i++)
            {
                pros[i].update();
                if (pros[i].speed >= 40)
                {
                    pros[i].speed = 40;
                }
            }

            #endregion

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
                        score += enemys_inside_window[i].score(this);
                    }
                    enemys[i].clear();
                    enemys.Remove(enemys_inside_window[i]);
                    enemys_inside_window.Remove(enemys_inside_window[i]);
                }
            }

            for (int i = 0; i < pros.Count; i++)
            {
                if (Function.hitcircle(pros[i].x, pros[i].y, 0, player.x, player.y, 20))
                {
                    player.sword += pro_swords[i];
                    pros.Remove(pros[i]);
                    pro_swords.Remove(pro_swords[i]);
                }
            }
            step[0]++;

        }//update end

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

            if (enemyexist > 0)
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

            d.Draw(new Vector(0, 0), DataBase.getTex("leftside1"), DepthID.StateFront);
            d.Draw(new Vector(1000, 0), DataBase.getTex("rightside1"), DepthID.StateFront);

            
            if (player.sword < player.sword_max / 2)
            {
                
            }else
            {
                if (player.sword < player.sword_max)
                {

                }else
                {

                }
            }
            d.DrawLine(bar_pos, new Vector(bar_pos.X + player.sword * 5, bar_pos.Y), 20, Color.Gold, DepthID.Status);//剣ゲージ

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
