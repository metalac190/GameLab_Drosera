using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;

public class GunnerPrimaryFire : Ability
{
    public UnityEvent OnFire;

    [SerializeField] GameObject _projectile;

    Transform _gunEnd;
    Gunner _gunner;

    void Start()
    {
        _gunner = GetComponent<Gunner>();
        _gunEnd = _gunner.gunEnd;
    }

    protected override void ActivateAbility()
    {
        StartCoroutine(CooldownTimer());
        Instantiate(_projectile, _gunEnd.position, _gunEnd.rotation);
        _gunner.Ammo -= _ammoCost;

        OnFire?.Invoke();
    }
}
