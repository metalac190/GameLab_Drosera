using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HyperSeed : InteractableBase
{
    [Header("Hyperseed")]
    [SerializeField] GameObject seedModel;

    public override bool Interact(PlayerBase player)
    {
        if (!base.Interact(player))
            return false;

        seedModel.SetActive(false);
        Debug.Log(seedModel + " turned off.");
        FindObjectOfType<CameraShake>().TriggerCameraShake(.5f, .5f);
        StartCoroutine(FreezeTime());
        VFX();
        // When hyperseed is grabbed - all enemies turn aggressive
        EnemyGroup[] enemies = FindObjectsOfType<EnemyGroup>();
        foreach(EnemyGroup group in enemies)
        {
            group.GrabHyperseed.Invoke();
        }

        //Go to Main stage two game state
        GameManager.Instance.GameState = DroseraGlobalEnums.GameState.MainTwo;
        Debug.Log("Game State changed to: " + GameManager.Instance.GameState);

        return true;
    }

    IEnumerator FreezeTime()
    {
        Time.timeScale = 0;
        AudioListener.pause = true;
        yield return new WaitForSecondsRealtime(4f);
        AudioListener.pause = false;
        Time.timeScale = 1;
    }
}
