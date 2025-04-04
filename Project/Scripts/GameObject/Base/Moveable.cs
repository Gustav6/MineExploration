using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineExploration
{
    public abstract class Moveable : GameObject
    {
        public bool CanMove { get; private set; }
        public Vector2 MoveDirection {  get; protected set; }
        protected bool canFlipSprite = true;

        public float movementSpeed;

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            MoveGameObject(gameTime);
        }

        private void MoveGameObject(GameTime gameTime)
        {
            if (MoveDirection == Vector2.Zero)
            {
                return;
            }

            MoveDirection.Normalize();

            SetPosition(Position + (MoveDirection * movementSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds));

            ServerHandler.SendMessage($"{(int)ServerCommands.Echo}:{gameObjectData.MoveData}");
        }

        public void LockMovement()
        {
            CanMove = false;
        }

        public void UnlockMovement()
        {
            CanMove = true;
        }
    }
}
