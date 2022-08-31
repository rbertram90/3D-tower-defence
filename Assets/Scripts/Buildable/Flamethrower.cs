using UnityEngine;

public class Flamethrower : AbstractTurret, IBuildable
{
    public int ShopIdentifier => throw new System.NotImplementedException();

    public int Cost => throw new System.NotImplementedException();

    public GameObject ShopButton => throw new System.NotImplementedException();

    public Vector3 GetBuildPosition(Placement parent)
    {
        throw new System.NotImplementedException();
    }

    public Quaternion GetBuildRotation(Placement parent)
    {
        throw new System.NotImplementedException();
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
