using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GunnerGrenadeProjectile : MonoBehaviour
{
    public UnityEvent OnExplode;

    [SerializeField] float _forwardForce = 5f;
    [SerializeField] float _upwardForce = 2f;
    [SerializeField] float _explosionDelay = .2f;
    [SerializeField] float _explosionRadius = 5f;
    [SerializeField] int _damage = 30;

    Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        Destroy(gameObject, 5f);
    }

    private void Start()
    {
        _rb.AddForce(((Vector3.up * _upwardForce) + transform.forward * _forwardForce));
    }

    private void OnCollisionEnter(Collision collision)
    {
        StartCoroutine(Explode());
    }

    IEnumerator Explode()
    {
        yield return new WaitForSeconds(_explosionDelay);
        OnExplode?.Invoke();

        Collider[] hits = Physics.OverlapSphere(transform.position, _explosionRadius);
        foreach (var item in hits)
        {
            EntityBase entity = item.gameObject.GetComponent<EntityBase>();
            if (entity != null)
            {
                if (entity.GetComponent<EnemyBase>() != null)
                {
                    entity.TakeDamage(_damage);
                }
                else if (entity.GetComponent<PlayerBase>() != null)
                {
                    entity.TakeDamage(_damage * 0.67f);
                }
            }
        }

        Destroy(gameObject);
    }
}
