using System.Collections.Generic;
using UnityEngine;

public class TopDownCamera : MonoBehaviour
{
    #region Variables 
    [SerializeField] public Transform target; //the player's transform
    [SerializeField] public float height = 5f;
    [SerializeField] public float distance = 10.0f;
    private float angle = 45.0f;
    private float smoothSpeed = 0.5f;
    private Vector3 refVelocity;
    #endregion

    #region Main Methods 
    // Start is called before the first frame update
    void Start()
    {
        HandleCamera();
    }

    // Update is called once per frame
    void Update()
    {
        HandleCamera();
    }
    #endregion

    #region Helper Methods 
    protected virtual void HandleCamera()
    {
        if (!target)
        {
            return;
        }

        //Build world position vector 
        Vector3 worldPosition = (Vector3.forward * -distance) + (Vector3.up * height);
        //Debug.DrawLine(target.position, worldPosition, Color.red);

        //Build our rotated vector 
        Vector3 rotatedVector = Quaternion.AngleAxis(angle, Vector3.up) * worldPosition;
        //Debug.DrawLine(target.position, rotatedVector, Color.green);

        //Move our position
        Vector3 flatTargetPosition = target.position;
        flatTargetPosition.y = 0.0f;
        Vector3 finalPosition = flatTargetPosition + rotatedVector;
        //Debug.DrawLine(target.position, finalPosition, Color.blue);

        transform.position = Vector3.SmoothDamp(transform.position, finalPosition, ref refVelocity, smoothSpeed);
        transform.LookAt(target.position);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.25f);
        if (target)
        {
            Gizmos.DrawLine(transform.position, target.position);
            Gizmos.DrawSphere(target.position, 1.5f);
        }
        Gizmos.DrawSphere(transform.position, 1.5f);
    }
    #endregion
}
