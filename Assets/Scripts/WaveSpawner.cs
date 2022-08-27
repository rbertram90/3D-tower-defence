using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class WaveSpawner : NetworkBehaviour {

    public int waveNumber;
    public Transform spawnPoint;
    public GameObject enemyPrefab;
    public GameObject roundCompleteUI;
    public Text waveNumberText;

    public static WaveSpawner instance;

    private List<GameObject> enemies;

    void Awake()
    {
        instance = this; // singleton
    }

    // Use this for initialization
    void Start ()
    {
        enemies = new List<GameObject>();
        waveNumber = 0;
        roundCompleteUI.SetActive(false);
    }
	
	// Update is called once per frame
	void Update ()
    {
        waveNumberText.text = "Round " + waveNumber.ToString();
    }

    public int getWaveNumber()
    {
        return waveNumber;
    }

    public void runSpawnWave()
    {
        StartCoroutine(SpawnWave());

        roundCompleteUI.SetActive(false);
    }

    public void notifyDeath(Enemy e)
    {
        enemies.Remove(e.gameObject);

        if (enemies.Count == 0)
        {
            roundCompleteUI.SetActive(true);
        }
    }

    IEnumerator SpawnWave()
    {
        waveNumber++;

        for (int i = 0; i < waveNumber; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(0.25f); // delay
        }

        PlayerStats.rounds++;
    }

    /**
     * Want to spawn random enemies - e.g. total health is 100, first one takes 10 (random)
     * second 13 (random) and so on until the 100 health has run out
     */

    void SpawnEnemy()
    {
        GameObject newEnemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

        newEnemy.GetComponent<NetworkObject>().Spawn();

        enemies.Add(newEnemy);
    }
}