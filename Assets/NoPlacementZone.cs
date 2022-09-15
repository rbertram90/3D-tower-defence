using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoPlacementZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Placepoint") {
            Debug.Log("Unable to place on this one " + other.gameObject.name);
            other.GetComponent<Placement>().IsSuitableForBuilding = false;
        }
    }
}
