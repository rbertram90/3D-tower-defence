using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour {

    public Text roundsText;

    void OnEnable()
    {
        WaveSpawner spawner = FindObjectOfType<WaveSpawner>();
        roundsText.text = spawner.WaveNumber.ToString();
    }

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Menu()
    {
        SceneManager.LoadScene(0); // todo: build menu!
    }
}
