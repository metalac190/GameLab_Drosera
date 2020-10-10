using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public abstract class EnemyBase : EntityBase {

    public enum EnemyState { Dead, Passive, Stunned, Aggressive, Attacking };

    [Header("Enemy State")]
    [SerializeField] protected bool aggressive;
    [SerializeField] protected bool hyperseed;
    [SerializeField] protected EnemyState currentState;
    public UnityEvent TurnAggressive, TurnAggressiveHyperseed;
    protected Coroutine currentBehavior;

    [Header("Behavior Variables")]
    [SerializeField] protected float idleWanderRange;
    [SerializeField] protected int spawnRoom;
    [SerializeField] protected GameObject targetPlayer;
    protected Vector3 spawnPosition;
    protected Vector3 targetPosition; // Navigation target position
    protected NavMeshAgent _agent;

    [Header("Other")]
    [SerializeField] protected float stoppingDistance; // Distance to stop away from player when attacking
    [SerializeField] protected float healRate; // Rate = health / 1 second
    protected bool isHealing;
    protected float hyperseedHealthMultiplier = 0.7f;
    protected float hyperseedDamageMultiplier = 1.2f;
    protected float cooldownTimer; // Timer for attack cooldowns
    [HideInInspector] public bool attackDone;

    [System.Serializable]
    public class EnemyFX {
        [Header("VFX")]
        public GameObject burrow; // TODO

        [Header("SFX")]
        public UnityEvent Alerted;
        public UnityEvent IdleState, AlertState;
        public UnityEvent DamageTaken, Death;
    }
    [Header("Enemy SFX & VFX")] [SerializeField] protected EnemyFX _enemyFX;

    // -------------------------------------------------------------------------------------------

    protected override void Awake() {
        base.Awake();
        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = _moveSpeed;

        spawnPosition = transform.position;
    }

    protected override void Start() {
        base.Start();

        // Add turn aggressive listeners
        TurnAggressive.AddListener(() => {
            TurnAggressiveWrapper(false);
        });
        TurnAggressiveHyperseed.AddListener(() => {
            TurnAggressiveWrapper(true);
        });
        // Aggro scurriers when damage is taken & play damaged SFX
        OnTakeDamage.AddListener(() => {
            _enemyFX.DamageTaken.Invoke();
            GetComponentInParent<EnemyGroup>()?.OnEnemyDamage.Invoke();
        });
        // Death Event
        OnDeath.AddListener(() => {
            StopCoroutine(currentBehavior);
            currentBehavior = StartCoroutine(Die());
        });

        if(currentState == EnemyState.Aggressive) {
            currentBehavior = StartCoroutine(Idle());
            TurnAggressive.Invoke();
        } else
            currentBehavior = StartCoroutine(Idle());
    }

    // -------------------------------------------------------------------------------------------
    // Aggressive

    /// <summary>
    /// Non-IEnumerator wrapper function of TurnAggressive
    /// </summary>
    /// <param name="hyperseed">Whether to run the hyperseed variant of TurnAggressive</param>
    public void TurnAggressiveWrapper(bool hyperseed = false) {
        // Don't restart aggressive behavior if already aggressive/attacking, UNLESS hyperseed is grabbed
        if(currentState < EnemyState.Aggressive || (hyperseed && !this.hyperseed)) {
            currentState = EnemyState.Aggressive;
            StopCoroutine(currentBehavior);
            currentBehavior = StartCoroutine(TurnAggressiveFunction(hyperseed));
        }
    }

    /// <summary>
    /// Turns the enemy aggressive when called.
    /// </summary>
    /// <param name="hyperseed">Whether to run the hyperseed variant of TurnAggressive</param>
    protected virtual IEnumerator TurnAggressiveFunction(bool hyperseed = false) {
        // First time aggressive
        if(!aggressive) {
            // TODO - Turn whole group of enemies aggressive

            // Stop in place
            _agent.SetDestination(transform.position);

            // TODO - Turn aggressive animation

            _enemyFX.Alerted.Invoke();
        }

        // First time Hyperseed
        if(hyperseed && !this.hyperseed) {
            this.hyperseed = true;
            _health *= hyperseedHealthMultiplier;
            _maxHealth *= hyperseedHealthMultiplier;

            Hitbox[] hitboxes = GetComponentsInChildren<Hitbox>(true);
            foreach(Hitbox hitbox in hitboxes) {
                hitbox.baseDamage *= hyperseedDamageMultiplier;
                hitbox.damage *= hyperseedDamageMultiplier;
            }
        }

        // Set stats
        aggressive = true;
        isHealing = false;

        // Change behavior
        StopCoroutine(currentBehavior);
        currentBehavior = StartCoroutine(AggressiveMove());
        yield return null;
    }

    // -------------------------------------------------------------------------------------------
    // Targetting

    /// <summary>
    /// Determines which player the enemy should target
    /// </summary>
    protected virtual void FindTarget() {
        // TODO - check player room
        targetPlayer = PlayerBase.instance?.gameObject;
    }

    /// <summary>
    /// Returns the vector towards the to targetted player. Returns Vector3.zero if no player is targetted
    /// </summary>
    protected Vector3 VectorToPlayer() {
        if(targetPlayer == null)
            return Vector3.zero;

        Vector3 vector = targetPlayer.transform.position - transform.position;
        vector.y = 0;
        return vector;
    }

    // -------------------------------------------------------------------------------------------
    // Behavior Coroutines - Main

    /// <summary>
    /// Idle function of the enemy (either non-aggressive or no available targets)
    /// </summary>
    /// <param name="regen">Whether the enemy should be regenerating health.</param>
    protected abstract IEnumerator Idle(bool regen = false);

    /// <summary>
    /// Move function while the enemy is aggressive and has a target player
    /// </summary>
    /// <returns></returns>
    protected abstract IEnumerator AggressiveMove();

    /// <summary>
    /// Attack function of the enemy
    /// </summary>
    protected abstract IEnumerator Attack();

    /// <summary>
    /// Death function of the enemy
    /// </summary>
    protected virtual IEnumerator Die() {
        _enemyFX.Death.Invoke();
        Destroy(gameObject);
        yield return null;
    }

    // -------------------------------------------------------------------------------------------
    // Behavior Coroutines - Other

    /// <summary>
    /// Checks whether the aggressive condition for the enemy is true
    /// </summary>
    protected virtual void CheckAggression() { }

    /// <summary>
    /// Heals the enemy over time, at a total rate of healRate / 1 sec, healing once every 0.1 seconds
    /// </summary>
    protected IEnumerator Regenerate() {
        while(isHealing) {
            _health += healRate * 0.1f;

            if(_health >= _maxHealth) { // Reached max health
                _health = _maxHealth;
                isHealing = false;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    // -------------------------------------------------------------------------------------------
    // Behavior Coroutines - Control

    /// <summary>
    /// Returns the enemy to its proper state (such as after being stunned or after a player re-enters a room)
    /// </summary>
    public virtual void ResetEnemy() {
        _agent.autoBraking = true;
        _agent.speed = _moveSpeed;

        if(currentState == EnemyState.Passive && !aggressive) // Don't re-start idle behavior if not aggressive
            return;
        StopCoroutine(currentBehavior);

        if(aggressive)
            currentBehavior = StartCoroutine(AggressiveMove());
        else
            currentBehavior = StartCoroutine(Idle(true));
    }

    /// <summary>
    /// Forces the enemy into its idle state (but stays aggressive if already so)
    /// </summary>
    public virtual void ForceIdle() {
        StopCoroutine(currentBehavior);
        currentBehavior = StartCoroutine(Idle(true));
    }

}
