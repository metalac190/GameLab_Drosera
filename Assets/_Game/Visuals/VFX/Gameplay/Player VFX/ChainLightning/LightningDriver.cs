using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningDriver : MonoBehaviour
{
    [SerializeField] List<GameObject> targets;
    [SerializeField] LightningAnimator lightningAnimator;
    [SerializeField] GameObject chainLightning;
    // Start is called before the first frame update
    void Start()
    {
        if (lightningAnimator == null)
        {
            lightningAnimator = Instantiate(chainLightning, targets[0].transform.position, Quaternion.identity).GetComponent<LightningAnimator>();
        }
        lightningAnimator.SetTargets(targets);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            lightningAnimator.DestroyLightning();
        }
    }
}
