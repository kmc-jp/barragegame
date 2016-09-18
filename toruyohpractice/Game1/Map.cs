using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonPart {
    class Map {
        Player player;
        List<Bullet> bullets = new List<Bullet>();
        List<Enemy> enemys = new List<Enemy>();
        public int score = 0;
        public int bulletexist = 0;
        public int enemyexist = 0;
        public Vector scroll_speed;

        int total_height = 0;
        public int scroll_start;
        public int scroll_time;
        public double changed_scroll_speed;

        public double defaultspeed_x=0;
        public double defaultspeed_y=1;


        public List<string> background_names = new List<string>();
        public string music_title;
        public List<int> step = new List<int>();

        /// <summary>
        /// 背景画像の描画位置
        /// </summary>
        public List<Vector> v = new List<Vector>();

        /// <summary>
        /// 左側のバーの右端のx座標
        /// </summary>
        public static int leftside = 280;

        public Vector camera = new Vector(leftside, 0);


        public Map(string _background_name= "testbackground.png")//コンストラクタ
        {
            background_names.Add(_background_name);
            background_names.Add(_background_name);
            step.Add(0);
            v.Add(new Vector(leftside, DataBase.WindowSlimSizeY - DataBase.getTex(background_names[0]).Height));
            v.Add(new Vector(leftside, v[0].Y - DataBase.getTex(background_names[1]).Height));
           
            scroll_speed = new Vector(defaultspeed_x, defaultspeed_y);
            player = new Player(DataBase.WindowDefaultSizeX/2, 500, 6, 10, 5);

            set_change_scroll(600,20,120);

            for (int i = 0; i < background_names.Count; i++)
            {
                total_height += DataBase.getTex(background_names[i]).Height;
            }
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
            #region 生成
            Random cRandom = new System.Random();
            int iRandom = cRandom.Next(1280);

            double random = Function.GetRandomDouble(280, 1000);

            if (step[0] % 100 == 0)
            {
                enemys.Add(new Enemy(random, 0, 0, 1, 10, 10, 100));
                enemyexist++;
            }
            #endregion

            #region move
            input.Update();


            camera += scroll_speed;//カメラupdate
            update_scroll_speed();

            player.update(input);
            
            if (input.IsKeyDown(KeyID.Select) == true)
            {
                bullets.Add(player.shot());
                bulletexist++;
            }
            if (enemyexist > 0)
            {
                for (int i = 0; i < enemys.Count; i++)
                {
                    enemys[i].update();
                }
            }

            if (bulletexist > 0)
            {
                for (int i = 0; i < bullets.Count; i++)
                {
                    bullets[i].update();
                }
            }
            #endregion

            if (step[0] % 100 == 0)
            {
                for (int i = 0; i < enemys.Count; i++)
                {
                    enemys[i].shot1(player);
                }
            }

            #region あたり判定
            if (enemyexist > 0)
            {
                for (int j = 0; j < bullets.Count; j++)
                {
                    if (bullets[j] != null)
                    {
                        bool hit = false;
                        for (int i = 0; i < enemys.Count; i++)
                        {

                            if (enemys[i] != null)
                            {
                                if (Function.hitcircle(enemys[i].x, enemys[i].y, enemys[i].radius, bullets[j].x, bullets[j].y, bullets[j].radius))
                                {
                                    hit = true;
                                    enemys[i].life--;
                                    if (enemys[i].life <= 0)
                                    {
                                        score = score + enemys[i].score;
                                        player.sword = player.sword + enemys[i].bullets.Count * 3;
                                        enemys[i].remove(Unit_state.dead);
                                    }
                                 }
                                if(enemys[i].x > 1280 || enemys[i].x < 0 || enemys[i].y > 720 || enemys[i].y < 0)
                                {
                                    enemys[i].remove(Unit_state.out_of_window);
                                }

                                if (enemys[i].delete == true)
                                {
                                    enemys.Remove(enemys[i]);
                                }
                            }
                         }
                         if (hit)
                         {
                             bullets[j].remove(bullets);
                         }
                     }
                 }
            }


            for (int i = 0; i < enemys.Count; i++)
            {
                for (int j = 0; j < enemys[i].bullets.Count; j++)
                {
                    if (Function.hitcircle(player.x, player.y, player.radius, enemys[i].bullets[j].x, enemys[i].bullets[j].y, enemys[i].bullets[j].radius))
                    {
                        enemys[i].bullets[j].life--;
                        player.life--;
                        if (enemys[i].bullets[j].life <= 0|| enemys[i].bullets[j].x > 1000 || enemys[i].bullets[j].x < 280 || enemys[i].bullets[j].y > 720 || enemys[i].bullets[j].y < 0)
                        {
                            enemys[i].bullets[j].remove(enemys[i].bullets);
                        }
                    }
                }
            }

            if (input.GetKeyPressed(KeyID.Select) == true) { step[0] = -1000; }
            if (player.life <= 0)
            {
                //step[0] = -100;
            }
            #endregion

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


            if (true)
            {
                if (enemyexist > 0)
                {
                    for (int i = 0; i < enemys.Count; i++)
                    {
                        if (enemys[i] != null)
                        {
                            enemys[i].draw(d);
                        }
                        for (int j = 0; j < enemys[i].bullets.Count; j++)
                        {
                            enemys[i].bullets[j].draw(d);
                        }
                    }
                }

                if (bulletexist > 0)
                {
                    for (int i = 0; i < bullets.Count; i++)
                    {
                        bullets[i].draw(d);
                    }
                }

                player.draw(d);
            }
        }//draw end
    }// class end

}// namespace end
