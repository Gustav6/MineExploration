using Microsoft.Xna.Framework.Input;
using MineExploration;
using System;
using System.Diagnostics;
using System.Net;
using TCPServer;

class Program
{
    static async Task Main()
    {
        Console.Title = "TCP Server";

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