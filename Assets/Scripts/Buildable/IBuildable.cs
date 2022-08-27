using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

interface IBuildable
{
    int ShopIdentifier { get; }

    int Cost { get; }

    GameObject ShopButton { get; }

    TargetingMode TargetingMode { get; set; }

    Vector3 GetBuildPosition(Placement parent);

    Quaternion GetBuildRotation(Placement parent);
}

public enum TargetingMode {
    Closest,
    Furthest,
    Fastest,
    Slowest,
    First,
    Last,
    Strongest,
    Weakest
}
