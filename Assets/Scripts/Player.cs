using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Random = System.Random;
using System.Collections.Generic;

public class Player : NetworkBehaviour
{
    public enum States { Busy, Ready };

    public NetworkVariable<States> Status = new NetworkVariable<States>(default, default, NetworkVariableWritePermission.Owner);

    public NetworkVariable<NetworkString> Name = new NetworkVariable<NetworkString>();

    public NetworkVariable<ulong> ClientID = new NetworkVariable<ulong>();

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
        if (IsServer) {
            Status.Value = States.Busy;

            Random random = new Random();
            Name.Value = names[random.Next(0, names.Length - 1)] + surnames[random.Next(0, surnames.Length)];

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
            };
        }

        Status.OnValueChanged += (States oldValue, States newValue) => {
            FindObjectOfType<NetworkUI>().UpdatePlayerList();
        };

        base.OnNetworkSpawn();
    }

    public void ReadyUp()
    {
        Status.Value = States.Ready;
    }

}
