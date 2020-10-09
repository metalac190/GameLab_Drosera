using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Brawler : EnemyBase {

#pragma warning disable 0649 // Disable "Field is never assigned" warning for SerializeField

    [Header("Brawler Specific")]
    [SerializeField] private float attackRange;
    [SerializeField] private List<GameObject> waypoints;
    private List<Vector3> waypointPositions = new List<Vector3>();
    private int currentWaypoint;

    [System.Serializable]
    public class FX {
        [Header("VFX")]

        [Header("SFX")]
        public UnityEvent OnHit;
    }
    [Header("Brawler VFX & SFX")] [SerializeField] private FX _brawlerFX;

#pragma warning restore 0649

    // -------------------------------------------------------------------------------------------

    protected override void Awake() {
        base.Awake();

        GetWaypoints();
        // If no waypoints are set - set temp ones
        if(waypoints.Count == 0) {
            AddWaypoint();
            AddWaypoint();
            //Debug.LogError(gameObject.name + " in " + transform.parent.parent.name + " has no waypoints.");

            int i = 1;
            foreach(GameObject waypoint in waypoints) {
                waypoint.transform.position = transform.position + new Vector3(i++, 0, 0);
            }
        }
        // Set waypoint positions
        foreach(GameObject waypoint in waypoints)
            waypointPositions.Add(waypoint.transform.position);
        currentWaypoint = 1;
    }

    protected override void Start() {
        base.Start();
    }

    // -------------------------------------------------------------------------------------------
    // Behavior Coroutines - Main

    protected override IEnumerator Idle(bool regen = false) {
        _agent.stoppingDistance = 0f;
        _agent.SetDestination(transform.position);

        if(regen) {
            isHealing = true;
            StartCoroutine(Regenerate());
        }
        currentState = EnemyState.Passive;

        Vector3 forward;
        while(true) {
            // Get next position
            try {
                if(currentWaypoint >= waypointPositions.Count)
                    currentWaypoint = 0;
                targetPosition = waypointPositions[currentWaypoint++];
            } catch {
                targetPosition = transform.position;
                Debug.LogError(name + " does not have any waypoints.");
            }
            forward = targetPosition - transform.position;
            forward.y = 0;

            // Turn to look towards position over 1 sec
            for(float i = 0; i < 1; i += Time.deltaTime) {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(forward, Vector3.up), Time.deltaTime * 180f);
                yield return null;
                CheckAggression();
            }

            // Wait 0.5 sec
            for(float i = 0; i < 0.5; i += Time.deltaTime) {
                yield return null;
                CheckAggression();
            }

            // Move towards position
            _agent.SetDestination(targetPosition);
            while(_agent.pathPending)
                yield return null;
            while(_agent.remainingDistance > 0.2f)
                yield return null;

            // Wait at position for 1.5 sec
            for(float i = 0; i < 1.5f; i += Time.deltaTime) {
                yield return null;
                CheckAggression();
            }
        }
    }

    // ------

    protected override IEnumerator AggressiveMove() {
        _agent.stoppingDistance = stoppingDistance;
        attackDone = false;
        currentState = EnemyState.Aggressive;

        while(true) {
            yield return null;
            FindTarget();

            // No target player available - idle instead
            if(targetPlayer == null) {
                ForceIdle();
                yield break;
            }

            // Move towards player
            _agent.SetDestination(targetPlayer.transform.position);

            // Turn if standing still
            if(_agent.velocity.magnitude < 0.5f)
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(VectorToPlayer(), Vector3.up), Time.deltaTime * _agent.angularSpeed);

            // Check attack
            if(cooldownTimer == 0) {
                if(Vector3.Distance(transform.position, targetPlayer.transform.position) <= attackRange && // Check melee range
                    Vector3.Angle(transform.forward, VectorToPlayer()) < 20f) { // Check player in front of brawler (total 40° cone)
                    currentBehavior = StartCoroutine(Attack());
                    yield break;
                }
            } else { // On cooldown
                cooldownTimer -= Time.deltaTime;
                if(cooldownTimer <= 0)
                    cooldownTimer = 0;
            }
        }
    }

    // -------------------------------------------------------------------------------------------
    // Attacks

    protected override IEnumerator Attack() {
        currentState = EnemyState.Attacking;
        _animator.SetTrigger("Attack");

        while(!attackDone) {
            yield return null;
        }

        cooldownTimer = _cooldown;
        currentBehavior = StartCoroutine(AggressiveMove());
    }

    // -------------------------------------------------------------------------------------------
    // Waypoint Management

    /// <summary>
    /// Gets all waypoints in the container object, and adds them to the list
    /// </summary>
    public void GetWaypoints() {
        waypoints.Clear();
        Transform container = transform.Find("Waypoints");
        foreach(Transform child in container)
            waypoints.Add(child.gameObject);
    }

    /// <summary>
    /// Adds a waypoint to the list and container object
    /// </summary>
    public void AddWaypoint() {
        GetWaypoints();

        Transform container = transform.Find("Waypoints");
        GameObject waypoint = new GameObject();
        waypoint.transform.parent = container;
        waypoint.transform.localPosition = Vector3.zero;
        waypoints.Add(waypoint);
        waypoint.name = "Waypoint " + waypoints.Count;
    }

    /// <summary>
    /// Deletes and removes all waypoints
    /// </summary>
    public void ClearWaypoints() {
        GetWaypoints();

        foreach(GameObject child in waypoints)
            DestroyImmediate(child);
        waypoints.Clear();
    }

    // -------------------------------------------------------------------------------------------

    /// <summary>
    /// Plays swat SFX - called in the animator
    /// </summary>
    public void PlayAttackSound() {
        _brawlerFX.OnHit.Invoke();
    }
}
