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

    #region Move Request
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
    #endregion

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

    #region Assign objcets identification
    public class AssignServerIdentification : MessagePayload
    {
        public string LocalIdentification = String.Empty;
        public int ObjectIdentification;

        public override byte[] Serialize()
        {
            using var ms = new MemoryStream();
            using var writer = new BinaryWriter(ms);

            writer.Write(ObjectIdentification);
            writer.Write(LocalIdentification);

            return ms.ToArray();
        }

        public override void Deserialize(byte[] data)
        {
            using var ms = new MemoryStream(data);
            using var reader = new BinaryReader(ms);

            ObjectIdentification = reader.ReadInt32();
            LocalIdentification = reader.ReadString();
        }
    }
    #endregion

    #region Update object
    public class ObjectUpdate : MessagePayload
    {
        public ObjectType Type;
        public int ObjectIdentification;
        public Vec2 Position = new(0, 0);

        public override byte[] Serialize()
        {
            using var ms = new MemoryStream();
            using var writer = new BinaryWriter(ms);

            writer.Write(ObjectIdentification);
            writer.Write(Position.X);
            writer.Write(Position.Y);
            writer.Write((int)Type);

            return ms.ToArray();
        }

        public override void Deserialize(byte[] data)
        {
            using var ms = new MemoryStream(data);
            using var reader = new BinaryReader(ms);

            ObjectIdentification = reader.ReadInt32();
            Position = new Vec2(reader.ReadSingle(), reader.ReadSingle());
            Type = (ObjectType)reader.ReadSingle();
        }
    }
    #endregion

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

    #region Spawn Object
    public class ObjectSpawnRequest : MessagePayload
    {
        public ObjectType Type;
        public Vec2 Size = new(0, 0), Position = new(0, 0);
        public string LocalIdentification = string.Empty;
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
            writer.Write(LocalIdentification);
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
            LocalIdentification = reader.ReadString();
            ObjectIdentification = reader.ReadInt32();
        }
    }
    #endregion
}
