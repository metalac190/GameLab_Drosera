using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OreVein : InteractableBase
{
    [Header("Ore Vein")]
    [SerializeField]
    [Tooltip("Make sure these are children of this object, also the sizes are in decending order starting with an empty vein")]
    private GameObject[] _orePrefabSizes;
    [SerializeField]
    GameObject mineEffect;
    [SerializeField]
    float mineEffectDuration;
    [SerializeField]
    Vector3 VFXPlayerCenterOffset;

    [Tooltip("Check only on the starting room ore vein.")]
    public bool isInfinite;

    //private Animator _animator; // Might be useless, keeping for now if it turns out it is needed

    private void Start()
    {
        //_animator = GetComponent<Animator>();
        //_animator.SetInteger("stage", _uses);
        foreach(GameObject obj in _orePrefabSizes)
        {
            obj.SetActive(false);
        }
        _orePrefabSizes[Mathf.Clamp(_uses, 0, _orePrefabSizes.Length - 1)].SetActive(true);
    }

    public override bool Interact(PlayerBase player)
    {
        if (_uses > 0)
        {
            if (isInfinite && player.Ammo + player.AmmoPerOre <= player.MaxAmmo)
                player.Ammo += player.AmmoPerOre;
            else if (!isInfinite)
                player.HeldAmmo += player.AmmoPerOre;

            if (!base.Interact(player))
                return false;

            if (effect != null)
                VFXSpawner.vfx.SpawnVFX(effect, effectDuration, player.transform.position).transform.parent = player.transform;
            if(mineEffect != null)
            {
                RaycastHit hit;
                Ray ray = new Ray(player.transform.position + VFXPlayerCenterOffset, transform.position - VFXPlayerCenterOffset - player.transform.position);
                if(Physics.Raycast(ray, out hit, 10f, LayerMask.GetMask("Hitbox"), QueryTriggerInteraction.Ignore))
                {
                    VFXSpawner.vfx.SpawnVFX(mineEffect, mineEffectDuration, hit.point).transform.up = -ray.direction.normalized;//hit.normal;
                }
            }
            ChangeState();
        }

        //_animator.SetInteger("stage", _uses);
        //VFX();
        

        //player.Ammo = Mathf.Clamp(player.Ammo, 0 , player.MaxAmmo); ????
        if (_uses <= 0)
        {
            if (isInfinite)
            {
                _uses = _maxUses;
            }
            else
            {
                //GetComponent<Collider>().enabled = false;
            }
        }

        return true;
    }

    private void ChangeState()
    {
        foreach (GameObject obj in _orePrefabSizes)
        {
            obj.SetActive(false);
        }
        _orePrefabSizes[Mathf.Clamp(_uses, 0, _orePrefabSizes.Length-1)].SetActive(true);
    }
}
