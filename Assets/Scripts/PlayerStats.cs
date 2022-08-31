using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using System;

/**
 * Maybe rename to player manager?
 */
public class PlayerStats : NetworkBehaviour
{
    public static PlayerStats instance;

    void Awake()
    {
        instance = this;
    }

    public bool PlayerIsHost
    {
        get => IsHost;
    }

    public void DoForAllPlayers(Action<Player> processAction)
    {
        if (!IsHost) {
            return;
        }

        foreach (KeyValuePair<ulong, NetworkClient> client in NetworkManager.Singleton.ConnectedClients) {
            processAction(client.Value.PlayerObject.GetComponent<Player>());
        }
    }
}
