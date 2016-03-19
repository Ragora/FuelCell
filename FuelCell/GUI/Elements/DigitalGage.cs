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
    /// An interface element representing a digital meter of sorts.
    /// </summary>
    public class DigitalGage : Element
    {
        /// <summary>
        /// The internal percentage value that is read and written to by the Percentage
        /// property.
        /// </summary>
        private float InternalPercentage;
        
        /// <summary>
        /// The texture drawn when the digital gage's percentage is > 50%.
        /// </summary>
        private Texture2D GreenSprite;

        /// <summary>
        /// The texture drawn when the digital gage's percentage is <= 50%.
        /// </summary>
        private Texture2D YellowSprite;

        /// <summary>
        /// The texture drawn when the digital gage's percentage is <= 30%.
        /// </summary>
        private Texture2D RedSprite;

        /// <summary>
        /// The texture representing the "container" of the digital gage contents itself.
        /// </summary>
        private Texture2D BezelSprite;

        /// <summary>
        /// A boolean representing whether or not the digital gage from fill in from its "right" side or not. This right-hand
        /// association is assuming a 0* Theta. That is, it will only appear to fill in from the right side if it isn't rotated.
        /// </summary>
        public bool FillFromRight;

        /// <summary>
        /// A readable & writable property that modifies the percentage that this digital gage will draw with.
        /// The value must be in the range of 0.0f to 1.0f, or it will be clamped.
        /// </summary>
        public float Percentage
        {
            get
            {
                return InternalPercentage;
            }

            set
            {
                InternalPercentage = value >= 0.0f && value <= 1.0f ? value : 1.0f;
            }
        }

        /// <summary>
        /// A constructor accepting a game instance.
        /// </summary>
        /// <param name="game">
        /// The game instance to associate this new digital gage with.
        /// </param>
        public DigitalGage(Microsoft.Xna.Framework.Game game) : base(game)
        {
            Theta = 0;
            Percentage = 1.0f;
            FillFromRight = false;
        }

        /// <summary>
        /// Initializes the digital gage by calling the base initialize as well as loading all of the
        /// required textures for use.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            Scale = 3.0f;

            FillFromRight = true;

            this.GreenSprite = InternalGame.Content.Load<Texture2D>("Images/greenFiller");
            this.YellowSprite = InternalGame.Content.Load<Texture2D>("Images/yellowFiller");
            this.RedSprite = InternalGame.Content.Load<Texture2D>("Images/redFiller");
            this.BezelSprite = InternalGame.Content.Load<Texture2D>("Images/Bezel");
        }

        /// <summary>
        /// Draws the digital gage to the frame buffer using its current state.
        /// </summary>
        /// <param name="batch">
        /// The sprite batch to draw to.
        /// </param>
        public override void Draw(SpriteBatch batch)
        {
            Texture2D fillerSkin = GreenSprite;
            if (InternalPercentage <= 0.5f && InternalPercentage > 0.4f)
                fillerSkin = YellowSprite;
            else if (InternalPercentage <= 0.3f)
                fillerSkin = RedSprite;

            // Draw the filler
            Rectangle drawnRectangle = new Rectangle(0, 0, (int)(fillerSkin.Width * InternalPercentage), (int)fillerSkin.Height);

            if (FillFromRight)
            {
                Vector2 fillDirection = new Vector2((float)Math.Sin(Theta), (float)Math.Cos(Theta));
                Vector2 drawnPosition = new Vector2(this.Position.X * fillDirection.X, (this.Position.Y * fillDirection.Y)  + (fillerSkin.Width - drawnRectangle.Width));
                batch.Draw(fillerSkin, drawnPosition, drawnRectangle, Color.White, Theta, Origin, Scale, SpriteEffects.None, 0);
            }
            else
                batch.Draw(fillerSkin, this.Position, drawnRectangle, Color.White, Theta, Origin, Scale, SpriteEffects.None, 0);

            batch.Draw(BezelSprite, this.Position, Color.White);
        }
    }
}
