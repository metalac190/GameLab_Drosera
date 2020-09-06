using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class EntityBase : MonoBehaviour
{
    public UnityEvent OnTakeDamage;
    public UnityEvent OnDeath;
    public UnityEvent OnHeal;

    protected float _maxHealth;
    protected float _health;
    protected bool _canAct;
    protected bool _canStun;
    protected float _knockbackResistance;
    protected float _coolDown;
    protected float _moveSpeed;

    protected CharacterController _controller;
    protected Animator _animator;

    protected virtual void Awake() {
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
    }

    protected virtual IEnumerator Knockback(float force, Vector3 direction)
    {
        //apply knockback here
        yield return null;
    }

    protected virtual IEnumerator TakeDamage(float value, float knockbackForce, Vector3 knockbackDir)
    {
        StartCoroutine(Knockback(knockbackForce, knockbackDir));
        TakeDamage(value);
        yield return null;
    }

    protected virtual void TakeDamage(float value)
    {
        OnTakeDamage?.Invoke();
        _health -= value;
        if (_health <= 0)
        {
            OnDeath?.Invoke();
        }
    }

    protected virtual float TakeHealing(float value)
    {
        OnHeal?.Invoke();
        _health += value;
        if (_health > _maxHealth)
        {
            _health = _maxHealth;
        }
        return _health;
    }
}
