using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Laser : AbstractTurret, IBuildable
{
    public LineRenderer lineRenderer;
    public int damagePerSecond = 2;
    public float slowPercent = 0.5f;

    public int ShopIdentifier { get => 2; }

    public int Cost { get => 200; }

    public GameObject button;
    public GameObject ShopButton { get => button; }

    public Vector3 GetBuildPosition(Placement parent)
    {
        throw new System.NotImplementedException();
    }

    public Quaternion GetBuildRotation(Placement parent)
    {
        throw new System.NotImplementedException();
    }

    void Start()
    {

    }

    void Update()
    {
        // Ensure we've got a target
        if (target == null) {
            // Stop the lazer
            if (lineRenderer.enabled) {
                lineRenderer.enabled = false;
            }
            return;
        }

        LockOnTarget();

        target.GetComponent<Enemy>().TakeDamage(damagePerSecond * Time.deltaTime);
        // targetEnemy.Slow(slowPercent);

        if (!lineRenderer.enabled)
            lineRenderer.enabled = true;

        lineRenderer.SetPosition(0, firePoint.position);
        lineRenderer.SetPosition(1, target.position);
    }


}
