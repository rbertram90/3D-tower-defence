using UnityEngine.UI;
using Unity.Netcode;

public class MoneyUI : NetworkBehaviour {

    public Text moneyText;

	void Update ()
    {
        if (GameManager.Instance.GetLocalPlayer() != null) {
            moneyText.text = "£" + GameManager.Instance.GetLocalPlayer().Balance.Value;
        }
        else {
            moneyText.text = "£0";
        }
    }
}
