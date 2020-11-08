using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionSwarmFollow : MonoBehaviour
{
    public enum StopBehavior { Destroy = 0,Disable};
    [Tooltip("What to do if target becomes null, disables system by default")]
    public StopBehavior stopBehavior = StopBehavior.Disable;
    [Tooltip("How fast it lerps between current position and target position, higher = faster")]
    [SerializeField] float followSpeed = 3f;
    [SerializeField] ParticleSystem particles;
    bool isBrawler;
    public bool useFixedUpdate = false;
    public Transform target { get; private set; }
    Vector3 offset;

    private void Awake()
    {
        if (particles == null)
        {
            particles = GetComponentInChildren<ParticleSystem>();
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (!useFixedUpdate)
        {
            if (target != null)
            {
                transform.position = Vector3.Lerp(transform.position, target.position + offset, Time.deltaTime * followSpeed);
            }
            else
            {
                var emission = particles.emission;
                if (emission.enabled == true)
                {
                    emission.enabled = false;
                }
                else if (particles.particleCount == 0)
                {
                    if (stopBehavior == StopBehavior.Destroy)
                    {
                        Destroy(this.gameObject);
                    }
                    else if (stopBehavior == StopBehavior.Disable)
                    {
                        gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (useFixedUpdate)
        {
            if (target != null)
            {
                transform.position = Vector3.Lerp(transform.position, target.position + offset, Time.fixedDeltaTime * followSpeed);
            }
            else
            {
                var emission = particles.emission;
                if (emission.enabled == true)
                {
                    emission.enabled = false;
                }
                else if (particles.particleCount == 0)
                {
                    if (stopBehavior == StopBehavior.Destroy)
                    {
                        Destroy(this.gameObject);
                    }
                    else if (stopBehavior == StopBehavior.Disable)
                    {
                        gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    /// <summary>
    /// sets the transform for the swarm to follow
    /// </summary>
    /// <param name="target"></param>
    public void SetFollowTarget(Transform followTarget)
    {
        target = followTarget;
        var emission = particles.emission;
        emission.enabled = true;
        offset = new Vector3(0, 0, 0);
    }

    public void SetFollowTarget(Transform followTarget, Vector3 followOffset)
    {
        target = followTarget;
        var emission = particles.emission;
        emission.enabled = true;
        offset = followOffset;
    }
}
