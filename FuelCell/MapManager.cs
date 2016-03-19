using System;
using System.IO;
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
    /// The map manager singleton controls all systems related to the map itself.
    /// </summary>
    public static class MapManager
    {
        /// <summary>
        /// Enumeration for possible entity types in the map.
        /// </summary>
        public enum EntityType
        {
            /// <summary>
            /// Nothing.
            /// </summary>
            Empty,

            /// <summary>
            /// A static, solid block.
            /// </summary>
            Block,

            /// <summary>
            /// A collectible star.
            /// </summary>
            Star,

            /// <summary>
            /// An enemy goomba.
            /// </summary>
            Goomba,
        };

        /// <summary>
        /// A list of blocks to process for collision using collision boxes. Note that these items
        /// are not drawn.
        /// </summary>
        public static List<ANode> MapBlocks;

        /// <summary>
        /// A list of pickups to process for collision using spheres. Note that these items are
        /// not drawn.
        /// </summary>
        public static List<Model> Pickups;

        /// <summary>
        /// A list of general objects to update.
        /// </summary>
        public static List<ANode> UpdatedItems;

        /// <summary>
        /// A list of general objects to draw.
        /// </summary>
        public static List<ANode> DrawnItems;

        /// <summary>
        /// The internally stored game to operate with.
        /// </summary>
        private static Game InternalGame;

        /// <summary>
        /// The 3D generated map.
        /// </summary>
        private static EntityType[,,] Map;

        public static Point FloorTiles = new Point(72, 300);

        /// <summary>
        /// Helper method to choose an unused map slot.
        /// </summary>
        /// <param name="x">
        /// The result X index.
        /// </param>
        /// <param name="y">
        /// The result Y index.</param>
        /// <param name="z">
        /// The result Z index.</param>
        /// <param name="surface">
        /// Whether or not we should only pick positions on a solid surface.
        /// </param>
        private static void ChooseUnusedSlot(out int x, out int y, out int z, bool surface)
        {
            Random random = new Random();

            x = random.Next(0, Map.GetUpperBound(0) - 1);
            y = random.Next(0, Map.GetUpperBound(1) - 1);
            z = random.Next(0, Map.GetUpperBound(2) - 1);

            while (Map[x, y, z] != EntityType.Empty || (surface && y != 0 && Map[x, y - 1, z] != EntityType.Block))
            {
                x = random.Next(0, Map.GetUpperBound(0) - 1);
                y = random.Next(0, Map.GetUpperBound(1) - 1);
                z = random.Next(0, Map.GetUpperBound(2) - 1);
            }
        }

        /// <summary>
        /// Creates and initializes a new game map with the given width, height and depth
        /// as playable areas.
        /// </summary>
        /// <param name="game">
        /// The game to initialize with.
        /// </param>
        /// <param name="width">
        /// The width of the game world.
        /// </param>
        /// <param name="height">
        /// The height of the game world.</param>
        /// <param name="depth">
        /// The depth of the game world.
        /// </param>
        public static void CreateMap(Game game, int width, int height, int depth)
        {
            InternalGame = game;

            Map = new EntityType[width, height, depth];
            MapBlocks = new List<ANode>();
            DrawnItems = new List<ANode>();
            Pickups = new List<Model>();
            UpdatedItems = new List<ANode>();

            Texture2D marioFloor = game.Content.Load<Texture2D>("Skins/marioFloor");

            // What's the real size of our map?
            Model cubeData = new Model(game, "Shapes/cube10uR");

            Vector3 mapReal = new Vector3(width * cubeData.AbsoluteDimensions.X, height * cubeData.AbsoluteDimensions.Y, depth * cubeData.AbsoluteDimensions.Z);

            // The player should be updated before the skybox but drawn after
            UpdatedItems.Add(game.Player);


            // The Skybox has a special draw routine to draw behind everything, so it must be drawn first.
            Skybox Skybox = new Skybox(game)
            {
                Scale = new Vector3(1.0f),
                UseFog = true,
            };

            DrawnItems.Add(Skybox);
            UpdatedItems.Add(Skybox);

            MapManager.DrawnItems.Add(game.Player);

            Plane floor = new Plane(game, FloorTiles.X, FloorTiles.Y)
            {
                Texture = marioFloor,
                Scale = new Vector3(1, 1, 1),
                UseFog = true,
            };
            floor.UpdateTransformation();
            DrawnItems.Add(floor);

            // Drop a freaking castle
            Model castle = new Model(game, "Shapes/castle")
            {
                Position = new Vector3(0, -100, -320),
                UseFog = true,
            };
            castle.UpdateTransformation();
            DrawnItems.Add(castle);

            ScoreManager.CreateSign(game);

            // Begin map randomization
            Random random = new Random();

            Texture2D marioBox = game.Content.Load<Texture2D>("Skins/marioBox");
            Texture2D marioStar = game.Content.Load<Texture2D>("Skins/star");

            // Randomize our map
            int starCount = 80;
            int blockCount = 1000;

            // Place blocks
            for (int cell = 0; cell < blockCount + starCount; cell++)
            {
                int x, y, z;
                ChooseUnusedSlot(out x, out y, out z, false);

                if (cell < blockCount)
                    Map[x, y, z] = EntityType.Block;
                else
                    Map[x, y, z] = EntityType.Star;
            }

            // Place enemies
            int enemyCount = 50;

            for (int enemy = 0; enemy < enemyCount; enemy++)
            {
                int x, y, z;
                ChooseUnusedSlot(out x, out y, out z, true);

                Map[x, y, z] = EntityType.Goomba;
            }

            // Create the map
            BoundingSphere collision = new BoundingSphere(new Vector3(0, 0, 0), 16);

            Vector3 absoluteDimensions = new Vector3(cubeData.AbsoluteDimensions.X * 2, cubeData.AbsoluteDimensions.Y * 3, cubeData.AbsoluteDimensions.Z * 4);

            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    for (int z = 0; z < depth; z++)
                    {
                        Model newEntity = null;

                        switch (Map[x, y, z])
                        {
                            case EntityType.Star:
                                {
                                    newEntity = new Star(game)
                                    {
                                        Scale = new Vector3(0.07f),
                                        Sphere = collision,
                                        UseFog = true,
                                    };

                                    Pickups.Add(newEntity);
                                    DrawnItems.Add(newEntity);
                                    UpdatedItems.Add(newEntity);

                                    if (y == 0)
                                        newEntity.Position += new Vector3(0, absoluteDimensions.Y / 2, 0);

                                    break;
                                }
                            case EntityType.Goomba:
                                {
                                    newEntity = new Goomba(game)
                                    {
                                        Scale = new Vector3(0.008f),
                                        Sphere = collision,
                                        UseFog = true,
                                    };

                                    Pickups.Add(newEntity);
                                    DrawnItems.Add(newEntity);
                                    UpdatedItems.Add(newEntity);

                                    if (y == 0)
                                        newEntity.Position += new Vector3(0, absoluteDimensions.Y / 2, 0);

                                    break;
                                }
                            case EntityType.Block:
                                {
                                    newEntity = new Model(game, "Shapes/cube10uR")
                                    {
                                        Scale = new Vector3(1, 1, 1),
                                        Sphere = collision,
                                        UseFog = true,
                                    };

                                    MapBlocks.Add(newEntity);
                                    DrawnItems.Add(newEntity);
                                    break;
                                }
                        }

                        if (newEntity == null)
                            continue;

                        newEntity.Position = newEntity.Position + new Vector3(absoluteDimensions.X * x, absoluteDimensions.Y * y, absoluteDimensions.Z * z);
                        newEntity.UpdateTransformation();
            }
        }

        /// <summary>
        /// Called every frame to draw all the drawn objects to the game world.
        /// </summary>
        /// <param name="effect">
        /// The basic effect to draw with.
        /// </param>
        public static void Draw(BasicEffect effect)
        {
            foreach (ANode drawnItem in DrawnItems)
                drawnItem.Draw(effect);
        }

        /// <summary>
        /// Called every frame to update all the updated objects.
        /// </summary>
        /// <param name="time">
        /// The current game timing snapshot.
        /// </param>
        public static void Update(GameTime time)
        {
            foreach (ANode updated in UpdatedItems)
                updated.Update(time);
        }
    }
}
