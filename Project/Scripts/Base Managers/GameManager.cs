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

            _ = ServerHandler.TryToConnect("127.0.0.1", 13000, 1000);

            Library.playerInstance = (Player)Library.CreateLocalGameObject(new Player(Vector2.Zero));
            Library.MainCamera.SetTarget(Library.playerInstance);

            //WindowManager.Fullscreen(true);

            MapManager.Test();
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
                    int idToRemove = Library.serverGameObjects[i].serverData.ID;

                    Library.serverGameObjects[i].RunOnDestroy();
                    Library.serverGameObjects.RemoveAt(i);

                    Library.serverIDGameObjectPair.Remove(idToRemove);
                }
            }

            //for (int i = Library.serverIDGameObjectPair.Keys.Count - 1; i >= 0; i--)
            {
                //if (Library.serverIDGameObjectPair.ElementAt(i).Value.IsDestroyed)
                //{
                //    Library.serverIDGameObjectPair.ElementAt(i).Value.RunOnDestroy();

                //    Library.serverGameObjects.Remove(Library.serverIDGameObjectPair.ElementAt(i).Value);
                //    Library.localGameObjects.Remove(Library.serverIDGameObjectPair.ElementAt(i).Value);

                //    Library.serverIDGameObjectPair.Remove(Library.serverIDGameObjectPair.ElementAt(i).Key);
                //}
                //else if (Library.localGameObjects.Contains(Library.serverIDGameObjectPair.ElementAt(i).Value))
                //{
                //    Library.localGameObjects.ElementAt(i).Update(gameTime);
                //}
            }
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null, null, null, Library.MainCamera.Transform);

            MapManager.DrawActiveChunks(spriteBatch);

            for (int i = 0; i < Library.localGameObjects.Count; i++)
            {
                Library.localGameObjects[i].Draw(spriteBatch);
            }

            for (int i = 0; i < Library.serverGameObjects.Count; i++)
            {
                Library.serverGameObjects[i].Draw(spriteBatch);
            }

            spriteBatch.End();

            spriteBatch.Begin();

            spriteBatch.DrawString(TextureManager.Fonts[FontIdentifier.Text], "Local game objects: " + Library.localGameObjects.Count, Vector2.Zero, Color.Green, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, TextureManager.SpriteLayers[SpriteLayerIdentifier.UI]);
            spriteBatch.DrawString(TextureManager.Fonts[FontIdentifier.Text], "Server game objects: " + Library.serverGameObjects.Count, new Vector2(0, 50), Color.Green, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, TextureManager.SpriteLayers[SpriteLayerIdentifier.UI]);

            spriteBatch.End();
        }
    }
}
