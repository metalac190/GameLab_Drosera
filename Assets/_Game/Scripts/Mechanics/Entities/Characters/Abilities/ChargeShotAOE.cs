using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeShotAOE : Hitbox
{
    GunnerAltFire _altFire;
    float charge;
    [HideInInspector] public bool crit = false;
    [SerializeField] float _damageMultiplier = 15;
    [SerializeField] float _scaleMultiplier = 4;

    private void Start()
    {
        _altFire = FindObjectOfType<GunnerAltFire>();
        charge = _altFire.Charge;
        damage = 5 + charge * _damageMultiplier;
        if (crit)
        {
            damage *= 2;
        }
        transform.localScale = Vector3.one * (1 + charge * _scaleMultiplier);
        Destroy(gameObject, .1f);
    }

    public void IgnoreTarget(GameObject target)
    {
        hitTargets.Add(target);
    }
}
