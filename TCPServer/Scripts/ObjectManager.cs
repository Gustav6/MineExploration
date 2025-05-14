using System;
using ServerToGame;

namespace TCPServer
{
    public static class ObjectManager
    {
        private readonly static Dictionary<int, Object> objects = [];

        private readonly static Queue<int> availableIdentifications = new();
        private static int nextAvailableIdentification = 1;

        public static void Update(float deltaTime)
        {
            foreach (Object obj in objects.Values)
            {
                obj.Update(deltaTime);
            }
        }

        public static void Add(Object obj)
        {
            objects.Add(obj.Identification, obj);
        }

        public static void Destroy(int id)
        {
            ReleaseIdentification(id);
            objects.Remove(id);

            NetworkMessage message = new()
            {
                Payload = new DestroyObject()
                {
                    ObjectIdentification = id
                },
                Type = MessageType.DestroyObject
            };

            Server.Echo(message);
        }

        public static Object? Get(int id)
        {
            return objects.TryGetValue(id, out var obj) ? obj : null;
        }

        public static IEnumerable<Object> GetUpdatedObjects()
        {
            return objects.Values.Where(obj => obj.IsDirty);
        }

        public static void ClearDirtyFlags()
        {
            foreach (Object obj in objects.Values)
            {
                obj.ClearDirtyFlag();
            }
        }

        #region Object identification
        public static int GetObjectIdentification()
        {
            if (availableIdentifications.Count > 0)
            {
                return availableIdentifications.Dequeue();
            }

            return nextAvailableIdentification++; // Assign a new ID
        }

        private static void ReleaseIdentification(int identification)
        {
            objects.Remove(identification);

            availableIdentifications.Enqueue(identification);

            Server.Log($"Released identification: [{identification}]", LogType.Server);
        }
        #endregion
    }
}
