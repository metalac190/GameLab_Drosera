using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
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


}
