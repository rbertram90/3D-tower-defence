using Mono.Cecil.Cil;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Enemy : NetworkBehaviour {

    public float startSpeed = 4f;

    // [HideInInspector]
    // public float speed;

    public NetworkVariable<float> Speed = new NetworkVariable<float>();

    public NetworkVariable<float> Health = new NetworkVariable<float>();

    // public float health = 100;
    public int worth = 10;
    
    public GameObject deathEffect;

    private GameManager gM;
    private WaveSpawner wS;

	void Start ()
    {
        gM = GameManager.instance;
        wS = WaveSpawner.instance;

        if (IsHost) {
            // Server sets the speed.
            Speed.Value = 5f + (Random.value * 4f);

            Health.Value = 2 + wS.WaveNumber;
        }

        // int wavenum = gM.GetWaveSpawner.getWaveNumber();
        // health = 100 + 10 * wavenum;
    }

    public override void OnNetworkSpawn()
    {
        Health.OnValueChanged += (float oldValue, float newValue) => {
            if (newValue <= 0) {
                // Enemy has been killed
                // Host only
                if (IsHost) {
                    PlayerStats.instance.DoForAllPlayers((Player p) => {
                        p.Balance.Value += 50;
                    });

                    wS.notifyDeath(this, true);

                    Destroy(gameObject);
                }

                // All clients
                Die();
            }
        };
    }

    public void TakeDamage(float amount)
    {
        if (IsHost) {
            Health.Value -= amount;
        }
    }

    // public void Slow(float percent)
    // {
    //     speed = startSpeed * (1f - percent);
    // }

    void Die()
    {
        GameObject effect = Instantiate(deathEffect, transform.position, Quaternion.identity);

        Destroy(effect, 5f);
    }

}