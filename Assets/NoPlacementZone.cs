using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoPlacementZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Placepoint") {
            other.GetComponent<Placement>().IsSuitableForBuilding = false;
        }
    }
}
