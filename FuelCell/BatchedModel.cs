using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FuelCell
{
    /// <summary>
    /// A batched model is class with which one may batch together the geometric primitives
    /// of arbitrary models into a single mesh at runtime. This compresses multiple draw calls
    /// to draw unchanging scenary into a single one to help reduce overhead in the GPU pipeline
    /// and process.
    /// </summary>
    public class BatchedModel : ANode
    {
        /// <summary>
        /// The actual 
        /// </summary>
        private VertexPositionTexture[] Geometry;

        /// <summary>
        /// A temporary list to store all the vertices until we're satisified with
        /// what we've built.
        /// </summary>
        private List<VertexPositionTexture> GeometryList;

        /// <summary>
        /// How many primitives are stored in this batch.
        /// </summary>
        private int PrimitiveCount;

        /// <summary>
        /// Constructor accepting an input game.
        /// </summary>
        /// <param name="game">
        /// The game to construct with.
        /// </param>
        public BatchedModel(Game game) : base(game)
        {
            PrimitiveCount = 0;
            GeometryList = new List<VertexPositionTexture>();
        }

        /// <summary>
        /// Unimplemented collision detection.
        /// </summary>
        /// <param name="col">
        /// The collision sphere to check against.
        /// </param>
        /// <returns>
        /// True if a collision occurred, false if not.
        /// </returns>
        public override bool Collides(BoundingSphere col)
        {
            throw new NotImplementedException();
        }

        public override void Draw(BasicEffect effect)
        {
            if (Geometry == null)
                return;

            effect.TextureEnabled = true;

            effect.FogEnabled = UseFog;
            effect.FogStart = 150;
            effect.FogEnd = 1000;
            Vector3 color = new Vector3(100, 149, 237);
            color.Normalize();
            effect.FogColor = color;

          //  effect.Texture = Texture;
         //   effect.World = Transformation;
            effect.World = Matrix.Identity;

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                Game.GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>(PrimitiveType.TriangleStrip, Geometry, 0, PrimitiveCount);
            }
        }

        /// <summary>
        /// Add a model to the batch by asset name using the given transformation.
        /// </summary>
        /// <param name="name">The asset name of the model.</param>
        /// <param name="transform">The transformation to use.</param>
        public void AddModel(string name, Matrix transform)
        {
            AddModel(Game.Content.Load<Microsoft.Xna.Framework.Graphics.Model>(name), transform);
        }

        /// <summary>
        /// Add a model to the batch with the given transformation.
        /// </summary>
        /// <param name="model">The model to add.</param>
        /// <param name="transform">The transformation to use.</param>
        public void AddModel(Microsoft.Xna.Framework.Graphics.Model model, Matrix transform)
        {
            foreach (ModelMesh mesh in model.Meshes)
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    VertexBuffer geometry = meshPart.VertexBuffer;

                    VertexPositionTexture[] geometryBuffer = new VertexPositionTexture[geometry.VertexCount];
                    geometry.GetData<VertexPositionTexture>(geometryBuffer);

                    foreach (VertexPositionTexture vertex in geometryBuffer)
                    {
                        Vector3 transformedVertex = Vector3.Transform(vertex.Position, transform);
                        GeometryList.Add(new VertexPositionTexture(transformedVertex, vertex.TextureCoordinate));
                    }

                    PrimitiveCount += meshPart.PrimitiveCount;
                }

        }

        /// <summary>
        /// Called when you're done adding geometry to the batch -- this finalizes the geometry
        /// and actually builds the buffer used for draw operations.
        /// </summary>
        public void ConstructGeometry()
        {
            Geometry = GeometryList.ToArray();
            GeometryList = null;
        }
    }
}
