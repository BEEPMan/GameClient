using ProtoBuf;
using System;
using System.IO;
using System.Net;
using UnityEngine;

public class ServerSession : PacketSession
{
    public override void OnRecvPacket(ArraySegment<Byte> buffer)
    {
        byte[] recvPacket = new byte[buffer.Count];
        Buffer.BlockCopy(buffer.Array, buffer.Offset, recvPacket, 0, buffer.Count);
        PacketQueue.Instance.Push(recvPacket);
    }

    public override void OnConnected(EndPoint endPoint)
    {
        Debug.Log($"Connected");
    }

    public override void OnDisconnected(EndPoint endPoint)
    {
        C_LEAVEGAME packet = new C_LEAVEGAME();
        byte[] packetBytes = Utils.SerializePacket(PacketType.PKT_C_LEAVEGAME, packet);
        Send(packetBytes);
        Debug.Log($"Disconnected");
    }

    public override void OnSend(int numOfBytes)
    {

    }
}