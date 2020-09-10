using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Hitbox : MonoBehaviour {

    public enum HitboxTarget { Player, Enemy, Neutral };

    public float damage;
    public bool canDamageWalls;
    [SerializeField] protected HitboxTarget hitboxTarget;

    protected List<GameObject> hitTargets = new List<GameObject>();

    protected virtual void OnTriggerEnter(Collider other) {
        // Check if target has already been hit - avoid double hitting
        if(hitTargets.Contains(other.gameObject))
            return;

        // Check target
        if((other.gameObject.layer == LayerMask.NameToLayer("Player") && CanHitPlayer()) || // Hit player
            (other.gameObject.layer == LayerMask.NameToLayer("Enemy") && CanHitEnemy()) || // Hit enemy
            (other.gameObject.layer == LayerMask.NameToLayer("BreakableWall") && canDamageWalls)) { // Hit wall

            // Deal damage
            other.GetComponent<EntityBase>().TakeDamage(damage);
            hitTargets.Add(other.gameObject);
        }
    }

    /// <summary>
    /// Whether or not the hitbox can damage players
    /// </summary>
    protected bool CanHitPlayer() {
        return hitboxTarget != HitboxTarget.Enemy;
    }

    /// <summary>
    /// Whether or not the hitbox can damage enemies
    /// </summary>
    protected bool CanHitEnemy() {
        return hitboxTarget != HitboxTarget.Player;
    }

    /// <summary>
    /// Resets the hitbox - enabling it to hit all targets again
    /// </summary>
    public void ResetHitbox() {
        hitTargets.Clear();
    }

}
