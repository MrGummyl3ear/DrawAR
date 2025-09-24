using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    private int SceneLevel = 0;
    [Header("UI References")]
    public TextMeshProUGUI Text;
    public Slider tutorialSlider;

    [Header("Tutorial Content")]
    [TextArea(3, 10)]
    public List<string> tutorialPages;

    void Start()
    {
        // Set the initial text to the first page when the scene starts
        UpdateTutorialText();

        // Add a listener to the slider so it calls our function whenever it's moved
        tutorialSlider.onValueChanged.AddListener(delegate { UpdateTutorialText(); });
    }

    public void StartGame()
    {
        Debug.Log(SceneManager.GetActiveScene().buildIndex + 1 + SceneLevel);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1 + SceneLevel);
    }

    public void Quit()
    {
        Debug.Log("Quit");
        Application.Quit();
    }

    public void ImageTargetMode(Boolean IsOn)
    {
        if (IsOn)
        {
            Debug.Log("ImageTargetMode Mode is turned on");
            SceneLevel = 1;
        }
        else
            SceneLevel = 0;
    }

    public void UpdateText(float f)
    {
        Text.text = f.ToString();
    }

    public void GoBackToMenu()
    {
        SceneManager.LoadScene(0);
    }
    
    public void UpdateTutorialText()
    {
        // Convert the slider's float value to an integer index
        int pageIndex = (int)tutorialSlider.value;

        // Make sure the index is valid before trying to update the text
        if (tutorialPages != null && pageIndex < tutorialPages.Count)
        {
            Text.text = tutorialPages[pageIndex];
        }
    }

}
