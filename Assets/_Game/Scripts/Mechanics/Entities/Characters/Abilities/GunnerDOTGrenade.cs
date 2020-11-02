using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GunnerDOTGrenade : Ability
{
    public UnityEvent OnFire;

    [SerializeField] GameObject _projectile;

    Transform _gunEnd;
    Gunner _gunner;


    private void Awake()
    {
        _gunEnd = transform.GetChild(0).transform;
        _gunner = GetComponent<Gunner>();
    }

    protected override void ActivateAbility()
    {
        StartCoroutine(CooldownTimer());
        Instantiate(_projectile, _gunEnd.position, _gunEnd.rotation);
        _gunner.Ammo -= _ammoCost;
        OnFire?.Invoke();
    }
}
