using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// 9/3 - worked on by Vinson Kok
public class MenuManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] GameObject mainMenuPanel;
    [SerializeField] GameObject instructionsPanel;
    [SerializeField] GameObject extrasPanel;
    [SerializeField] GameObject settingsPanel;

    [Header("Buttons")]
    [SerializeField] Image[] buttons;
    int currentlySelectedButton = -1;
    int previouslySelectedButton = -1;

    private void Awake()
    {
        foreach (Image b in buttons)
        {
            b.enabled = false;
        }
    }

    private void Start()
    {
        DisplayMainMenuPanel();
    }

    public void OnHoverMenuButton(int index)
    {
        if (index != currentlySelectedButton)
        {
            currentlySelectedButton = index;

            buttons[currentlySelectedButton].enabled = true;
            if (previouslySelectedButton >= 0)
                buttons[previouslySelectedButton].enabled = false;

            previouslySelectedButton = currentlySelectedButton;
        }
    }

    public void StartGame(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void DisplayMainMenuPanel()
    {
        CloseAllPanels();
        mainMenuPanel.SetActive(true);
    }

    public void DisplayInstructionsPanel()
    {
        CloseAllPanels();
        instructionsPanel.SetActive(true);
    }

    public void DisplayExtrasPanel()
    {
        CloseAllPanels();
        extrasPanel.SetActive(true);
    }

    public void DisplaySettingsPanel()
    {
        CloseAllPanels();
        settingsPanel.SetActive(true);
    }

    public void CloseAllPanels()
    {
        mainMenuPanel.SetActive(false);
        instructionsPanel.SetActive(false);
        extrasPanel.SetActive(false);
        settingsPanel.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
