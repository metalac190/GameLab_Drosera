using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Scurrier : EnemyBase {

    [Header("Scurrier Specific")]
    [SerializeField] private float aggressiveRange; // Range at which Scurrier turns aggressive
    [SerializeField] private float swatRange; // Range at which Scurrier will swat instead of gore (charge)
    [SerializeField] private float cooldownGore; // Cooldown timer for attempting another gore (charge) attack
    private float cooldownTimerGore; // Timer for gore (charge) attack cooldowns

    // -------------------------------------------------------------------------------------------



    // -------------------------------------------------------------------------------------------

    protected override IEnumerator Idle(bool regen) {
        _agent.stoppingDistance = 0f;
        if(regen) {
            isHealing = true;
            StartCoroutine(Regenerate());
        }
        _agent.SetDestination(transform.position);
        yield return new WaitForSeconds(1f);

        Vector3 forward;
        while(true) {
            // Get target position
            targetPosition = spawnPosition + (new Vector3(Random.Range(-idleWanderRange, idleWanderRange), 0, Random.Range(-idleWanderRange, idleWanderRange)));
            forward = targetPosition - transform.position;

            // Turn to look towards position over 1 sec, then wait 0.5 sec
            for(float i = 0; i < 1; i += Time.deltaTime) {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(forward, Vector3.up), Time.deltaTime * 180f);
                yield return null;
            }
            yield return new WaitForSeconds(0.5f);

            // Move towards position
            _agent.SetDestination(targetPosition);

            // Wait at position
            yield return new WaitForSeconds(3f);
        }
    }

    protected override IEnumerator AggressiveMove() {
        _agent.stoppingDistance = stoppingDistance;

        // No target player - exit
        FindTarget();
        if(targetPlayer == null) {
            ResetEnemy();
            yield break;
        }

        while(true) {
            yield return null;

            // Move towards player
            _agent.SetDestination(targetPlayer.transform.position);

            // Swat (Melee) Attack
            if(cooldownTimer == 0) { // check cooldown
                if(Vector3.Distance(transform.position, targetPlayer.transform.position) <= swatRange) { // check melee range
                    currentBehavior = StartCoroutine(AttackSwat());
                    yield break;
                }
            } else { // On cooldown
                cooldownTimer += Time.deltaTime;
                if(cooldownTimer >= _cooldown)
                    cooldownTimer = 0;
            }

            // Gore (Charge) Attack
            if(cooldownTimerGore == 0) { // check cooldown
                if(true /* TODO - raycast to player */) { // raycast
                    currentBehavior = StartCoroutine(AttackGore());
                    yield break;
                }
            } else { // On cooldown
                cooldownTimerGore += Time.deltaTime;
                if(cooldownTimerGore >= cooldownGore)
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
        yield return null;
    }

    /// <summary>
    /// Function for if gore finishes successfully, and Scurrier starts skidding
    /// </summary>
    private IEnumerator GoreSkid() {
        yield return null;

        currentBehavior = StartCoroutine(AggressiveMove());
    }

    /// <summary>
    /// Function for if Scurrier crashes into a wall during gore
    /// </summary>
    private IEnumerator GoreCrash() {
        yield return null;

        currentBehavior = StartCoroutine(AggressiveMove());
    }

    /// <summary>
    /// Swat attack
    /// </summary>
    private IEnumerator AttackSwat() {
        yield return null;

        currentBehavior = StartCoroutine(AggressiveMove());
    }

}
