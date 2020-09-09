using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class BreakableWall : EntityBase
{
    [Header("Breakable Wall")]
    [SerializeField]
    private Transform _wall;
    [SerializeField]
    private ParticleSystem _crumbleParticles;
    [SerializeField]
    [Tooltip("Percent damage dealt per second")]
    private float _DOTRate = 0.5f;
    [SerializeField]
    private float _fallDist = 3;

    private void Start()
    {
        _health = _maxHealth;
    }

    public override void TakeDamage(float value)
    {
        if (_health <= 0) return;
        OnTakeDamage?.Invoke();
        StartCoroutine(Crumble(value));
        if (_health <= 0)
        {
            GetComponent<Collider>().enabled = false;
            OnDeath?.Invoke();
        }
    }

    private IEnumerator Crumble(float damage)
    {
        float startingDamage = damage;
        while(_health > 0 && damage > 0)
        {
            if(_crumbleParticles) _crumbleParticles.Play();
            float damageDone = Time.deltaTime * _DOTRate * startingDamage;
            damage -= damageDone;
            _health -= damageDone;
            _wall.position -= new Vector3(0, _fallDist *  damageDone / _maxHealth, 0);
            yield return new WaitForEndOfFrame();
            if (_crumbleParticles) _crumbleParticles?.Pause();
        }
    }

    /// <summary>
    /// Does nothing
    /// </summary>
    /// <param name="value"></param>
    /// <param name="knockbackForce"></param>
    /// <param name="knockbackDir"></param>
    /// <returns></returns>
    public override IEnumerator TakeDamage(float value, float knockbackForce, Vector3 knockbackDir)
    {
        yield return null;
    }

    /// <summary>
    /// Does nothing
    /// </summary>
    /// <param name="force"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    public override IEnumerator Knockback(float force, Vector3 direction)
    {
        yield return null;
    }

    /// <summary>
    /// Does nothing
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public override float TakeHealing(float value)
    {
        return _health;
    }
}
