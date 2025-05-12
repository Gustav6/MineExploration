using Microsoft.Xna.Framework;

namespace TCPServer
{
    public static class ObjectHandler
    {
        private readonly static Dictionary<int, Object> objects = [];

        private readonly static Queue<int> availableIdentifications = new();
        private static int nextAvailableIdentification = 1;

        public static Object CreateObject(ObjectType type, Vector2 position, Vector2 size)
        {
            Object obj = new(GetObjectIdentification(), type, position, size);

            objects.Add(obj.Id, obj);
            return obj;
        }

        public static void RemoveObject(int id)
        {
            ReleaseIdentification(id);
            objects.Remove(id);
        }

        public static Object? Get(int id)
        {
            return objects.TryGetValue(id, out var obj) ? obj : null;
        }

        public static IEnumerable<Object> GetGameObjects()
        {
            return objects.Values;
        }

        public static int GetObjectIdentification()
        {
            if (availableIdentifications.Count > 0)
            {
                return availableIdentifications.Dequeue();
            }

            return nextAvailableIdentification++; // Assign a new ID
        }

        public static void ReleaseIdentification(int identification)
        {
            objects.Remove(identification);

            availableIdentifications.Enqueue(identification);

            Server.Log($"Released identification: [{identification}]", LogType.Server);
        }

    }
}
