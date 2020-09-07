using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// 9/3 - worked on by Vinson Kok
public class MenuManager : MonoBehaviour
{
    [SerializeField] GameObject mainMenuPanel;
    [SerializeField] GameObject instructionsPanel;
    [SerializeField] GameObject extrasPanel;
    [SerializeField] GameObject settingsPanel;

    private void Start()
    {
        DisplayMainMenuPanel();
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
