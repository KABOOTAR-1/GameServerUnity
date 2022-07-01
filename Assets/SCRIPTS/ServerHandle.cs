using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class ServerHandle 
{
    // Start is called before the first frame update
    public static void WelcomeReceived(int _fromClient, Packet _packet)
    {
        int _clientIdCheck = _packet.ReadInt();
        string _username = _packet.ReadString();


        Debug.Log($" {Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint} connected succesfully and is now player {_fromClient}.");
        if (_fromClient != _clientIdCheck)
        {
            Debug.Log($"Player \"{_username}\"(ID:{_fromClient}) has assumed the wromg cliend ID :({_clientIdCheck}");
        }
        Server.clients[_fromClient].SendIntoGame(_username);
    }

    internal static void PlayerMovement(int _fromCLient, Packet _packet)
    {
        bool[] inputs = new bool[_packet.ReadInt()];
        for (int i = 0; i < inputs.Length; i++)
        {
            inputs[i] = _packet.ReadBool();
        }

        Quaternion _rotation = _packet.ReadQuaternions();

        Server.clients[_fromCLient].player.SetInput(inputs, _rotation);

    }
}
