using ProtoBuf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Analytics;

public class PacketManager
{
    private bool _firstMove = true;

    public void HandlePacket(byte[] packet)
    {
        // Extract Header
        int headerSize = sizeof(ushort) * 2;
        byte[] headerBytes = new byte[headerSize];
        Buffer.BlockCopy(packet, 0, headerBytes, 0, headerSize);

        PacketHeader header = new PacketHeader();
        header.Size = BitConverter.ToUInt16(headerBytes, 0);
        header.PacketType = BitConverter.ToUInt16(headerBytes, sizeof(ushort));

        // Extract Data
        int dataSize = header.Size - headerSize;
        byte[] dataBytes = new byte[dataSize];
        Buffer.BlockCopy(packet, headerSize, dataBytes, 0, dataSize);

        switch ((PacketType)header.PacketType)
        {
            case PacketType.PKT_S_ENTERGAME:
                using (MemoryStream dataStream = new MemoryStream(dataBytes))
                {
                    S_ENTERGAME data = Serializer.Deserialize<S_ENTERGAME>(dataStream);
                    Handle_S_ENTERGAME(data);
                }
                break;
            case PacketType.PKT_S_LEAVEGAME:
                using (MemoryStream dataStream = new MemoryStream(dataBytes))
                {
                    S_LEAVEGAME data = Serializer.Deserialize<S_LEAVEGAME>(dataStream);
                    Handle_S_LEAVEGAME(data);
                }
                break;
            case PacketType.PKT_S_PLAYERLIST:
                using (MemoryStream dataStream = new MemoryStream(dataBytes))
                {
                    S_PLAYERLIST data = Serializer.Deserialize<S_PLAYERLIST>(dataStream);
                    Handle_S_PLAYERLIST(data);
                }
                break;
            case PacketType.PKT_S_CHAT:
                using (MemoryStream dataStream = new MemoryStream(dataBytes))
                {
                    S_CHAT data = Serializer.Deserialize<S_CHAT>(dataStream);
                    Handle_S_CHAT(data);
                }
                break;
            case PacketType.PKT_S_MOVE:
                using (MemoryStream dataStream = new MemoryStream(dataBytes))
                {
                    S_MOVE data = Serializer.Deserialize<S_MOVE>(dataStream);
                    Handle_S_MOVE(data);
                }
                break;
            case PacketType.PKT_S_MOVE_V2:
                using (MemoryStream dataStream = new MemoryStream(dataBytes))
                {
                    S_MOVE_V2 data = Serializer.Deserialize<S_MOVE_V2>(dataStream);
                    Handle_S_MOVE_V2(data);
                }
                break;
            case PacketType.PKT_S_MOVE_V3:
                using (MemoryStream dataStream = new MemoryStream(dataBytes))
                {
                    S_MOVE_V3 data = Serializer.Deserialize<S_MOVE_V3>(dataStream);
                    Handle_S_MOVE_V3(data);
                }
                break;
            case PacketType.PKT_S_POS:
                using (MemoryStream dataStream = new MemoryStream(dataBytes))
                {
                    S_POS data = Serializer.Deserialize<S_POS>(dataStream);
                    Handle_S_POS(data);
                }
                break;
            default:
                Debug.Log($"Unknown packet type: {header.PacketType}");
                break;
        }
    }

    public void Handle_S_ENTERGAME(S_ENTERGAME data)
    {
        if (data.PlayerId == Managers.Game.PlayerId)
        {
            // Managers.Game.SimulateMoveEvents();
            // Debug.Log($"Login success");
        }
        else
        {
            Managers.Game.SpawnPlayer(data.PlayerId, new Vector3(data.PosX, data.PosY, data.PosZ));
            // Debug.Log($"[User {data.PlayerId}] Enter game");
        }
    }

    public void Handle_S_LEAVEGAME(S_LEAVEGAME data)
    {
        Managers.Game.DespawnPlayer(data.PlayerId);
        // Debug.Log($"[User {data.PlayerId}] Leave game");
    }

    public void Handle_S_PLAYERLIST(S_PLAYERLIST data)
    {
        Managers.Game.PlayerId = data.PlayerId;
        Managers.Game.SpawnPlayer(data.PlayerId, new Vector3(3.0f, 1.0f, 3.0f));
        foreach (PlayerInfo player in data.Players)
        {
            if (player.playerId == data.PlayerId) continue;
            Managers.Game.SpawnPlayer(player.playerId, new Vector3(player.posX, player.posY, player.posZ));
        }
    }

    public void Handle_S_CHAT(S_CHAT data)
    {
        if (data.PlayerId == Managers.Game.PlayerId || Managers.Game.FindPlayer(data.PlayerId) == false)
        {
            return;
        }

        UI_Game ui = GameObject.Find("UI_Game").GetComponent<UI_Game>();
        if(ui != null)
        {
            //ui.EnterChat(data.PlayerId, data.Chat);
        }
    }

    public int COUNT = 0;

    public void Handle_S_MOVE(S_MOVE data)
    {
        if(data.PlayerId == Managers.Game.PlayerId || Managers.Game.FindPlayer(data.PlayerId) == false)
        {
            return;
        }
        if (_firstMove)
        {
            Managers.Game.SimulateMoveEvents();
            _firstMove = false;
        }
        COUNT++;
        Managers.Network.Delay = ((long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds - data.TimeStamp);
        Managers.Game.SetPlayerPosition(data.PlayerId, new Vector3(data.PosX, data.PosY, data.PosZ));
    }

    public void Handle_S_MOVE_V2(S_MOVE_V2 data)
    {
        if (data.PlayerId == Managers.Game.PlayerId || Managers.Game.FindPlayer(data.PlayerId) == false)
        {
            return;
        }
        if (_firstMove)
        {
            Managers.Game.SimulateMoveEvents();
            _firstMove = false;
        }
        long delay = ((long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds - data.TimeStamp);
        Managers.Network.Delay = delay;
        Managers.Game.SetPlayerVelocity(data.PlayerId, new Vector3(data.VelX, data.VelY, data.VelZ));
    }

    public void Handle_S_MOVE_V3(S_MOVE_V3 data)
    {
        if (data.PlayerId == Managers.Game.PlayerId || Managers.Game.FindPlayer(data.PlayerId) == false)
        {
            return;
        }
        if (_firstMove)
        {
            Managers.Game.SimulateMoveEvents();
            _firstMove = false;
        }
        float velX = data.Speed * Mathf.Cos(data.Theta * 2 * Mathf.PI);
        float velZ = data.Speed * Mathf.Sin(data.Theta * 2 * Mathf.PI);
        Managers.Game.SetPlayerVelocity(data.PlayerId, new Vector3(velX, 0, velZ));
    }

    Dictionary<int, long> _prevDelays = new Dictionary<int, long>();

    public void Handle_S_POS(S_POS data)
    {
        if (data.PlayerId == Managers.Game.PlayerId || Managers.Game.FindPlayer(data.PlayerId) == false)
        {
            return;
        }
        if (_firstMove)
        {
            Managers.Game.SimulateMoveEvents();
            _firstMove = false;
        }
        long delay = ((long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds - data.TimeStamp);
        Managers.Network.Delay = delay;
        Managers.Game.InterpolatePlayerPosition(data.PlayerId, new Vector3(data.PosX, data.PosY, data.PosZ), new Vector3(data.VelX, data.VelY, data.VelZ), delay);
        //if (_prevDelays.ContainsKey(data.PlayerId) == false)
        //{
        //    _prevDelays.Add(data.PlayerId, delay);
        //    Managers.Game.InterpolatePlayerPosition(data.PlayerId, new Vector3(data.PosX, data.PosY, data.PosZ), new Vector3(data.VelX, data.VelY, data.VelZ), delay);
        //}
        //else
        //{
        //    long delayDiff = delay - _prevDelays[data.PlayerId];
        //    _prevDelays[data.PlayerId] = delay;
        //    Managers.Game.InterpolatePlayerPosition(data.PlayerId, new Vector3(data.PosX, data.PosY, data.PosZ), new Vector3(data.VelX, data.VelY, data.VelZ), delayDiff);
        //}
    }
}
