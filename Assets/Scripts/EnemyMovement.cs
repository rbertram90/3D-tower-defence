using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(Enemy))]
public class EnemyMovement : NetworkBehaviour {

    private Transform target;
    private int waypointIndex = 0;

    private Enemy enemy;

    // Use this for initialization
    void Start ()
    {
        enemy = GetComponent<Enemy>();
        target = waypoint.waypoints[0];
    }
	
	// Update is called once per frame
	void Update () {
        Vector3 dir = target.position - transform.position;
        transform.Translate(dir.normalized * enemy.Speed.Value * Time.deltaTime, Space.World);

        if (Vector3.Distance(transform.position, target.position) <= 0.2f)
        {
            GetNextWaypoint();
        }

        // enemy.speed = enemy.startSpeed;
    }

    void GetNextWaypoint()
    {
        if (waypointIndex >= waypoint.waypoints.Length - 1)
        {
            EndPath();
            return;
        }

        waypointIndex++;
        target = waypoint.waypoints[waypointIndex];
    }

    void EndPath()
    {
        WaveSpawner.instance.notifyDeath(enemy, false);

        Destroy(gameObject);
    }
}
