using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scurrier : EnemyBase {

    [Header("Scurrier Specific")]
    [SerializeField] protected float aggressiveRange; // Range at which Scurrier turns aggressive
    [SerializeField] protected float swatRange; // Range at which Scurrier will swat instead of gore (charge)
    [SerializeField] protected float cooldownGore; // Cooldown timer for attempting another gore (charge) attack

    // -------------------------------------------------------------------------------------------



    // -------------------------------------------------------------------------------------------

    protected override IEnumerator Idle(bool regen) {
        _agent.stoppingDistance = 0f;
        yield return new WaitForSeconds(1f);

        while(true) {
            // Select random position within idle wander range
            targetPosition = spawnPosition + (new Vector3(Random.Range(-idleWanderRange, idleWanderRange), 0, Random.Range(-idleWanderRange, idleWanderRange)));

            // Move towards position
            _agent.SetDestination(targetPosition);

            yield return new WaitForSeconds(5f);
        }
    }

    protected override IEnumerator AggressiveMove() {
        throw new System.NotImplementedException();
    }
    protected override IEnumerator Die() {
        throw new System.NotImplementedException();
    }

    // -------------------------------------------------------------------------------------------
    // Attacks

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
    }

    /// <summary>
    /// Function for if Scurrier crashes into a wall during gore
    /// </summary>
    private IEnumerator GoreCrash() {
        yield return null;
    }

    /// <summary>
    /// Swat attack
    /// </summary>
    private IEnumerator AttackSwat() {
        yield return null;
    }

}
