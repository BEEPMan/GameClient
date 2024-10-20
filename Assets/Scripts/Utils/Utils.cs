using ProtoBuf;
using System;
using System.IO;
using UnityEngine;

public class Utils
{
    public static byte[] SerializePacket(PacketType packetType, object data)
    {
        byte[] headerBytes = new byte[sizeof(ushort) * 2];
        byte[] dataBytes;

        using (MemoryStream dataStream = new MemoryStream())
        {
            Serializer.Serialize(dataStream, data);
            dataBytes = dataStream.ToArray();
        }

        ushort size = (ushort)(dataBytes.Length + sizeof(ushort) * 2);
        BitConverter.TryWriteBytes(new Span<byte>(headerBytes, 0, sizeof(ushort)), size);
        BitConverter.TryWriteBytes(new Span<byte>(headerBytes, sizeof(ushort), sizeof(ushort)), (ushort)packetType);
        //PacketHeader header = new PacketHeader { Size = size, PacketType = (ushort)packetType };
        //using (MemoryStream headerStream = new MemoryStream())
        //{
        //    Serializer.Serialize(headerStream, header);
        //    headerBytes = headerStream.ToArray();
        //}

        byte[] finalPacket = new byte[headerBytes.Length + dataBytes.Length];
        Buffer.BlockCopy(headerBytes, 0, finalPacket, 0, headerBytes.Length);
        Buffer.BlockCopy(dataBytes, 0, finalPacket, headerBytes.Length, dataBytes.Length);

        return finalPacket;
    }

    public static T GetOrAddComponent<T>(GameObject go) where T : UnityEngine.Component
    {
        T component = go.GetComponent<T>();
        if(component == null)
            component = go.AddComponent<T>();

        return component;
    }

    public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
    {
        Transform transform = FindChild<Transform>(go, name, recursive);
        if (transform == null)
            return null;

        return transform.gameObject;
    }

    public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
    {
        if (go == null)
            return null;

        if (!recursive)
        {
            for (int i = 0; i < go.transform.childCount; i++)
            {
                Transform transform = go.transform.GetChild(i);
                if (string.IsNullOrEmpty(name) || transform.name == name)
                {
                    T component = transform.GetComponent<T>();
                    if(component != null)
                        return component;
                }
            }
        }
        else
        {
            foreach(T component in go.GetComponentsInChildren<T>())
            {
                if (string.IsNullOrEmpty(name) || component.name == name)
                {
                    return component;
                }
            }
        }
        return null;
    }
}