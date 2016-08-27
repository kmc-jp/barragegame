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

        Player player;
        public int bulletexist = 0;
        public int enemyexist = 0;
        List<Bullet> bullets = new List<Bullet>();
        List<Enemy> enemys = new List<Enemy>();
        public int frame = 0;
        public int scenenumber = 0;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
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
            player = new Player(0, 0, 6, player_texture, spriteBatch);

            bullet_texture = Content.Load<Texture2D>("36 40-bul1.png");

            player = new Player(0, 0, 6 ,1, player_texture, spriteBatch);

            //bullet = new Bullet(player.x,player.y,0,5,1,bullet_texture,spriteBatch);
            //bullets.Add(new Bullet(player.x, player.y, 0, 5, 1, bullet_texture, spriteBatch));

            enemy_textures=new List<Texture2D>();

            enemy_textures.Add(Content.Load<Texture2D>("36 40-ene1.png")) ;

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
            if (keymanager.IsKeyDown(KeyID.Select) == true) { scenenumber++; }


           
            int iRandom = cRandom.Next(600);
            iRandom = cRandom.Next(600);

            /*spriteBatchは異なると描画の前後が必ず新しいspriteBatchの描画一番上になると思います。どのみちちょっと勿体無い感じです。
             * 
             * なので、どうするかを考えましょう。
           　*/
            if (frame % 100 == 0)
            {
                enemyspriteBatchs.Add(new SpriteBatch(GraphicsDevice));
                enemys.Add(new Enemy(iRandom, 0, 0, 1, 1, enemy_textures[0], enemyspriteBatchs[frame/100]));
                enemyexist++;
            }
            
            if (enemyexist>0)

            if (scenenumber > 0)

            {

                ///move
                player.move();
                player.shot(bullet_texture);

                //enemy生成
                int iRandom = cRandom.Next(600);
                iRandom = cRandom.Next(600);
                if (frame % 100 == 0)
                {
                    enemys.Add(new Enemy(iRandom, 0, 0, 1, 1,10, enemy_textures[0], spriteBatch));
                    enemyexist++;
                }

            }




                if (enemyexist > 0)
                {
                    for (int i = 0; i < enemys.Count; i++)
                    {
                        enemys[i].move();
                    }
                }
                /*これはBullet.csの方に書いています。
                if (bulletexist > 0)
                {
                    for (int i = 0; i < bullets.Count; i++)
                    {
                        bullets[i].move();
                    }
                }
                else { player.x = 100;player.y = 100; }
                */

                ///remove
                
                if (enemyexist>0)
                {
                    for (int i = 0; i < enemys.Count; i++)
                    {
                        if (enemys[i] != null) { enemys[i].remove(i,player); }
                    }
                } //例外がでる
                

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

            if (scenenumber > 0)
            {
                //if (bulletexist>0) {for(int i = 0; i < bullets.Count; i++){ bullets[i].draw();} }
                if (enemyexist > 0)
                {
                    for (int i = 0; i < enemys.Count; i++)
                    {
                        if (enemys[i] != null) { enemys[i].draw(); }
                    }
                }

                player.draw();
            }

            spriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
                }
        }
    }


