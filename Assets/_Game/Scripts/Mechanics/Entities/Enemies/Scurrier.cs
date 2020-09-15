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
    [SerializeField] private float goreDistance; // Distance scurrier will travel during gore
    private float cooldownTimerGore; // Timer for gore (charge) attack cooldowns

    // -------------------------------------------------------------------------------------------



    // -------------------------------------------------------------------------------------------

    protected override IEnumerator Idle(bool regen) {
        _agent.stoppingDistance = 0f;
        _agent.SetDestination(transform.position);
        if(regen) {
            isHealing = true;
            StartCoroutine(Regenerate());
        }
        currentState = EnemyState.Passive;
        yield return new WaitForSeconds(1f);

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
        _agent.SetDestination(transform.position);
        Debug.Log("Gore Attack");

        // TODO - windup animation
        // Turn to look towards position over 1 sec
        Vector3 forward = targetPlayer.transform.position - transform.position;
        for(float i = 0; i < 0.5; i += Time.deltaTime) {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(forward, Vector3.up), Time.deltaTime * 360f);
            yield return null;
        }

        // Charge
        yield return new WaitForSeconds(1f);
        _agent.speed *= goreSpeedMultiplier;
        _agent.destination = targetPlayer.transform.position;

        float timeout = 0; // Gore will timout if stuck on edge of NavMesh with no wall
        while(_agent.remainingDistance > 0.2) { // Last 20% of distance is skidding
            // TODO - check wall collision

            yield return null;

            // Check timeout
            timeout += Time.deltaTime;
            if(timeout > 5f) {
                Debug.LogError(gameObject.name + "'s gore attack timed out - likely stuck at edge of a NavMesh with no wall.");
                break;
            }
        }

        // Did not crash - begin skidding
        currentBehavior = StartCoroutine(GoreSkid());
    }

    /// <summary>
    /// Function for if gore finishes successfully, and Scurrier starts skidding
    /// </summary>
    private IEnumerator GoreSkid() {
        yield return null;

        cooldownTimerGore = cooldownGore;
        currentBehavior = StartCoroutine(AggressiveMove());
    }

    /// <summary>
    /// Function for if Scurrier crashes into a wall during gore
    /// </summary>
    private IEnumerator GoreCrash() {
        yield return null;

        cooldownTimerGore = cooldownGore;
        currentBehavior = StartCoroutine(AggressiveMove());
    }

    /// <summary>
    /// Swat attack
    /// </summary>
    private IEnumerator AttackSwat() {
        Debug.Log("Swat Attack");
        yield return new WaitForSeconds(1f);

        cooldownTimer = _cooldown;
        currentBehavior = StartCoroutine(AggressiveMove());
    }

}
