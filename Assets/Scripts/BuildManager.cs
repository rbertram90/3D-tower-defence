using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour {

    public static BuildManager instance;
    
    private int turretToBuild; // Once turret has been selected on the menu always do this
    private Placement selectedPlacement; // Currently selected placement
    public GameObject buildEffect; // Effect to show when item is built
    public GameObject sellEffect; // Effect to show when item is sold
    public PlacementUI placementUI; // Interface which shows when building

    public List<GameObject> turretPrefabs;

    private bool sellMode;

    void Awake ()
    {
        sellMode = false;
        instance = this; // singleton
    }

    // Dynamic Property
    public bool CanBuild { get { return turretToBuild != -1; } }
    public bool HasMoney {
        get {
            return GameManager.LocalPlayer.Balance.Value >= 0;
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

        selectedPlacement = placement;
        turretToBuild = -1;

        placementUI.SetTarget(placement);
        placementUI.Show();

        if (placement.turret.transform.GetChild(2).name == "ShootRadius") {
            placement.turret.transform.GetChild(2).gameObject.SetActive(true);
        }
    }

    public void SelectTurretToBuild(int turret)
    {
        turretToBuild = turret;
        DeselectPlacement();
        DisableSellMode();
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
