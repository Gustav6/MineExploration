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

        public static Vector2 MouseInWorld()
        {
            Matrix inverseTransform = Matrix.Invert(Library.MainCamera.Transform);
            return Vector2.Transform(new Vector2(CurrentState.X, CurrentState.Y), inverseTransform);
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

        public static bool HasBeenPressed(MouseKey mouseKey)
        {
            ButtonState currentButtonState, prevButtonState;

            switch (mouseKey)
            {
                case MouseKey.Left:
                    currentButtonState = CurrentState.LeftButton;
                    prevButtonState = PreviousState.LeftButton;
                    break;
                case MouseKey.Middle:
                    currentButtonState = CurrentState.MiddleButton;
                    prevButtonState = PreviousState.MiddleButton;
                    break;
                case MouseKey.Right:
                    currentButtonState = CurrentState.RightButton;
                    prevButtonState = PreviousState.RightButton;
                    break;
                default:
                    return false;
            }

            if (currentButtonState == ButtonState.Pressed && prevButtonState == ButtonState.Released)
            {
                return true;
            }

            return false;
        }
    }

    public enum MouseKey
    {
        Left,
        Middle,
        Right,
    }
}
