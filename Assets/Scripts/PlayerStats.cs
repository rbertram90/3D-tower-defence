using System.Collections.Generic;
using Unity.Netcode;
using System;

/**
 * Maybe rename to player manager?
 */
public class PlayerStats : NetworkBehaviour
{
    private static PlayerStats _instance;
    public static PlayerStats Instance { get { return _instance; } }

    void Awake()
    {
        if (_instance != null && _instance != this) {
            Destroy(gameObject);
        }
        else {
            _instance = this;
        }
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

    public Player GetPlayerByClientId(ulong clientId)
    {
        if (NetworkManager.ConnectedClients.ContainsKey(clientId)) {
            return NetworkManager.ConnectedClients[clientId].PlayerObject.GetComponent<Player>();
        }

        return null;
    }
}
