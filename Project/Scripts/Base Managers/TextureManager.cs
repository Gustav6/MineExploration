using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineExploration
{
    public static class TextureManager
    {
        public static Dictionary<TileType, Texture2D> TileTextures { get; private set; }
        public static Dictionary<TextureIdentifier, Texture2D> Textures { get; private set; }
        public static Dictionary<FontIdentifier, SpriteFont> Fonts { get; private set; }
        public static Dictionary<SpriteLayerIdentifier, float> SpriteLayers { get; private set; }

        public static void LoadTextures(ContentManager content)
        {
            Textures = new Dictionary<TextureIdentifier, Texture2D>()
            {
                { TextureIdentifier.Player, CreateTexture(64, 128, paint => Color.LawnGreen) },
                { TextureIdentifier.Enemy, CreateTexture(64, 128, paint => Color.Red) },
            };

            TileTextures = new Dictionary<TileType, Texture2D>()
            {
                { TileType.Traversable, CreateTexture(32, 32, paint => Color.White) },
                { TileType.UnTraversable, CreateTexture(32, 32, paint => Color.Black) }
            };

            SpriteLayers = new Dictionary<SpriteLayerIdentifier, float>()
            {
                { SpriteLayerIdentifier.Default, 0.5f },
                { SpriteLayerIdentifier.Player, 0.6f },
                { SpriteLayerIdentifier.Hitbox, 0.7f }
            };

            /*
            Fonts = new Dictionary<FontIdentifier, SpriteFont>()
            {
                { FontIdentifier.DamageNumber, content.Load<SpriteFont>("Fonts/FontTest") },
                { FontIdentifier.Text, content.Load<SpriteFont>("Fonts/FontTest") }
            };
            */
        }

        private static Texture2D CreateTexture(int width, int height, Func<int, Color> paint)
        {
            Texture2D texture = new(Game1.Graphics.GraphicsDevice, width, height);

            Color[] colorArray = new Color[width * height];

            for (int pixel = 0; pixel < colorArray.Length; pixel++)
            {
                colorArray[pixel] = paint(pixel);
            }

            texture.SetData(colorArray);

            return texture;
        }
    }
}

public enum FontIdentifier
{
    DamageNumber,
    Text
}

public enum TextureIdentifier
{
    Player,
    Enemy
}

public enum SpriteLayerIdentifier
{
    Default,
    Player,
    Hitbox
}
