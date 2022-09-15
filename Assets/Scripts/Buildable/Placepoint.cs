using System;
using Unity.Netcode;
using UnityEngine;

public class Placepoint : NetworkBehaviour, IBuildable
{
    public GameObject button;
    public GameObject ShopButton { get => button; }
        
    public int ShopIdentifier { get => 0; }

    public int Cost { get => 50; }

    public Vector3 positionOffset;

    protected TargetingMode _targetingMode;

    public TargetingMode TargetingMode
    {
        get => _targetingMode;
        set => _targetingMode = value;
    }

    void Start()
    {
        name = Guid.NewGuid().ToString("N");
    }

    public Vector3 GetBuildPosition(Placement parent)
    {
        switch (parent.LookDirection) {
            case Placement.Facing.Up:
                positionOffset = new Vector3(0, 0, 0);
                break;
            case Placement.Facing.Down:
                positionOffset = new Vector3(0, -1.0f, 0);
                break;
            case Placement.Facing.Forwards:
                positionOffset = new Vector3(0, -.5f, .5f);
                break;
            case Placement.Facing.Backwards:
                positionOffset = new Vector3(0, -.5f, -.5f);
                break;
            case Placement.Facing.Left:
                positionOffset = new Vector3(-.5f, -.5f, 0);
                break;
            case Placement.Facing.Right:
                positionOffset = new Vector3(.5f, -.5f, 0);
                break;
        }

        return parent.transform.position + positionOffset;
    }

    public Quaternion GetBuildRotation(Placement parent)
    {
        return Quaternion.LookRotation(Vector3.forward, Vector3.up);
    }
}