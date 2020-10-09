using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthFruit : InteractableBase
{
    [Header("Health Fruit")]
    [SerializeField]
    private float _healingAmount;

    public override bool Interact(PlayerBase player)
    {
        if(!base.Interact(player)) return false;
        player.TakeHealing(_healingAmount);
        return true;
    }
}
