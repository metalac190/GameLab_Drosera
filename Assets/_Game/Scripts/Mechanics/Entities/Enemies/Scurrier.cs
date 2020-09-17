using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Scurrier : EnemyBase {

    [Header("Scurrier Specific")]
    [SerializeField] private float aggressiveRange; // Range at which Scurrier turns aggressive
    [SerializeField] private float swatRange; // Range at which Scurrier will swat instead of gore (charge)
    [SerializeField] private float cooldownGore; // Cooldown timer for attempting another gore (charge) attack
    [SerializeField] private float goreSpeedMultiplier; // Speed multiplier for gore
    [SerializeField] private float goreSkidDistance; // Distance scurrier will travel during gore skid
    private float cooldownTimerGore; // Timer for gore (charge) attack cooldowns

    // -------------------------------------------------------------------------------------------

    protected override void Start() {
        base.Start();
        _moveSpeed = _agent.speed;
    }

    // -------------------------------------------------------------------------------------------

    protected override IEnumerator Idle(bool regen) {
        _agent.stoppingDistance = 0f;
        _agent.SetDestination(transform.position);

        if(regen) {
            isHealing = true;
            StartCoroutine(Regenerate());
        }
        currentState = EnemyState.Passive;

        // Fix variables from gore (if interrupted)
        _agent.autoBraking = true;
        _agent.speed = _moveSpeed;
        _agent.updatePosition = true;


        Vector3 forward;
        while(true) {
            // Get target position
            targetPosition = spawnPosition + (new Vector3(Random.Range(-idleWanderRange, idleWanderRange), 0, Random.Range(-idleWanderRange, idleWanderRange)));
            forward = targetPosition - transform.position;

            // Turn to look towards position over 1 sec
            for(float i = 0; i < 1; i += Time.deltaTime) {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(forward, Vector3.up), Time.deltaTime * 180f);
                yield return null;

                if(CheckAggression())
                    TurnAggressiveWrapper();
            }

            // Wait 0.5 sec
            for(float i = 0; i < 0.5; i += Time.deltaTime) {
                yield return null;
                if(CheckAggression())
                    TurnAggressiveWrapper();
            }

            // Move towards position
            _agent.SetDestination(targetPosition);

            // Wait at position for 3 sec
            for(float i = 0; i < 3; i += Time.deltaTime) {
                yield return null;
                if(CheckAggression())
                    TurnAggressiveWrapper();
            }
        }
    }

    protected override bool CheckAggression() {
        return false;
    }

    // ------

    protected override IEnumerator AggressiveMove() {
        _agent.stoppingDistance = stoppingDistance;
        currentState = EnemyState.Aggressive;

        // Fix variables from gore (if interrupted)
        _agent.autoBraking = true;
        _agent.speed = _moveSpeed;
        _agent.updatePosition = true;

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

            // Swat (Melee) Attack
            if(cooldownTimer == 0) { // check cooldown
                if(Vector3.Distance(transform.position, targetPlayer.transform.position) <= swatRange) { // check melee range
                    currentBehavior = StartCoroutine(AttackSwat());
                    yield break;
                }
            } else { // On cooldown
                cooldownTimer -= Time.deltaTime;
                if(cooldownTimer <= 0)
                    cooldownTimer = 0;
            }

            // Gore (Charge) Attack
            if(cooldownTimerGore == 0) { // check cooldown
                if(true /* TODO - raycast to player */ && Vector3.Distance(transform.position, _agent.destination) > swatRange * 1.5) { // raycast & check distance
                    currentBehavior = StartCoroutine(AttackGore());
                    yield break;
                }
            } else { // On cooldown
                cooldownTimerGore -= Time.deltaTime;
                if(cooldownTimerGore <= 0)
                    cooldownTimerGore = 0;
            }
        }
    }
    protected override IEnumerator Die() {
        yield return null;
    }

    // -------------------------------------------------------------------------------------------
    // Attacks

    /// <summary>
    /// Irrelevant for Scurrier - use AttackGore or AttackSwat instead
    /// </summary>
    protected override IEnumerator Attack() {
        yield return null;
    }

    /// <summary>
    /// Gore (charge) attack
    /// </summary>
    private IEnumerator AttackGore() {
        currentState = EnemyState.Attacking;
        _agent.stoppingDistance = 0;
        _agent.autoBraking = false;
        _agent.SetDestination(transform.position);
        Debug.Log("Gore Attack - Prep");

        // TODO - windup animation
        // Turn to look towards position over 1 sec
        Vector3 forward = targetPlayer.transform.position - transform.position;
        for(float i = 0; i < 0.5; i += Time.deltaTime) {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(forward, Vector3.up), Time.deltaTime * 360f);
            yield return null;
        }
        yield return new WaitForSeconds(1f);

        // Set vars
        _agent.speed *= goreSpeedMultiplier;
        _agent.SetDestination(targetPlayer.transform.position);
        while(_agent.pathPending)
            yield return null;

        // Charge
        Debug.Log("Gore Attack - Charge");
        float timeout = 0; // Failsafe to prevent infinite gore
        while(_agent.remainingDistance > 1) {
            // TODO - check wall collision

            yield return null;

            // Check timeout
            timeout += Time.deltaTime;
            if(timeout > 5f) {
                Debug.LogError(gameObject.name + "'s gore attack timed out.");
                break;
            }
        }

        // Did not crash - begin skidding
        _agent.speed = _moveSpeed;
        currentBehavior = StartCoroutine(GoreSkid());
        //currentBehavior = StartCoroutine(AggressiveMove());
    }

    /// <summary>
    /// Function for if gore finishes successfully, and Scurrier starts skidding
    /// </summary>
    private IEnumerator GoreSkid() {
        Debug.Log("start skid");
        yield return null;

        // Δx = (v + v_o)t/2 => t = 2(Δx)/(v + v_o) => v = 0, so t = 2(Δx)/v_0
        Vector3 baseVelocity = _agent.velocity;
        float skidTime = 2 * goreSkidDistance / _agent.velocity.magnitude;

        // Set new destination
        _agent.SetDestination(transform.position + (transform.forward * goreSkidDistance));
        _agent.updatePosition = false;
        while(_agent.pathPending)
            yield return null;

        // Skid
        for(float i = 0; i < skidTime; i += Time.deltaTime) {
            _agent.velocity = Vector3.Lerp(baseVelocity, Vector3.zero, 1 - (i / skidTime));

            yield return null;
        }
        _agent.velocity = Vector3.zero;
        yield return new WaitForSeconds(0.25f);

        // Set cooldown & return to movement
        Debug.Log("end skid");
        cooldownTimerGore = cooldownGore;
        currentBehavior = StartCoroutine(AggressiveMove());
    }

    /// <summary>
    /// Function for if Scurrier crashes into a wall during gore
    /// </summary>
    private IEnumerator GoreCrash() {
        _agent.speed = _moveSpeed;

        yield return null;

        cooldownTimerGore = cooldownGore;
        currentBehavior = StartCoroutine(AggressiveMove());
    }

    /// <summary>
    /// Swat attack
    /// </summary>
    private IEnumerator AttackSwat() {
        currentState = EnemyState.Attacking;
        Debug.Log("Swat Attack");
        yield return new WaitForSeconds(1f);

        cooldownTimer = _cooldown;
        currentBehavior = StartCoroutine(AggressiveMove());
    }

}
