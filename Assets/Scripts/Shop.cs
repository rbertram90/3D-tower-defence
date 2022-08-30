using UnityEngine;
using UnityEngine.UI;
// Note - not UnityEngine.UIElements ?

public class Shop : MonoBehaviour
{
    BuildManager bM;

    // Use this for initialization
    void Start ()
    {
		bM = BuildManager.instance;

        // Create the buttons for the shop
        // Add new options by adding a Buildable prefab to the BuildManager.
        foreach (GameObject prefab in bM.turretPrefabs) {
            IBuildable build = prefab.GetComponent<IBuildable>();

            GameObject turretButton = Instantiate(build.ShopButton, transform.position, Quaternion.identity);

            turretButton.GetComponent<Button>().onClick.AddListener(() => {
                bM.SelectTurretToBuild(build.ShopIdentifier);

                Button btn = turretButton.GetComponent<Button>();
                // btn.style.borderBottomColor = Color.green;
                // btn.style.borderBottomWidth = 3;
            });

            turretButton.transform.SetParent(this.transform);
        }
	}

    public void SelectSell()
    {
        bM.EnableSellMode();
    }
}
