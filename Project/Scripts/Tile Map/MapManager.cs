using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineExploration
{
    static class MapManager
    {
        public static HashSet<Point> ActiveChunks { get; private set; } = [];
        public static Dictionary<Point, Chunk> ChunkPositions { get; set; } = [];
        public const int chunkSize = 16, tileSize = 32;

        public static void Update()
        {

        }

        public static void LoadAndSetChunkTiles(Point chunk, TileType type)
        {
            SetTilesInChunk(chunk, type);
            LoadChunk(chunk);
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
