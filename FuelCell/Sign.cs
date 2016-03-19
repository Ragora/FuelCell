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
    /// A sign is an object in the game world we can use to render arbitrary text upon in its
    /// contents space. This sign supports new lines and whatever else XNA text supports.
    /// </summary>
    public class Sign : Model
    {
        /// <summary>
        /// The render target sandbox for us to produce sign content in.
        /// </summary>
        private RenderTarget2D SignContent;

        /// <summary>
        /// The texture rendered behind whatever content is to be drawn on the sign.
        /// </summary>
        private Texture2D BackgroundContent;

        /// <summary>
        /// The font used when rendering the textual content on the sign.
        /// </summary>
        private SpriteFont Font;

        /// <summary>
        /// The transformation matrix to use when rendering the content mesh for this sign.
        /// </summary>
        private Matrix ContentMeshTransform;

        /// <summary>
        /// A static geometry buffer for all signs to use when drawing the content as a workaround
        /// for XNA apparently having trouble drawing textures on meshes inserted via the Visual Studio
        /// model editor.
        /// </summary>
        private static VertexPositionTexture[] PlaneGeometry =
        {
            new VertexPositionTexture(new Vector3(-5, 0, -5), Vector2.Zero),
            new VertexPositionTexture(new Vector3(5, 0, -5), new Vector2(1, 0)),
            new VertexPositionTexture(new Vector3(-5, 0, 5), new Vector2(0, 1)),
            new VertexPositionTexture(new Vector3(5, 0, 5), new Vector2(1, 1)),
        };

        /// <summary>
        /// The internally stored text buffer for the text rendered on the sign.
        /// </summary>
        private string InternalText;

        /// <summary>
        /// Prperty used to read/write the sign text. Any time this property is written to,
        /// the sign redraws its contents -- so you should build your final end result into some
        /// string aside before writing to this.
        /// </summary>
        public string Text
        {
            set
            {
                InternalText = value;

                GraphicsDevice.SetRenderTarget(SignContent);
                GraphicsDevice.Clear(Color.Red);

                SpriteBatch batch = new SpriteBatch(GraphicsDevice);
                batch.Begin();
                batch.Draw(BackgroundContent, new Rectangle(0, 0, SignContent.Width, SignContent.Height), Color.White);
                batch.DrawString(Font, InternalText, Vector2.Zero, Color.White);
                batch.End();

                GraphicsDevice.SetRenderTarget(null);
            }
            get
            {
                return InternalText;
            }
        }

        /// <summary>
        /// Constructor accepting a game.
        /// </summary>
        /// <param name="game">
        /// The game to construct with.
        /// </param>
        public Sign(Game game) : base(game, "Shapes/sign")
        {
            SignContent = new RenderTarget2D(game.GraphicsDevice, 200, 200);
            BackgroundContent = game.Content.Load<Texture2D>("Skins/signContent");
            Font = game.Content.Load<SpriteFont>("Fonts/Arial");

            // Generate the transform for our geometry buffer above because XNA is retarded about texturing.
            ContentMeshTransform = Matrix.CreateTranslation(-0.2f, 0.5f, -6f);
            ContentMeshTransform *= Matrix.CreateRotationX(MathHelper.PiOver2);
            ContentMeshTransform *= Matrix.CreateScale(2f, 1.2f, 2f);

            Text = "Text\nNot\nSet";
        }

        /// <summary>
        /// Called once per frame to draw the sign in the game world.
        /// </summary>
        /// <param name="effect">
        /// The BasicEffect object to draw with.
        /// </param>
        public override void Draw(BasicEffect effect)
        {
            base.Draw(effect);

            effect.TextureEnabled = true;
            effect.World = ContentMeshTransform * Transformation;
            effect.Texture = SignContent;

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                Game.GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>(PrimitiveType.TriangleStrip, PlaneGeometry, 0, 2);
            }
        }
    }
}
