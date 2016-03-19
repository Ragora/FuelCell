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
    /// The ScoreGUI is displayed when the player has completed the game and is attempting to input their
    /// initials on the score board or is otherwise looking at the scores with no such luck to have beaten
    /// anyone.
    /// </summary>
    class ScoreGUI : GUIContext
    {
        /// <summary>
        /// The first button used for name entry.
        /// </summary>
        GUI.Elements.Button FirstButton;

        /// <summary>
        /// The second button used for name entry.
        /// </summary>
        GUI.Elements.Button SecondButton;

        /// <summary>
        /// The third button used for name entry.
        /// </summary>
        GUI.Elements.Button ThirdButton;

        /// <summary>
        /// The button used to signal that the player is done with the name entry.
        /// </summary>
        GUI.Elements.Button DoneButton;

        /// <summary>
        /// The text used to display your game play result.
        /// </summary>
        GUI.Elements.Text ResultText;

        /// <summary>
        /// The text used to display your actual score value.
        /// </summary>
        GUI.Elements.Text ScoreText;

        /// <summary>
        /// The GUI to be drawn when the player ha
        /// </summary>
        /// <param name="game">
        /// The game instance to associate this new PlayGUI with.
        /// </param>
        public ScoreGUI(Game game) : base(game)
        {
            FirstButton = new GUI.Elements.Button(game, "Fonts/Arial", "A", "Images/button_up", "Images/button_down")
            {
                Position = new Vector2(150, 110),
                Responder = OnLetterCycle,
            };
            AddElement(FirstButton);

            SecondButton = new GUI.Elements.Button(game, "Fonts/Arial", "A", "Images/button_up", "Images/button_down")
            {
                Position = new Vector2(350, 110),
                Responder = OnLetterCycle,
               
            };
            AddElement(SecondButton);

            ThirdButton = new GUI.Elements.Button(game, "Fonts/Arial", "A", "Images/button_up", "Images/button_down")
            {
                Position = new Vector2(550, 110),
                Responder = OnLetterCycle,
            };
            AddElement(ThirdButton);

            DoneButton = new GUI.Elements.Button(game, "Fonts/Arial", "Done", "Images/button_up", "Images/button_down")
            {
                Position = new Vector2(750, 450),
                Responder = OnScoreDone,
            };
            AddElement(DoneButton);

            ResultText = new GUI.Elements.Text(game, "Fonts/Arial")
            {
                DisplayText = "<UNSET>",
                Position = new Vector2(250, 40),
                BackgroundColor = Color.Black,
            };
            AddElement(ResultText);

            ScoreText = new GUI.Elements.Text(game, "Fonts/Arial")
            {
                DisplayText = "<UNSET>",
                Position = new Vector2(250, 70),
                BackgroundColor = Color.Black,
            };
            AddElement(ScoreText);
        }

        #region GUI Responders
        private void OnScoreDone(GUI.Elements.Button button)
        {
            GUI.GUIManager.SetGUI("menu");

            ScoreManager.UpdateScores();
            ScoreManager.Score = 0;
        }

        private void OnLetterCycle(GUI.Elements.Button button)
        {
            char letter = button.DisplayText[0];

            ++letter;
            letter = letter > 90 ? 'A' : letter;

            button.DisplayText = string.Format("{0}", letter);

            // Assemble the name name
            string name = string.Format("{0}{1}{2}", FirstButton.DisplayText, SecondButton.DisplayText, ThirdButton.DisplayText);

            ScoreManager.Scores[ScoreManager.Score] = name;
            ScoreManager.UpdateScoreSign();
        }

        /// <summary>
        /// Called when the GUI is first set to be the active GUI.
        /// </summary>
        public override void OnWake()
        {
            Game game = (Game)InternalGame;
            game.Paused = true;

            game.Player.CaptureInput(false);
            GUIManager.BindControllerListeners();

            MapManager.MapBlocks.Clear();
            MapManager.DrawnItems.Clear();

            ScoreText.DisplayText = "Score: " + ScoreManager.Score;

            game.Player.Energy = 100;
            game.Player.Adrenaline = 100;
            game.Player.Position = new Vector3(60, 2, 75);
            game.ActiveCamera = game.SignCamera;

            MapManager.CreateMap(game, 55, 10, 55);

            // Does our current score mandate a name entry?
            bool foundBeatenScore = false;
            int beatenScore = -1;

            foreach (var score in ScoreManager.Scores.Reverse())
                if (ScoreManager.Score > score.Key)
                {
                    beatenScore = score.Key;
                    foundBeatenScore = true;
                    break;
                }

            FirstButton.Visible = SecondButton.Visible = ThirdButton.Visible = foundBeatenScore;
            if (!foundBeatenScore)
            {
                ResultText.DisplayText = "You didn't make it to the score board, sorry!";

                GUI.GUIManager.SelectedButton = DoneButton;
                DoneButton.IsDown = true;

                SoundManager.PlayMusic("lose");
            }
            else
            {
                ResultText.DisplayText = "You made it on the score board! Use the buttons below to type your name!";

                ScoreManager.Scores.Remove(beatenScore);
                ScoreManager.Scores[ScoreManager.Score] = "AAA";
                ScoreManager.UpdateScoreSign();

                SoundManager.PlayMusic("win");
            }

           base.OnWake();
        }
        #endregion
    }
}
