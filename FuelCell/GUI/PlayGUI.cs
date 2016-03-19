using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace FuelCell.GUI
{
    /// <summary>
    /// The PlayGUI is the GUI that is drawn when the player is currently playing the game --
    /// in control of the player object.
    /// </summary>
    class PlayGUI : GUIContext
    {
        /// <summary>
        /// The text element used to draw the score on the PlayGUI.
        /// </summary>
        private Elements.Text ScoreText;

        /// <summary>
        /// The text element used to draw the FPS on screen.
        /// </summary>
        private Elements.Text FPSText;

        /// <summary>
        /// The text element used to draw the energy on screen.
        /// </summary>
        private Elements.Text EnergyText;

        /// <summary>
        /// The text element used to draw the adrenaline on screen.
        /// </summary>
        private Elements.Text AdrenalineText;

        /// <summary>
        /// Elapsed time since the last update.
        /// </summary>
        private float ElapsedTime;

        /// <summary>
        /// How many frames have been drawn in any given second.
        /// </summary>
        private int FrameCount;

        /// <summary>
        /// The GUI to be drawn when the player is playing the game.
        /// </summary>
        /// <param name="game">
        /// The game instance to associate this new PlayGUI with.
        /// </param>
        public PlayGUI(Game game) : base(game)
        {
            ScoreText = new Elements.Text(game, "fonts/Arial")
            {
                DisplayText = "Score: <UNSET>",
                Position = new Vector2(150, 50),
                BackgroundColor = Color.Black,
            };
            AddElement(ScoreText);

            FPSText = new Elements.Text(game, "fonts/Arial")
            {
                DisplayText = "FPS: <UNSET>",
                Position = new Vector2(150, 100),
                BackgroundColor = Color.Black,
            };
            AddElement(FPSText);

            EnergyText = new Elements.Text(game, "fonts/Arial")
            {
                DisplayText = "Energy: <UNSET>",
                Position = new Vector2(150, 150),
                BackgroundColor = Color.Black,
            };
            AddElement(EnergyText);

            AdrenalineText = new Elements.Text(game, "fonts/Arial")
            {
                DisplayText = "Adrenaline: <UNSET>",
                Position = new Vector2(350, 150),
                BackgroundColor = Color.Black,
            };
            AddElement(AdrenalineText);
        }

        /// <summary>
        /// Called when the GUI is first set to be the active GUI.
        /// </summary>
        public override void OnWake()
        {
            base.OnWake();

            InputManager.StartButtonListener = OnPause;

            Game game = (Game)InternalGame;
            game.Paused = false;
            game.Player.CaptureInput(true);
            game.ActiveCamera = game.Player.OrbitingCamera;

            SoundManager.PlayMusic("play");
        }

        public override void Update(GameTime time)
        {
            base.Update(time);

            #region Framerate Counter
            ElapsedTime += (float)time.ElapsedGameTime.Milliseconds;

            if (ElapsedTime >= 1000)
            {
                FPSText.DisplayText = "FPS: " + FrameCount;
                FrameCount = 0;
                ElapsedTime = 0;
            }
            #endregion

            Game game = (Game)InternalGame;
            EnergyText.DisplayText = "Energy: " + game.Player.Energy;
            ScoreText.DisplayText = "Score: " + ScoreManager.Score;

            AdrenalineText.DisplayText = "Adrenaline: " + game.Player.Adrenaline;
        }

        public override void Draw(SpriteBatch batch)
        {
            base.Draw(batch);

            ++FrameCount;
        }

        private void OnPause(bool pressed)
        {
            if (pressed)
                GUIManager.SetGUI("pause");
        }
    }
}
