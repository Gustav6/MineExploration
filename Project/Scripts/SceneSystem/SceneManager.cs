using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineExploration
{
    public class SceneManager
    {
        public static readonly Dictionary<Scene, IScene> sceneRegistry = new()
        {
            { Scene.Main, new ServerSelectionScene() },
            { Scene.Game, new GameScene() }
        };

        public static IScene CurrentScene { get; private set; }

        public static void ChangeScene(IScene newScene)
        {
            CurrentScene?.UnloadContent();
            CurrentScene = newScene;
            CurrentScene.LoadContent();
        }

        public static void Update(GameTime gameTime)
        {
            CurrentScene?.Update(gameTime);
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            CurrentScene?.Draw(spriteBatch);
        }
    }

    public enum Scene
    {
        Main,
        Game
    }
}
