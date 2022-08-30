using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ReadyUpButton : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick()
    {
        Player p = NetworkManager.LocalClient.PlayerObject.GetComponent<Player>();
        p.ReadyUp();
    }

    public override void OnNetworkSpawn()
    {
        if (IsHost) {
            gameObject.SetActive(false);
        }

        base.OnNetworkSpawn();
    }
}
