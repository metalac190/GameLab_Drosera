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

    void Awake()
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
