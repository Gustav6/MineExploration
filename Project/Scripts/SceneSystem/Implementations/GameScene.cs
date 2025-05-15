using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerToGame;

namespace MineExploration
{
    internal class GameScene : IScene
    {
        public async void LoadContent()
        {
            await ServerManager.TryToConnect("127.0.0.1", 13000, 1000);

            Library.playerInstance = (Player)Library.CreateLocalGameObject(new Player(Vector2.Zero));
            Library.MainCamera.SetTarget(Library.playerInstance);

            MapManager.Test();
        }

        public void UnloadContent()
        {

        }

        public void Update(GameTime gameTime)
        {
            if (KeyboardInput.IsPressed(Keys.F1))
            {
                if (Library.MainCamera != null)
                {
                    Library.MainCamera.Zoom -= 0.1f;
                    Debug.WriteLine("Zoom out");
                }
            }
            else if (KeyboardInput.IsPressed(Keys.F2))
            {
                if (Library.MainCamera != null)
                {
                    Library.MainCamera.Zoom += 0.1f;
                    Debug.WriteLine("Zoom in");
                }
            }

            for (int i = Library.localGameObjects.Count - 1; i >= 0; i--)
            {
                if (Library.localGameObjects[i].IsDestroyed)
                {
                    Library.localGameObjects.Remove(Library.localGameObjects[i]);

                    continue;
                }

                Library.localGameObjects[i].Update(gameTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            MapManager.DrawActiveChunks(spriteBatch);

            foreach (GameObject gameObject in Library.gameObjects.Values)
            {
                gameObject.Draw(spriteBatch);
            }
        }
    }
}
