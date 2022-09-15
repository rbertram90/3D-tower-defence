using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlacementUI : MonoBehaviour {

    private Placement target; // Object that has been placed on this placement
    private IBuildable turret;  // Turret Component of target

    public GameObject ui;     // Placement menu that shows when clicked
    public Text upgradeCost;  // Text that shows amount it costs to upgrade target
    public Button upgradeButton; // Button to upgrade target

    public Color32 defaultButtonColour; // Colour on default state

    // Other buttons
    public Button closestButton;
    public Button furthestButton;
    public Button stongestButton;
    public Button weakestButton;
    public Button fastestButton;
    public Button slowestButton;

    public Text sellPrice;
    // public Button sellButton;

    public void Start()
    {
        Hide();
    }

    public void SetTarget(Placement target)
    {
        this.target = target;

        if (target.turret == null) {
            // We've just clicked a placement on a placepoint so give the option to sell?
            // target.turret = target.transform.parent.gameObject;
            // return;
            turret = target.transform.parent.gameObject.GetComponent<IBuildable>();
        }
        else {
            turret = target.turret.GetComponent<IBuildable>();
        }

        // includes offset
        transform.position = target.GetBuildPosition();

        upgradeCost.text = "N/A";
        upgradeButton.interactable = false;

        switch (turret.GetType().ToString()) {
            case "Turret3D":
                Turret3D turret3 = (Turret3D) turret;
                int MaxLevel = Turret3D.MaxLevel;
                if (turret3.Level.Value < MaxLevel) {
                    upgradeCost.text = "£" + Turret3D.UpgradeCosts[turret3.Level.Value - 1].ToString();
                    upgradeButton.interactable = true;
                }
                closestButton.interactable = true;
                furthestButton.interactable = true;
                stongestButton.interactable = true;
                weakestButton.interactable = true;
                fastestButton.interactable = true;
                slowestButton.interactable = true;
                break;
            case "Placepoint":
                // Placepoint placepoint = (Placepoint) turret;
                closestButton.interactable = false;
                furthestButton.interactable = false;
                stongestButton.interactable = false;
                weakestButton.interactable = false;
                fastestButton.interactable = false;
                slowestButton.interactable = false;
                break;
        }

        sellPrice.text = "£" + (turret.Cost / 2);

        highlightSelectedTargetingModeButton();

        Show();
    }

    public void Show()
    {
        ui.SetActive(true);
    }

    public void Hide()
    {
        ui.SetActive(false);
    }

    public void Upgrade()
    {
        // Run Upgrade
        target.UpgradeTurret();

        // Close menu
        BuildManager.Instance.DeselectPlacement();
    }

    public void Sell()
    {
        target.SellTurret();

        BuildManager.Instance.DeselectPlacement();
    }

    // onclick function set in Unity inspector
    public void changeTargetingMode(int targetingMode)
    {
        turret.TargetingMode = (TargetingMode) targetingMode;

        highlightSelectedTargetingModeButton();
    }

    void highlightSelectedTargetingModeButton()
    {
        defaultButtonColour = Color.white;

        furthestButton.GetComponentInChildren<Text>().fontStyle = FontStyle.Normal;
        furthestButton.image.color = defaultButtonColour;
        closestButton.GetComponentInChildren<Text>().fontStyle = FontStyle.Normal;
        closestButton.image.color = defaultButtonColour;
        stongestButton.GetComponentInChildren<Text>().fontStyle = FontStyle.Normal;
        stongestButton.image.color = defaultButtonColour;
        weakestButton.GetComponentInChildren<Text>().fontStyle = FontStyle.Normal;
        weakestButton.image.color = defaultButtonColour;
        fastestButton.GetComponentInChildren<Text>().fontStyle = FontStyle.Normal;
        fastestButton.image.color = defaultButtonColour;
        slowestButton.GetComponentInChildren<Text>().fontStyle = FontStyle.Normal;
        slowestButton.image.color = defaultButtonColour;

        Button activeButton;

        switch (turret.TargetingMode)
        {
            case TargetingMode.Fastest:
                activeButton = fastestButton;
                break;

            case TargetingMode.Slowest:
                activeButton = slowestButton;
                break;

            case TargetingMode.Strongest:
                activeButton = stongestButton;
                break;

            case TargetingMode.Weakest:
                activeButton = weakestButton;
                break;

            case TargetingMode.Furthest:
                activeButton = furthestButton;
                break;

            case TargetingMode.Closest:
            default:
                activeButton = closestButton;
                break;
        }

        activeButton.GetComponentInChildren<Text>().fontStyle = UnityEngine.FontStyle.Bold;
        // activeButton.GetComponent<Image>().mainTexture;
        activeButton.image.color = Color.yellow;
    }
}
