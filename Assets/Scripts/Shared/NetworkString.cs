using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Netcode;

/**
 * For whatever reason unity network variables doesn't accept
 * normal strings. So we need to implement a class like this.
 */
public struct NetworkString : INetworkSerializable
{
    private FixedString32Bytes info;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref info);
    }

    public override string ToString()
    {
        return info.ToString();
    }

    public static implicit operator string(NetworkString s) => s.ToString();

    public static implicit operator NetworkString(string s) => new NetworkString() {
        info = new FixedString32Bytes(s)
    };
}
