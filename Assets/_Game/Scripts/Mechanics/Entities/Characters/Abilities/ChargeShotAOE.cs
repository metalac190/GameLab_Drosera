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
    

    private void Start()
    {
        _altFire = FindObjectOfType<GunnerAltFire>();
        charge = _altFire.Charge;
        damage = 5 + charge * _damageMultiplier;
        transform.localScale = Vector3.one * (1 + charge * _scaleMultiplier);
        Destroy(gameObject, _lightningDuration);
    }

    public void IgnoreTarget(GameObject target)
    {
        hitTargets.Add(target);
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        StartCoroutine(Hide());

        LightningAnimator animator = Instantiate(_chainLightning, transform.position, transform.rotation).GetComponent<LightningAnimator>();
        animator.SetTargets(hitTargets);
        StartCoroutine(DestroyLightning(animator));
    }

    IEnumerator Hide()
    {
        yield return new WaitForSeconds(.1f);
        GetComponent<SphereCollider>().enabled = false;
        GetComponent<MeshRenderer>().enabled = false;
    }

    IEnumerator DestroyLightning(LightningAnimator animator)
    {
        yield return new WaitForSeconds(_lightningDuration);
        animator.DestroyLightning();  
    }
}
