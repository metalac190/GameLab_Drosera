using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ChargeShot : MonoBehaviour
{
    public UnityEvent OnHit;

    GunnerAltFire _altFire;
    Hitbox _hitbox;
    Rigidbody _rb;

    bool _isCharging = true;
    float _charge;
    Vector3 _startScale;

    [Header("Shot Properties")]
    [SerializeField] float _moveSpeed;
    [SerializeField] float _damageMultiplier = 1f;
    [SerializeField] float _scaleMultiplier = 1f;
    [SerializeField] float _lifespan = 5f;

    private void Awake()
    {
        _altFire = FindObjectOfType<GunnerAltFire>();
        _hitbox = GetComponent<Hitbox>();
        _rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        _startScale = transform.localScale;
        _hitbox.baseDamage = 1f;
        Destroy(gameObject, _lifespan);
    }

    private void OnEnable()
    {
        _altFire.OnFire.AddListener(Fire);
    }

    private void OnDisable()
    {
        _altFire.OnFire.RemoveListener(Fire);
    }

    private void Update()
    {
        if (!_isCharging)
        {
            if (_charge < 1)
            {
                _rb.MovePosition(transform.position + transform.forward * Time.deltaTime * _moveSpeed);
            }
            else
            {
                _rb.MovePosition(transform.position + transform.forward * _charge * Time.deltaTime * _moveSpeed);
            }
        }
        else
        {
            _charge = _altFire.Charge;
            transform.position = _altFire.GunEnd.position;
            transform.rotation = _altFire.GunEnd.rotation;
            transform.localScale = _startScale + Vector3.one * _charge * _scaleMultiplier;
            if (_charge >= 1)
            {
                _hitbox.baseDamage = _altFire.Charge * _damageMultiplier;
            }
        }
    }

    public void Fire()
    {
        _isCharging = false;
    }

    void OnTriggerEnter(Collider other)
    {
        int layer = other.gameObject.layer;
        if (!(layer == 11 || layer == 13 || layer == 15)) //hit anything but player and other hitboxes
        {
            OnHit?.Invoke();
            Destroy(gameObject);
        }
    }
}
