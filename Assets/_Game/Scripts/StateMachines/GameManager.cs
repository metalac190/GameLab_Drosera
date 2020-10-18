using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    [SerializeField] string mainMenuScene;
    [SerializeField] string loseScene;
    [SerializeField] string winScene;
    [SerializeField] GameObject[] Cutscenes;
    private LevelGeneration levelGen;
    private int currentCutscene;
    private bool gameWon = false;
    private GameObject cutsceneInstance;

    public delegate void OnGameStateChangeHandler();
    public event OnGameStateChangeHandler OnStateChange;
    public UnityEvent LevelStart;
    private static GameManager _instance = null;

    [SerializeField] bool showLightLines = true;

    private DroseraGlobalEnums.GameState gameState;
    public DroseraGlobalEnums.GameState GameState { get => gameState; set {
            gameState = value;
            OnStateChange?.Invoke();
        } }
    private DroseraGlobalEnums.Biome currentBiome;
    public DroseraGlobalEnums.Biome CurrentBiome { get => currentBiome; set => currentBiome = value; }

    public static GameManager Instance
    {
        get
        {
            return GameManager._instance;
        }
    }

    private void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public void LevelComplete()
    {
        GameState = DroseraGlobalEnums.GameState.CutScene;
        Time.timeScale = 0;
        //May change to play random cutscene
        cutsceneInstance = Instantiate(Cutscenes[currentCutscene++]);

        if (!gameWon)
        {
            if (!levelGen)
                levelGen = FindObjectOfType<LevelGeneration>();
            levelGen.GenerateLevelTrigger();
        }
    }

    public void CutSceneComplete()
    {
        if(gameWon)
        {
            GameState = DroseraGlobalEnums.GameState.Menu;
            gameWon = false;
            UnityEngine.SceneManagement.SceneManager.LoadScene(winScene);
        }

        GameState = DroseraGlobalEnums.GameState.MainOne;
        Time.timeScale = 1;

        Destroy(cutsceneInstance);

        LevelStart.Invoke();

    }

    //Likely set these up later to bring up a UI or go to their own scene.
    public void GameLost()
    {
        currentCutscene = 0;
        GameState = DroseraGlobalEnums.GameState.Menu;
        UnityEngine.SceneManagement.SceneManager.LoadScene(loseScene);
    }

    public void GameWon()
    {
        currentCutscene = 0;
        gameWon = true;
        LevelComplete();
    }

    public bool GetShowLightLines()
    {
        return showLightLines;
    }

}
