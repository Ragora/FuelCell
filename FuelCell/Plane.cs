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
    /// The plane is an efficient variable-dimensions repeating floor implementation that also prevents
    /// texture stretching from the mere scaling of geometry.
    /// </summary>
    public class Plane : ANode
    {
        /// <summary>
        /// How many primitives exist in the plane.
        /// </summary>
        private int PrimitiveCount;

        /// <summary>
        /// The geometry buffer to draw with.
        /// </summary>
        private VertexPositionTexture[] Geometry;

        /// <summary>
        /// Constructor acceping a game, width and height.
        /// </summary>
        /// <param name="game">
        /// The game to initialize with.
        /// </param>
        /// <param name="width">
        /// The width in 15x15 tiles to build.
        /// </param>
        /// <param name="height">
        /// The height in 15x15 tiles to build.
        /// </param>
        public Plane(Microsoft.Xna.Framework.Game game, int width, int height) : base(game)
        {
            PrimitiveCount = (width * height) * 6;
            Geometry = new VertexPositionTexture[PrimitiveCount];

            Vector3[] positions = new Vector3[]
            {
                new Vector3(0, 0, 0),
                new Vector3(15, 0, 15),
                new Vector3(0, 0, 15),

                new Vector3(15, 0, 0),
                new Vector3(15, 0, 15),
                new Vector3(0, 0, 0),
            };

            // Generate foreach row
            int index = 0;

            for (int row = 0; row < height; row++)
                for (int column = 0; column < width; column++)
                {
                    Vector3 offset = new Vector3(column * 15, 0, row * 15);

                    Geometry[index] = new VertexPositionTexture(positions[0] + offset, Vector2.Zero);
                    Geometry[index + 1] = new VertexPositionTexture(positions[1] + offset, new Vector2(1, 0));
                    Geometry[index + 2] = new VertexPositionTexture(positions[2] + offset, new Vector2(0, 1));

                    Geometry[index + 3] = new VertexPositionTexture(positions[3] + offset, -new Vector2(0, 1));
                    Geometry[index + 4] = new VertexPositionTexture(positions[4] + offset, Vector2.Zero);
                    Geometry[index + 5] = new VertexPositionTexture(positions[5] + offset, -new Vector2(1, 0));

                    index += 6;
                }
        }

        /// <summary>
        /// Called each frame to draw the plane to the game world.
        /// </summary>
        /// <param name="effect">
        /// The basic effect to draw with.
        /// </param>
        public override void Draw(BasicEffect effect)
        {
            effect.TextureEnabled = true;

            effect.FogEnabled = UseFog;
            effect.FogStart = 150;
            effect.FogEnd = 1000;
            Vector3 color = new Vector3(100, 149, 237);
            color.Normalize();
            effect.FogColor = color;

            effect.Texture = Texture;
            effect.World = Transformation;

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                Game.GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>(PrimitiveType.TriangleList, Geometry, 0, PrimitiveCount / 6);
            }
        }
    }
}
