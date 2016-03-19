using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace FuelCell
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        /// <summary>
        /// The graphics device we are drawing with.
        /// </summary>
        GraphicsDeviceManager graphics;

        /// <summary>
        /// The currently active camera.
        /// </summary>
        public StaticCamera ActiveCamera;

        /// <summary>
        /// The camera used to render the sign.
        /// </summary>
        public StaticCamera SignCamera;

        /// <summary>
        /// The player object we're operating with.
        /// </summary>
        public Player Player;

        /// <summary>
        /// The render target we use to draw the 3D scene buffer to.
        /// </summary>
        RenderTarget2D SceneBuffer;

        public static Vector2 FloorTiles = new Vector2(60, 150);

        /// <summary>
        /// Is the game currently paused?
        /// </summary>
        public bool Paused;

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;
            graphics.IsFullScreen = false;

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
            Paused = true;

            // Initialize with CullMode
            #region Graphics Initialization
            RasterizerState state = new RasterizerState();
            state.CullMode = CullMode.CullCounterClockwiseFace;
            GraphicsDevice.RasterizerState = state;

            SceneBuffer = new RenderTarget2D(GraphicsDevice, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight, false, SurfaceFormat.Rgba64, DepthFormat.Depth24);
            #endregion

            #region Preinitialization of Map
            Player = new Player(this, new Vector3(60, 2, 75));
            SignCamera = new StaticCamera(this, new Vector3(60, 15, 75), new Vector3(60, 15, 60), Vector3.Up);
            ActiveCamera = SignCamera;
            #endregion

            #region Input Initialization
            IsMouseVisible = true;

            InputManager.Create(this);

            // Tell the input manager to use keyboard or controller accordingly.
            if (GamePad.GetState(PlayerIndex.One).IsConnected)
               InputManager.UseController = true;
            else
                InputManager.UseKeyboard = true;

            InputManager.MouseCapture = false;
            #endregion

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            #region Sound Initialization
            SoundManager.Create(this);
            SoundManager.Load("Sounds/menu", "menu");
            SoundManager.Load("Sounds/play", "play");
            SoundManager.Load("Sounds/lose", "lose");
            SoundManager.Load("Sounds/pause", "pause");
            SoundManager.Load("Sounds/win", "win");
            SoundManager.Load("Sounds/goomba", "goomba");
            SoundManager.Load("Sounds/star", "star");
            #endregion

            #region GUI Initialization
            GUI.GUIManager.Create(this);
            GUI.PlayGUI playGUI = new GUI.PlayGUI(this);
            GUI.GUIManager.AddGUI(playGUI, "play");
            GUI.MenuGUI menuGUI = new GUI.MenuGUI(this);
            GUI.GUIManager.AddGUI(menuGUI, "menu");
            GUI.PauseGUI pauseGUI = new GUI.PauseGUI(this);
            GUI.GUIManager.AddGUI(pauseGUI, "pause");
            GUI.ScoreGUI scoreGUI = new GUI.ScoreGUI(this);
            GUI.GUIManager.AddGUI(scoreGUI, "score");
            GUI.CreditsGUI creditsGUI = new GUI.CreditsGUI(this);
            GUI.GUIManager.AddGUI(creditsGUI, "credits");

            GUI.GUIManager.Initialize();
            GUI.GUIManager.SetGUI("menu");
            #endregion

            MapManager.CreateMap(this, 55, 10, 55);

            Player.Update(new GameTime());
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {

        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            #region Update all Managers
            InputManager.Update(gameTime);
            SoundManager.Update();

            if (!Paused)
                MapManager.Update(gameTime);

            GUI.GUIManager.Update(gameTime);
            #endregion

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            #region 3D Draw Sequence
            // Reset the graphics state since XNA breaks it for 3D ops after 2D ops
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            GraphicsDevice.SetRenderTarget(SceneBuffer);
            GraphicsDevice.Clear(Color.CornflowerBlue);

            BasicEffect effect = new BasicEffect(GraphicsDevice);

            effect.View = ActiveCamera.View;
            effect.Projection = ActiveCamera.Projection;
            effect.World = Matrix.Identity;

            MapManager.Draw(effect);
            #endregion

            #region 2D Draw Sequence
            // Now we draw the 3D buffer to the screen and all UI elements on top of it
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.CornflowerBlue);

            SpriteBatch batch = new SpriteBatch(GraphicsDevice);
            batch.Begin();
            batch.Draw(SceneBuffer, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), Color.White);

            GUI.GUIManager.Draw(batch);
            batch.End();
            #endregion

            base.Draw(gameTime);
        }
    }
}
