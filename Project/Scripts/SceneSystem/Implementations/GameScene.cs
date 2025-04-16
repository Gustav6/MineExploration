using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineExploration
{
    internal class GameScene : IScene
    {
        public void LoadContent()
        {
            _ = ServerHandler.TryToConnect("127.0.0.1", 13000, 1000);

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
                    Library.localGameObjects[i].RunOnDestroy();
                    Library.localGameObjects.RemoveAt(i);
                    continue;
                }

                Library.localGameObjects[i].Update(gameTime);
            }

            for (int i = Library.serverGameObjects.Count - 1; i >= 0; i--)
            {
                if (Library.serverGameObjects[i].IsDestroyed)
                {
                    int identificationToDestroy = Library.serverGameObjects[i].serverData.identification;

                    Library.serverGameObjects[i].RunOnDestroy();
                    Library.serverGameObjects.RemoveAt(i);

                    Library.IdentificationToGameObject.Remove(identificationToDestroy);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            MapManager.DrawActiveChunks(spriteBatch);

            for (int i = 0; i < Library.localGameObjects.Count; i++)
            {
                Library.localGameObjects[i].Draw(spriteBatch);
            }

            for (int i = 0; i < Library.serverGameObjects.Count; i++)
            {
                Library.serverGameObjects[i].Draw(spriteBatch);
            }
        }
    }
}
