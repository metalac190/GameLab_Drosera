using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Projectile : MonoBehaviour
{
    public UnityEvent OnHit;
    public UnityEvent OnCriticalHit;

    [SerializeField]
    protected float moveSpeed = 15f;
    [SerializeField]
    protected float lifespan = 5f;

    [Header("VFX")]
    [SerializeField]
    protected GameObject effect;
    [SerializeField]
    protected float effectDuration;

    private Rigidbody rb;
    Hitbox hitbox;
    bool crit;

    void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        hitbox = GetComponent<Hitbox>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, lifespan);
        int temp = Random.Range(0, 6);
        if (temp == 0)
        {
            crit = true;
            hitbox.baseDamage *= 2;
        }
    }

    private void FixedUpdate()
    {
        rb.MovePosition(transform.position + transform.forward * moveSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        int layer = other.gameObject.layer;
        if(!(layer == 11 || layer == 13 || layer == 15)) //hit anything but player, other hitboxes, and invisible walls
        {
            if (crit)
            {
                OnCriticalHit?.Invoke();
            }
            else
            {
                OnHit?.Invoke();
            }
            Destroy(gameObject);
        }
    }

    public void SpawnHitVFX()
    {
        if (effect != null && VFXSpawner.vfx != null)
        {
            GameObject vfx = VFXSpawner.vfx.SpawnVFX(effect, effectDuration, transform.position, transform.rotation);
        }
    }
}
