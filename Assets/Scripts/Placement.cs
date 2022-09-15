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

    // Determine on build if this collides with any
    // NoPlacementZones if it does, then no turret
    // can be built on it.
    [HideInInspector]
    public bool IsSuitableForBuilding = true;

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
        bM = BuildManager.Instance;
    }

    void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject()) {
            return;
        }

        if (turret != null) {
            // Already got a turret attached
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
        else if (bM.IsInBuildMode && IsSuitableForBuilding) {
            BuildTurretServerRpc(bM.GetTurretToBuild());
        }
        // If it's not a world placement
        else if (gameObject.tag == "Placepoint") {
            bM.SelectPlacement(this);
        }
    }

    void OnMouseEnter()
    {
        // Are we hoving over an UI element
        if (EventSystem.current.IsPointerOverGameObject()) {
            return;
        }

        // Have we got a shop object selected?
        // If not use this yellow colour
        if (!bM.IsInBuildMode) {
            rend.material.color = sellColor;
            return;
        }

        // Trying to build
        if (!IsSuitableForBuilding || !bM.HasMoney) {
            rend.material.color = notEnoughMoneyColor;
            return;
        }
        
        rend.material.color = hoverColor;
    }

    void OnMouseExit()
    {
        rend.material.color = startColor;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Debug.Log("trigger enter with " + other.name);

        if (other.GetComponent<Placement>() != null) {
            turret = other.transform.parent.gameObject;
        }
    }

    public void performOnMouseDown()
    {
        OnMouseDown();
    }

    void SellPlacement()
    {
        if (IsHost) {
            // @todo make this a serverrpc?
            GameManager.Instance.GetLocalPlayer().Balance.Value += 25;
        }

        GameObject effect = Instantiate(bM.sellEffect, GetBuildPosition(), Quaternion.identity);
        Destroy(effect, 3f);

        Destroy(transform.parent.gameObject);
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

        // Create the build affect
        GameObject effect = Instantiate(bM.buildEffect, GetBuildPosition(), Quaternion.identity);

        // Spawn for everyone
        // effect.GetComponent<NetworkObject>().Spawn();

        // Destroy for everyone - @todo how do we do this in a few seconds?
        // effect.GetComponent<NetworkObject>().Despawn();
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

        if (GameManager.Instance.GetLocalPlayer().Balance.Value < UpgradeCost) {
            // Not enough monies
            Debug.Log("Cannot upgrade, insufficient funds.");
            return;
        }

        turretComponent.Level.Value++;

        if (IsHost) {
            // @todo do this in the RPC or make variable owner updatable?
            GameManager.Instance.GetLocalPlayer().Balance.Value -= UpgradeCost;
        }

        // UpgradeTurretServerRpc();
    }

    [ServerRpc]
    public void UpgradeTurretServerRpc()
    {

    }

    public void SellTurret()
    {
        // Note - when an empty placement is clicked on a placepoint
        // 'turret' is the placepoint itself
        GameObject localTurret;
        if (turret == null) {
            localTurret = transform.parent.gameObject;
        }
        else {
            localTurret = turret;
        }

        if (localTurret.GetComponent<Placepoint>() != null) {
            bool canSell = true;
            int attachedPlacepoints = 0;
            bool worldPlacepoint = false;

            for (int i = 0; i < localTurret.transform.childCount; i++) {
                Transform child = localTurret.transform.GetChild(i);

                if (child.GetComponent<Placement>() != null && child.GetComponent<Placement>().turret != null) {
                    GameObject attachedTurret = child.GetComponent<Placement>().turret;

                    if (attachedTurret.GetComponent<Placepoint>() != null) {
                        // It's a placepoint...
                        attachedPlacepoints++;
                    }
                    else if (attachedTurret.CompareTag("MapPlacepoint")) {
                        worldPlacepoint = true;
                    }
                    else {
                        canSell = false; // cannot sell with an attached turret
                        break;
                    }

                    if (attachedPlacepoints >= 1 && worldPlacepoint) {
                        canSell = false;
                        break;
                    }
                    else if (attachedPlacepoints > 1) {
                        canSell = false; // cannot sell with more than 1 other attached placepoint
                        // little bit simple logic - really need to check if this is in the critical
                        // path back to the world placement
                        break;
                    }
                }
            }

            if (! canSell) {
                return;
            }
        }

        // Should we be giving the local player this money, sharing, or giving to the original buyer?
        GameManager.Instance.GetLocalPlayer().Balance.Value += (localTurret.GetComponent<IBuildable>().Cost / 2);

        // @todo Spawn and Despawn on server
        GameObject effect = Instantiate(bM.sellEffect, GetBuildPosition(), Quaternion.identity);
        Destroy(effect, 3f);

        Destroy(localTurret);
    }

    public Vector3 GetBuildPosition()
    {
        return transform.position + positionOffset;
    }

}
