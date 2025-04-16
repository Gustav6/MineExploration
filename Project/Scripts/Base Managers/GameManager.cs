using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MineExploration
{
    public static class GameManager
    {
        public static void Initialize()
        {
            Library.MainCamera = new Camera();

            WindowManager.ChangeSize(800, 480);

            SceneManager.ChangeScene(SceneManager.sceneRegistry[Scene.Game]);
        }

        public static void LoadContent(ContentManager content)
        {
            TextureManager.LoadTextures(content);
        }

        public static void Update(GameTime gameTime)
        {
            TransitionSystem.UpdateTransitions(gameTime);
            TimedEventSystem.UpdateTimers(gameTime);
            InputManager.UpdateInputStates();
            SceneManager.Update(gameTime);
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null, null, null, Library.MainCamera.Transform);

            SceneManager.Draw(spriteBatch);

            spriteBatch.End();

            spriteBatch.Begin();

            spriteBatch.DrawString(TextureManager.Fonts[FontIdentifier.Text], "Local game objects: " + Library.localGameObjects.Count, Vector2.Zero, Color.Green, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, TextureManager.SpriteLayers[SpriteLayerIdentifier.UI]);
            spriteBatch.DrawString(TextureManager.Fonts[FontIdentifier.Text], "Server game objects: " + Library.serverGameObjects.Count, new Vector2(0, 50), Color.Green, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, TextureManager.SpriteLayers[SpriteLayerIdentifier.UI]);

            spriteBatch.End();
        }
    }
}
