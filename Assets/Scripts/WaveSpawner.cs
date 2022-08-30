using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class WaveSpawner : NetworkBehaviour {

    public int WaveNumber;
    public Transform spawnPoint;
    public GameObject enemyPrefab;
    public GameObject roundCompleteUI;
    public Text waveNumberText;

    public static WaveSpawner instance;

    private List<GameObject> enemies;
    private GameManager gameManager;

    void Awake()
    {
        instance = this; // singleton
    }

    // Use this for initialization
    void Start ()
    {
        gameManager = FindObjectOfType<GameManager>();
        enemies = new List<GameObject>();
        WaveNumber = 0;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsHost) {
            roundCompleteUI.transform.Find("NextRound").gameObject.SetActive(false);
        }

        // @todo disable the button for the host until all players are ready

        base.OnNetworkSpawn();
    }
	
	// Update is called once per frame
	void Update ()
    {
        waveNumberText.text = "Round " + WaveNumber.ToString();
    }

    public int getWaveNumber()
    {
        return WaveNumber;
    }

    public void runSpawnWave()
    {
        if (!IsHost) {
            return;
        }

        // Run enemy spawning on server
        // Let each client handle the positions and gun calculations
        // Likely will have lost sync by the time the round is over
        // Much like the existing game the round may appear to be over
        // before/after the server finishes, but don't see it as a critical
        // problem as syncronising the position, rotation of the guns
        // sounds like it would be intensive on server when there are
        // many guns placed in scene.
        StartCoroutine(SpawnWave());

        roundCompleteUI.SetActive(false);

        // RunSpawnWaveClientRpc();
    }

    /*
    [ClientRpc]
    public void RunSpawnWaveClientRpc()
    {
        StartCoroutine(SpawnWave());

        roundCompleteUI.SetActive(false);
    }
    */

    public void notifyDeath(Enemy e, bool WasKilled)
    {
        if (IsHost) {
            if (WasKilled) {
                gameManager.KillsThisRound.Value++;

                gameManager.TotalKillsMade.Value++;
            }
            else {
                gameManager.Lives.Value--;
            }

            enemies.Remove(e.gameObject);
        }

        if (enemies.Count == 0)
        {
            roundCompleteUI.SetActive(true);
        }
    }

    IEnumerator SpawnWave()
    {
        gameManager.KillsThisRound.Value = 0;
        WaveNumber++;

        for (int i = 0; i < WaveNumber; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(0.25f); // delay
        }
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