using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MineExploration
{
    public static class MapManager
    {
        public static HashSet<Point> ActiveChunks { get; private set; } = [];
        public static Dictionary<Point, Chunk> ChunkPositions { get; set; } = [];
        public const int chunkSize = 16, tileSize = 32;

        private static Point prevTopLeftCameraChunk, prevBottomRightCameraChunk;
        public static Vector2 CameraExtendViewDistance { get { return WindowManager.WindowSize * 0.2f; } }

        public static void CheckForNewActiveChunks()
        {
            Point topLeftCameraPosition = Vector2.Floor((Library.MainCamera.Position - (WindowManager.WindowSize * 0.5f) - CameraExtendViewDistance) / tileSize / chunkSize).ToPoint();
            Point bottomRightCameraPosition = Vector2.Floor((Library.MainCamera.Position + (WindowManager.WindowSize * 0.5f) + CameraExtendViewDistance) / tileSize / chunkSize).ToPoint();
            
            if (topLeftCameraPosition != prevTopLeftCameraChunk || bottomRightCameraPosition != prevBottomRightCameraChunk)
            {
                prevTopLeftCameraChunk = topLeftCameraPosition;
                prevBottomRightCameraChunk = bottomRightCameraPosition;
                
                UpdateActiveChunks(topLeftCameraPosition, bottomRightCameraPosition);
            }
        }

        private static void UpdateActiveChunks(Point topLeftCameraPosition, Point bottomRightCameraPosition)
        {
            ActiveChunks.Clear();
            
            for (int x = topLeftCameraPosition.X; x <= bottomRightCameraPosition.X; x++)
            {
                for (int y = topLeftCameraPosition.Y; y <= bottomRightCameraPosition.Y; y++)
                {
                    Point chunkPosition = new(x, y);
                    
                    if (ChunkPositions.ContainsKey(chunkPosition))
                    {
                        LoadChunk(chunkPosition);
                    }
                }
            }
        }

        public static void Test()
        {
            List<Point> chunksTest = [];

            for (int x = -15; x < 15; x++)
            {
                for (int y = -15; y < 15; y++)
                {
                    chunksTest.Add(new Point(x, y));
                }
            }

            TileType[] tileTypesTest = new TileType[chunksTest.Count];
            Array.Fill(tileTypesTest, TileType.Traversable);

            Thread thread = new(() =>
            {
                SetChunks([.. chunksTest], tileTypesTest);
            })
            {
                IsBackground = true
            };

            thread.Start();
        }

        private static void SetChunks(Point[] chunks, TileType[] tileTypes)
        {
            bool settingChunks = true;

            while (settingChunks)
            {
                for (int i = 0; i < chunks.Length; i++)
                {
                    SetTilesInChunk(chunks[i], tileTypes[i]);
                }

                settingChunks = false;
            }
        }

        /// <summary>
        /// Will get tile from the specified chunk and tile position
        /// </summary>
        /// <param name="chunkPosition"></param>
        /// <param name="tilePosition"></param>
        /// <param name="tile"></param>
        public static Tile GetTileInChunk(Point chunkPosition, Point tilePosition)
        {
            if (ChunkPositions.TryGetValue(chunkPosition, out Chunk chunk))
            {
                return chunk.Tiles[tilePosition.Y * chunkSize + tilePosition.X];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Will set 1 tile, tile position limited to a grid size of 16 x 16
        /// </summary>
        /// <param name="chunkPosition"></param>
        /// <param name="tilePosition"></param>
        /// <param name="tile"></param>
        public static void SetTileInChunk(Point chunkPosition, Point tilePosition, Tile tile)
        {
            if (ChunkPositions.TryGetValue(chunkPosition, out Chunk chunk))
            {
                chunk.SetTileInChunk(tilePosition, tile);
            }
            else
            {
                ChunkPositions.Add(chunkPosition, new Chunk(chunkPosition));

                ChunkPositions[chunkPosition].SetTileInChunk(tilePosition, tile);
            }
        }

        /// <summary>
        /// Will set the whole chunk to specified tile
        /// </summary>
        /// <param name="chunkPosition"></param>
        /// <param name="tilePosition"></param>
        /// <param name="tile"></param>
        public static void SetTilesInChunk(Point chunkPosition, TileType type)
        {
            if (ChunkPositions.TryGetValue(chunkPosition, out Chunk chunk))
            {
                chunk.SetTilesInChunk(type);
            }
            else
            {
                ChunkPositions.Add(chunkPosition, new Chunk(chunkPosition));

                ChunkPositions[chunkPosition].SetTilesInChunk(type);
            }
        }

        public static void LoadChunk(Point chunk)
        {
            ActiveChunks.Add(chunk);
        }

        public static void UnloadChunk(Point chunk)
        {
            ActiveChunks.Remove(chunk);
        }

        public static void DrawActiveChunks(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < ActiveChunks.Count; i++)
            {
                ChunkPositions[ActiveChunks.ElementAt(i)].Draw(spriteBatch);
            }
        }
    }
}
