using System;
using System.Net;
using System.Net.Sockets;


class ServerTCP
{
    private static TcpListener serverSocket;

    public Clients[] Client = new Clients[1500];

    public void InitNetwork()
    {
        serverSocket = new TcpListener(IPAddress.Any, 5555);
        serverSocket.Start();
        serverSocket.BeginAcceptTcpClient(ClientConnectCallback, null);
    }

    private void ClientConnectCallback(IAsyncResult result)
    {
        TcpClient tempClient = serverSocket.EndAcceptTcpClient(result);
        serverSocket.BeginAcceptTcpClient(ClientConnectCallback, null);

        for (int i = 0; i < Client.Length; i++)
        {
            if (Client[i].socket == null)
            {
                Client[i].socket = tempClient;
                Client[i].connectionID = i;
                Client[i].ip = tempClient.Client.RemoteEndPoint.ToString();
                Client[i].Start();
                Console.WriteLine("Connection received from " + Client[i].ip + ".");
                return;
            }
        }
    }
}
