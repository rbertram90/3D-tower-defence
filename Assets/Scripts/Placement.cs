using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public class Placement : NetworkBehaviour
{
    public Color hoverColor = Color.cyan;
    public Color notEnoughMoneyColor = Color.red;
    public Color sellColor = Color.yellow;
    public Vector3 positionOffset;

    [HideInInspector]
    public GameObject turret;

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
        if (IsHost) {
            // @todo make this a serverrpc?
            GameManager.LocalPlayer.Balance.Value += 25;
        }

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
    void BuildTurretServerRpc(int TurretType, ServerRpcParams serverRpcParams = default)
    {
        GameObject turretPrefab = bM.turretPrefabs[TurretType];

        var clientId = serverRpcParams.Receive.SenderClientId;
        if (! NetworkManager.ConnectedClients.ContainsKey(clientId)) {
            return;
        }
        Player player = NetworkManager.ConnectedClients[clientId].PlayerObject.GetComponent<Player>();

        if (!turretPrefab) {
            Debug.Log("Unable to find prefab for type " + TurretType);
            return; // do something more helpful?
        }

        IBuildable buildable = turretPrefab.GetComponent<IBuildable>();

        if (player.Balance.Value < buildable.Cost) {
            Debug.Log("Cannot build turret - not enough monies :(");
            return; // not enough monies
        }

        // Take the money
        player.Balance.Value -= buildable.Cost;

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
        AbstractTurret turretComponent = turret.GetComponent<AbstractTurret>();
        int MaxLevel = 1;
        int UpgradeCost = 0;

        Debug.Log(turretComponent.GetType().ToString());

        switch (turretComponent.GetType().ToString()) {
            case "Turret3D":
                MaxLevel = Turret3D.MaxLevel;
                UpgradeCost = Turret3D.UpgradeCosts[turretComponent.Level.Value - 1];
                break;
        }

        if (turretComponent.Level.Value == MaxLevel) {
            // No more upgrades
            Debug.Log("Cannot upgrade, at max level.");
            return;
        }

        if (GameManager.LocalPlayer.Balance.Value < UpgradeCost) {
            // Not enough monies
            Debug.Log("Cannot upgrade, insufficient funds.");
            return;
        }

        turretComponent.Level.Value++;

        if (IsHost) {
            // @todo do this in the RPC or make variable owner updatable?
            GameManager.LocalPlayer.Balance.Value -= UpgradeCost;
        }

        // UpgradeTurretServerRpc();
    }

    [ServerRpc]
    public void UpgradeTurretServerRpc()
    {

    }

    public void SellTurret()
    {
        GameManager.LocalPlayer.Balance.Value += 50; // @todo work something out for this.

        GameObject effect = (GameObject)Instantiate(bM.sellEffect, GetBuildPosition(), Quaternion.identity);
        Destroy(effect, 3f);

        Destroy(turret);
    }

    public Vector3 GetBuildPosition()
    {
        return transform.position + positionOffset;
    }

}
