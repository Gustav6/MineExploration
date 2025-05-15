using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerToGame;

namespace MineExploration
{
    public abstract class Moveable : GameObject
    {
        public bool CanMove { get; private set; } = true;
        public Vector2 MoveDirection { get; protected set; } = Vector2.Zero;
        private Vector2 previousMoveDirection = Vector2.Zero;
        protected bool canFlipSprite = true;

        public float movementSpeed;

        protected void SendMoveDirection()
        {
            if (!CanMove || MoveDirection == previousMoveDirection)
            {
                return;
            }

            if (MoveDirection != Vector2.Zero)
            {
                MoveDirection.Normalize();
            }

            NetworkMessage moveRequest = new()
            {
                Payload = new ObjectMoveRequest()
                {
                    Direction = new Vec2(MoveDirection.X, MoveDirection.Y),
                    speed = movementSpeed,
                    ObjectIdentification = ObjectIdentification
                },
                Type = MessageType.MoveGameObject
            };

            ServerManager.SendMessage(moveRequest);

            previousMoveDirection = MoveDirection;
        }

        protected void SimulateMovement(GameTime gameTime)
        {
            if (MoveDirection != Vector2.Zero)
            {
                MoveDirection.Normalize();
            }

            Position += MoveDirection * movementSpeed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
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
