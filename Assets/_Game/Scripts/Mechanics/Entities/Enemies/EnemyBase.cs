using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using Random = UnityEngine.Random;

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
    protected float hyperseedSpeedMultiplier = 1.4f;
    protected float cooldownTimer; // Timer for attack cooldowns
    [HideInInspector] public bool attackDone, aggroAnimDone;

    [Header("Modifiers")]
    [SerializeField] protected float jungleHealthMultiplier = 1f;
    [SerializeField] protected float jungleDamageMultiplier = 1f;
    [SerializeField] protected float jungleSpeedMultiplier = 1f;
    [SerializeField] protected float desertHealthMultiplier = 1f;
    [SerializeField] protected float desertDamageMultiplier = 1f;
    [SerializeField] protected float desertSpeedMultiplier = 1f;
    [SerializeField] protected string currentBiomeRegistered = "None";
    [SerializeField] protected bool enemySlowed = false;

    [System.Serializable]
    public class EnemyFX {
        [Header("VFX")]
        public GameObject burrow; // No longer used
        public GameObject deathEffect;

        [Header("SFX")]
        public UnityEvent Alerted;
        public UnityEvent IdleState, AlertState;
        public UnityEvent DamageTaken, Death;
        public GameObject deathSoundObj;
    }
    [Header("Enemy SFX & VFX")] [SerializeField] protected EnemyFX _enemyFX;

    // -------------------------------------------------------------------------------------------

    protected override void Awake() {
        base.Awake();
        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = _moveSpeed;

        aggroAnimDone = true;
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
        // Aggro enemies when damage is taken & play damaged SFX
        OnTakeDamage.AddListener(() => {
            // If enemy damaged sound is playing, don't repeat
            if(EnemySoundSingleton.instance.DamageTakenSoundActive == false) {
                EnemySoundSingleton.instance.DamageTakenSoundActive = true;
                _enemyFX.DamageTaken.Invoke();
            }
            // Aggro group
            GetComponentInParent<EnemyGroup>()?.OnEnemyDamage.Invoke();
        });
        // Death Event
        OnDeath.AddListener(() => {
            if(currentBehavior != null)
                StopCoroutine(currentBehavior);
            currentBehavior = StartCoroutine(Die());
        });

        spawnPosition = transform.position;

        // Start behavior
        if(currentState == EnemyState.Aggressive) {
            currentBehavior = StartCoroutine(Idle());
            TurnAggressive.Invoke();
        } else {
            currentBehavior = StartCoroutine(Idle());
        }

        StartCoroutine(CheckBehavior());
        StartCoroutine(EnemyModifications());

    }

    protected virtual void LateUpdate() {
        // Control animations
        if(_agent.velocity.magnitude > 0.5f)
            _animator.SetBool("Moving", true);
        else
            _animator.SetBool("Moving", false);
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
            if(currentBehavior != null)
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
            // Stop in place
            _agent.SetDestination(transform.position);

            // Turn aggressive animation
            aggroAnimDone = false;
            yield return new WaitForSeconds(Random.Range(0f, 0.3f));
            _animator.SetTrigger("Alerted");

            _enemyFX.Alerted.Invoke();
        }

        // First time Hyperseed
        if(hyperseed && !this.hyperseed) {
            this.hyperseed = true;
            _health *= hyperseedHealthMultiplier;
            _maxHealth *= hyperseedHealthMultiplier;

            _moveSpeed *= hyperseedSpeedMultiplier;
            _agent.speed *= hyperseedSpeedMultiplier;

            Hitbox[] hitboxes = GetComponentsInChildren<Hitbox>(true);
            foreach(Hitbox hitbox in hitboxes) {
                hitbox.baseDamage *= hyperseedDamageMultiplier;
                hitbox.damage *= hyperseedDamageMultiplier;
            }
        }

        // Set stats
        aggressive = true;
        isHealing = false;

        // Wait for aggro animation to finish
        while(!aggroAnimDone)
            yield return null;

        // Change behavior
        currentBehavior = StartCoroutine(AggressiveMove());
        yield return null;
    }

    // -------------------------------------------------------------------------------------------
    // Targetting

    /// <summary>
    /// Determines which player the enemy should target
    /// </summary>
    protected virtual void FindTarget() {
        if(hyperseed || (PlayerInRoom() && EnemyInRoom()))
            targetPlayer = PlayerBase.instance.gameObject;
        else
            targetPlayer = null;
    }

    /// <summary>
    /// Checks if the player is in the same room as the enemy
    /// </summary>
    protected virtual bool PlayerInRoom() {
        return GetComponentInParent<Room>() == PlayerBase.instance.currentRoom;
    }

    /// <summary>
    /// Checks if the enemy is in its parent room
    /// </summary>
    protected virtual bool EnemyInRoom() {
        Physics.Raycast(PlayerBase.instance.transform.position + Vector3.up, Vector3.down, out RaycastHit hit, 1.5f, LayerMask.GetMask("Terrain"));
        try {
            if(hit.transform.GetComponentInParent<Room>() == GetComponentInParent<Room>())
                return true;
        } catch {
            Debug.Log(gameObject.name + " in " + GetComponentInParent<Room>().name + ": Error in detecting if enemy is in the right room");
        }
        return false;
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
        //_enemyFX.deathSoundObj.GetComponent<AudioScript>().audioPrefabParent = PlayerBase.instance.gameObject;
        _enemyFX.deathSoundObj.GetComponent<AudioScript>().audioPrefabParent = GetComponentInParent<Room>().gameObject;
        _enemyFX.Death.Invoke();
        VFXSpawner.vfx.SpawnVFX(_enemyFX.deathEffect, 1f, transform.position, transform.rotation); //Putting this here for now. Bill feel free to set this up how you want to later.
        Destroy(gameObject);
        yield return null;
    }
    
    /// <summary>
    /// Periodically checks behavior, and resets if none is active
    /// </summary>
    protected virtual IEnumerator CheckBehavior() {
        while(gameObject.activeSelf) {
            yield return new WaitForSeconds(0.25f);
            if(currentBehavior == null) {
                try { Debug.Log(gameObject.name + " in " + GetComponentInParent<Room>().name + " encountered an error in its behavior."); } catch { }
                ResetEnemy();
            }
        }
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
        if(currentBehavior != null)
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
        if(currentState == EnemyState.Passive) // Don't restart idle behavior
            return;

        if(currentBehavior != null)
            StopCoroutine(currentBehavior);
        currentBehavior = StartCoroutine(Idle(true));
    }

    /// <summary>
    /// Changes enemy stas depending on current biome
    /// </summary>
    protected IEnumerator EnemyModifications() {
        yield return new WaitForSeconds(0.1f);
        //Change Enemy Stats based on biome
        if (GameManager.Instance != null)       //check to be sure instance exists
        {
            if (GameManager.Instance.CurrentBiome == DroseraGlobalEnums.Biome.Jungle)
            {
                currentBiomeRegistered = "Jungle";
                Hitbox[] hitboxes = GetComponentsInChildren<Hitbox>(true);
                foreach (Hitbox hitbox in hitboxes)
                {
                    hitbox.baseDamage *= jungleDamageMultiplier;
                    hitbox.damage *= jungleDamageMultiplier;
                }
                _health *= jungleHealthMultiplier;
                _maxHealth *= jungleHealthMultiplier;
                _moveSpeed *= jungleSpeedMultiplier;
            }
            else if (GameManager.Instance.CurrentBiome == DroseraGlobalEnums.Biome.Desert)
            {
                currentBiomeRegistered = "Desert";
                Hitbox[] hitboxes = GetComponentsInChildren<Hitbox>(true);
                foreach (Hitbox hitbox in hitboxes)
                {
                    hitbox.baseDamage *= desertDamageMultiplier;
                    hitbox.damage *= desertDamageMultiplier;
                }
                _health *= desertHealthMultiplier;
                _maxHealth *= desertHealthMultiplier;
                _moveSpeed *= desertSpeedMultiplier;
            }
            else
            {
                currentBiomeRegistered = "Nothing Registered";
            }
        }//end of biome modifier code
        yield return null;
    }

    // -------------------------------------------------------------------------------------------

    /// <summary>
    /// Plays attack SFX - called in the animator
    /// </summary>
    public abstract void PlayAttackSound();

    /// <summary>
    /// Coroutine that can be called on enemy being hit by player alt fire.
    /// enemySlowModifier --> should be a number between 0 and 1
    /// enemySlowDuration--> float duration for enemy slow
    /// </summary>
    public IEnumerator AltFireEnemySlowed(float enemySlowModifier, float enemySlowDuration)
    {
        if (enemySlowed == false)   //prevents modifiers from stacking
        {
            float originalSpeed = _moveSpeed;
            enemySlowed = true;
            _moveSpeed *= enemySlowModifier;
            _agent.speed = _moveSpeed;
            yield return new WaitForSeconds(enemySlowDuration);
            _moveSpeed = originalSpeed;
            _agent.speed = _moveSpeed;
            enemySlowed = false;
        }
    }
}
