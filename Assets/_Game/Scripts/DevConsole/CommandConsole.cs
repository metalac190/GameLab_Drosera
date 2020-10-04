using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(ConsoleLog))]
public class CommandConsole : MonoBehaviour
{
    public static event Action RevertConsole = delegate { };
    public static event Action<bool> ToggleHotkeys = delegate { };

    [HideInInspector] public static CommandConsole commandConsole;
    [Header("Settings")]
    [SerializeField] private bool enableInBuild = true;
    [SerializeField] private bool enableConsole = true;
    [SerializeField] public bool enableHotkeys = true;
    [SerializeField] private bool debugInEditor = false;

    [Header("Scene Settings")]
    [SerializeField] bool generateLevel = true;
    [SerializeField] GameObject generateRoom = null;

    [Header("Button References")]//as needed
    [SerializeField] Text speedMultiplyerText = null;

    [Header("Visuals")]
    [SerializeField] private GameObject consoleWindow = null;

    private ConsoleLog log = null;

    private bool isInEditor;

    private bool isPaused = false;
    private float speedMultiplyer = 1f;
    private bool hotkeyMode;


    #region Init
    private void Awake()
    {

#if UNITY_EDITOR
        isInEditor = true;
#else
        isInEditor = false;
#endif

        if (commandConsole != null)
            Destroy(this.gameObject);
        else
        {
            commandConsole = this;
            DontDestroyOnLoad(this.gameObject);
        }

        log = GetComponent<ConsoleLog>();
        hotkeyMode = enableHotkeys;
    }

    private void Start()
    {
        speedMultiplyerText.text = speedMultiplyer.ToString("F1") + "x";
        consoleWindow?.SetActive(false);
        if (!debugInEditor && isInEditor)
            log.CloseLog();
    }
    #endregion

    #region Respond to input events
    /// <summary>
    /// Public functions to react to input and run a command.
    /// Hook these up to the Unity Events provided on the Console_Button game object
    /// William Austin
    /// </summary>

    public void PauseCommand()
    {
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void PlayCommand()
    {
        Time.timeScale = speedMultiplyer;
        isPaused = false;
    }

    public void FastForwardCommand()
    {
        IncrementSpeedMultiplyer();
        if(!isPaused)
            Time.timeScale = speedMultiplyer;
    }

    public void QuitCommand()
    {
        if (isInEditor)
            UnityEditor.EditorApplication.isPlaying = false;
        else
            Application.Quit();
    }

    public void ResetSceneCommand()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ActivateHyperSeedCommand()
    {
        Debug.Log("Activating the hyperseed has not been implemented yet.");
    }





    #endregion

    #region Command Helper Methods

    void IncrementSpeedMultiplyer()
    {
        speedMultiplyer += 0.5f;
        if (speedMultiplyer > 2.0f)
            speedMultiplyer = 0.5f;

        speedMultiplyerText.text = speedMultiplyer.ToString("F1") + "x";
    }





    #endregion

    private void Update()
    {
        if (!ConsoleEnabled())
            return;

        if(Input.GetKeyDown(KeyCode.BackQuote))
            SetConsole(!consoleWindow.activeInHierarchy);

        if (Input.GetKeyDown(KeyCode.Escape))
            RevertConsole.Invoke();

        //Check for change in editor during play
        if(isInEditor && enableHotkeys != hotkeyMode)
        {
            ToggleHotkeys.Invoke(enableHotkeys);
            hotkeyMode = enableHotkeys;
        }
        
    }

    private void SetConsole(bool toState)
    {
        if (!ConsoleEnabled())
            return;

        consoleWindow.SetActive(toState);
        if (!enableHotkeys)
            ToggleHotkeys.Invoke(toState);
        if(!toState)
            RevertConsole();
    }
    public void OpenCommandConsole()
    {
        SetConsole(true);
    }

    public void CloseCommandConsole()
    {
        SetConsole(false);
    }

    private bool ConsoleEnabled()
    {
        if (!enableConsole)
            return false;
        if (!isInEditor && !enableInBuild)
            return false;

        return true;
    }


}
