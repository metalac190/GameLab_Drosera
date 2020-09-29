using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    protected float moveSpeed = 15f;
    [SerializeField]
    protected float damage = 1f;
    [SerializeField]
    protected float lifespan = 5f;

    private Rigidbody rb;

    void Awake()
    {
        rb = gameObject.AddComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        rb.AddForce(rb.transform.forward * moveSpeed);
        Destroy(gameObject, lifespan);
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.GetComponent("PlayerBase") == null) //hit anything but player
        {
            Destroy(gameObject);
        }
        
    }
}
