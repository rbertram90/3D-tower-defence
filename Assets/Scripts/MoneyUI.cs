using UnityEngine.UI;
using Unity.Netcode;

public class MoneyUI : NetworkBehaviour {

    public Text moneyText;

	void Update ()
    {
        if (NetworkManager.LocalClient != null) {
            moneyText.text = "£" + NetworkManager.LocalClient.PlayerObject.GetComponent<Player>().Balance.Value;
        }
        else {
            moneyText.text = "£0";
        }
    }
}
