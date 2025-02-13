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
    public class Chunk
    {
        public readonly Point position;
        public Tile[] Tiles { get; private set; }

        public Chunk(Point position)
        {
            Tiles = new Tile[MapManager.chunkSize * MapManager.chunkSize];
            this.position = position;
        }

        public void SetTileInChunk(Point tilePosition, Tile tile)
        {
            if (tile != null)
            {
                tile.Position = tilePosition.ToVector2() + (position.ToVector2() * MapManager.chunkSize * MapManager.tileSize);
            }

            Tiles[tilePosition.Y * MapManager.chunkSize + tilePosition.X] = tile;
        }
        public void SetTilesInChunk(TileType type)
        {
            Vector2 tempPosition;
            Tile tile;

            for (int i = 0; i < Tiles.Length; i++)
            {
                tempPosition = new Vector2(i % MapManager.chunkSize, i / MapManager.chunkSize) * MapManager.tileSize;
                tempPosition += position.ToVector2() * MapManager.chunkSize * MapManager.tileSize;
                tile = new(tempPosition, type);

                Tiles[i] = tile;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < Tiles.Length; i++)
            {
                Tiles[i]?.Draw(spriteBatch);
            }
        }
    }
}
