using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ElectricRoundExpandFire))]
public class ElectricRoundDriver : MonoBehaviour
{
    ElectricRoundExpandFire roundScript;
    
    [Header("Charge")]
    [Tooltip("The time it takes for the round to charge up to max")]
    [SerializeField] float m_chargeTime = 1f;
    [Header("Fire")]
    [Tooltip("The direction to fire in, leave it at all zeroes to fire straight forward")]
    [SerializeField] Vector3 fireDirection;
    [Tooltip("Controls whether the script adjusts direction locally or by world coords. Only matters if fire direction is non-zero")]
    [SerializeField] bool localFireDirection = false;
    [Tooltip("The speed that the round moves with")]
    [SerializeField] float speed = 10f;

    private void Awake()
    {
        roundScript = GetComponent<ElectricRoundExpandFire>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            roundScript.Charge(m_chargeTime);
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (fireDirection == Vector3.zero)
            {
                roundScript.Fire(speed);
            }
            else
            {
                roundScript.Fire(speed, fireDirection);
            }
        }
    }
}
