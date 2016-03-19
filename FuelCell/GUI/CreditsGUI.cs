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
using System.IO;

namespace FuelCell.GUI
{
    /// <summary>
    /// GUI used to display the credits.
    /// anyone.
    /// </summary>
    class CreditsGUI : GUIContext
    {
        /// <summary>
        /// The GUI to be drawn when the player ha
        /// </summary>
        /// <param name="game">
        /// The game instance to associate this new PlayGUI with.
        /// </param>
        public CreditsGUI(Game game) : base(game)
        {
            GUI.Elements.Text myText = new GUI.Elements.Text(game, "Fonts/Arial")
            {
                Position = new Vector2(200, 100),
                BackgroundColor = Color.Black,
                DisplayText = "Programming Work: Robert MacGregor",
            };
            AddElement(myText);

            GUI.Elements.Text nintendoText = new GUI.Elements.Text(game, "Fonts/Arial")
            {
                Position = new Vector2(200, 150),
                BackgroundColor = Color.Black,
                DisplayText = "Super Mario 64, Mario and the various Assets are trademark of Nintendo",
            };
            AddElement(nintendoText);

            GUI.Elements.Button backButton = new GUI.Elements.Button(game, "Fonts/Arial", "Back", "Images/button_up", "Images/button_down")
            {
                Position = new Vector2(50, 400),
                Responder = button => GUI.GUIManager.SetGUI("menu"),
            };
            AddElement(backButton);
        }
    }
}
