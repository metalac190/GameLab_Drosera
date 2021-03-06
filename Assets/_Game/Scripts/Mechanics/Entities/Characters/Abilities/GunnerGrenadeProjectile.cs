﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

public class GunnerGrenadeProjectile : MonoBehaviour
{
    public UnityEvent OnExplode;

    [SerializeField] float _forwardForce = 5f;
    [SerializeField] float _upwardForce = 2f;
    [SerializeField] float _explosionDelay = .2f;
    [SerializeField] float _explosionRadius = 5f;
    [SerializeField] int _damage = 30;
    [SerializeField] float _screenShakeDuration = .1f;
    [SerializeField] float _screenShakeMagnitude = .1f;


    [SerializeField] GameObject _vfx;

    bool _exploded = false;
 
    Rigidbody _rb;

    AudioScript _audioScript;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _audioScript = GetComponent<AudioScript>();
        Destroy(gameObject, 5f);
    }

    private void Start()
    {
        _rb.AddForce(((Vector3.up * _upwardForce) + transform.forward * _forwardForce));
        _rb.AddTorque(transform.right * -10);
        OnExplode.AddListener(ShakeScreen);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<EnemyBase>() != null)
        {
            StopAllCoroutines();
            StartCoroutine(Explode(0));
            _exploded = true;
        }
        if (!_exploded)
        {
            StartCoroutine(Explode(_explosionDelay));
            _exploded = true;
        }
    }

    IEnumerator Explode(float explosionDelay)
    {
        yield return new WaitForSeconds(explosionDelay);
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

        _audioScript.PlaySound(0);

        GameObject vfx = Instantiate(_vfx, transform.position, Quaternion.identity);
        vfx.GetComponentInChildren<SphereCollider>().enabled = false;
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        gameObject.GetComponent<SphereCollider>().enabled = false;

        StartCoroutine(ClearVFX(vfx));
    }

    IEnumerator ClearVFX(GameObject vfx)
    {
        PlayableDirector director = vfx.GetComponentInChildren<PlayableDirector>();

        yield return new WaitForSeconds((float)director.duration);

        Destroy(vfx);
        Destroy(gameObject);
    }

    private void ShakeScreen()
    {
        FindObjectOfType<CameraShake>().TriggerCameraShake(_screenShakeDuration, _screenShakeMagnitude);
    }
}
