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
        public bool CanMove { get; private set; }
        public Vector2 MoveDirection {  get; protected set; }
        protected bool canFlipSprite = true;

        public float movementSpeed;

        protected void MoveGameObject(GameTime gameTime)
        {
            if (!CanMove || MoveDirection == Vector2.Zero)
            {
                return;
            }

            MoveDirection.Normalize();

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
