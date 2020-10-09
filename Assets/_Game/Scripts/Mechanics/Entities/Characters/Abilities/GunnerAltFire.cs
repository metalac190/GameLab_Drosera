﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;

public class GunnerAltFire : Ability
{
    public UnityEvent OnFire;

    Transform _gunEnd;
    Gunner _gunner;

    AudioScript[] _audioScripts;

    [SerializeField] GameObject _projectile;

    bool _startCharging = false;
    float _charge = 0f;
    [SerializeField] float _chargeRate;
    [SerializeField] float _maxCharge;

    public float Charge { get { return _charge; } }
    public Transform GunEnd { get { return _gunEnd; } }

    private void Awake()
    {
        _gunEnd = transform.GetChild(0).transform;
        _gunner = GetComponent<Gunner>();
        _audioScripts = GetComponents<AudioScript>();
    }

    protected override void ActivateAbility()
    {
        StartCoroutine(ChargeShot());
    }

    IEnumerator ChargeShot()
    {
        _audioScripts[2].PlaySound(0);
        Instantiate(_projectile, _gunEnd.position, _gunEnd.rotation);
        while (_gunner.AltFireButton && _charge < _maxCharge)
        {
            _charge += _chargeRate * Time.deltaTime;
            yield return null;
        }
        StartCoroutine(CooldownTimer());
        OnFire?.Invoke();
        _audioScripts[2].StopSound();
        _audioScripts[1].PlaySound(0);
        _charge = 0;
    }
}
