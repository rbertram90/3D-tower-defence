using UnityEngine;
using UnityEngine.UI;

public class LivesUI : MonoBehaviour {

    public Text LivesText;

	private GameManager _gameManager;

	// Use this for initialization
	void Start () {
        _gameManager = FindObjectOfType<GameManager>();
    }
	
	// Update is called once per frame
	void Update () {
        LivesText.text = _gameManager.Lives.Value + " Lives";
	}
}
