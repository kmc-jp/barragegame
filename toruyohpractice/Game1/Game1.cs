using cellgame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Game1
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D player_texture;
        Texture2D bullet_texture;
        List<Texture2D> enemy_textures;
        Random cRandom = new System.Random();
        KeyManager keymanager = new KeyManager();
        SpriteFont font;

        Player player;
        public int bulletexist = 0;
        public int enemyexist = 0;
        List<Bullet> bullets = new List<Bullet>();
        List<Enemy> enemys = new List<Enemy>();
        public int frame = 0;
        public int scenenumber = 0;
        public int enemyhit = 0;
        public int score = 0;

        [DllImport("kernel32")]
        static extern bool AllocConsole();
        public Game1()
        {
            this.Window.Title = "Barrage Game";
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.ApplyChanges();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            //AllocConsole();//コンソールモード

            /*
            Dictionary<string, int> scenmm = new Dictionary<string, int>();
            scene.Add("title", new TS);
            string x=Console.ReadLine();
            x.Remove(x.Length - 1, 1);
            Console.Write(scene[x]);
            */
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            player_texture = Content.Load<Texture2D>("36 40-hex1.png");
            //player = new Player(0, 0, 6,10,5, player_texture, spriteBatch);

            bullet_texture = Content.Load<Texture2D>("18 20-bul1.png");

            enemy_textures=new List<Texture2D>();
            enemy_textures.Add(Content.Load<Texture2D>("36 40-ene1.png")) ;

            //font = Content.Load<SpriteFont>("");
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            player_texture.Dispose();
            enemy_textures[0].Dispose();
            bullet_texture.Dispose();
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// 



        protected override void Update(GameTime gameTime)//mainloop
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            keymanager.Update();
            if (keymanager.IsKeyDown(KeyID.Select) == true && scenenumber == 0)
            {
                player = new Player(640, 720, 6,10,5,0, player_texture, spriteBatch);
                scenenumber++;
                
            }

            if (scenenumber > 0)
            {
                ///move
                player.move();
                if (keymanager.IsKeyDown(KeyID.Select) == true)
                {
                    bullets.Add(player.shot(bullet_texture));
                    bulletexist++;
                }

                //enemy生成
                /*
                int iRandom = cRandom.Next(1280);
                iRandom = cRandom.Next(1280);
                */
                double random = Function.GetRandomDouble(280, 1000);

                if (frame % 100 == 0)
                {
                    enemys.Add(new Enemy(random, 0, 0, 1, 10, 10,100, enemy_textures[0], spriteBatch));
                    enemyexist++;
                }






                if (enemyexist > 0)
                {
                    for (int i = 0; i < enemys.Count; i++)
                    {
                        enemys[i].move();
                        for (int j = 0; j < enemys[i].bullets.Count; j++)
                        {
                            enemys[i].bullets[j].move();
                        }
                    }
                }

                if (bulletexist > 0)
                {
                    for (int i = 0; i < bullets.Count; i++)
                    {
                        bullets[i].move();
                    }
                }

                if (frame % 100 == 0)
                {
                    for (int i = 0; i < enemys.Count; i++)
                    {
                        enemys[i].shot1(player, bullet_texture);
                    }
                }

                ///remove

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
                                    if (Function.hitcircle(enemys[i].x, enemys[i].y, enemys[i].radius, bullets[j].x, bullets[j].y, bullets[j].radius) || enemys[i].x > 1280 || enemys[i].x < 0 || enemys[i].y > 720 || enemys[i].y < 0)
                                    {
                                        hit = true;
                                        enemys[i].life--;
                                        if (enemys[i].life <= 0)
                                        {
                                            score = score + enemys[i].score;
                                            player.sword = player.sword + enemys[i].bullets.Count * 3;
                                            enemys[i].remove(enemys);
                                            
                                        }
                                        enemyhit++;
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

                /*
                if (bulletexist > 0)
                {
                    for(int i=0;i<bullets.Count;i++)
                    {
                        for (int j = 0; j < enemys.Count; j++)
                        {
                            if (Function.hitcircle(bullets[i].x, bullets[i].y, bullets[i].radius, enemys[j].x, enemys[j].y, enemys[j].radius)&&(bullets[i]!=null))
                                {
                                    bullets[i].remove(bullets);
                                }
                        }
                    }
                }
                */

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
                    scenenumber = 0;
                }
            }
                frame++;
                // TODO: Add your update logic here


                

                base.Update(gameTime);
            
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// 


        protected override void Draw(GameTime gameTime)//render
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();

            if (scenenumber == 0)
            {
                
            }

            if (scenenumber > 0)
            {
                //if (bulletexist>0) {for(int i = 0; i < bullets.Count; i++){ bullets[i].draw();} }
                if (enemyexist > 0)
                {
                    for (int i = 0; i < enemys.Count; i++)
                    {
                        if (enemys[i] != null)
                        {
                            enemys[i].draw(enemys[i].texture);
                        }
                        for(int j = 0; j < enemys[i].bullets.Count; j++)
                        {
                            enemys[i].bullets[j].draw(enemys[i].bullets[j].texture);
                        }
                    }
                }

                if (bulletexist > 0)
                {
                    for (int i = 0; i < bullets.Count; i++)
                    {
                        bullets[i].draw(bullets[i].texture);
                    }
                }

                player.draw(player.texture);
            }

            spriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
         }
        }
    }


