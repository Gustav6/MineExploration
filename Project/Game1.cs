using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MineExploration
{
    public class Game1 : Game
    {
        public static GraphicsDeviceManager Graphics { get; private set; }
        private SpriteBatch spriteBatch;

        //private readonly IHost host;

        public Game1()
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            //host = Host.CreateDefaultBuilder()
            //    .ConfigureServices((context, services) =>
            //    {
            //        services.AddHostedService<Worker>();
            //    })
            //    .Build();

            //host.Start(); // Start the background service
        }

        protected override void Initialize()
        {
            base.Initialize();

            GameManager.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            GameManager.LoadContent(Content);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            GameManager.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Dispose(bool disposing)
        {
            //host.Dispose();
            base.Dispose(disposing);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            GameManager.Draw(spriteBatch);

            base.Draw(gameTime);
        }
    }
}
