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

namespace FuelCell.GUI.Elements
{
    /// <summary>
    /// A UI element representing text to be drawn.
    /// </summary>
    public class Text : Element
    {
        /// <summary>
        /// A resource path to the font to be used when drawing the text.
        /// </summary>
        private string FontPath;

        /// <summary>
        /// The text to be drawn.
        /// </summary>
        public string DisplayText;

        /// <summary>
        /// The font to use when drawing the text.
        /// </summary>
        public SpriteFont Font;

        /// <summary>
        /// The color drawn behind the text.
        /// </summary>
        public Color BackgroundColor;

        /// <summary>
        /// The texture used for drawing the background.
        /// </summary>
        private Texture2D BackgroundTexture;

        /// <summary>
        /// A constructor accepting a game and a font path.
        /// </summary>
        /// <param name="game">
        /// The game instance to associate this text with.
        /// </param>
        /// <param name="fontPath">
        /// The resource path for the font to be used when drawing
        /// the text.
        /// </param>
        public Text(Microsoft.Xna.Framework.Game game, String fontPath) : base(game)
        {
            FontPath = fontPath;
            DisplayText = "<UNSET>";
            BackgroundColor = new Color(0, 0, 0, 0);
        }

        /// <summary>
        /// Initializes the text by calling the base initialize as well as loading the
        /// requested font.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            Font = InternalGame.Content.Load<SpriteFont>(FontPath);
            BackgroundTexture = InternalGame.Content.Load<Texture2D>("Images/white");
        }

        /// <summary>
        /// Draws the text to the frame buffer.
        /// </summary>
        /// <param name="batch">
        /// The sprite batch to draw to.
        /// </param>
        public override void Draw(SpriteBatch batch)
        {
            Vector2 stringDimensions = Font.MeasureString(DisplayText);

            batch.Draw(BackgroundTexture, new Rectangle((int)Position.X, (int)Position.Y, (int)stringDimensions.X, (int)stringDimensions.Y), BackgroundColor);
            batch.DrawString(Font, DisplayText, Position, Color, Theta, Origin, Scale, SpriteEffects.None, 0);
        }
    }
}
