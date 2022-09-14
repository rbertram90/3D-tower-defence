using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class BuildManager : MonoBehaviour {

    private static BuildManager _instance;
    public static BuildManager Instance { get { return _instance; } }

    private int turretToBuild = -1; // Selected shop item
    private Placement selectedPlacement; // Currently selected placement
    public GameObject buildEffect; // Effect to show when item is built
    public GameObject sellEffect; // Effect to show when item is sold
    public PlacementUI placementUI; // Interface which shows when building

    public List<GameObject> turretPrefabs;

    private bool sellMode;

    void Awake ()
    {
        sellMode = false;

        if (_instance != null && _instance != this) {
            Destroy(gameObject);
        }
        else {
            _instance = this;
        }
    }

    // Dynamic Property
    public bool IsInBuildMode { get { return turretToBuild != -1; } }
    public bool HasMoney {
        get {
            if (GameManager.Instance.GetLocalPlayer()) {
                return GameManager.Instance.GetLocalPlayer().Balance.Value >= 0;
            }
            return false;
        }
    }

    public void SelectPlacement(Placement placement)
    {
        // If the same placement has been clicked again
        // Then close it.
        if (placement == selectedPlacement)
        {
            DeselectPlacement();
            return;
        }

        if (selectedPlacement != null) {
            if (selectedPlacement.turret.transform.Find("ShootRadius") != null) {
                selectedPlacement.turret.transform.Find("ShootRadius").gameObject.SetActive(false);
            }
        }

        selectedPlacement = placement;

        DeselectTurretToBuild();

        placementUI.SetTarget(placement);
        placementUI.Show();

        if (placement.turret.transform.Find("ShootRadius") != null) {
            placement.turret.transform.Find("ShootRadius").gameObject.SetActive(true);
        }
    }

    public void SelectTurretToBuild(int turret)
    {
        if (turretToBuild == turret) {
            DeselectTurretToBuild();
            return;
        }

        turretToBuild = turret;
        DeselectPlacement();
        DisableSellMode();

        foreach (GameObject button in Shop.Buttons) {
            button.transform.Find("ActiveBorder").gameObject.SetActive(false);
        }

        Shop.Buttons[turret].transform.Find("ActiveBorder").gameObject.SetActive(true);
    }

    public void DeselectTurretToBuild()
    {
        if (turretToBuild == -1) {
            return;
        }

        Shop.Buttons[turretToBuild].transform.Find("ActiveBorder").gameObject.SetActive(false);

        turretToBuild = -1;
    }

    public int GetTurretToBuild()
    {
        return turretToBuild;
    }

    public void DeselectPlacement()
    {
        if (selectedPlacement != null && selectedPlacement.turret != null) {
            if (selectedPlacement.turret.transform.GetChild(2).name == "ShootRadius") {
                selectedPlacement.turret.transform.GetChild(2).gameObject.SetActive(false);
            }
        }

        selectedPlacement = null;
        placementUI.Hide();
    }

    public void ToggleSellMode()
    {
        if (sellMode) DisableSellMode();
        else EnableSellMode();
    }

    public void EnableSellMode()
    {
        sellMode = true;
        turretToBuild = -1;
        DeselectPlacement();
    }

    public void DisableSellMode()
    {
        sellMode = false;
    }

    public string GetBuildMode()
    {
        if(sellMode)
        {
            return "Sell";
        }
        else if(turretToBuild != -1)
        {
            return "Buy";
        }
        else
        {
            return "None";
        }
    }

}
