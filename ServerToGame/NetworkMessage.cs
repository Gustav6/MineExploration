using System;

namespace ServerToGame
{
    /// <summary>
    /// [Message Length][Message Type][Payload] - (Structure)
    /// </summary>
    public class NetworkMessage
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

        public static NetworkMessage FromBytes(byte[] data)
        {
            using var ms = new MemoryStream(data);
            using var reader = new BinaryReader(ms);

            var type = (MessageType)reader.ReadByte();
            var length = reader.ReadInt32();
            var payloadData = reader.ReadBytes(length);

            var payload = PayloadRegistry.CreatePayload(type);
            payload.Deserialize(payloadData);

            return new NetworkMessage { Type = type, Payload = payload };
        }
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