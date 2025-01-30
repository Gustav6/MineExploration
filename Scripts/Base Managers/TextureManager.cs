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
        public static Dictionary<TextureIdentifier, Texture2D> Textures { get; private set; }
        public static Dictionary<FontIdentifier, SpriteFont> Fonts { get; private set; }
        public static Dictionary<SpriteLayerIdentifier, float> SpriteLayers { get; private set; }

        public static void LoadTextures(ContentManager content)
        {
            /*
            Textures = new Dictionary<TextureIdentifier, Texture2D>()
            {
                { TextureIdentifier.Building, content.Load<Texture2D>("GameObjects/Buildings/stronghold") },
                { TextureIdentifier.Troop, content.Load<Texture2D>("GameObjects/Troops/archer") }
            };

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
    Building,
    Troop,
}

public enum SpriteLayerIdentifier
{
    Default,
    Building,
    Troop,
    Hitbox
}
