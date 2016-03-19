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
    /// A model class represents a drawable model in the game world whose geometry and other data
    /// is sourced from a .x or a .fbx file.
    /// </summary>
    public class Model : ANode
    {
        /// <summary>
        /// The model to be drawn.
        /// </summary>
        public Microsoft.Xna.Framework.Graphics.Model Rendered;

        /// <summary>
        /// A delegate representing a collision response method.
        /// </summary>
        /// <param name="collided">
        /// The model that was collided with.
        /// </param>
        public delegate void OnCollideResponder(Model collided);

        /// <summary>
        /// A list of collision response handlers to call.
        /// </summary>
        public List<OnCollideResponder> OnCollideResponders;

        /// <summary>
        /// A dictionary of materials to render for a given mesh in the model.
        /// </summary>
        public Dictionary<int, Texture2D> Materials;

        /// <summary>
        /// The internal bounding sphere to be used when a custom collision sphere is
        /// necessary.
        /// </summary>
        private BoundingSphere? InternalSphere;

        /// <summary>
        /// A property that returns the manually specified sphere or the bounding sphere
        /// of the first mesh on file. When set, the internally stored sphere is used. If
        /// set to null, then the first mesh in the model is returned on any get thereafter.
        /// </summary>
        public override BoundingSphere Sphere
        {
            get
            {
                BoundingSphere result = Rendered.Meshes[0].BoundingSphere;

                if (InternalSphere != null)
                    result = (BoundingSphere)InternalSphere;

                return result.Transform(Matrix.CreateTranslation(Position));
            }
            set
            {
                InternalSphere = value;
            }
        }

        /// <summary>
        /// Overriding position property to automatically update the collision
        /// box.
        /// </summary>
        public new Vector3 Position
        {
            set
            {
                base.Position = value;
                CollisionBox = CollisionBox;
            }
            get
            {
                return base.Position;
            }
        }

        /// <summary>
        /// Internally stored bounding box to be used as the collision box.
        /// </summary>
        private BoundingBox InternalBox;

        /// <summary>
        /// Property used to set and get the collision box of this model.
        /// </summary>
        public BoundingBox CollisionBox
        {
            get
            {
                return InternalBox;
            }
            set
            {
                InternalBox = value;

                InternalBox.Min += Position;
                InternalBox.Max += Position;
            }
        }

        private Vector3 InternalDimensions;
        public Vector3 AbsoluteDimensions
        { 
            get
            {
                return InternalDimensions;
            }
        }

        /// <summary>
        /// Invoke all collision response handlers.
        /// </summary>
        public void OnCollide()
        {
            foreach (OnCollideResponder responder in OnCollideResponders)
                responder(this);
        }

        /// <summary>
        /// A constructor accepting the game and fileName as input. These parameters are an
        /// absolute necessicity when instantiating a model at the very least, therefore they
        /// are not initializer list parameters.
        /// </summary>
        /// <param name="game">The game instantiating this model.</param>
        /// <param name="shapeName">The model file to use as geometry source.</param>
        public Model(Microsoft.Xna.Framework.Game game, string shapeName) : base(game)
        {
            Materials = new Dictionary<int, Texture2D>();

            Rendered = game.Content.Load<Microsoft.Xna.Framework.Graphics.Model>(shapeName);
            InternalSphere = null;

            OnCollideResponders = new List<OnCollideResponder>();

            // Process for dimensions
            // Only one mesh with one material so we just use the first mesh and its first mesh part to figure out model dimensions
            ModelMeshPart modelData = Rendered.Meshes[0].MeshParts[0];
            VertexBuffer geometry = modelData.VertexBuffer;

            Vector3[] geometryBuffer = new Vector3[geometry.VertexCount];
            geometry.GetData<Vector3>(geometryBuffer);

            float minX = float.MaxValue;
            float minY = float.MaxValue;
            float minZ = float.MaxValue;
            float maxX = float.MinValue;
            float maxY = float.MinValue;
            float maxZ = float.MinValue;

            foreach (Vector3 vertex in geometryBuffer)
            {
                // Get min and max values
                if (vertex.X < minX)
                    minX = vertex.X;
                if (vertex.Y < minY)
                    minY = vertex.Y;
                if (vertex.Z < minZ)
                    minZ = vertex.Z;

                if (vertex.X > maxX)
                    maxX = vertex.X;
                if (vertex.Y > maxY)
                    maxY = vertex.Y;
                if (vertex.Z > maxZ)
                    maxZ = vertex.Z;
            }

            Vector3 min = new Vector3(minX, minY, minZ);
            Vector3 max = new Vector3(maxX / 2, maxY, maxZ / 2);

            CollisionBox = new BoundingBox(min, max);

            // We multiply the dimensions here to account for bounding sphere inaccuracies
            InternalDimensions = new Vector3((Math.Abs(maxX) - Math.Abs(minX)),
                Math.Abs(maxY) - Math.Abs(minY),
                Math.Abs(maxZ) - Math.Abs(minZ));
            InternalDimensions = new Vector3(Math.Abs(InternalDimensions.X), Math.Abs(InternalDimensions.Y), Math.Abs(InternalDimensions.Z));
        }

        /// <summary>
        /// An empty update method to be overwritten on child classes.
        /// </summary>
        /// <param name="time">
        /// A snapshot of the current timing values.
        /// </param>
        public override void Update(GameTime time)
        {

        }

        /// <summary>
        /// Draws the model to the game world.
        /// </summary>
        /// <param name="effect">
        /// The basic effect to draw with. This parameter is ignored for models, its used by other
        /// drawn objects though.
        /// </param>
        public override void Draw(BasicEffect effect)
        {
            Matrix[] transforms = new Matrix[Rendered.Bones.Count];
            Rendered.CopyAbsoluteBoneTransformsTo(transforms);

            int material = 0;
            foreach (ModelMesh mesh in Rendered.Meshes)
            {
                foreach (BasicEffect meshEffect in mesh.Effects)
                {
                    meshEffect.EnableDefaultLighting();

                    meshEffect.FogEnabled = UseFog;
                    meshEffect.FogStart = 150;
                    meshEffect.FogEnd = 1000;
                    Vector3 color = new Vector3(100, 149, 237);
                    color.Normalize();
                    meshEffect.FogColor = color;
                    
                    if (Materials.ContainsKey(material))
                        meshEffect.Texture = Materials[material];

                    meshEffect.View = Game.ActiveCamera.View;
                    meshEffect.Projection = Game.ActiveCamera.Projection;
                    meshEffect.World = transforms[mesh.ParentBone.Index] * Transformation;
                }
                mesh.Draw();

                ++material;
            }
        }

        /// <summary>
        /// Tests whether or not this model collides with the given sphere.
        /// </summary>
        /// <param name="col">
        /// The bounding sphere to test against.
        /// </param>
        /// <returns>
        /// True when a collision is present, false otherwise.
        /// </returns>
        public override bool Collides(BoundingSphere col)
        {
            if (Sphere.Transform(Matrix.CreateTranslation(Position)).Intersects(col))
                return true;

            return false;
        }
    }
}
