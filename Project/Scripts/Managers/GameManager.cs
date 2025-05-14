using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ServerToGame;
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
        }
    }
}
