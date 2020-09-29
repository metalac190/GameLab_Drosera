using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class Ability : MonoBehaviour
{
    [SerializeField] protected float _cooldown;
    [SerializeField] public int _ammoCost;
    protected bool _onCooldown = false;

    //call this when using ability
    public void Fire()
    {
        if (!_onCooldown)
        {
            ActivateAbility();
            _onCooldown = true;
        }
    }

    protected IEnumerator CooldownTimer()
    {
        yield return new WaitForSeconds(_cooldown);
        _onCooldown = false;
    }

    //ability functionality defined in inheriting classes
    protected abstract void ActivateAbility();
}
