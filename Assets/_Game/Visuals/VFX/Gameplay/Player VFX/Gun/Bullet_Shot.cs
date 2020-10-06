using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Shot : MonoBehaviour
{
    void Start()
    {
        ParticleSystem[] childrenParticleSytems;
        childrenParticleSytems = gameObject.GetComponentsInChildren<ParticleSystem>();

        foreach (ParticleSystem childPS in childrenParticleSytems)
        {
            childPS.Play();
        }
    }
}
