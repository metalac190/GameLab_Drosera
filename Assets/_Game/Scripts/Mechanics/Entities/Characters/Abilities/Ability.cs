using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class Ability : MonoBehaviour
{
    [SerializeField] protected float _cooldown;
    public float Cooldown { get { return _cooldown; } }
    [SerializeField] public int _ammoCost;
    protected bool _onCooldown = false;
    public bool OnCooldown { get { return _onCooldown; } }

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
        if (other.GetComponent<RoomCollision>() != null)
        {
            _currentRoom = other.GetComponent<RoomCollision>().room;
            
            if (_currentRoom != null)
            {
                if (_currentRoom.GetComponentInChildren<EnemyGroup>() != null)
                {
                    _enemyGroup = _currentRoom.GetComponentInChildren<EnemyGroup>();
                }
                else
                {
                    _enemyGroup = null;
                }
            }
            else
            {
                _enemyGroup = null;
            }
        }
    }

    //ability functionality defined in inheriting classes
    protected abstract void ActivateAbility();
}
