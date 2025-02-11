using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineExploration
{
    public class SceneManager
    {
        public static Dictionary<SceneId, Scene> Scenes { get; private set; }

        public void ChangeScene(SceneId scene)
        {
            Scenes[scene].Load();
        }
    }
}

public enum SceneId
{
    Main,
}
