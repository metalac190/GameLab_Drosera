using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthFruit : InteractableBase
{
    [Header("Health Fruit")]
    [SerializeField]
    private float _healingAmount;
    [SerializeField]
    private bool disableOnPickup = false;
    [SerializeField]
    private float effectHeight = .1f;




    public override bool Interact(PlayerBase player)
    {
        if(!base.Interact(player)) return false;
        player.TakeHealing(_healingAmount);
        if (effect != null)
            VFXSpawner.vfx.SpawnVFX(effect, effectDuration, player.transform.position + (Vector3.up * effectHeight)).transform.parent = player.transform;
        gameObject.SetActive(disableOnPickup);
        return true;
    }
}
