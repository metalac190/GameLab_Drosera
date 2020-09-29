using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;

public class GunnerAltFire : Ability
{
    public UnityEvent OnFire;

    Transform _gunEnd;
    Gunner _gunner;

    bool _startCharging = false;
    float _charge = 0f;
    [SerializeField] float _chargeRate;
    [SerializeField] float _maxCharge;

    public float Charge { get { return _charge; } }

    private void Awake()
    {
        _gunEnd = transform.GetChild(0).transform;
        _gunner = GetComponent<Gunner>();
    }

    protected override void ActivateAbility()
    {
        StartCoroutine(ChargeShot());
    }

    IEnumerator ChargeShot()
    {
        Instantiate(AssetDatabase.LoadAssetAtPath("Assets/_Game/Prefabs/ChargeShot.prefab", typeof(GameObject)), _gunEnd.position, _gunEnd.rotation);
        while (_gunner.AltFireButton && _charge < _maxCharge)
        {
            Debug.Log("nuts");
            _charge += _chargeRate * Time.deltaTime;
            yield return null;
        }
        StartCoroutine(CooldownTimer());
        OnFire?.Invoke();
        _charge = 0;
    }
}
