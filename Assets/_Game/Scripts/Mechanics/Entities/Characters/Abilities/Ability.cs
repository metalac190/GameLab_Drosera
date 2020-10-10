using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class Ability : MonoBehaviour
{
    [SerializeField] protected float _cooldown;
    [SerializeField] public int _ammoCost;
    protected bool _onCooldown = false;

    Room _currentRoom;
    EnemyGroup _enemyGroup;

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
        _enemyGroup?.OnShotFired?.Invoke();
        yield return new WaitForSeconds(_cooldown);
        _onCooldown = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Door>() != null)
        {
            _currentRoom = other.GetComponent<Door>().room;
            _enemyGroup = _currentRoom.GetComponentInChildren<EnemyGroup>();
        }
    }

    //ability functionality defined in inheriting classes
    protected abstract void ActivateAbility();
}
