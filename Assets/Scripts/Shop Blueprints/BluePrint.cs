using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BluePrint
{
    public GameObject prefab;
    public int cost;

    public int GetSellPrice()
    {
        return cost / 2;
    }
}
