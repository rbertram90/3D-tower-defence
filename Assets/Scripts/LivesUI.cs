using UnityEngine;
using UnityEngine.UI;

public class LivesUI : MonoBehaviour {

    public Text LivesText;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        LivesText.text = PlayerStats.lives + " LIVES";
	}
}
