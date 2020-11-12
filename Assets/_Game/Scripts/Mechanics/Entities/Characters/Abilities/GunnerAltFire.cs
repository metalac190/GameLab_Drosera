using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;

public class GunnerAltFire : Ability
{
    public UnityEvent OnCharge;
    public UnityEvent OnFire;

    Transform _gunEnd;
    Gunner _gunner;

    [SerializeField] GameObject _projectile;

    bool _startCharging = false;
    float _charge = 0f;
    [SerializeField] float _chargeRate;
    public float ChargeRate { get { return _chargeRate; } }
    [SerializeField] float _maxCharge;

    public float Charge { get { return _charge; } }
    public Transform GunEnd { get { return _gunEnd; } }

    private void Start()
    {
        _gunner = GetComponent<Gunner>();
        _gunEnd = _gunner.gunEnd;
    }

    protected override void ActivateAbility()
    {
        StartCoroutine(ChargeShot());
    }

    IEnumerator ChargeShot()
    {
        _gunner.Animator.SetBool("chargingAni", true);
        _charge = 0;
        OnCharge?.Invoke();
        Instantiate(_projectile, _gunEnd.position, _gunEnd.rotation);
        while (_gunner.AltFireButton && _charge < _maxCharge)
        {
            _charge += _chargeRate * Time.deltaTime;
            yield return null;
        }
        _gunner.Animator.SetBool("chargingAni", false);
        _gunner.Animator.SetBool("altShootAni", true);
        StartCoroutine(CooldownTimer());
        OnFire?.Invoke();
    }
}
