using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PesticideGrenadeProjectile : MonoBehaviour
{
    public UnityEvent OnExplode;

    [SerializeField] float _forwardForce = 5f;
    [SerializeField] float _upwardForce = 2f;
    [SerializeField] float _explosionDelay = .2f;
    [SerializeField] float _explosionRadius = 5f;
    [SerializeField] GameObject _pesticideHitbox;

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

        Vector3 pos = transform.position;
        pos.y = 0.2f;
        GameObject hitbox = Instantiate(_pesticideHitbox, pos, Quaternion.identity);
        Vector3 scale = hitbox.transform.localScale;
        scale.x = _explosionRadius;
        scale.z = _explosionRadius;
        hitbox.transform.localScale = scale;

        Destroy(gameObject);
    }
}
