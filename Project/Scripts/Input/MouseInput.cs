using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineExploration
{
    public static class MouseInput
    {
        public static MouseState CurrentState { get; private set; }
        public static MouseState PreviousState { get; private set; }

        public static void SetStates()
        {
            PreviousState = CurrentState;
            CurrentState = Mouse.GetState();
        }

        public static Rectangle GetBounds(bool useCurrentState)
        {
            if (useCurrentState)
            {
                return new Rectangle(CurrentState.X, CurrentState.Y, 1, 1);
            }
            else
            {
                return new Rectangle(PreviousState.X, PreviousState.Y, 1, 1);
            }
        }

        public static bool IsPressed(ButtonState buttonState)
        {
            if (buttonState == ButtonState.Pressed)
            {
                return true;
            }

            return false;
        }

        public static bool HasBeenPressed(ButtonState currentButtonState, ButtonState prevButtonState)
        {
            if (currentButtonState == ButtonState.Pressed && prevButtonState == ButtonState.Released)
            {
                return true;
            }

            return false;
        }
    }
}
