using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CommandConsole : MonoBehaviour
{
    public static event Action RevertConsole = delegate { };
    public static event Action<bool> ToggleHotkeys = delegate { };

    [HideInInspector] public static CommandConsole commandConsole;
    [Header("Settings")]
    [SerializeField] private bool enableConsole = true;
    [SerializeField] private bool enableHotkeysWhileClosed = true;

    [Header("Scene Settings")]
    [SerializeField] bool generateLevel = true;
    [SerializeField] GameObject generateRoom = null;

    [Header("Button References")]//as needed
    [SerializeField] Text speedMultiplyerText = null;

    [Header("Visuals")]
    [SerializeField] private GameObject consoleWindow = null;
    

    private bool isInEditor;

    private float lastTimeScale = 1f;
    private float speedMultiplyer = 1f;


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
    }

    private void Start()
    {
        speedMultiplyerText.text = speedMultiplyer.ToString("F1") + "x";
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
        lastTimeScale = Time.timeScale;
        Time.timeScale = 0f;
    }

    public void PlayCommand()
    {
        Time.timeScale = lastTimeScale;
    }

    public void FastForwardCommand()
    {
        IncrementSpeedMultiplyer();
        Time.timeScale = speedMultiplyer;
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
        if (!isInEditor || !enableConsole)
            return;

        if(Input.GetKeyDown(KeyCode.BackQuote))
            SetConsole(!consoleWindow.activeInHierarchy);

        if (Input.GetKeyDown(KeyCode.Escape))
            RevertConsole.Invoke();
        
    }

    private void SetConsole(bool toState)
    {
        if (!isInEditor || !enableConsole)
            return;

        consoleWindow.SetActive(toState);
        if (!enableHotkeysWhileClosed)
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



}
