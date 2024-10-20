using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacketQueue
{
    public static PacketQueue Instance { get; } = new PacketQueue();

    Queue<byte[]> _packetQueue = new Queue<byte[]>();
    object _lock = new object();

    public void Push(byte[] packet)
    {
        lock (_lock)
        {
            _packetQueue.Enqueue(packet);
        }
    }

    public byte[] Pop()
    {
        lock (_lock)
        {
            if (_packetQueue.Count > 0)
            {
                return _packetQueue.Dequeue();
            }
            return null;
        }
    }

    public void Flush()
    {
        lock (_lock)
        {
            while(_packetQueue.Count > 0)
            {
                byte[] packet = _packetQueue.Dequeue();
                Managers.Packet.HandlePacket(packet);
            }
            _packetQueue.Clear();
        }
    }

    public bool IsEmpty()
    {
        lock (_lock)
        {
            return _packetQueue.Count == 0;
        }
    }
}
