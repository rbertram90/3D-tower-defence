using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour
{
    public static GameManager instance;
    
    [HideInInspector]
    public static bool gameEnded;
    public Text killsText;

    public GameObject gameOverUI;

    // Private variables
    private NetworkUI _networkUI;

    // Network variables
    public NetworkVariable<int> TotalKillsMade = new NetworkVariable<int>();
    public NetworkVariable<int> KillsThisRound = new NetworkVariable<int>();
    public NetworkVariable<int> Lives = new NetworkVariable<int>();

    private NetworkVariable<int> ActivePlayerCount = new NetworkVariable<int>();

    public int PlayersInGame
    {
        get => ActivePlayerCount.Value;
    }

    void Awake()
    {
        instance = this; // singleton
    }

    // Use this for initialization
    void Start ()
    {
        Time.timeScale = 1f;
        gameEnded = false;
        ActivePlayerCount.Value = 1;
        Lives.Value = 20;

        // GameObjects
        _networkUI = FindObjectOfType<NetworkUI>();

        NetworkManager.OnClientConnectedCallback += (id) => {
            if (IsHost) {
                ActivePlayerCount.Value++;

                Player player = NetworkManager.Singleton
                    .ConnectedClients[id]
                    .PlayerObject
                    .GetComponent<Player>();

                player.ClientID.Value = id;

                foreach (KeyValuePair<ulong, NetworkClient> client in NetworkManager.Singleton.ConnectedClients) {
                    Player p = client.Value.PlayerObject.GetComponent<Player>();
                    if (p.Status.Value == Player.States.Busy) {
                        GameObject.Find("NextRound").GetComponent<Button>().interactable = false;
                        break;
                    }
                }
            }
        };

        NetworkManager.OnClientDisconnectCallback += (id) => {
            if (IsServer) {
                ActivePlayerCount.Value--;
            }
        };

        ActivePlayerCount.OnValueChanged += (int oldValue, int newValue) => {
            _networkUI.UpdatePlayerList();
        };
    }

    // Update is called once per frame
    void Update ()
    {
        killsText.text = "Kills: total = " + TotalKillsMade.Value + ", round = " + KillsThisRound.Value;

        if (gameEnded) {
            return;
        }

		if (Lives.Value <= 0) {
            EndGame();
        }
	}

    void EndGame()
    {
        Debug.Log("Game Ended");

        gameEnded = true;

        gameOverUI.SetActive(true);

        Time.timeScale = 0.2f; // slow things down.
    }

}
