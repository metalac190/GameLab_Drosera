using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyBase : EntityBase {

    public enum EnemyState { Dead, Passive, Aggressive, Attacking };

    [Header("Enemy State")]
    [SerializeField] protected bool aggresive;
    [SerializeField] protected bool hyperseed;
    [SerializeField] protected EnemyState currentState;
    protected IEnumerator currentBehavior;

    [Header("Behavior Variables")]
    protected Vector3 spawnPosition;
    [SerializeField] protected float idleWanderRange;
    [SerializeField] protected int spawnRoom;
    [SerializeField] protected GameObject targetPlayer;
    protected Vector3 targetPosition;
    protected NavMeshAgent _agent;

    [Header("Other")]
    [SerializeField] protected float healRate; // Rate = health / 1 second

    // -------------------------------------------------------------------------------------------

    protected override void Awake() {
        base.Awake();
        _agent = GetComponent<NavMeshAgent>();
        spawnPosition = transform.position;

        currentState = EnemyState.Passive;
    }

    protected void Start() {
        currentBehavior = Idle();
        StartCoroutine(currentBehavior);
    }

    // -------------------------------------------------------------------------------------------

    /// <summary>
    /// Non-IEnumerator wrapper function of TurnAggressive
    /// </summary>
    /// <param name="hyperseed">Whether to run the hyperseed variant of TurnAggressive</param>
    public void TurnAggressiveWrapper(bool hyperseed = false) {
        StopCoroutine(currentBehavior);

        if(!hyperseed)
            currentBehavior = TurnAggressive();
        else
            currentBehavior = TurnAggressiveHyperseed();
        StartCoroutine(currentBehavior);
    }

    /// <summary>
    /// Turns the enemy aggressive when called.
    /// </summary>
    public virtual IEnumerator TurnAggressive() {
        aggresive = true;
        currentState = EnemyState.Aggressive;

        // Stop in place
        _agent.SetDestination(transform.position);

        // TODO - Turn aggressive animation
        yield return null;
    }

    /// <summary>
    /// Turns the enemy aggressive when called. Hyperseed variant of TurnAggressive.
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator TurnAggressiveHyperseed() {
        hyperseed = true;
        yield return StartCoroutine(TurnAggressive());
    }

    /// <summary>
    /// Determines which player the enemy should target
    /// </summary>
    protected virtual void FindTarget() {

    }

    // -------------------------------------------------------------------------------------------
    // Behavior Coroutines

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
    protected abstract IEnumerator Die();

    /// <summary>
    /// Returns the enemy to the idle state, but stays aggressive if already aggressive
    /// </summary>
    public virtual void ResetEnemy() {
        StopCoroutine(currentBehavior);

        currentBehavior = Idle(true);
        StartCoroutine(currentBehavior);
    }

}
