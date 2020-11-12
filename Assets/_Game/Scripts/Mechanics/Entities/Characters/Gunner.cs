using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gunner : PlayerBase
{
    GunnerPrimaryFire _primaryFire;
    GunnerAltFire _altFire;
    GunnerGrenade _grenade;
    GunnerDOTGrenade _dotGrenade;

    [SerializeField] public Transform gunEnd;
    [SerializeField] public Transform granadeSpawn;

    bool _firstShot = false;

    bool _altAbility = false;
    bool infiniteAmmo = false;
    int oldAmmoCost;

    protected override void Awake()
    {
        base.Awake();
        instance = this;

        _primaryFire = GetComponent<GunnerPrimaryFire>();
        _altFire = GetComponent<GunnerAltFire>();
        _grenade = GetComponent<GunnerGrenade>();
        _dotGrenade = GetComponent<GunnerDOTGrenade>();
    }

    protected override void Update()
    {
        base.Update();

        if (currentState != PlayerState.Dead)
        {
            if (swapAbilityButton)
            {
                if (!_altAbility)
                {
                    _altAbility = true;
                }
                else
                {
                    _altAbility = false;
                }
            }
        }
    }

    protected override void Attacking()
    {
        if (ammo > 0 && !altFireButton)
        {
            if ((shootButtonKey || shootButtonGamepad == 1))
            {
                if (_animator.GetInteger("shootAni") == 0)
                {
                    _animator.SetInteger("shootAni", 1);
                    StartCoroutine(FirstShotDelay());
                }
                else if (_firstShot)
                {
                    _primaryFire.Fire();
                }
            }
            else
            {
                _firstShot = false;
                currentState = PlayerState.Neutral;
            }
        }
        else
        {
            currentState = PlayerState.Reloading;
        }

        if (altFireButton)
        {
            _altFire.Fire();
            currentState = PlayerState.Neutral;
        }
    }

    protected override void Ability()
    {
        if (!_altAbility)
        {
            _grenade.Fire();
        }
        else if (_altAbility)
        {
            _dotGrenade.Fire();
        }
        currentState = PlayerState.Neutral;
    }

    IEnumerator FirstShotDelay()
    {
        yield return new WaitForSeconds(0.1f);
        _firstShot = true;
        
    }

    public void SetInfiniteAmmo(bool isInfinite)
    {
        infiniteAmmo = isInfinite;
        if (infiniteAmmo)
        {
            print("Set infinite");
            //Somehow being set twice, so check that we haven't.
            if(_primaryFire._ammoCost > 0)
                oldAmmoCost = _primaryFire._ammoCost;
            _primaryFire._ammoCost = 0;
        }
        else
        {
            _primaryFire._ammoCost = oldAmmoCost;
        }
    }
}
