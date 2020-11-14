using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

public class PesticideGrenadeProjectile : MonoBehaviour
{
    public UnityEvent OnExplode;

    [SerializeField] float _forwardForce = 5f;
    [SerializeField] float _upwardForce = 2f;
    [SerializeField] float _explosionDelay = .2f;
    [SerializeField] float _explosionRadius = 5f;
    [SerializeField] GameObject _pesticideHitbox;

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

        StartCoroutine(SpawnDecal());

        _audioScript.PlaySound(0);

        GameObject vfx = Instantiate(_vfx, transform.position, Quaternion.identity);
        vfx.GetComponentInChildren<SphereCollider>().enabled = false;
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        gameObject.GetComponent<SphereCollider>().enabled = false;

        StartCoroutine(ClearVFX(vfx));
    }

    IEnumerator SpawnDecal()
    {
        yield return new WaitForSeconds(0.35f);

        Vector3 pos = transform.position;
        pos.y = 0.2f;
        GameObject hitbox = Instantiate(_pesticideHitbox, pos, Quaternion.identity);
        Vector3 scale = hitbox.transform.localScale;
        scale.x = _explosionRadius;
        scale.z = _explosionRadius;
        hitbox.transform.localScale = scale;
    }

    //Not used currently
    IEnumerator ClearVFX(GameObject vfx)
    {
        PlayableDirector director = vfx.GetComponentInChildren<PlayableDirector>();

        yield return new WaitForSeconds((float)director.duration);

        Destroy(vfx);
        Destroy(gameObject);
    }
}
