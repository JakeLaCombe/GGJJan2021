using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("mgn start scene");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnStartGameButtonClicked()
    {
        Debug.Log("mgn start game");
        SceneManager.LoadScene("SampleScene");
    }

    public void OnReturnToMainMenuButtonClicked()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void OnInstructionsButtonClicked()
    {
        SceneManager.LoadScene("Instructions");
    }
}
