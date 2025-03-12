using Microsoft.Xna.Framework.Input;
using MineExploration;
using System;
using System.Diagnostics;
using System.Net;
using TCPServer;

class Program
{
    static void Main()
    {
        Console.Title = "TCP Server"; // Set window title

        DisplayKeyBinds();

        Server server = new();

        Task.Run(() => server.Start(13000, IPAddress.Parse("127.0.0.1")));

        while (!server.IsRunning)
        {
            Debug.WriteLine("Waiting for server");
        }

        while (server.IsRunning)
        {
            if (int.TryParse(Console.ReadLine(), out int input))
            {
                if (Enum.IsDefined(typeof(ServerCommands), input))
                {
                    ServerCommands command = (ServerCommands)input;

                    switch (command)
                    {
                        case ServerCommands.Clear:
                            Console.Clear();
                            DisplayKeyBinds();
                            break;
                        case ServerCommands.KeyBinds:
                            DisplayKeyBinds();
                            break;
                        case ServerCommands.Stop:
                            server.Stop();
                            break;
                        case ServerCommands.Clients:
                            if (ClientManager.clients.Keys.Count > 0)
                            {
                                for (int i = 0; i < ClientManager.clients.Keys.Count; i++)
                                {
                                    Console.WriteLine($"Client {i + 1} : " + ClientManager.clients.Keys.ElementAt(i));
                                }
                            }
                            else
                            {
                                Console.WriteLine("No clients found");
                            }
                                break;
                        default:
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Please input valid command");
                }
            }
            else
            {
                Console.WriteLine("Please input valid command");
            }

            Thread.Sleep(100);
        }
    }
    private static void DisplayKeyBinds()
    {
        Console.WriteLine("Server Commands:");

        Console.WriteLine("1: Clear Console");
        Console.WriteLine("2: Display KeyBinds");
        Console.WriteLine("3: Stop server");
        Console.WriteLine("4: Display Clients");
    }


    private enum ServerCommands
    {
        Clear = 1,
        KeyBinds = 2,
        Stop = 3,
        Clients = 4,
    }
}