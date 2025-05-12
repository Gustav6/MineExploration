using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerToGame
{
    public abstract class MessagePayload
    {
        public abstract byte[] Serialize();
        public abstract void Deserialize(byte[] data);
    }

    public static class PayloadRegistry
    {

        private static readonly Dictionary<MessageType, Func<MessagePayload>> registry = [];

        public static void Register(MessageType type, Func<MessagePayload> payload)
        {
            if (registry.ContainsKey(type))
            {
                throw new InvalidOperationException($"Message type {type} already registered.");
            }

            registry[type] = payload;
        }

        public static MessagePayload CreatePayload(MessageType type)
        {
            if (!registry.TryGetValue(type, out var payload))
            {
                throw new NotSupportedException($"Message type {type} is not registered.");
            }

            return payload();
        }
    }

    public class ObjectMoveRequest : MessagePayload
    {
        public float X, Y;

        public override byte[] Serialize()
        {
            using var ms = new MemoryStream();
            using var writer = new BinaryWriter(ms);
            writer.Write(X);
            writer.Write(Y);
            return ms.ToArray();
        }

        public override void Deserialize(byte[] data)
        {
            using var ms = new MemoryStream(data);
            using var reader = new BinaryReader(ms);
            X = reader.ReadSingle();
            Y = reader.ReadSingle();
        }
    }

    public class ObjectSpawnRequest : MessagePayload
    {
        public float X, Y;

        public override byte[] Serialize()
        {
            using var ms = new MemoryStream();
            using var writer = new BinaryWriter(ms);
            return ms.ToArray();
        }

        public override void Deserialize(byte[] data)
        {
            using var ms = new MemoryStream(data);
            using var reader = new BinaryReader(ms);
        }
    }
}
