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
    /// A UI element that happens to be clickable somewhere on the XNA screen.
    /// </summary>
    public class Button : Text
    {
        /// <summary>
        /// A delegate type used to represent what function will respond to the button being pressed.
        /// </summary>
        public delegate void ButtonResponder(Button button);

        /// <summary>
        /// The texture that is drawn when the button is in its down state.
        /// </summary>
        private Texture2D DownSprite;

        /// <summary>
        /// The texture that is drawn when the button is in its up state.
        /// </summary>
        private Texture2D UpSprite;

        public bool IsDown;
        private MouseState LastMouseState;

        /// <summary>
        /// The responder to execute when the button is pressed.
        /// </summary>
        public ButtonResponder Responder = null;
        public Color FontColor = Color.Black;

        /// <summary>
        /// The path to the image to use as the up texture.
        /// </summary>
        private String UpSpritePath;

        /// <summary>
        /// The path to the image to use as the down texture.
        /// </summary>
        private String DownSpritePath;

        /// <summary>
        /// A constructor accepting the game, font path, display text, up texture path and down texture path.
        /// </summary>
        /// <param name="game">
        /// The instance of the game to use with this new button.
        /// </param>
        /// <param name="fontPath">
        /// A path to the desired font to use.
        /// </param>
        /// <param name="displayText">
        /// The text to display on the button.
        /// </param>
        /// <param name="upPath">
        /// A path to the texture to be drawn when the button is up.
        /// </param>
        /// <param name="downPath">
        /// A path to the texture to be drawn when the button is down.
        /// </param>
        public Button(Microsoft.Xna.Framework.Game game, String fontPath, String displayText, String upPath, String downPath) : base(game, fontPath)
        {
            DisplayText = displayText;
            LastMouseState = Mouse.GetState();

            UpSpritePath = upPath;
            DownSpritePath = downPath;
        }

        /// <summary>
        /// Initializes the button by calling the base initialize as well as loading the up and down textures
        /// for use.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            DownSprite = InternalGame.Content.Load<Texture2D>(DownSpritePath);
            UpSprite = InternalGame.Content.Load<Texture2D>(UpSpritePath);
        }

        /// <summary>
        /// Draws the button to the frame buffer in its current state.
        /// </summary>
        /// <param name="batch">
        /// The sprite batch to draw to.
        /// </param>
        public override void Draw(SpriteBatch batch)
        {
            if (!Visible)
                return;

            Texture2D drawnSprite = UpSprite;

            if (IsDown)
                drawnSprite = DownSprite;

            batch.Draw(drawnSprite, Position, Color);

            // What is the center of our button?
            Vector2 centerOffset = new Vector2(DownSprite.Width / 2, DownSprite.Height / 2);
            batch.DrawString(Font, DisplayText, Position + centerOffset, FontColor, Theta, Origin, Scale, SpriteEffects.None, 0);
        }

        /// <summary>
        /// Performs an update on this button.
        /// </summary>
        /// <param name="time">
        /// The GameTime object passed in by the game's primary Update method.
        /// </param>
        public override void Update(GameTime time)
        {
            if (!Visible)
                return;

            MouseState mouse = Mouse.GetState();
            Point mousePosition = new Point(mouse.X, mouse.Y);

            // What is our rectangle?
            Rectangle rectangle = new Rectangle((int)Position.X, (int)Position.Y, (int)(DownSprite.Width * Scale), (int)(DownSprite.Height * Scale));

            if (InputManager.UseKeyboard && !IsDown && mouse.LeftButton == ButtonState.Pressed && LastMouseState.LeftButton == ButtonState.Released && rectangle.Contains(mousePosition))
                IsDown = true;
            else if (InputManager.UseKeyboard && IsDown && mouse.LeftButton == ButtonState.Released)
            {
                if (rectangle.Contains(mousePosition) && Responder != null)
                    Responder(this);

                IsDown = false;
            }

            LastMouseState = mouse;
        }
    }
}
