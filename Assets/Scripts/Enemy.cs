using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    public float startSpeed = 4f;
    [HideInInspector]
    public float speed;

    public float health = 100;
    public int worth = 10;
    
    public GameObject deathEffect;

    private GameManager gM;
    private WaveSpawner wS;

	// Use this for initialization
	void Start ()
    {
        speed = 5f + (Random.value * 4f);

        gM = GameManager.instance;
        wS = WaveSpawner.instance;

        int wavenum = gM.GetWaveSpawner.getWaveNumber();
        health = 100 + 10 * wavenum;
    }

    public void TakeDamage(float amount)
    {
        health -= amount;

        if(health <= 0)
        {
            Die();
        }
    }

    public void Slow(float percent)
    {
        speed = startSpeed * (1f - percent);
    }

    void Die()
    {
        PlayerStats.kills++;

        PlayerStats.money += worth;

        GameObject effect = Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(effect, 5f);

        wS.notifyDeath(this);

        Destroy(gameObject);
    }

	// Update is called once per frame
	void Update ()
    {

    }
}