using System;
using System.Collections.Generic;
using System.Drawing;
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
                throw new InvalidOperationException($"Message type [{type}] is already registered.");
            }

            registry[type] = payload;
        }

        public static MessagePayload FetchPayload(MessageType type)
        {
            if (!registry.TryGetValue(type, out var payload))
            {
                throw new NotSupportedException($"Message type [{type}] is not registered.");
            }

            return payload();
        }
    }

    public class ObjectMoveRequest : MessagePayload
    {
        public Vec2 Direction = new(0, 0);
        public float speed;
        public int ObjectIdentification;

        public override byte[] Serialize()
        {
            using var ms = new MemoryStream();
            using var writer = new BinaryWriter(ms);

            writer.Write(Direction.X);
            writer.Write(Direction.Y);
            writer.Write(speed);
            writer.Write(ObjectIdentification);

            return ms.ToArray();
        }

        public override void Deserialize(byte[] data)
        {
            using var ms = new MemoryStream(data);
            using var reader = new BinaryReader(ms);

            Direction = new Vec2(reader.ReadSingle(), reader.ReadSingle());
            speed = reader.ReadSingle();
            ObjectIdentification = reader.ReadInt32();
        }
    }

    public class GameData : MessagePayload
    {
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

    public class ObjectUpdate : MessagePayload
    {
        public string? ClientIdentification = string.Empty;

        public int ObjectIdentification;
        public Vec2 Position;

        public override byte[] Serialize()
        {
            using var ms = new MemoryStream();
            using var writer = new BinaryWriter(ms);

            writer.Write(ObjectIdentification);
            writer.Write(Position.X);
            writer.Write(Position.Y);

            if (ClientIdentification != null)
            {
                writer.Write(ClientIdentification);
            }

            return ms.ToArray();
        }

        public override void Deserialize(byte[] data)
        {
            using var ms = new MemoryStream(data);
            using var reader = new BinaryReader(ms);

            ObjectIdentification = reader.ReadInt32();
            Position = new Vec2(reader.ReadSingle(), reader.ReadSingle());
            ClientIdentification = reader.ReadString();
        }
    }

    #region Destroy object 
    public class DestroyObject : MessagePayload
    {
        public int ObjectIdentification;

        public override byte[] Serialize()
        {
            using var ms = new MemoryStream();
            using var writer = new BinaryWriter(ms);

            writer.Write(ObjectIdentification);

            return ms.ToArray();
        }

        public override void Deserialize(byte[] data)
        {
            using var ms = new MemoryStream(data);
            using var reader = new BinaryReader(ms);

            ObjectIdentification = reader.ReadInt32();
        }
    }
    #endregion

    #region Spawn request & repsone
    public class ObjectSpawnRequest : MessagePayload
    {
        public ObjectType Type;
        public Vec2 Size = new(0, 0), Position = new(0, 0);
        public string TempIdentification = string.Empty;
        public int ObjectIdentification;

        public override byte[] Serialize()
        {
            using var ms = new MemoryStream();
            using var writer = new BinaryWriter(ms);

            writer.Write((int)Type);
            writer.Write(Position.X);
            writer.Write(Position.Y);
            writer.Write(Size.X);
            writer.Write(Size.Y);
            writer.Write(TempIdentification);
            writer.Write(ObjectIdentification);

            return ms.ToArray();
        }

        public override void Deserialize(byte[] data)
        {
            using var ms = new MemoryStream(data);
            using var reader = new BinaryReader(ms);

            Type = (ObjectType)reader.ReadSingle();
            Position = new Vec2(reader.ReadSingle(), reader.ReadSingle());
            Size = new Vec2(reader.ReadSingle(), reader.ReadSingle());
            TempIdentification = reader.ReadString();
            ObjectIdentification = reader.ReadInt32();
        }
    }
    #endregion
}
