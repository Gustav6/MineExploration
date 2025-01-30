using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineExploration
{
    public static class InputManager
    {
        public static bool ControllerActive { get; set; }

        public static void UpdateInputStates()
        {
            MouseInput.SetStates();
            KeyboardInput.SetStates();
        }
    }
}
