using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

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
        List<SpriteBatch> enemyspriteBatchs = new List<SpriteBatch>();
        List<SpriteBatch> bulletspriteBatchs = new List<SpriteBatch>();
        Random cRandom = new System.Random();

        Player player;
        public int bulletexist = 0;
        public int enemyexist = 0;
        List<Bullet> bullets = new List<Bullet>();
        List<Enemy> enemys = new List<Enemy>();
        public int frame=0;

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
            player_texture = Content.Load<Texture2D>("hex1.png");
            player = new Player(0, 0, 6, player_texture, spriteBatch);

            bullet_texture = Content.Load<Texture2D>("hex1.png");
            //bullet = new Bullet(player.x,player.y,0,5,1,bullet_texture,spriteBatch);
            //bullets.Add(new Bullet(player.x, player.y, 0, 5, 1, bullet_texture, spriteBatch));

            enemy_textures=new List<Texture2D>();
            enemy_textures.Add(Content.Load<Texture2D>("hex1.png")) ;
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

            player.move();
            player.shot(bullet_texture);
           

           
            int iRandom = cRandom.Next(600);
            iRandom = cRandom.Next(600);
            if (frame % 100 == 0)
            {
                enemyspriteBatchs.Add(new SpriteBatch(GraphicsDevice));
                enemys.Add(new Enemy(iRandom, 0, 0, 1, 1, enemy_textures[0], enemyspriteBatchs[frame/100]));
                enemyexist++;
            }
            
            if (enemyexist>0)
            {
                for (int i = 0; i < enemys.Count; i++)
                {
                    enemys[i].move();
                }
            }
            /*
            if (bulletexist > 0)
            {
                for (int i = 0; i < bullets.Count; i++)
                {
                    bullets[i].move();
                }
            }
            else { player.x = 100;player.y = 100; }
            */

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
            for (int i = 0;i < enemyspriteBatchs.Count;i++)
            {
                enemyspriteBatchs[i].Begin();
            }
            for (int i = 0; i < bulletspriteBatchs.Count; i++)
            {
                bulletspriteBatchs[i].Begin();
            }

            //if (bulletexist>0) {for(int i = 0; i < bullets.Count; i++){ bullets[i].draw();} }
            if(enemyexist>0){ for (int i = 0; i < enemys.Count; i++) { enemys[i].draw(); } }
            player.draw();

            spriteBatch.End();
            for (int i = 0; i < enemyspriteBatchs.Count; i++)
            {
                enemyspriteBatchs[i].End();
            }
            for (int i = 0; i < bulletspriteBatchs.Count; i++)
            {
                bulletspriteBatchs[i].End();
            }
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}

