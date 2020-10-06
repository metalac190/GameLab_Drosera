using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCompleteTrigger : MonoBehaviour
{
    [SerializeField] LevelGeneration levelGen;

    private void Start()
    {
        if(!levelGen)
            levelGen = FindObjectOfType<LevelGeneration>();

        levelGen.GenerateLevelTrigger();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<PlayerBase>() && GameManager.Instance.GameState == DroseraGlobalEnums.GameState.MainTwo)
        {
            print("Player completed level: " + levelGen.LevelNumber);
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
}
