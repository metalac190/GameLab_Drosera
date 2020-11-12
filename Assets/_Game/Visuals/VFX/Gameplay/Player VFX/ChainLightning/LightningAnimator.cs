using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class LightningAnimator : MonoBehaviour
{
    List<GameObject> targets = new List<GameObject>();
    bool destroyThis = false;
    [Tooltip("The time it takes to move between each target")]
    [SerializeField] float chainTime = 0.1f;
    [SerializeField] ParticleSystem particles;
    int index = 1;
    float lerpProgress = 0f;
    Vector3 previousLocation;

    private void Awake()
    {
        if (particles == null)
        {
            particles = GetComponent<ParticleSystem>();
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        previousLocation = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!destroyThis)
        {
            //we only need to chain between targets if there's more than one target
            if (targets.Count > 1)
            {
                lerpProgress += Time.deltaTime;
                lerpProgress = Mathf.Min(lerpProgress, chainTime);
                transform.position = Vector3.Lerp(previousLocation, targets[index].transform.position, lerpProgress / chainTime);
                //if we're at the next target, reset everything
                if (lerpProgress == chainTime)
                {
                    index++;
                    //modulo to go back to the beginning of the list when we hit the end
                    index %= targets.Count;
                    previousLocation = transform.position;
                    lerpProgress = 0;
                }
            }
        }
        else
        {
            var emission = particles.emission;
            if (emission.enabled)
            {
                emission.enabled = false;
            }
            else if (particles.particleCount == 0)
            {
                Destroy(gameObject);
            }
        }
    }

    public void SetTargets(List<GameObject> newTargets)
    {
        targets = newTargets;
    }

    public void DestroyLightning()
    {
        destroyThis = true;
    }
}
