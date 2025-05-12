using TCPServer;
using System.Net;

class Program
{
    static async Task Main()
    {
        Console.Title = "TCP Server";

        //PayloadRegistry.Register(MessageType.CreateGameObject, () => new CreateGameObjectPayload());

        Server serverInstance = new();

        await Task.Run(() =>
        {
            serverInstance.Start(13000, IPAddress.Parse("127.0.0.1"));
        });

        while (serverInstance.Active)
        {
            await Task.Delay(100);
        }
    }
}