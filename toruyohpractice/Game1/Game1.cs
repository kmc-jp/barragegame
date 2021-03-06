﻿using CommonPart;
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

namespace CommonPart
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SceneManager scenem;
        Drawing d;

        public static double enemySkills_update_fps = 60;
        public static double enemyBullets_update_fps = 60;
        public static double fps = 30;
        public const int WindowSizeX = 1280;
        public const int WindowSizeY = 960;
        internal static readonly Vector WindowSize = new Vector(WindowSizeX, WindowSizeY);

        /// <summary>
        /// -1がnomal、1がhard
        /// </summary>
        public static int difficulty = -1;
        /// <summary>
        /// -1がarcade、1がpractice
        /// </summary>
        public static int play_mode = -1;
        public static int playerLife =5*Map.lifesPerPiece; //最初がnormalだったら 5* にする

        //倍率込みのサイズ　ふつうは扱わなくてよい　staticなのは苦しまぎれ
        public static int _WindowSizeX;
        public static int _WindowSizeY;

#if DEBUG
        [DllImport("kernel32")]
        static extern bool AllocConsole();
#endif
        public Game1()
        {
            this.Window.Title = "Barrage Game";
            this.IsMouseVisible = true;
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
            //コンソールモード
#if DEBUG
            AllocConsole();
#endif
            // TODO: Add your initialization logic here
            ChangeWindowSize(1);
            base.Initialize();
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Settings.WindowStyle = 1;
            d = new Drawing(spriteBatch, new Drawing3D(GraphicsDevice), this);
            scenem = new SceneManager(d);
            SetFPS(60);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            DataBase.Load_Contents(Content);
            // TODO: use this.Content to load your game content here

            TextureManager.Load(Content);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            SoundManager.Music.Close();
            if (DataBase.database_singleton != null)
            {
                DataBase.database_singleton.Dispose();
            }
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// 

        bool exited;
        protected void exitGame()
        {
            DataBase.database_singleton.Dispose();
        }
        protected override void Update(GameTime gameTime)//mainloop
        {
            /*
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
                */

            // TODO: Add your update logic here            
            if (!scenem.Update() && !exited) {
                exitGame();
                Exit();
                SoundManager.Music.Close();
                exited = true;
            }
            base.Update(gameTime);
            SoundManager.Update();
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// 

        protected override void Draw(GameTime gameTime)//render
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            // TODO: Add your drawing code here

            scenem.Draw();

            base.Draw(gameTime);
        }
        public void ChangeWindowSize(int style)
        {
            _WindowSizeX = 1280;
            if (style == 1) _WindowSizeY = 720;
            else _WindowSizeY = 720;

            graphics.PreferredBackBufferWidth = _WindowSizeX;
            graphics.PreferredBackBufferHeight = _WindowSizeY;
            graphics.ApplyChanges();
        }

        void SetFPS(double value)
        {
            fps = value;
            if (value < enemyBullets_update_fps) { enemyBullets_update_fps = value; }
            if (value < enemySkills_update_fps) { enemySkills_update_fps = value; }
            this.TargetElapsedTime = TimeSpan.FromSeconds(1.0 / value);

        }
    }
}


