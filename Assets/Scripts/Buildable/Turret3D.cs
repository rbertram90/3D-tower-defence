using UnityEngine;

public class Turret3D : AbstractTurret, IBuildable
{
    public GameObject button;
    public GameObject ShopButton { get => button; }

    public int ShopIdentifier { get => 1; }
    public int Cost { get => 50; }

    // Set to 1 if not upgradable
    public const int MaxLevel = 3;
    public static readonly int[] UpgradeCosts = { 50, 100 };

    public TargetingMode targetingMode;

    public float fireRate = 1f;
    public float fireCountdown = 0f;

    public GameObject bulletPrefab;
    public Material Level2Material;
    public Material Level3Material;

    public Vector3 positionOffset;

    // Use this for initialization
    void Start()
    {
        // Search for targets every 1/2 second
        InvokeRepeating("UpdateTarget", 0f, 0.5f);

        targetingMode = TargetingMode.Closest;

        range = 5f; // this is the radius

        // Set the size of the target circle - note x and z should be the same.
        // Multiply by 2 to get the diameter
        transform.GetChild(2).transform.localScale = new Vector3(range / transform.localScale.x * 2, 0, range / transform.localScale.z * 2);
    }

    // Update is called once per frame
    void Update()
    {
        // Ensure we've got a target
        if (target == null) {            
            return;
        }

        LockOnTarget();

        if (fireCountdown <= 0f) {
            Shoot();
            fireCountdown = 1f / fireRate;
        }

        fireCountdown -= Time.deltaTime;
    }

    public override void OnNetworkSpawn()
    {
        if (IsHost) {
            Level.Value = 1;
        }

        Level.OnValueChanged += (int oldValue, int newValue) => {
            // Do upgrade
            switch (newValue) {
                case 2:
                    transform.Find("Base").GetComponent<MeshRenderer>().material = Level2Material;
                    break;
                case 3:
                    transform.Find("Base").GetComponent<MeshRenderer>().material = Level3Material;
                    break;
            }
        };
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

    void UpdateTarget()
    {
        // Get all enemies, possible to optimise this?
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject localTarget;

        switch (targetingMode) {
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
        if (localTarget != null) {
            target = localTarget.transform;
        }
        else {
            target = null;
        }
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

}
