using Unity.Netcode;
using UnityEngine;

public abstract class AbstractTurret : NetworkBehaviour
{
    [HideInInspector]
    public Placement placement;

    protected float _range = 5f;
    public float range
    {
        get => _range;
        set => _range = value;
    }

    protected TargetingMode _targetingMode;

    public TargetingMode TargetingMode
    {
        get => _targetingMode;
        set => _targetingMode = value;
    }

    protected bool canShoot(GameObject enemy, float distanceToEnemy)
    {
        // Check that enemy is in range
        if (distanceToEnemy > range) return false;

        switch (placement.LookDirection) {
            case Placement.Facing.Up:
                return transform.position.y <= enemy.transform.position.y;
            case Placement.Facing.Down:
                return transform.position.y >= enemy.transform.position.y;
            case Placement.Facing.Forwards:
                return transform.position.z <= enemy.transform.position.z;
            case Placement.Facing.Backwards:
                return transform.position.z >= enemy.transform.position.z;
            case Placement.Facing.Right:
                return transform.position.x <= enemy.transform.position.x;
            case Placement.Facing.Left:
                return transform.position.x >= enemy.transform.position.x;
        }

        return true;
    }

    protected GameObject getClosestEnemy(GameObject[] enemies)
    {
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (GameObject enemy in enemies) {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (!canShoot(enemy, distanceToEnemy)) continue;

            if (distanceToEnemy < shortestDistance) {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }

        return nearestEnemy;
    }

    protected GameObject getFurthestEnemy(GameObject[] enemies)
    {
        float furthestDistance = 0f;
        GameObject furthestEnemy = null;

        foreach (GameObject enemy in enemies) {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (!canShoot(enemy, distanceToEnemy)) continue;

            if (distanceToEnemy > furthestDistance && distanceToEnemy <= range) {
                furthestDistance = distanceToEnemy;
                furthestEnemy = enemy;
            }
        }

        return furthestEnemy;
    }

    protected GameObject getStrongestEnemy(GameObject[] enemies)
    {
        float highestHealth = 0f;
        GameObject strongestEnemy = null;

        foreach (GameObject enemy in enemies) {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (!canShoot(enemy, distanceToEnemy)) continue;

            float enemyHealth = enemy.GetComponent<Enemy>().health;

            if (enemyHealth > highestHealth && distanceToEnemy <= range) {
                highestHealth = enemyHealth;
                strongestEnemy = enemy;
            }
        }

        return strongestEnemy;
    }

    protected GameObject getWeakestEnemy(GameObject[] enemies)
    {
        float lowestHealth = Mathf.Infinity;
        GameObject weakestEnemy = null;

        foreach (GameObject enemy in enemies) {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (!canShoot(enemy, distanceToEnemy)) continue;

            float enemyHealth = enemy.GetComponent<Enemy>().health;

            if (enemyHealth < lowestHealth && distanceToEnemy <= range) {
                lowestHealth = enemyHealth;
                weakestEnemy = enemy;
            }
        }

        return weakestEnemy;
    }

    protected GameObject getFastestEnemy(GameObject[] enemies)
    {
        float fastest = 0f;
        GameObject fastestEnemy = null;

        foreach (GameObject enemy in enemies) {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (!canShoot(enemy, distanceToEnemy)) continue;

            float enemySpeed = enemy.GetComponent<Enemy>().speed;

            if (enemySpeed > fastest && distanceToEnemy <= range) {
                fastest = enemySpeed;
                fastestEnemy = enemy;
            }
        }

        return fastestEnemy;
    }

    protected GameObject getSlowestEnemy(GameObject[] enemies)
    {
        float slowest = Mathf.Infinity;
        GameObject slowestEnemy = null;

        foreach (GameObject enemy in enemies) {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (!canShoot(enemy, distanceToEnemy)) continue;

            float enemySpeed = enemy.GetComponent<Enemy>().speed;

            if (enemySpeed < slowest && distanceToEnemy <= range) {
                slowest = enemySpeed;
                slowestEnemy = enemy;
            }
        }

        return slowestEnemy;
    }

}
