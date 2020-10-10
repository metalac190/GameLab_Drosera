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

    AudioScript[] _audioScripts;

    private void Awake()
    {
        _gunEnd = transform.GetChild(0).transform;
        _gunner = GetComponent<Gunner>();
        _audioScripts = GetComponents<AudioScript>();
    }

    protected override void ActivateAbility()
    {
        StartCoroutine(CooldownTimer());
        _audioScripts[3].PlaySound(0);
        Instantiate(_projectile, _gunEnd.position, _gunEnd.rotation);
        _gunner.Ammo -= _ammoCost;
        OnFire?.Invoke();
    }
}
