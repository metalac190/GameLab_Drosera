using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelCompleteTrigger : MonoBehaviour
{
    [SerializeField] LevelGeneration levelGen;
    public UnityEvent LevelComplete;

    [SerializeField] GameObject directLight;

    private void Start()
    {
        if(!levelGen)
            levelGen = FindObjectOfType<LevelGeneration>();

        //Not technically a level compomplete, but will play first cutscene and gen the first level.
        GameManager.Instance.LevelComplete();
        //levelGen.GenerateLevelTrigger();
        GameManager.Instance.OnStateChange += OnLevelStart;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<PlayerBase>() && GameManager.Instance.GameState == DroseraGlobalEnums.GameState.MainTwo)
        {
            print("Player completed level: " + levelGen.LevelNumber);
            LevelComplete.Invoke();
            if(levelGen.LevelNumber >= 6)
            {
                GameManager.Instance.GameWon();
            }
            else
            {
                GameManager.Instance.LevelComplete();
            }
        }
    }

    //Temporary Alpha
    void OnLevelStart()
    {
        if (GameManager.Instance.GameState == DroseraGlobalEnums.GameState.MainOne)
        {
            if(directLight != null)
                directLight.SetActive(true);
        }
    }
}
