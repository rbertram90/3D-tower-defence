using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;

public class NetworkUI : MonoBehaviour
{
    [SerializeField]
    public Button StartServerButton;
    [SerializeField]
    public Button StopServerButton;
    [SerializeField]
    public Button JoinServerButton;
    [SerializeField]
    public TextMeshProUGUI PlayersInGameText;

    private PlayerStats PlayerStats;

    // Start is called before the first frame update
    void Start()
    {
        PlayerStats = GameObject.Find("GameMaster").GetComponent<PlayerStats>();

        StartServerButton.onClick.AddListener(() => {
            if (NetworkManager.Singleton.StartHost()) {
                Debug.Log("Host started!");
            }
            else {
                Debug.Log("Host not started :(");
            }
        });

        // StopServerButton.onClick.AddListener(() => {
        //
        // });

        JoinServerButton.onClick.AddListener(() => {
            if (NetworkManager.Singleton.StartClient()) {
                Debug.Log("Client started!");
            }
            else {
                Debug.Log("Client not started :(");
            }
        });
    }

    // Update is called once per frame
    void Update()
    {
        PlayersInGameText.text = $"Players in game: {PlayerStats.PlayersInGame} \n"
            + (PlayerStats.PlayerIsHost ? "Host" : "Client");
    }
}
