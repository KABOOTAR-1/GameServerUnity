using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System;
public class Server 
{
    // Start is called before the first frame update
   
        public static int maxPlayers { get; private set; }
        public static int Port { get; private set; }
        private static TcpListener Tcplistener;
        private static UdpClient udpListner;

        public static Dictionary<int, Client> clients = new Dictionary<int, Client>();
        public delegate void PacketHandler(int _fromCLient, Packet _packet);
        public static Dictionary<int, PacketHandler> packetHandlers;
        public static void Start(int _maxplayers, int _port)
        {
            maxPlayers = _maxplayers;
            Port = _port;
            Debug.Log("Starting Server");
            InitilizeServerData();
            Tcplistener = new TcpListener(IPAddress.Any, Port);
            Tcplistener.Start();
            Tcplistener.BeginAcceptTcpClient(new AsyncCallback(TPconnectCallback), null);
            udpListner = new UdpClient(Port);
            udpListner.BeginReceive(UDPReceiveCallBack, null);
            Debug.Log($"Server started on {Port}.");
        }

        private static void TPconnectCallback(IAsyncResult _result)
        {
            TcpClient _client = Tcplistener.EndAcceptTcpClient(_result);
            Tcplistener.BeginAcceptTcpClient(new AsyncCallback(TPconnectCallback), null);
            Debug.Log($"Incoming Connection from {_client.Client.RemoteEndPoint}");

            for (int i = 1; i <= maxPlayers; i++)
            {
                if (clients[i].tcp.socket == null)
                {
                    clients[i].tcp.Connect(_client);
                    return;
                }
            }

            Debug.Log($" {_client.Client.RemoteEndPoint} failed to connect: Server full!");
        }

        private static void UDPReceiveCallBack(IAsyncResult _result)
        {
            try
            {
                IPEndPoint _clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = udpListner.EndReceive(_result, ref _clientEndPoint);
                udpListner.BeginReceive(UDPReceiveCallBack, null);

                if (data.Length < 4)
                {
                    return;
                }

                using (Packet _packet = new Packet(data))
                {
                    int cliendId = _packet.ReadInt();

                    if (cliendId == 0)
                    {
                        return;
                    }

                    if (clients[cliendId].udp.endPoint == null)
                    {
                        clients[cliendId].udp.Connect(_clientEndPoint);
                        return;
                    }

                    if (clients[cliendId].udp.endPoint.ToString() == _clientEndPoint.ToString())
                    {
                        clients[cliendId].udp.HandleData(_packet);
                    }

                }
            }
            catch (Exception ex)
            {
                Debug.Log($" Exception UDP occured {ex}");
            }
        }

        public static void SendUDPData(IPEndPoint _clientEndPoint, Packet _packet)
        {
            try
            {
                if (_clientEndPoint != null)
                {
                    udpListner.BeginSend(_packet.ToArray(), _packet.Length(), _clientEndPoint, null, null);
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"Error Sending data to {_clientEndPoint} via UDP:{ex} ");
            }
        }
        private static void InitilizeServerData()
        {
            for (int i = 1; i <= maxPlayers; i++)
            {
                clients.Add(i, new Client(i));
            }

            packetHandlers = new Dictionary<int, PacketHandler>() {
                {
                    (int)ClientPackets.welcomeReceived,ServerHandle.WelcomeReceived },
                { (int)ClientPackets.playerMovement,ServerHandle.PlayerMovement }
                };

            Debug.Log("Initilaized Packets");
        }
    
}
