using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionSwarmDriver : MonoBehaviour
{
    public Transform[] targets;
    [SerializeField] SelectionSwarmFollow swarm;
    [SerializeField] GameObject swarmPrefab;
    int current = 0;
    // Start is called before the first frame update
    void Start()
    {
        if (swarm == null)
        {
            swarm = Instantiate(swarmPrefab).GetComponent<SelectionSwarmFollow>();
        }
        swarm.SetFollowTarget(targets[0]);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            CycleTarget();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ToggleSwarm();
        }
    }

    void CycleTarget()
    {
        current++;
        current = current % targets.Length;
        swarm.SetFollowTarget(targets[current]);
    }

    void ToggleSwarm()
    {
        if (swarm.target == null)
        {
            swarm.gameObject.SetActive(true);
            swarm.SetFollowTarget(targets[current]);
        }
        else
        {
            swarm.SetFollowTarget(null);
        }
    }

}
