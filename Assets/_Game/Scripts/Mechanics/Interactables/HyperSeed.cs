using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HyperSeed : InteractableBase
{
    [SerializeField] GameObject seedModel;

    public override bool Interact(PlayerBase player)
    {
        Debug.Log(player.name + "interacted with " + name);
        if (!base.Interact(player))
            return false;
        print(GameManager.Instance.GameState);
        GameManager.Instance.GameState = DroseraGlobalEnums.GameState.MainTwo;
        print(GameManager.Instance.GameState);
        seedModel.SetActive(false);
        return true;
    }
}
