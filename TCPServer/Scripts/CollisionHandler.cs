using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPServer
{
    public class CollisionHandler
    {
        public static IEnumerable<Object> GetCollisions(Object obj)
        {
            foreach (Object other in ObjectHandler.GetGameObjects())
            {
                if (other.Id == obj.Id)
                {
                    continue;
                }

                if (obj.BoundingBox.Intersects(other.BoundingBox))
                {
                    yield return other;
                }
            }
        }

    }
}
