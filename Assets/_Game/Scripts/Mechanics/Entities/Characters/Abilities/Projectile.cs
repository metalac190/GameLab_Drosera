using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Projectile : MonoBehaviour
{
    public UnityEvent OnHit;

    [SerializeField]
    protected float moveSpeed = 15f;
    [SerializeField]
    protected float lifespan = 5f;

    private Rigidbody rb;

    void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, lifespan);
    }

    private void FixedUpdate()
    {
        rb.MovePosition(transform.position + transform.forward * moveSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if(!(other.gameObject.layer == 11 || other.gameObject.layer == 13)) //hit anything but player and other hitboxes
        {
            OnHit?.Invoke();
            Destroy(gameObject);
        }
    }
}
