using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// Note - not UnityEngine.UIElements ?

public class Shop : MonoBehaviour
{
    BuildManager bM;

    public static List<GameObject> Buttons = new();

    // Use this for initialization
    void Start ()
    {
		bM = BuildManager.Instance;

        // Create the buttons for the shop
        // Add new options by adding a Buildable prefab to the BuildManager.
        foreach (GameObject prefab in bM.turretPrefabs) {
            IBuildable build = prefab.GetComponent<IBuildable>();

            GameObject turretButton = Instantiate(build.ShopButton, transform.position, Quaternion.identity);

            Buttons.Add(turretButton);

            turretButton.GetComponent<Button>().onClick.AddListener(() => {
                bM.SelectTurretToBuild(build.ShopIdentifier);
            });

            turretButton.transform.SetParent(this.transform);
        }
	}

    public void SelectSell()
    {
        bM.EnableSellMode();
    }
}
