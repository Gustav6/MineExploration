using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineExploration
{
    public class SaveData
    {
        // * ALL VARIABLES NEED TO BE PROPERTIES * \\
        public float BaseMovementSpeed { get; set; } = PlayerStats.BaseMovementSpeed;
        public float MaxHealth { get; set; } = PlayerStats.MaxHealth;

        // Index 0 in array = x axes, index 1 in array = y axes
        public float[] SavedPosition { get; set; } = new float[2];
    }
}
