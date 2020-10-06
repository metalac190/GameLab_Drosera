using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PesticideHitbox : MonoBehaviour
{
    [SerializeField] float _damagePerSecond = 5f;
    [SerializeField] float _lifetime = 8f;

    private void Awake()
    {
        Destroy(gameObject, _lifetime);
    }

    private void OnTriggerStay(Collider other)
    {
        EnemyBase enemy = other.gameObject.GetComponent<EnemyBase>();
        if (enemy != null)
        {
            enemy.TakeDamage(_damagePerSecond * Time.deltaTime);
        }
    }

    private void OnDrawGizmos()
    {
        Mesh mesh = GetComponent<MeshCollider>().sharedMesh;
        Gizmos.DrawWireMesh(mesh, transform.position, Quaternion.identity, transform.localScale);
    }
}
