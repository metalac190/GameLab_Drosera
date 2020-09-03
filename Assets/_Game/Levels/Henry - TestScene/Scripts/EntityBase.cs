using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class EntityBase : MonoBehaviour
{
    public event Action TookDamage = delegate { };
    public event Action Die = delegate { };
    
    protected float _health;
    protected bool _canAct;
    protected float _coolDown;
    protected float _moveSpeed;

    protected abstract IEnumerator Knockback(float force, Vector3 direction);

    protected virtual float TakeDamage(float value, float knockbackForce, Vector3 knockbackDir)
    {
        TookDamage?.Invoke();
        StartCoroutine(Knockback(knockbackForce, knockbackDir));
        _health -= value;
        if (_health <= 0)
        {
            Die?.Invoke();
        }
        return _health;
    }
}
