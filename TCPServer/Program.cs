using Microsoft.Xna.Framework.Input;
using MineExploration;
using System;
using System.Diagnostics;
using System.Net;
using TCPServer;

class Program
{
    public static Server? ServerInstance { get; private set; }

    static async Task Main()
    {
        Console.Title = "TCP Server"; // Set window title

        ServerInstance = new();

        await Task.Run(() =>
        {
            ServerInstance.Start(13000, IPAddress.Parse("127.0.0.1"));
        });

        while (ServerInstance.Active)
        {
            await Task.Delay(100);
        }
    }
}