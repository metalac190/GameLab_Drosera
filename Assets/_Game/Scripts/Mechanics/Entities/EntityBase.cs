using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class EntityBase : MonoBehaviour
{
    public UnityEvent OnTakeDamage;
    public UnityEvent OnDeath;
    public UnityEvent OnHeal;

    [SerializeField] protected float _maxHealth;
    public float MaxHealth { get => _maxHealth; }
    [SerializeField] protected float _health;
    public float Health { get => _health; set => _health = value; }
    [SerializeField] protected bool _canAct;
    [SerializeField] protected bool _canStun;
    [SerializeField] protected float _knockbackResistance;
    [SerializeField] protected float _cooldown;
    [SerializeField] protected float _moveSpeed;

    [SerializeField] protected bool _isInvincible = false;

    [SerializeField] protected CharacterController _controller;
    [SerializeField] protected Animator _animator;

    public Animator Animator { get { return _animator; } }

    protected virtual void Awake() {
        _controller = GetComponent<CharacterController>();
        _animator = GetComponentInChildren<Animator>();
    }

    protected virtual void Start() 
    {
        _health = _maxHealth;
    }

    public virtual IEnumerator Knockback(float force, Vector3 direction)
    {
        //apply knockback here
        yield return null;
    }

    public virtual IEnumerator TakeDamage(float value, float knockbackForce, Vector3 knockbackDir)
    {
        StartCoroutine(Knockback(knockbackForce, knockbackDir));
        TakeDamage(value);
        yield return null;
    }

    public virtual void TakeDamage(float value)
    {
        if (!_isInvincible)
        {
            _health -= value;
            OnTakeDamage?.Invoke();
            if (_health <= 0)
            {
                OnDeath?.Invoke();
                Destroy(gameObject);
            }
        }
    }

    public virtual float TakeHealing(float value)
    {
        _health += value;
        if (_health > _maxHealth)
        {
            _health = _maxHealth;
        }
        OnHeal?.Invoke();
        return _health;
    }
}
