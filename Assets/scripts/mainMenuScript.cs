using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public GameObject levelSelect;
    public GameObject levelsButton;
    public GameObject exitButton;
    public GameObject controlsButton;
    public GameObject controlsPanel;
    public TMP_Text artifactsNumber;

    public int artifactsCollected;
    
    private void OnEnable()
    {
        artifactsCollected = PlayerPrefs.GetInt("artifacts in possession");
    }


    public void levelsButtonClick()
    {
        levelSelect.SetActive(true);  
        
        levelsButton.SetActive(false);
        exitButton.SetActive(false);
        controlsButton.SetActive(false);
    }


    public void backButtonClick()
    {
        levelsButton.SetActive(true);
        exitButton.SetActive(true);
        controlsButton.SetActive(true);

        levelSelect.SetActive(false);
        controlsPanel.SetActive(false);
    }

    public void ControlsButtonClick()
    {
        levelsButton.SetActive(false);
        exitButton.SetActive(false);
        controlsButton.SetActive(false);

        controlsPanel.SetActive(true);
    }

    public void level1Select()
    {
        SceneManager.LoadScene("level 1");
    }

    public void Level2Select()
    {
        if(artifactsCollected >= 6)
        {
            SceneManager.LoadScene("level 2");
        }
    }

    public void exitButtonClick()
    {
        Debug.Log("Game exited!");
        Application.Quit();
    }

    
}
