using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    [HideInInspector]
    public static bool gameEnded;
    public Text killsText;

    public GameObject gameOverUI;

    private TimedWaveSpawner wS;

    public TimedWaveSpawner GetWaveSpawner { get { return wS; } }

    void Awake()
    {
        instance = this; // singleton
    }

    // Use this for initialization
    void Start ()
    {
        Time.timeScale = 1f;
        gameEnded = false;
        wS = GetComponent<TimedWaveSpawner>();
    }

    // Update is called once per frame
    void Update ()
    {
        killsText.text = "Kills: " + PlayerStats.kills;

        if (gameEnded)
            return;

		if(PlayerStats.lives <= 0)
        {
            // end game
            EndGame();
        }
	}

    void EndGame()
    {
        Debug.Log("Game Ended");

        gameEnded = true;
        gameOverUI.SetActive(true);
        Time.timeScale = 0.2f;
    }

}
