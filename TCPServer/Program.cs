using TCPServer;
using System.Net;
using ServerToGame;
using System.Diagnostics;

class Program
{
    static async Task Main()
    {
        Console.Title = "TCP Server";

        PayloadRegistry.Register(MessageType.ObjectSpawnRequest, () => new ObjectSpawnRequest());
        PayloadRegistry.Register(MessageType.AssignServerIdentification, () => new AssignServerIdentification());
        PayloadRegistry.Register(MessageType.MoveGameObject, () => new ObjectMoveRequest());
        PayloadRegistry.Register(MessageType.UpdateObject, () => new ObjectUpdate());

        Server serverInstance = new();
        await Task.Run(() => serverInstance.Start(13000, IPAddress.Parse("127.0.0.1")));

        var stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();

        // Game loop
        while (serverInstance.Active)
        {
            long start = stopwatch.ElapsedMilliseconds;

            Server.Tick();

            // Wait for next tick
            long elapsed = stopwatch.ElapsedMilliseconds - start;
            int delay = Math.Max(0, Server.tickDelay - (int)elapsed);
            await Task.Delay(delay);
        }
    }
}