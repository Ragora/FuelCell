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
    /// The score manager deals with all scores in the game -- importing, exporting and it also manages
    /// that cute score sign you see at the beginning of the game as well as in the actual game world.
    /// </summary>
    public static class ScoreManager
    {
        /// <summary>
        /// The sign to manage.
        /// </summary>
        private static Sign ScoreSign;

        /// <summary>
        /// A dictionary mapping scores to player names.
        /// </summary>
        public static SortedDictionary<int, string> Scores;

        /// <summary>
        /// The current game session score value.
        /// </summary>
        public static int Score;

        /// <summary>
        /// Updates the text on the score sign to reflect those in the Scores
        /// sorted dictionary.
        /// </summary>
        public static void UpdateScoreSign()
        {
            string signText = "       -- LeaderBoard --\n";

            int number = 1;
            foreach (var entry in Scores.Reverse())
            {
                signText += string.Format("{0}. {1} {2}\n", number, entry.Value, entry.Key);
                ++number;
            }

            ScoreSign.Text = signText;
        }

        /// <summary>
        /// Load the scores from the scores text file.
        /// </summary>
        public static void LoadScores()
        {
            Scores = new SortedDictionary<int, string>();

            // Attempt the score board
            try
            {
                StreamReader handle = new StreamReader("scores.txt");

                while (!handle.EndOfStream)
                {
                    string player = handle.ReadLine();
                    int score = int.Parse(handle.ReadLine());
                    Scores[score] = player;
                }

                handle.Close();
            }
            catch
            {
                Scores[1337] = "Yoshi";
                Scores[900] = "Bowser";

                Scores[700] = "Peach";
                Scores[400] = "Mario";
                Scores[200] = "Luigi";
                Scores[100] = "Donkey Kong";
                Scores[50] = "Tubba Blubba";
                Scores[-500] = "Toad";
            }
        }

        /// <summary>
        /// Exports the scores in memory to a text file on disk.
        /// </summary>
        public static void UpdateScores()
        {
            // Export the scores
            StreamWriter handle = new StreamWriter("scores.txt");

            foreach (var score in ScoreManager.Scores)
            {
                handle.WriteLine(score.Value);
                handle.WriteLine(score.Key);
            }

            handle.Close();
        }

        /// <summary>
        /// Creates the game world sign as well as loads the current scores.
        /// </summary>
        /// <param name="game">
        /// The game to initialize the sign with.
        /// </param>
        public static void CreateSign(Game game)
        {
            LoadScores();

            ScoreSign = new Sign(game)
            {
                UseFog = true,

                Scale = new Vector3(1f),
                Position = new Vector3(60, 4, 60),
            };

            ScoreSign.UpdateTransformation();
            MapManager.DrawnItems.Add(ScoreSign);
            MapManager.MapBlocks.Add(ScoreSign);

            UpdateScoreSign();
        }
    }
}
