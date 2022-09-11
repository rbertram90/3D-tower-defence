using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    [HideInInspector]
    public static bool gameEnded;
    public Text killsText;

    public GameObject gameOverUI;

    // Private variables
    private NetworkUI _networkUI;

    private NetworkManager NM;

    // Network variables
    public NetworkVariable<int> TotalKillsMade = new();
    public NetworkVariable<int> KillsThisRound = new();
    public NetworkVariable<int> Lives = new();
    private NetworkVariable<int> ActivePlayerCount = new();

    private Player LocalPlayer;

    public float GameSpeed;

    public int PlayersInGame
    {
        get => ActivePlayerCount.Value;
    }

    void Awake()
    {
        // singleton
        if (_instance != null && _instance != this) {
            Destroy(gameObject);
        }
        else {
            _instance = this;
        }
    }

    public override void OnNetworkSpawn()
    {
        NM = FindObjectOfType<NetworkManager>();

        GetLocalPlayer();
    }

    public Player GetLocalPlayer()
    {
        if (LocalPlayer != null) {
            return LocalPlayer;
        }

        try {
            LocalPlayer = NM.LocalClient.PlayerObject.GetComponent<Player>();
            return LocalPlayer;
        }
        catch (Exception e) {
            return null;
        }
    }

    // Use this for initialization
    void Start ()
    {
        // Time.timeScale = 1f;
        Time.timeScale = GameSpeed;
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
        Time.timeScale = Mathf.Abs(GameSpeed); // useful when debugging things

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
