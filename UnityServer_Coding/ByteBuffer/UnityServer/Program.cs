using System;

namespace UnityServer
{

    class Program
    {
        static void Main(string[] args)
        {
            ServerHandlePackets.InitializePackets();

            TCPServer server = new TCPServer();
            server.InitNetwork();
            Console.ReadLine();
        }
    }
}
