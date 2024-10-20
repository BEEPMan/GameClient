﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public abstract class PacketSession : Session
{
    public static readonly int HeaderSize = 2;

    public override int OnRecv(ArraySegment<byte> buffer)
    {
        int processLen = 0;

        while (true)
        {
            if (buffer.Count < HeaderSize)
                break;

            ushort dataSize = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
            if (buffer.Count < dataSize)
                break;

            OnRecvPacket(new ArraySegment<byte>(buffer.Array, buffer.Offset, dataSize));

            processLen += dataSize;
            buffer = new ArraySegment<byte>(buffer.Array, buffer.Offset + dataSize, buffer.Count - dataSize);
        }

        return processLen;
    }

    public abstract void OnRecvPacket(ArraySegment<byte> buffer);
}

public abstract class Session
{
    Socket _socket;
    int _disconnected = 0;

    RecvBuffer _recvBuffer = new RecvBuffer(65535);

    object _lock = new object();
    Queue<byte[]> _sendQueue = new Queue<byte[]>();
    List<byte[]> _pendingList = new List<byte[]>();
    SocketAsyncEventArgs _sendArgs = new SocketAsyncEventArgs();
    SocketAsyncEventArgs _recvArgs = new SocketAsyncEventArgs();

    public int _id;

    public void Start(Socket socket)
    {
        _socket = socket;
        _socket.NoDelay = true;

        _recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);
        _sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);

        RegisterRecv();
    }

    public void Send(byte[] sendBuffer)
    {
        lock (_lock)
        {
            _sendQueue.Enqueue(sendBuffer);
            if (_pendingList.Count == 0)
                RegisterSend();
        }
    }

    public void Disconnect()
    {
        if (Interlocked.Exchange(ref _disconnected, 1) == 1)
            return;
        OnDisconnected(_socket.RemoteEndPoint);
        _socket.Shutdown(SocketShutdown.Both);
        _socket.Close();
    }

    private void RegisterSend()
    {
        while (_sendQueue.Count > 0)
        {
            byte[] buffer = _sendQueue.Dequeue();
            _pendingList.Add(buffer);
        }
        foreach (byte[] buffer in _pendingList)
            _sendArgs.BufferList = new List<ArraySegment<byte>> { new ArraySegment<byte>(buffer) };

        bool pending = _socket.SendAsync(_sendArgs);
        if (!pending)
            OnSendCompleted(null, _sendArgs);
    }

    private void OnSendCompleted(object sender, SocketAsyncEventArgs args)
    {
        lock (_lock)
        {
            if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
            {
                try
                {
                    _sendArgs.BufferList = null;
                    _pendingList.Clear();

                    OnSend(_sendArgs.BytesTransferred);

                    if (_sendQueue.Count > 0)
                        RegisterSend();
                }
                catch (Exception e)
                {
                    Debug.Log($"OnSendCompleted Failed {e}");
                }
            }
            else
            {
                Disconnect();
            }
        }
    }

    private void RegisterRecv()
    {
        _recvBuffer.Clean();

        ArraySegment<byte> segment = _recvBuffer.WriteSegment;
        _recvArgs.SetBuffer(segment.Array, segment.Offset, segment.Count);

        bool pending = _socket.ReceiveAsync(_recvArgs);
        if (!pending)
            OnRecvCompleted(null, _recvArgs);
    }

    private void OnRecvCompleted(object sender, SocketAsyncEventArgs args)
    {
        if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
        {
            try
            {
                //Debug.Log($"OnRecvCompleted: {args.BytesTransferred}");
                if (!_recvBuffer.OnWrite(args.BytesTransferred))
                {
                    Disconnect();
                    return;
                }
                int processLen = OnRecv(_recvBuffer.ReadSegment);
                if (processLen < 0 || _recvBuffer.DataSize < processLen)
                {
                    Disconnect();
                    return;
                }

                if (!_recvBuffer.OnRead(processLen))
                {
                    Disconnect();
                    return;
                }

                RegisterRecv();
            }
            catch (Exception e)
            {
                Debug.Log($"OnRecvCompleted Failed {e}");
            }
        }
        else
        {
            Disconnect();
        }
    }

    public abstract void OnConnected(EndPoint endPoint);
    public abstract int OnRecv(ArraySegment<Byte> buffer);
    public abstract void OnSend(int numOfBytes);
    public abstract void OnDisconnected(EndPoint endPoint);
}