using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeShotAOE : Hitbox
{
    GunnerAltFire _altFire;
    float charge;
    [SerializeField] GameObject _chainLightning;
    [SerializeField] float _damageMultiplier = 15;
    [SerializeField] float _scaleMultiplier = 4;
    [SerializeField] float _lightningDuration = 1;
    [SerializeField] float _slowModifier = .4f;
    [SerializeField] float _slowDuration = 3f;
    

    private void Start()
    {
        _altFire = FindObjectOfType<GunnerAltFire>();
        charge = _altFire.Charge;
        damage = 5 + charge * _damageMultiplier;
        transform.localScale = Vector3.one * (1 + charge * _scaleMultiplier);
        StartCoroutine(Hide());

        if (_slowDuration > _lightningDuration)
            Destroy(gameObject, _slowDuration + 0.5f);
        else
            Destroy(gameObject, _lightningDuration + 0.5f);
    }

    public void IgnoreTarget(GameObject target)
    {
        hitTargets.Add(target);
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        foreach (var enemy in hitTargets)
        {
            StartCoroutine(enemy.GetComponent<EnemyBase>().AltFireEnemySlowed(_slowModifier, _slowDuration));
        }

        LightningAnimator animator = Instantiate(_chainLightning, transform.position, transform.rotation).GetComponent<LightningAnimator>();
        animator.SetTargets(hitTargets);
        StartCoroutine(DestroyLightning(animator));
    }

    IEnumerator Hide()
    {
        yield return new WaitForSeconds(.1f);
        GetComponent<SphereCollider>().enabled = false;
        //GetComponent<MeshRenderer>().enabled = false;
    }

    IEnumerator DestroyLightning(LightningAnimator animator)
    {
        yield return new WaitForSeconds(_lightningDuration);
        animator.DestroyLightning();  
    }
}
