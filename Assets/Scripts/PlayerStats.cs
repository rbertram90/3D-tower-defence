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

    // public static int money;
    public int startingBalance = 300;

    public static int money; // legacy - to be removed

    private NetworkVariable<int> balance = new NetworkVariable<int>();   

    // public Dictionary<>

    void Awake()
    {
        instance = this;
    }

    public bool PlayerIsHost
    {
        get => IsHost;
    }

    public int Balance
    {
        get => balance.Value;
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer) {
            balance.Value = startingBalance;
        }
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

    void Start ()
    {
        // balance.Value = startingBalance;
    }

    // Note - probabily will change this as expect it will be part of larger logic
    // function to run on server.
    [ServerRpc]
    public void SubtractFromBalanceServerRpc(int amount)
    {
        balance.Value -= amount;
    }

    // Note - can accept ServerRpcParams params = default to get client
    // params.Recieve.SenderClientId

}
