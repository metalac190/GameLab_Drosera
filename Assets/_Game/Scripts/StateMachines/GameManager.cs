using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] string mainMenuScene;
    private LevelGeneration levelGen;

    public delegate void OnGameStateChangeHandler();
    public event OnGameStateChangeHandler OnStateChange;
    private static GameManager _instance = null;

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
        //Start Cut scene
    }

    public void CutSceneComplete()
    {
        GameState = DroseraGlobalEnums.GameState.MainOne;
        Time.timeScale = 1;

        if (!levelGen)
            levelGen = FindObjectOfType<LevelGeneration>();
        levelGen.GenerateLevelTrigger();

    }

    //Likely set these up later to bring up a UI or go to their own scene.
    public void GameLost()
    {
        GameState = DroseraGlobalEnums.GameState.Menu;
        UnityEngine.SceneManagement.SceneManager.LoadScene(mainMenuScene);
    }

    public void GameWon()
    {
        GameState = DroseraGlobalEnums.GameState.Menu;
        UnityEngine.SceneManagement.SceneManager.LoadScene(mainMenuScene);
    }

    //Testing, remove once intergrated with cutscene
    private void Update()
    {
        if(gameState == DroseraGlobalEnums.GameState.CutScene && Input.anyKeyDown)
        {
            CutSceneComplete();
        }
    }

}
