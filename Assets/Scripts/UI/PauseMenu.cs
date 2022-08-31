using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {

    public GameObject ui;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            Toggle();
        }
	}

    // Show or hide the menu
    public void Toggle()
    {
        ui.SetActive(!ui.activeSelf); // Cannot pause as now multiplayer. Or could check if anyone else is playing?

        if(ui.activeSelf)
        {
            Debug.Log("Pausing time as pause menu is open?");

            // Time.timeScale = 0f;
            // needed if scaling time but not if completely stopped!
            // Time.fixedDeltaTime = 0f;
        }
        else
        {
            // Time.timeScale = 1f;
        }
    }

    public void Retry()
    {
        Toggle();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Menu()
    {
        SceneManager.LoadScene(0);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
