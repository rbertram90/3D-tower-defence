using Unity.Netcode;

public class ReadyUpButton : NetworkBehaviour
{
    public void OnClick()
    {
        GameManager.Instance.GetLocalPlayer().ReadyUpServerRpc();
    }

    public override void OnNetworkSpawn()
    {
        if (IsHost) {
            gameObject.SetActive(false);
        }

        base.OnNetworkSpawn();
    }
}
