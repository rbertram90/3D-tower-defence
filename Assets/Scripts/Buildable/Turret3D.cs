using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Turret3D : AbstractTurret, IBuildable
{
    private Transform target;
    private Enemy targetEnemy;
    private BuildManager buildManager;

    [Header("General")]
    public GameObject button;
    public GameObject ShopButton { get => this.button; }

    public int ShopIdentifier { get => 1; }

    public TargetingMode targetingMode;

    [HideInInspector]
    public bool isUpgraded;

    [Header("Use Bullets (Default)")]

    public float fireRate = 1f;
    public float fireCountdown = 0f;
    public GameObject bulletPrefab;

    [Header("Lazer Setup")]

    public bool useLazer = false;
    public LineRenderer lineRenderer;
    public int damagePerSecond = 2;
    public float slowPercent = 0.5f;

    [Header("Unity Setup")]

    public Transform partToRotate;
    public Transform firePoint;
    public float turnSpeed = 10f;
    public string enemyTag = "Enemy";
    public Vector3 positionOffset;

    public int Cost
    {
        get { return 50; }
    }

    // Do the same as placement action...hmmm
    // should we be putting more functionality into the turret?
    void OnMouseDown ()
    {
        RealPlacement.performOnMouseDown();
    }

    // Use this for initialization
    void Start()
    {
        // Search for targets every 1/2 second
        InvokeRepeating("UpdateTarget", 0f, 0.5f);
        targetingMode = TargetingMode.Closest;
        buildManager = BuildManager.instance;
        range = 5f; // this is the radius

        // Set the size of the target circle - note x and z should be the same.
        // Multiply by 2 to get the diameter
        transform.GetChild(2).transform.localScale = new Vector3(range / transform.localScale.x * 2, 0, range / transform.localScale.z * 2);
    }

    // Update is called once per frame
    void Update()
    {
        // Ensure we've got a target
        if (target == null)
        {
            if (useLazer)
            {
                // Stop the lazer
                if (lineRenderer.enabled)
                    lineRenderer.enabled = false;
            }

            return;
        }

        LockOnTarget();

        if (useLazer)
        {
            Lazer();
        }
        else
        {
            if (fireCountdown <= 0f)
            {
                Shoot();
                fireCountdown = 1f / fireRate;
            }

            fireCountdown -= Time.deltaTime;
        }
    }

    public Vector3 GetBuildPosition(Placement parent)
    {
        return parent.transform.position + positionOffset;
    }

    public Quaternion GetBuildRotation(Placement parent)
    {
        Vector3 upRotation = transform.up;
        Vector3 forwardRotation = transform.forward;

        switch (parent.LookDirection) {
            case Placement.Facing.Up:
                // All good
                break;
            case Placement.Facing.Down:
                upRotation = Vector3.down;
                break;
            case Placement.Facing.Forwards:
                forwardRotation = Vector3.down;
                // upRotation = Vector3.down;
                break;
            case Placement.Facing.Backwards:
                forwardRotation = Vector3.up;
                break;
            case Placement.Facing.Left:
                upRotation = Vector3.left;
                break;
            case Placement.Facing.Right:
                upRotation = Vector3.right;
                break;
        }

        return Quaternion.LookRotation(forwardRotation, upRotation);
    }

    void LockOnTarget()
    {
        // Look at target
        // Vector3 direction = target.position - transform.position;
        // Quaternion lookRotation = Quaternion.LookRotation(direction);
        // Lerp smooths the change
        // Vector3 rotation = Quaternion.Lerp(partToRotate.rotation, lookRotation, Time.deltaTime * turnSpeed).eulerAngles;
        // Only rotate around y axis
        // partToRotate.rotation = Quaternion.Euler(-90f, rotation.y, 0f);
        partToRotate.LookAt(target);
    }

    void Lazer()
    {
        targetEnemy.TakeDamage(damagePerSecond * Time.deltaTime);
        // targetEnemy.Slow(slowPercent);

        if (!lineRenderer.enabled)
            lineRenderer.enabled = true;

        lineRenderer.SetPosition(0, firePoint.position);
        lineRenderer.SetPosition(1, target.position);
    }

    void UpdateTarget()
    {
        // Get all enemies, possible to optimise this?
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        GameObject localTarget;

        switch (targetingMode)
        {
            case TargetingMode.Fastest:
                localTarget = getFastestEnemy(enemies);
                break;

            case TargetingMode.Slowest:
                localTarget = getSlowestEnemy(enemies);
                break;

            case TargetingMode.Strongest:
                localTarget = getStrongestEnemy(enemies);
                break;

            case TargetingMode.Weakest:
                localTarget = getWeakestEnemy(enemies);
                break;

            case TargetingMode.Furthest:
                localTarget = getFurthestEnemy(enemies);
                break;

            case TargetingMode.Closest:
            default:
                localTarget = getClosestEnemy(enemies);
                break;
        }

        // Have we found one?
        if (localTarget != null)
        {
            target = localTarget.transform;
            targetEnemy = localTarget.GetComponent<Enemy>();
        }
        else target = null;
    }

    void Shoot()
    {
        GameObject bulletGO = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Bullet bullet = bulletGO.GetComponent<Bullet>();
        if (bullet != null)
        {
            bullet.Seek(target);
        }
    }

    // Unity built in!
    // Show the range of the gun - this is for the inspector.
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
