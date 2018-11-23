using System;
using System.Net;
using System.Net.Sockets;

namespace UnityServer
{
    public enum ServerPackets
    {
        S_INFORMATION = 1,
        S_EXECUTEMETHODONCLIENT,
        S_PLAYERDATA,
    }

    public enum ClientPackets
    {
        C_THANKYOU = 1,
    }

    class TCPServer
    {
        private static TcpListener serverSocket;

        public static Clients[] Client = new Clients[1500];

        public void InitNetwork()
        {
            for (int i = 0; i < Client.Length; i++)
            {
                Client[i] = new Clients();
            }

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
                    SendJoinGame(i);
                    return;
                }
            }
        }

        public void SendDataTo(int connetionID, byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteLong((data.GetUpperBound(0) - data.GetLowerBound(0)) + 1);
            buffer.WriteBytes(data);
            Client[connetionID].myStream.BeginWrite(buffer.ToArray(), 0, buffer.ToArray().Length, null, null);
        }

        public void SendDataToAll(byte[] data)
        {
            for (int i = 0; i < Client.Length; i++)
            {
                if (Client[i].socket != null)
                {
                    SendDataTo(i, data);
                }
            }
        }

        //Send Methods To Client
        /// <summary>
        /// Our Packages
        /// </summary>
        /// <param name="connetionID"></param>
        public void SendInformation(int connetionID)
        {
            ByteBuffer buffer = new ByteBuffer(); //Create a new packet
            //Add the packet Identifier
            buffer.WriteLong((long)ServerPackets.S_INFORMATION);


            //Adds data to the packet.
            buffer.WriteString("Welcome to the Server");
            buffer.WriteString("Now you are able to play the game");
            buffer.WriteInteger(10);

            SendDataTo(connetionID, buffer.ToArray());
        }

        public void SendExecuteMethodOnClient(int connectionID)
        {
            ByteBuffer buffer = new ByteBuffer(); //Create a new packet
            //Packet Identifier
            buffer.WriteLong((long)ServerPackets.S_EXECUTEMETHODONCLIENT);
            SendDataTo(connectionID, buffer.ToArray());
        }


        //Sending Player data. Using enum.
        public byte[] PlayerData(int connectionID)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteLong((long)ServerPackets.S_PLAYERDATA);
            buffer.WriteInteger(connectionID);
            return buffer.ToArray();
        }

        public void SendJoinGame(int connectionID)
        {
            ByteBuffer buffer = new ByteBuffer();
            //Send all players on the current map to new player
            for (int i = 0; i < Client.Length; i++)
            {
                if (Client[i].socket != null)
                {
                    if (i != connectionID)
                    {
                        SendDataTo(connectionID, PlayerData(i));
                    }
                }
            }

            //Send new player data to everyone on the map including myself
            SendDataToAll(PlayerData(connectionID));
        }
    }
}
