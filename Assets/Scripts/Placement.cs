using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.GraphicsBuffer;

public class Placement : NetworkBehaviour
{
    public Color hoverColor = Color.cyan;
    public Color notEnoughMoneyColor = Color.red;
    public Color sellColor = Color.yellow;
    public Vector3 positionOffset;

    [HideInInspector]
    public GameObject turret;
    [HideInInspector]
    public TurretBlueprint turretBlueprint;

    private Renderer rend;
    private Color startColor;

    BuildManager bM;

    public Facing LookDirection;

    public enum Facing
    {
        Up,
        Down,
        Left,
        Right,
        Forwards,
        Backwards
    }

    // Use this for initialization
    void Start ()
    {
        rend = GetComponent<Renderer>();
        startColor = rend.material.color;
        bM = BuildManager.instance;
    }

    public void performOnMouseDown()
    {
        OnMouseDown();
    }

    void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject()) {
            return;
        }

        if (turret != null)
        {
            // Already turret
            switch (bM.GetBuildMode())
            {
                case "Sell":
                    SellTurret();
                    break;
                default:
                    bM.SelectPlacement(this);
                    return;
            }
        }
        else if (gameObject.tag == "Placepoint" && bM.GetBuildMode() == "Sell")
        {
            bool canSell = true;

            for (int i = 0; i < transform.parent.childCount; i++)
            {
                Transform child = transform.parent.GetChild(i);
                if(child.tag == "Placepoint" && child.GetComponent<Placement>().turret != null)
                {
                    canSell = false;
                }
            }

            if(canSell) SellPlacement();
        }

        if (!bM.CanBuild) {
            Debug.Log("Returning as cannot build");
            return;
        }

        BuildTurretServerRpc(bM.GetTurretToBuild());
    }

    void SellPlacement()
    {
        PlayerStats.money += 25; // HARD CODED!!!

        GameObject effect = Instantiate(bM.sellEffect, GetBuildPosition(), Quaternion.identity);
        Destroy(effect, 3f);

        Destroy(transform.parent.gameObject);
    }

    void OnMouseEnter ()
    {
        // Are we hoving over an UI element
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (bM.GetBuildMode() == "Sell")
            rend.material.color = sellColor;

        if (!bM.CanBuild)
            return;

        if (bM.HasMoney)
            rend.material.color = hoverColor;
        else
            rend.material.color = notEnoughMoneyColor;
    }

    void OnMouseExit ()
    {
        rend.material.color = startColor;
    }

    // Who is this owned by?!
    [ServerRpc(RequireOwnership = false)]
    void BuildTurretServerRpc(int TurretType)
    {
        GameObject turretPrefab = bM.turretPrefabs[TurretType];

        if (!turretPrefab) {
            Debug.Log("Unable to find prefab for type " + TurretType);
            return; // do something more helpful?
        }

        IBuildable buildable = turretPrefab.GetComponent<IBuildable>();

        if (PlayerStats.instance.Balance < buildable.Cost) {
            Debug.Log("Cannot build turret - not enough monies :(");
            return; // not enough monies
        }
        
        // Take the money
        PlayerStats.instance.SubtractFromBalanceServerRpc(buildable.Cost);

        // Build the placepoint or turret
        GameObject turret = Instantiate(turretPrefab, buildable.GetBuildPosition(this), buildable.GetBuildRotation(this));

        if (turret.tag == "Turret") {
            Turret3D turretComponent = turret.GetComponent<Turret3D>();
            // Debug.LogError("GO = " + gameObject.name);
            if (transform.GetComponent<NetworkObject>() == null) {
                // we can't set the placement gameobject to placement as it doesn't have the networkobject on directly
                turretComponent.AttachedPlacement.Value = transform.parent.gameObject;
                turretComponent.PlacepointPlacementIndex.Value = transform.GetSiblingIndex();
            }
            else {
                turretComponent.AttachedPlacement.Value = gameObject;
            }
        }

        // Send accross the network
        turret.GetComponent<NetworkObject>().Spawn();

        // Set the assigned turret - this will need to be a network variable?
        this.turret = turret;

        // turretBlueprint = blueprint;

        GameObject effect = Instantiate(bM.buildEffect, GetBuildPosition(), Quaternion.identity);
        Destroy(effect, 3f);
    }

    public void UpgradeTurret()
    {
        if (PlayerStats.money < turretBlueprint.upgradeCost)
            return; // not enough monies

        PlayerStats.money -= turretBlueprint.upgradeCost;

        // Remove old turret
        Destroy(this.turret);

        // Build a new turret
        GameObject turret = (GameObject)Instantiate(turretBlueprint.upgradedPrefab, GetBuildPosition(), Quaternion.identity);
        this.turret = turret;

        GameObject effect = (GameObject)Instantiate(bM.buildEffect, GetBuildPosition(), Quaternion.identity);
        Destroy(effect, 3f);

        Turret3D turretComponent = turret.GetComponent<Turret3D>();
        // turretComponent.placement = this;
        turretComponent.isUpgraded = true;
    }

    public void SellTurret()
    {
        PlayerStats.money += turretBlueprint.GetSellPrice();

        GameObject effect = (GameObject)Instantiate(bM.sellEffect, GetBuildPosition(), Quaternion.identity);
        Destroy(effect, 3f);

        Destroy(this.turret);

        turretBlueprint = null;
    }

    public Vector3 GetBuildPosition()
    {
        return transform.position + positionOffset;
    }
    /*
    public static explicit operator Placement(NetworkObjectReference v)
    {
        if (v.TryGet(out NetworkObject targetObject)) {
            return targetObject.GetComponent<Placement>();
        }

        return null;

        // throw new NotImplementedException();
    }
    */
}
