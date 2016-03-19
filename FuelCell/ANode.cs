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
    /// ANode is an abstract class representing a drawable node in the 3D game space.
    /// </summary>
    public abstract class ANode
    {
        /// <summary>
        /// The bounding sphere representing collision for this node.
        /// </summary>
        public virtual BoundingSphere Sphere { set; get; }

        /// <summary>
        /// Whether or not the node should be drawn with a fog effect over top of it.
        /// </summary>
        public bool UseFog;

        /// <summary>
        /// The graphics device in use by the node.
        /// </summary>
        protected GraphicsDevice GraphicsDevice;

        /// <summary>
        /// The texture to be drawn on the node.
        /// </summary>
        public Texture2D Texture;

        /// <summary>
        /// The current transformation of the node.
        /// </summary>
        public Matrix Transformation;

        /// <summary>
        /// The game that has instantiated this node.
        /// </summary>
        protected Game Game;

        /// <summary>
        /// An virtual empty method to draw the node on screen.
        /// </summary>
        /// <param name="effect">
        /// The effect to draw with. Some objects may ignore this.
        /// </param>
        public virtual void Draw(BasicEffect effect) {  }

        /// <summary>
        /// An virtual empty method to check collision of this node against a given
        /// collision sphere.
        /// </summary>
        /// <param name="col">
        /// The collision sphere to test against.
        /// </param>
        /// <returns>
        /// True if a collision occurred, false if not.
        /// </returns>
        public virtual bool Collides(BoundingSphere col) { return false; }

        /// <summary>
        /// An virtual empty method to update the node.
        /// </summary>
        /// <param name="time">
        /// The current time snapshot to operate with.
        /// </param>
        public virtual void Update(GameTime time) {  }

        /// <summary>
        /// A constructor accepting the instantiating game.
        /// </summary>
        /// <param name="game">The game that is instantiating this node.</param>
        public ANode(Microsoft.Xna.Framework.Game game)
        {
            this.Game = (Game)game;
            this.GraphicsDevice = game.GraphicsDevice;

            this.Scale = new Vector3(1, 1, 1);
        }

        /// <summary>
        /// The current position of the node in the game world.
        /// </summary>
        public Vector3 Position;

        /// <summary>
        /// The node's rotation.
        /// </summary>
        public Vector3 Rotation;

        /// <summary>
        /// The node's scale.
        /// </summary>
        public Vector3 Scale;

        /// <summary>
        /// The node's origin. This is essentially an offset by which the node will rotate about.
        /// </summary>
        public Vector3 Origin;

        /// <summary>
        /// The node's relative rotation. If Origin != Vector3.Zero, then this serves as the model-local
        /// rotation where the regulat rotation causes the node to orbit about Origin.
        /// </summary>
        public Vector3 RelativeRotation;

        /// <summary>
        /// Method that is called internally to update the node transformation matrix.
        /// </summary>
        public void UpdateTransformation()
        {
            Transformation = Matrix.CreateFromYawPitchRoll(RelativeRotation.X, RelativeRotation.Y, RelativeRotation.Z);
            Transformation *= Matrix.CreateTranslation(Origin);
            Transformation *= Matrix.CreateFromYawPitchRoll(Rotation.X, Rotation.Y, Rotation.Z);
            Transformation *= Matrix.CreateScale(Scale);
            Transformation *= Matrix.CreateTranslation(Position);
        }
    }
}
