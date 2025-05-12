using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPServer
{
    /// <summary>
    /// [Message Length][Message Type][Payload] - (Structure)
    /// </summary>
    public class Message
    {
        public MessageType Type { get; set; }
        public required MessagePayload Payload { get; set; }

        public byte[] ToBytes()
        {
            var payloadBytes = Payload.Serialize();
            using var ms = new MemoryStream();
            using var writer = new BinaryWriter(ms);

            writer.Write((byte)Type);
            writer.Write(payloadBytes.Length);
            writer.Write(payloadBytes);

            return ms.ToArray();
        }

        public static Message FromBytes(byte[] data)
        {
            using var ms = new MemoryStream(data);
            using var reader = new BinaryReader(ms);

            var type = (MessageType)reader.ReadByte();
            var length = reader.ReadInt32();
            var payloadData = reader.ReadBytes(length);

            var payload = PayloadRegistry.CreatePayload(type);
            payload.Deserialize(payloadData);

            return new Message { Type = type, Payload = payload };
        }
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

    // Abstract class for message payloads
    public abstract class MessagePayload
    {
        public abstract byte[] Serialize();
        public abstract void Deserialize(byte[] data);
    }

    public enum MessageType
    {
        CreateGameObject,
        DestroyGameObject,
        UpdateGameObject,
        FetchGameData,
        ClientDisconnect,
        ClientConnect,
        CollisionEvent,
    }
}
