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
    class PauseGUI : GUIContext
    {
        /// <summary>
        /// The GUI to be drawn when the player is playing the game.
        /// </summary>
        /// <param name="game">
        /// The game instance to associate this new PlayGUI with.
        /// </param>
        public PauseGUI(Game game) : base(game)
        {
            GUI.Elements.Picture background = new GUI.Elements.Picture(game, "Images/white")
            {
                Scale = 1400,
                Color = new Color(0, 0, 0, 180),
            };
            AddElement(background);

            GUI.Elements.Text pauseText = new GUI.Elements.Text(game, "Fonts/Arial")
            {
                DisplayText = "** PAUSED **\nPress Start Again to Continue",
                Scale = 2.0f,
                Position = new Vector2(320, 300),
            };
            AddElement(pauseText);
        }

        #region GUI Responders
        /// <summary>
        /// Called when the GUI is first set to be the active GUI.
        /// </summary>
        public override void OnWake()
        {
            InputManager.StartButtonListener = OnUnpause;

            Game game = (Game)InternalGame;
            game.Paused = true;

            SoundManager.PlayMusic("pause");
            base.OnWake();
        }

        public void OnUnpause(bool pressed)
        {
            if (pressed)
                GUI.GUIManager.SetGUI("play");
        }
        #endregion
    }
}
