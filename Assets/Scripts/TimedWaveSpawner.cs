using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TimedWaveSpawner : MonoBehaviour
{
    public Transform enemyPrefab;
    public Transform spawnPoint;
    public Text waveCountdownText;
    public float timeBetweenWaves = 20.5f;

    private float countDown = 2f;
    private int waveNumber;

	// Use this for initialization
	void Start ()
    {
        waveNumber = 1;
    }

    public int getWaveNumber()
    {
        return waveNumber;
    }
	
	// Update is called once per frame
	void Update ()
    {
		if (countDown <= 0f)
        {
            StartCoroutine(SpawnWave());
            countDown = timeBetweenWaves;
        }

        countDown -= Time.deltaTime;
        countDown = Mathf.Clamp(countDown, 0f, Mathf.Infinity);

        // Set the countdown - whole number only!
        waveCountdownText.text = string.Format("{0:00.00}", countDown);
	}

    // Co-routine
    IEnumerator SpawnWave()
    {
        for (int i = 0; i < waveNumber; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(0.5f); // delay
        }

        PlayerStats.rounds++;
        waveNumber++;
    }

    void SpawnEnemy()
    {
        Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
    }
}
