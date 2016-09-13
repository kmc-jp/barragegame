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
        public int scroll_speed = 1;
        public string background_name;


        string music_title;
        public List<int> step = new List<int>();
        
        /// <summary>
        /// 背景画像の描画位置
        /// </summary>
        Vector2 v1;
        Vector2 v2;

        /// <summary>
        /// 左側のバーの右端のx座標
        /// </summary>
        public static int leftside = 280;


        public Map(string _background_name= "testbackground.png")
        {
            background_name = _background_name;
            step.Add(0);
            v1 = new Vector2(leftside, 0);
            v2 = new Vector(v1.X, v1.Y - DataBase.getTex(background_name).Height);
            player = new Player(640, 500, 6, 10, 5, 0, 0);
        }

        public void update(InputManager input)
        {
            Random cRandom = new System.Random();
            int iRandom = cRandom.Next(1280);
            iRandom = cRandom.Next(1280);

            double random = Function.GetRandomDouble(280, 1000);

            if (step[0] % 100 == 0)
            {
                enemys.Add(new Enemy(random, 0, 0, 1, 10, 10, 100));
                enemyexist++;
            }
            //生成 end

            #region move
            input.Update();

            if (step[0] > 600 && step[0] < 1200) { scroll_speed = 10; } else { scroll_speed = 1; }
            v1.Y += scroll_speed;//背景のmove
            v2.Y += scroll_speed;
            if (v1.Y - v2.Y > DataBase.getTex(background_name).Height)
            {
                v2.Y = v1.Y + DataBase.getTex(background_name).Height;
            }
            if(v1.Y - v2.Y < -DataBase.getTex(background_name).Height)
            {
                v2.Y = v1.Y + DataBase.getTex(background_name).Height;
            }

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
                    for (int j = 0; j < enemys[i].bullets.Count; j++)
                    {
                        enemys[i].bullets[j].update();
                    }
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
                        if (enemys[i].bullets[j].life <= 0)
                        {
                            enemys[i].bullets[j].remove(enemys[i].bullets);
                        }
                    }
                }
            }
            
            if (player.life <= 0)
            {
                step[0] = -100;
            }
            #endregion

            step[0]++;
        }//updat end


        public void Draw(Drawing d)
        {
            
            d.SetDrawAbsolute();

            if (v1.Y >= 720) { v1.Y = -DataBase.getTex(background_name).Height; }
            if (v2.Y >= 720) { v2.Y = -DataBase.getTex(background_name).Height; }
            d.Draw(v1, DataBase.getTex(background_name), DepthID.BackGroundFloor);
            d.Draw(v2, DataBase.getTex(background_name), DepthID.BackGroundFloor);

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
