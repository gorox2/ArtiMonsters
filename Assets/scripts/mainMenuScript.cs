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
    public TMP_Text artifactsNumber;

    public int artifactsCollected;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        artifactsCollected = PlayerPrefs.GetInt("artifacts in possession");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void levelsButtonClick()
    {
        levelSelect.SetActive(true);    
        levelsButton.SetActive(false);
        exitButton.SetActive(false);
    }


    public void backButtonClick()
    {
        levelsButton.SetActive(true);
        levelSelect.SetActive(false);
        exitButton.SetActive(true);

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
        Application.Quit();
    }

    
}
