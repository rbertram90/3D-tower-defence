using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Random = System.Random;
using System.Collections.Generic;

public class Player : NetworkBehaviour
{
    public enum States { Busy, Ready };

    public NetworkVariable<States> Status = new();
    public NetworkVariable<NetworkString> Name = new();
    public NetworkVariable<ulong> ClientID = new();
    public NetworkVariable<int> Balance = new();

    string[] names = new string[] {
        "John", "James", "Janet", "Mary", "Paul",
        "Sarah", "Mike", "Lucy", "Fred", "Hillary",
        "Jeff", "Penelope", "Steve", "Daisy", "Rebecca",
        "Lauren", "Jessica", "Charlotte", "Hannah", "Sophie",
        "Amy", "Emily", "Laura", "Emma", "Thomas", "Sam",
        "Jack", "Daniel", "Matthew", "Ryan", "Josh", "Luke",
    };

    string[] surnames = new string[] {
        "Smith", "Collins", "Baker", "Jackson", "Johnson",
        "Brown", "Wilson", "Thomson", "Campbell", "Anderson",
        "Macdonald", "Scott", "Reid", "Murray", "Taylor",
        "Clark", "Mitchell", "Walker", "Paterson"
    };

    public override void OnNetworkSpawn()
    {
        // NetworkSpawn of any player, but this part is run on the server.
        if (IsHost) {
            Status.Value = States.Busy;

            // Generate a random name for the player
            // @todo allow the user to change this.
            Random random = new Random();
            Name.Value = names[random.Next(0, names.Length - 1)] + surnames[random.Next(0, surnames.Length)];

            Balance.Value = 300000;

            // Listen for status changes
            // i.e. when the player has finished building turrets and
            // is ready to play the next round.
            Status.OnValueChanged += (States oldValue, States newValue) => {
                if (newValue == States.Ready) {
                    bool allReady = true;

                    foreach (KeyValuePair<ulong, NetworkClient> client in NetworkManager.Singleton.ConnectedClients) {
                        Player p = client.Value.PlayerObject.GetComponent<Player>();
                        bool pIsHost = NetworkManager.Singleton.LocalClient.Equals(client.Value);
                        if (p.Status.Value == States.Busy && !pIsHost) {
                            allReady = false;
                            break;
                        }
                    }

                    if (allReady) {
                        GameObject.Find("NextRound").GetComponent<Button>().interactable = true;
                    }
                }
                else {
                    GameObject.Find("NextRound").GetComponent<Button>().interactable = false;
                }
            };
        }

        // When anyone changes status update the users list
        Status.OnValueChanged += (States oldValue, States newValue) => {
            FindObjectOfType<NetworkUI>().UpdatePlayerList();
        };

        base.OnNetworkSpawn();
    }

    [ServerRpc]
    public void ReadyUpServerRpc(ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;

        var player = PlayerStats.Instance.GetPlayerByClientId(clientId);

        if (player != null) {
            player.Status.Value = States.Ready;
        }
    }

}
