using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Hitbox : MonoBehaviour {

    public enum HitboxTarget { Player, Enemy, Neutral };

    public float baseDamage, damage;
    public bool canDamageWalls;
    [SerializeField] protected HitboxTarget hitboxTarget;
    [SerializeField] GameObject hitVFX;
    [SerializeField] float hitVFXDuration;

    protected List<GameObject> hitTargets = new List<GameObject>();

    public UnityEvent OnHit;

    private void Awake() {
        damage = baseDamage;
    }

    protected virtual void OnTriggerEnter(Collider other) {
        // Check if target has already been hit - avoid double hitting
        if(hitTargets.Contains(other.gameObject))
            return;

        // Check target
        if((other.gameObject.layer == LayerMask.NameToLayer("Player") && CanHitPlayer()) || // Hit player
            (other.gameObject.layer == LayerMask.NameToLayer("Enemy") && CanHitEnemy()) || // Hit enemy
            (other.gameObject.layer == LayerMask.NameToLayer("BreakableWall") && canDamageWalls)) { // Hit wall

            // Deal damage
            OnHit.Invoke();
            other.GetComponentInParent<EntityBase>()?.TakeDamage(damage);
            hitTargets.Add(other.gameObject);

            //Spawn VFX if hit player
            if ((other.gameObject.layer == LayerMask.NameToLayer("Player") && CanHitPlayer()) && hitVFX != null && VFXSpawner.vfx != null)
            {
                GameObject vfx = VFXSpawner.vfx.SpawnVFX(hitVFX, hitVFXDuration, other.transform.position, transform.rotation);
            }
        }
    }

    protected void OnDisable() {
        ResetHitbox();
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
        damage = baseDamage;
    }

    //Mainly for when an enemy hits the player
    public void SpawnHitVFX()
    {
        if (hitVFX != null && VFXSpawner.vfx != null)
        {
            GameObject vfx = VFXSpawner.vfx.SpawnVFX(hitVFX, hitVFXDuration, transform.position, transform.rotation);
        }
    }

}
