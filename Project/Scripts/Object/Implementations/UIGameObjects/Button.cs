using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MineExploration
{
    public class Button : UIGameObject
    {
        private BaseTransition[] transitionsOnEnable;
        private BaseTransition[] transitionsOnDisable;

        public delegate void ExectueOnButtonActivation();
        private readonly ExectueOnButtonActivation execute;

        public Button(Vector2 position, ExectueOnButtonActivation execute)
        {
            SetPosition(position);
            this.execute = execute;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
