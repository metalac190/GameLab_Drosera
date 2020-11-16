using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using TMPro;

[RequireComponent(typeof(ConsoleLog))]
#if(UNITY_EDITOR)
[RequireComponent(typeof(ConsoleLevelEditor))]
#endif
public class CommandConsole : MonoBehaviour
{
    public static event Action RevertConsole = delegate { };
    public static event Action<bool> ToggleHotkeys = delegate { };

    [HideInInspector] public static CommandConsole commandConsole;
    [Header("Settings")]
    [SerializeField] private bool enableInBuild = true;
    [SerializeField] private bool enableConsole = true;
    //[SerializeField] private bool enableHotkeysWhileClosed = true;

    [Header("Button References")]//as needed
    [SerializeField] Text speedMultiplyerText = null;

    [Header("Visuals")]
    [SerializeField] private GameObject consoleWindow = null;
    [SerializeField] private TextMeshProUGUI roomNamText = null;
    private Room currentRoom = null;

    private ConsoleLog log = null;
    #if (UNITY_EDITOR)
    private ConsoleLevelEditor levelEditor = null;
    #endif
    private PlayerBase playerRef = null;

    public bool IsInEditor => isInEditor;
    private bool isInEditor;
    public GameObject ConsoleCanvas => consoleWindow;
    public bool IsOpen => _isOpen;
    private bool _isOpen = false;

    private bool isPaused = false;
    private float speedMultiplyer = 1f;
    private bool hotkeyMode;

    ////////////////////////////////////////////////////////////////////////////////////////////////
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
#if UNITY_EDITOR
        levelEditor = GetComponent<ConsoleLevelEditor>();
#endif
        playerRef = FindObjectOfType<PlayerBase>();
    }

    private void Start()
    {
        speedMultiplyerText.text = speedMultiplyer.ToString("F1") + "x";
        consoleWindow?.SetActive(false);
        _isOpen = false;

        if (isInEditor)
            log.CloseLog();
    }
    #endregion

    ////////////////////////////////////////////////////////////////////////////////////////////////
    #region Respond to input events
    /// <summary>
    /// Public functions to react to input and run a command.
    /// Hook these up to the Unity Events provided on the Console_Button game object
    /// William Austin
    /// </summary>
    /// 
    ////////////////////////////////////////////////////////////////////////////////////////////////
    #region Enemy Commands

    public void AgroEnemiesCommand()
    {
        EnemyGroup roomEnemies = GetRoomEnemies();
        if(roomEnemies == null)
        {
            Debug.Log("Argo enemies command failed. Could not find EnemyGroup");
            return;
        }

        Debug.Log("Commanded enemies to be aggro");
        roomEnemies.TurnGroupAggressive.Invoke();
    }

    public void IdleEnemiesCommand()
    {
        EnemyGroup roomEnemies = GetRoomEnemies();
        if (roomEnemies == null)
        {
            Debug.Log("Idle enemies command failed. Could not find EnemyGroup");
            return;
        }

        Debug.Log("Commanded enemies to be idle");
        roomEnemies.TurnGroupPassive.Invoke();
    }

    public void ActivateHyperseedCommand()
    {
        HyperSeed seed = FindObjectOfType<HyperSeed>();
        if (seed != null)
            seed.Activate();
    }


    #endregion

    ////////////////////////////////////////////////////////////////////////////////////////////////
    #region Player Commands

    private bool invisToggleState = false;
    public void TogglePlayerInvincibilityCommand()
    {
        invisToggleState = !invisToggleState;
        playerRef.SetInvincibilityMode(invisToggleState);
    }

    public void SetPlayerInvincible()
    {
        invisToggleState = true;
        playerRef.SetInvincibilityMode(true);
    }

    public void RemovePlayerInvincibility()
    {
        invisToggleState = false;
        playerRef.SetInvincibilityMode(false);
    }


    private bool ammoToggle = false;
    public void ToggleInfiniteAmmoCommand()
    {
        ammoToggle = !ammoToggle;
        Gunner gunner = playerRef as Gunner;
        if (gunner != null)
            gunner.SetInfiniteAmmo(ammoToggle);
    }

    public void SetAmmoInfinite()
    {
        ammoToggle = true;
        Gunner gunner = playerRef as Gunner;
        if (gunner != null)
            gunner.SetInfiniteAmmo(true);
    }

    public void RemoveInfiniteAmmo()
    {
        ammoToggle = false;
        Gunner gunner = playerRef as Gunner;
        if (gunner != null)
            gunner.SetInfiniteAmmo(false);
    }

    public void KillPlayerCommand()
    {
        RemovePlayerInvincibility();
        playerRef.TakeDamage(playerRef.Health);
    }


    #endregion

    ////////////////////////////////////////////////////////////////////////////////////////////////
    public void SkipCutsceneCommand()
    {
        //GameManager.Instance?.CutSceneComplete();
        if (GameManager.Instance == null)
            return;
        GameObject cutscene = GameManager.Instance.CurrentCustscene;
        if (cutscene == null || !cutscene.activeInHierarchy)
            return;

        DialogueManager cutManager = cutscene.GetComponentInChildren<DialogueManager>();
        if(cutManager == null)
        {
            Debug.Log("Could not find the cutscene manager");
            return;
        }

        cutManager.DeactiveCutscene();

    }

    public void QuitCommand()
    {
        #if(UNITY_EDITOR)
        if (IsInEditor)
            UnityEditor.EditorApplication.isPlaying = false;
        else
            Application.Quit();
        #endif
    }
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

    #endregion

    /// ////////////////////////////////////////////////////////////////////////////////////////////////
    #region Command Helper Methods

    void IncrementSpeedMultiplyer()
    {
        speedMultiplyer += 0.5f;
        if (speedMultiplyer > 2.0f)
            speedMultiplyer = 0.5f;

        speedMultiplyerText.text = speedMultiplyer.ToString("F1") + "x";
    }

    private EnemyGroup GetRoomEnemies()
    {
        return currentRoom.GetComponentInChildren<EnemyGroup>();
    }


    #endregion

    ////////////////////////////////////////////////////////////////////////////////////////////////
    private void Update()
    {
        if (!ConsoleEnabled())
            return;

        if(Input.GetKeyDown(KeyCode.BackQuote))
            SetConsole(!consoleWindow.activeInHierarchy);

        if (Input.GetKeyDown(KeyCode.Escape))
            RevertConsole.Invoke();

        if(playerRef != null)
        {
            if (playerRef.currentRoom != null && playerRef.currentRoom != currentRoom)
            {
                currentRoom = playerRef.currentRoom;
                #if (UNITY_EDITOR)
                levelEditor?.PlayerChangedRoom(currentRoom);
                #endif
                roomNamText.text = currentRoom.name;
            }
        }
    }

    private void SetConsole(bool toState)
    {
        if (!ConsoleEnabled())
            return;

        _isOpen = toState;
        consoleWindow.SetActive(toState);

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
