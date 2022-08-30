using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using System;
using System.Collections.Generic;

public class NetworkUI : MonoBehaviour
{
    [SerializeField]
    public Button StartServerButton;
    [SerializeField]
    public Button StopServerButton;
    [SerializeField]
    public Button JoinServerButton;

    public TMP_InputField HostPortInput;
    public TMP_InputField IPAddressInput;
    public TMP_InputField PortNumberInput;

    [SerializeField]
    public TextMeshProUGUI PlayersInGameText;

    private PlayerStats PlayerStats;
    private GameManager GameManager;

    public List<float> playersIds;

    private string _playerList;

    // Start is called before the first frame update
    void Start()
    {
        GameManager = FindObjectOfType<GameManager>();
        PlayerStats = GameManager.GetComponent<PlayerStats>();

        StartServerButton.onClick.AddListener(() => {
            try {
                UnityTransport transport = this.GetComponentInParent<UnityTransport>();
                transport.ConnectionData.Port = ushort.Parse(HostPortInput.text);

                if (NetworkManager.Singleton.StartHost()) {
                    Debug.Log("Host started!");
                }
                else {
                    Debug.Log("Host not started :(");
                }
            }
            catch (FormatException) {
                Debug.Log("Server not started - Unable to parse port");
            }
        });

        // StopServerButton.onClick.AddListener(() => {
        //
        // });

        JoinServerButton.onClick.AddListener(() => {
            UnityTransport transport = this.GetComponentInParent<UnityTransport>();
            transport.ConnectionData.Address = IPAddressInput.text;

            try {
                transport.ConnectionData.Port = ushort.Parse(PortNumberInput.text);

                if (NetworkManager.Singleton.StartClient()) {
                    Debug.Log("Client started!");
                }
                else {
                    Debug.Log("Client not started :(");
                }
            }
            catch (FormatException) {
                Debug.Log("Client not started - Unable to parse port");
            }
        });
    }

    // Update is called once per frame
    void Update()
    {
        PlayersInGameText.text = $"Players in game: {GameManager.PlayersInGame} \n"
            + (PlayerStats.PlayerIsHost ? "Host" : "Client") + "\n" + _playerList;
    }

    public void UpdatePlayerList()
    {
        _playerList = "";
        Player[] players = FindObjectsOfType<Player>();

        foreach (Player p in players) {
            _playerList += p.Name.Value + "(" +  p.ClientID.Value + "): " + p.Status.Value + "\n";
        }
    }
}
