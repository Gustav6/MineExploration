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

            ServerHandler.TryConnect("127.0.0.1", 13000, 1000);

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

            if (MouseInput.HasBeenPressed(MouseKeys.Left))
            {
                // Code below will set the tile that the mouse clicks on to null

                Point Chunk = Vector2.Floor(Camera.PositionInWorld(MouseInput.CurrentState.Position.ToVector2()) / MapManager.tileSize / MapManager.chunkSize).ToPoint();
                Vector2 tilePosition = Camera.PositionInWorld(MouseInput.CurrentState.Position.ToVector2()) / MapManager.tileSize;
                Point tilePositionInChunk = (tilePosition - Chunk.ToVector2() * MapManager.chunkSize).ToPoint();

                MapManager.SetTileInChunk(Chunk, tilePositionInChunk, null);
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
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null, null, null, Library.MainCamera.Transform);

            MapManager.DrawActiveChunks(spriteBatch);

            for (int i = 0; i < Library.localGameObjects.Count; i++)
            {
                Library.localGameObjects[i].Draw(spriteBatch);
            }

            for (int i = 0; i < Library.serverGameObjects.Keys.Count; i++)
            {
                Library.serverGameObjects.ElementAt(i).Value.Draw(spriteBatch);
            }

            spriteBatch.End();

            spriteBatch.Begin();

            spriteBatch.DrawString(TextureManager.Fonts[FontIdentifier.Text], "Local: " + Library.localGameObjects.Count, Vector2.Zero, Color.Green, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, TextureManager.SpriteLayers[SpriteLayerIdentifier.UI]);
            spriteBatch.DrawString(TextureManager.Fonts[FontIdentifier.Text], "Server: " + Library.serverGameObjects.Keys.Count, new Vector2(0, 50), Color.Green, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, TextureManager.SpriteLayers[SpriteLayerIdentifier.UI]);

            spriteBatch.End();
        }
    }
}
