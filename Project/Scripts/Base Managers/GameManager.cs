using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
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
    public static class GameManager
    {
        public static void Initialize()
        {
            Player player = (Player)Library.CreateGameObject(new Player(Vector2.Zero));
            Library.MainCamera = new Camera(player);

            WindowManager.ChangeSize(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);

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

            for (int i = Library.gameObjects.Count - 1; i >= 0; i--)
            {
                if (Library.gameObjects[i].IsDestroyed)
                {
                    Library.gameObjects[i].RunOnDestroy();
                    Library.gameObjects.RemoveAt(i);
                    continue;
                }

                Library.gameObjects[i].Update(gameTime);
            }
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null, null, null, Library.MainCamera.Transform);

            MapManager.DrawActiveChunks(spriteBatch);

            for (int i = 0; i < Library.gameObjects.Count; i++)
            {
                Library.gameObjects[i].Draw(spriteBatch);
            }

            spriteBatch.End();
        }
    }
}
