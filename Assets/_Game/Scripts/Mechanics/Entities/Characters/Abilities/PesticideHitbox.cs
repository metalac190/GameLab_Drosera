using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class PesticideHitbox : MonoBehaviour
{
    [SerializeField] float _damagePerSecond = 5f;
    [SerializeField] float _lifetime = 8f;

    [SerializeField] GameObject _decal;

    private void Awake()
    {
        Vector3 pos = new Vector3(transform.position.x, 0f, transform.position.z);
        Vector3 rot = new Vector3(90, 0, 0);
        GameObject decal = Instantiate(_decal, pos, Quaternion.Euler(rot));

        StartCoroutine(FadeDecal(decal));
    }

    private void OnTriggerStay(Collider other)
    {
        EnemyBase enemy = other.gameObject.GetComponent<EnemyBase>();
        if (enemy != null)
        {
            enemy.TakeDamage(_damagePerSecond * Time.deltaTime);
        }
    }

    IEnumerator FadeDecal(GameObject decal)
    {
        yield return new WaitForSeconds(_lifetime);

        float fade = decal.GetComponent<DecalProjector>().fadeFactor;
        while (fade > 0)
        {
            fade -= Time.deltaTime * 2;
            decal.GetComponent<DecalProjector>().fadeFactor = fade;
            yield return null;
        }
        Destroy(decal);
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Mesh mesh = GetComponent<MeshCollider>().sharedMesh;
        Gizmos.DrawWireMesh(mesh, transform.position, Quaternion.identity, transform.localScale);
    }
}
