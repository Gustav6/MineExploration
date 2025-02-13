using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineExploration
{
    public class Tile(Vector2 position, TileType type)
    {
        public Vector2 Position { get; set; } = position;
        public Texture2D Texture { get; private set; } = TextureManager.TileTextures[type];
        public readonly TileType tileType = type;

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, Color.White);
        }
    }

    public enum TileType
    {
        Traversable,
        UnTraversable,
    }
}
