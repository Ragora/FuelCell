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
    /// A UI element representing a picture to be drawn somewhere on screen.
    /// </summary>
    class Picture : Element
    {
        /// <summary>
        /// The path tot he texture to load when the picture is initialized.
        /// </summary>
        private String TexturePath;

        /// <summary>
        /// The picture to be drawn.
        /// </summary>
        public Texture2D Texture;

        /// <summary>
        /// A constructor accepting a game instance and a texture path.
        /// </summary>
        /// <param name="game">
        /// The game instance to associate this new picture with.
        /// </param>
        /// <param name="texturePath">
        /// The path to the texture to be loaded and drawn.
        /// </param>
        public Picture(Microsoft.Xna.Framework.Game game, String texturePath) : base(game)
        {
            TexturePath = texturePath;
        }

        /// <summary>
        /// Initializes the picture by calling the base initialize as well as loading the
        /// texture to be drawn.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            Texture = InternalGame.Content.Load<Texture2D>(TexturePath);
        }

        /// <summary>
        /// Draws the picture to the frame buffer.
        /// </summary>
        /// <param name="batch">
        /// The sprite batch to draw to.
        /// </param>
        public override void Draw(SpriteBatch batch)
        {
            batch.Draw(Texture, Position, null, Color, Theta, Origin, Scale, SpriteEffects.None, 0);
        }
    }
}
