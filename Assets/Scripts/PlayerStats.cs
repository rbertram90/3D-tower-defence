using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerStats : NetworkBehaviour
{
    public static PlayerStats instance;

    // public static int money;
    public int startingBalance = 300;

    public static int money; // legacy - to be removed

    public static int lives;
    public int startLives = 20;

    public static int kills;
    public static int rounds;

    private NetworkVariable<int> balance = new NetworkVariable<int>();
    private NetworkVariable<int> playersInGame = new NetworkVariable<int>();

    void Awake()
    {
        instance = this;
    }

    public int PlayersInGame
    {
        get {
            return playersInGame.Value;
        }
    }

    public bool PlayerIsHost
    {
        get {
            return IsHost;
        }
    }

    public int Balance
    {
        get {
            return balance.Value;
        }
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer) {
            balance.Value = startingBalance;
        }
    }

    void Start ()
    {
        // balance.Value = startingBalance;
        lives = startLives;
        kills = 0;
        rounds = 0;
        playersInGame.Value = 1;

        NetworkManager.OnClientConnectedCallback += (id) => {
            if (IsServer) {
                playersInGame.Value++;
            }
        };

        NetworkManager.OnClientDisconnectCallback += (id) => {
            if (IsServer) {
                playersInGame.Value--;
            }
        };
    }

    // Note - probabily will change this as expect it will be part of larger logic
    // function to run on server.
    [ServerRpc]
    public void SubtractFromBalanceServerRpc(int amount)
    {
        balance.Value -= amount;
    }
}
