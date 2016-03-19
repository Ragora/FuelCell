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
    class MenuGUI : GUIContext
    {
        /// <summary>
        /// The GUI to be drawn when the player is playing the game.
        /// </summary>
        /// <param name="game">
        /// The game instance to associate this new PlayGUI with.
        /// </param>
        public MenuGUI(Game game) : base(game)
        {
            GUI.Elements.Button button = new GUI.Elements.Button(game, "Fonts/Arial", "Play", "Images/button_up", "Images/button_down")
            {
                Position = new Vector2(50, 490),
                Responder = OnPlay,
            };
            AddElement(button);

            button = new GUI.Elements.Button(game, "Fonts/Arial", "Credits", "Images/button_up", "Images/button_down")
            {
                Position = new Vector2(50, 590),
                Responder = responder => GUI.GUIManager.SetGUI("credits"),
            };
            AddElement(button);

            button = new GUI.Elements.Button(game, "Fonts/Arial", "Exit", "Images/button_up", "Images/button_down")
            {
                Position = new Vector2(50, 690),
                Responder = OnExit,
            };
            AddElement(button);

            GUI.Elements.Picture logo = new GUI.Elements.Picture(game, "Images/logo")
            {
                Position = new Vector2(250, 10),
            };
            AddElement(logo);
        }

        #region GUI Responders
        /// <summary>
        /// Called when the GUI is first set to be the active GUI.
        /// </summary>
        public override void OnWake()
        {
            GUIManager.BindControllerListeners();

            SoundManager.PlayMusic("menu");

            Game game = (Game)InternalGame;
            game.ActiveCamera = game.SignCamera;

            base.OnWake();
        }

        private void OnExit(GUI.Elements.Button button)
        {
            InternalGame.Exit();
        }

        private void OnPlay(GUI.Elements.Button button)
        {
            GUIManager.SetGUI("play");

            Game game = (Game)InternalGame;
            game.Player.CaptureInput(true);
        }
        #endregion
    }
}
