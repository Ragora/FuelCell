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


namespace FuelCell
{
    /// <summary>
    /// A skybox is a box in the sky that is used to render the edges of the universe (that the player sees
    /// anyway) with pretty textures.
    /// </summary>
    public class Skybox : Model
    {
        /// <summary>
        /// Constructor accepting a game.
        /// </summary>
        /// <param name="game">
        /// The game to initialize with.
        /// </param>
        public Skybox(Game game) : base(game, "Shapes/skybox")
        {
            Materials[0] = game.Content.Load<Texture2D>("Skins/mskyvert");
            Materials[1] = game.Content.Load<Texture2D>("Skins/msky");
            Materials[2] = game.Content.Load<Texture2D>("Skins/msky");
            Materials[3] = game.Content.Load<Texture2D>("Skins/msky");
            Materials[4] = game.Content.Load<Texture2D>("Skins/msky");
            Materials[5] = game.Content.Load<Texture2D>("Skins/mskyvert");
        }

        /// <summary>
        /// Update method called once for every tick.
        /// </summary>
        /// <param name="time">
        /// The snapshot of the current game timing values.
        /// </param>
        public override void Update(GameTime time)
        {
            base.Update(time);

            Position = Game.Player.ActiveCamera.Position;
            UpdateTransformation();
        }

        /// <summary>
        /// Draw the Skybox to this frame. Note that the skybox must be drawn before everything
        /// else so that the skybox draws behind everything correctly.
        /// </summary>
        /// <param name="effect">
        /// The basic effect to draw with.
        /// </param>
        public override void Draw(BasicEffect effect)
        {
            GraphicsDevice.DepthStencilState = DepthStencilState.None;
            base.Draw(effect);
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        }
    }
}
