using ProtoBuf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;

public class NetworkManager
{
    ServerSession _session = new ServerSession();

    public bool IsConnected { get; set; }

    private float sendTime = 0.25f;

    public long Delay = 0;

    public void Init()
    {
        string host = Dns.GetHostName();
        IPHostEntry ipHost = Dns.GetHostEntry(host);
        IPAddress ipAddr = ipHost.AddressList[0];
        IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

        Connector connector = new Connector();
        connector.Connect(endPoint, () => { return _session; }, 1);

        IsConnected = true;
    }

    public void OnUpdate()
    {
        if(!IsConnected)
        {
            return;
        }

        if (!PacketQueue.Instance.IsEmpty())
        {
            PacketQueue.Instance.Flush();
        }

        if (sendTime > 0.0f)
        {
            sendTime -= Time.deltaTime;
        }
        else
        {
            SendMovePacket();
            sendTime = 0.25f;
        }
    }

    public void SendMovePacket()
    {
        if(!Managers.Game.GetPlayer())
        {
            return;
        }

        C_MOVE movePacket = new C_MOVE();
        Vector3 position = Managers.Game.GetPlayer().transform.position;
        Vector3 velocity = Managers.Game.GetPlayer().GetComponent<Rigidbody>().velocity;
        movePacket.PosX = position.x;
        movePacket.PosY = position.y;
        movePacket.PosZ = position.z;
        movePacket.TimeStamp = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;
        byte[] sendByte = Utils.SerializePacket(PacketType.PKT_C_MOVE, movePacket);

        _session.Send(sendByte);
    }

    public void SendChatPacket(string chat)
    {
        if (!Managers.Game.GetPlayer())
        {
            return;
        }

        C_CHAT chatPacket = new C_CHAT();
        chatPacket.Chat = chat;
        byte[] sendByte = Utils.SerializePacket(PacketType.PKT_C_CHAT, chatPacket);

        _session.Send(sendByte);
    }

    public void Disconnect()
    {
        C_LEAVEGAME leavePacket = new C_LEAVEGAME();
        byte[] sendByte = Utils.SerializePacket(PacketType.PKT_C_LEAVEGAME, leavePacket);
        _session.Send(sendByte);
        _session.Disconnect();
    }
}
