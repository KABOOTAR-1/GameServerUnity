using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class ServerSend 
{
    // Start is called before the first frame update
    private static void SendTCPData(int _id, Packet _packet)
    {
        _packet.WriteLength();
        Server.clients[_id].tcp.SendData(_packet);

    }

    private static void SendUDPData(int _toClient, Packet _packet)
    {
        _packet.WriteLength();
        Server.clients[_toClient].udp.SendData(_packet);
    }

    private static void SendTCPToAll(Packet _packets)
    {
        _packets.WriteLength();
        for (int i = 1; i <= Server.maxPlayers; i++)
        {
            Server.clients[i].tcp.SendData(_packets);
        }
    }

    private static void SendTCPToAll(int _exceptionclint, Packet _packets)
    {
        _packets.WriteLength();
        for (int i = 1; i <= Server.maxPlayers; i++)
        {
            if (i != _exceptionclint)
                Server.clients[i].tcp.SendData(_packets);
        }
    }

    private static void SendUDPToAll(Packet _packets)
    {
        _packets.WriteLength();
        for (int i = 1; i <= Server.maxPlayers; i++)
        {
            Server.clients[i].udp.SendData(_packets);
        }
    }

    private static void SendUDPToAll(int _exceptionclint, Packet _packets)
    {
        _packets.WriteLength();
        for (int i = 1; i <= Server.maxPlayers; i++)
        {
            if (i != _exceptionclint)
                Server.clients[i].udp.SendData(_packets);
        }
    }

    #region Packets

    public static void Welcome(int _toclientid, string _msg)
    {
        using (Packet _packet = new Packet((int)ServerPackets.welcome))
        {
            _packet.Write(_msg);
            _packet.Write(_toclientid);

            SendTCPData(_toclientid, _packet);
        }
    }

    public static void SpawnPlayer(int _toclient, Player _player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.spawnPlayer))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.UserName);
            _packet.Write(_player.transform.position);
            _packet.Write(_player.transform.rotation);

            SendTCPData(_toclient, _packet);
        }
    }

    internal static void PlayerPosition(Player player)
    {
        using (Packet packet = new Packet((int)ServerPackets.playerPosition))
        {
            packet.Write(player.id);
            packet.Write(player.transform.position);

            SendUDPToAll(packet);
        }
    }
    internal static void PlayerRotation(Player player)
    {
        using (Packet packet = new Packet((int)ServerPackets.playerRotation))
        {
            packet.Write(player.id);
            packet.Write(player.transform.rotation);

            SendUDPToAll(player.id, packet);
        }
    }


    #endregion
}
