using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockwaveTest : MonoBehaviour
{
    [SerializeField] GameObject baseShockwave;
    [SerializeField] GameObject critShockwave;
    ParticleSystem[] baseParticles;
    ParticleSystem[] critParticles;

    private void Awake()
    {
        baseParticles = baseShockwave.GetComponentsInChildren<ParticleSystem>();
        critParticles = critShockwave.GetComponentsInChildren<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            //if both have been put in, play one based on random number
            if (baseParticles != null && critParticles != null)
            {
                int rand = Random.Range(0, 6);
                if (rand == 0)
                {
                    foreach (ParticleSystem particle in critParticles)
                    {
                        particle.Play();
                    }
                }
                else
                {
                    foreach (ParticleSystem particle in baseParticles)
                    {
                        particle.Play();
                    }
                }
            }
        }
    }
}
