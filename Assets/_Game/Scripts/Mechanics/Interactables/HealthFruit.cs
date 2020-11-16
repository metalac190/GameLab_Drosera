using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthFruit : InteractableBase
{
    [System.Serializable]
    public class Stat
    {
        public DroseraGlobalEnums.Biome biome;
        public float _healingAmount;
        public Stat(float amt, DroseraGlobalEnums.Biome b) { _healingAmount = amt; biome = b; }
    }
    [Header("Health Fruit")]
    [Tooltip("If there are multiple of the same biome, it will use the last entry")]
    [SerializeField]
    private Stat[] _healingAmounts = { new Stat(10, DroseraGlobalEnums.Biome.Desert), new Stat(12, DroseraGlobalEnums.Biome.Jungle) };
    private float _healingAmount;
    [SerializeField]
    private bool disableOnPickup = false;
    [SerializeField]
    private float effectHeight = .1f;

    private void Start()
    {
        foreach(Stat s in _healingAmounts)
        {
            if (s.biome == GameManager.Instance.CurrentBiome) _healingAmount = s._healingAmount;
        }
    }

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
