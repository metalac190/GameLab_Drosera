using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class Scurrier : EnemyBase {

#pragma warning disable 0649 // Disable "Field is never assigned" warning for SerializeField

    [Header("Scurrier Specific")]
    [SerializeField] private float aggressiveRange; // Range at which Scurrier turns aggressive
    [SerializeField] private float swatRange; // Range at which Scurrier will swat instead of gore (charge)
    [SerializeField] private Vector2 goreRange; // Min-max range for gore attack
    [SerializeField] private float cooldownGore; // Cooldown timer for attempting another gore (charge) attack
    [SerializeField] private float goreSpeedMultiplier; // Speed multiplier for gore
    [SerializeField] private float goreSkidDistance; // Distance scurrier will travel during gore skid
    private float cooldownTimerGore; // Timer for gore (charge) attack cooldowns

    [Header("Hitboxes")]
    [SerializeField] private Hitbox swatHitbox;
    [SerializeField] private Hitbox goreHitbox;

    private ScurrierCrashDetector crashDetector;
    private bool inGore, attemptingGore;

    [System.Serializable]
    public class FX {
        [Header("VFX")]
        public GameObject shockwave; // ??? - not used anymore?
        public GameObject goreTrail, goreImpact;

        [Header("SFX")]
        public UnityEvent GoreWindUp;
        public UnityEvent GoreAttack, GoreImpact, GoreSkid;
        public UnityEvent SwatAttack;
        public UnityEvent NearbyAlerted;
    }
    [Header("Scurrier VFX & SFX")] [SerializeField] private FX _scurrierFX;

#pragma warning disable 0649

    // -------------------------------------------------------------------------------------------

    protected override void Awake() {
        base.Awake();
        crashDetector = GetComponentInChildren<ScurrierCrashDetector>(true);
        crashDetector.gameObject.SetActive(false);
        inGore = false;
        attemptingGore = false;

        // Random initial cooldown timer for gore
        cooldownTimerGore = Random.Range(0, 2.5f);
    }

    protected override void Start() {
        base.Start();
        goreHitbox.OnHit.AddListener(() => {
            _scurrierFX.GoreImpact.Invoke();
            SpawnGoreHitVFX();
        });
        GetComponentInParent<EnemyGroup>()?.OnEnemyDamage.AddListener(() => {
            _scurrierFX.NearbyAlerted.Invoke();
        });
    }

    // TEMPORARY - PLACEHOLDER WHILE WAITING ON SCURRIER TURN AGGRO ANIMATIONS

    protected override IEnumerator TurnAggressiveFunction(bool hyperseed = false) {
        // First time aggressive
        if(!aggressive) {
            // Stop in place
            _agent.SetDestination(transform.position);

            // Turn aggressive animation
            /*aggroAnimDone = false;
            yield return new WaitForSeconds(Random.Range(0f, 0.3f));
            _animator.SetTrigger("Alerted");*/

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

        // Wait for aggro animation to finish
        /*while(!aggroAnimDone)
            yield return null;*/
        
        // Change behavior
        currentBehavior = StartCoroutine(AggressiveMove());
        yield return null;
    }

    // -------------------------------------------------------------------------------------------
    // Behavior Coroutines - Main

    protected override IEnumerator Idle(bool regen = false) {
        _agent.stoppingDistance = 0f;
        _agent.SetDestination(transform.position);
        yield return new WaitForSeconds(0.5f);

        if(regen) {
            isHealing = true;
            StartCoroutine(Regenerate());
        }
        currentState = EnemyState.Passive;

        GoreReset();

        bool firstIdle = true; // On first time, if away from spawn point, go back to spawn point
        Vector3 forward;
        while(true) {
            // Get target position
            targetPosition = GetIdleDestination(firstIdle);
            firstIdle = false;
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

            while(_agent.remainingDistance > 0.2f) {
                yield return null;
                CheckAggression();
            }

            // Wait at position for 1.5 to 2.5 sec
            for(float i = 0; i < Random.Range(1.5f, 2.5f); i += Time.deltaTime) {
                yield return null;
                CheckAggression();
            }

            // Play idle SFX
            _enemyFX.IdleState.Invoke();
        }
    }

    /// <summary>
    /// Determines the Scurrier's idle destination - stops it from attempting to go around walls or staying in the same spot
    /// </summary>
    /// <returns>Scurrier's next idle destination</returns>
    private Vector3 GetIdleDestination(bool firstIdle) {
        if(firstIdle)
            return spawnPosition;

        Vector3 destination = spawnPosition, forward;
        Vector3 heightOffset = new Vector3(0, -0.75f, 0); // Offset for detecting ground-level obstacles
        RaycastHit hit;

        // If tried too many times, return to spawn
        for(int i = 0; i < 5; i++) {
            destination = spawnPosition + (new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized * idleWanderRange);
            forward = destination - transform.position;
            forward.y = 0;
            //Debug.DrawRay(transform.position, forward, Color.red, 5f);

            // If point is floating over void, try again
            if(!Physics.Raycast(destination, Vector3.down, 2f, LayerMask.GetMask("Terrain")))
                continue;

            // Check wall between destination
            if(Physics.Raycast(transform.position + heightOffset, forward, out hit, forward.magnitude, LayerMask.GetMask("Terrain"))) {
                //Debug.Log(hit.point);
                destination = hit.point;
            }

            // If path is long enough, return (if path too short, try again)
            if((destination - transform.position).magnitude > 1f)
                break;
        }

        return destination;
    }

    protected override void CheckAggression() {
        if(hyperseed || // Hyperseed guarantees finding target
            (Vector3.Distance(PlayerBase.instance != null ? PlayerBase.instance.transform.position : Vector3.zero, transform.position) < aggressiveRange // Check distance
            && PlayerInRoom() // Check in same room
            && EnemyInRoom() // Check if in own room
            && !Physics.Raycast(transform.position, PlayerBase.instance.transform.position, VectorToPlayer().magnitude, LayerMask.GetMask("Terrain")))) { // Check wall obstruction
                TurnAggressive.Invoke();
        }
    }

    // ------

    protected override IEnumerator AggressiveMove() {
        _agent.stoppingDistance = stoppingDistance;
        attackDone = false;
        currentState = EnemyState.Aggressive;

        GoreReset();

        // Play aggro SFX
        // TODO - make looping
        _enemyFX.AlertState.Invoke();

        yield return null;
        FindTarget();
        while(true) {
            yield return null;

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

            // Swat (Melee) Attack
            if(cooldownTimer == 0) { // Check cooldown
                if(Vector3.Distance(transform.position, targetPlayer.transform.position) <= swatRange && // Check melee range
                    Vector3.Angle(transform.forward, VectorToPlayer()) < 15f) { // Check player in front of scurrier (total 30° cone)
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
                if(!Physics.Raycast(transform.position, VectorToPlayer(), // Raycast (contains max distance)
                    Mathf.Clamp(VectorToPlayer().magnitude, 0, goreRange.y), LayerMask.GetMask("Terrain")) &&
                    Vector3.Distance(transform.position, _agent.destination) > goreRange.x) { // Check min distance

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
    
    // -------------------------------------------------------------------------------------------
    // Attacks

    /// <summary>
    /// Irrelevant for Scurrier - use AttackGore or AttackSwat instead
    /// </summary>
    protected override IEnumerator Attack() {
        Debug.LogWarning(gameObject.name + " called irrelevant Coroutine \"Attack\" - use \"AttackGore\" or \"AttackSwat\" instead.");
        currentBehavior = StartCoroutine(AttackSwat());
        yield return null;
    }

    // -----

    /// <summary>
    /// Gore (charge) attack
    /// </summary>
    private IEnumerator AttackGore() {
        if(attemptingGore) {
            yield break;
        } else {
            attemptingGore = true;
        }

        currentState = EnemyState.Attacking;
        _agent.stoppingDistance = 0;
        _agent.autoBraking = false;
        _agent.isStopped = true;
        attackDone = false;
        _agent.velocity = Vector3.zero;

        // TODO - windup animation???

        // Turn to look towards position over 0.5 sec
        Vector3 forward = Vector3.zero;
        Vector3 initialTargetPos = PlayerBase.instance.transform.position;
        forward.y = 0;

        _scurrierFX.GoreWindUp.Invoke();
        for(float i = 0; i < 0.5; i += Time.deltaTime) {
            // Check if player left line of sight or left max range - exit
            if(Physics.Raycast(transform.position, VectorToPlayer(), goreRange.y, LayerMask.GetMask("Terrain")) || // Raycast
                Vector3.Distance(transform.position, initialTargetPos) >= goreRange.y) { // Check distance - out of range

                cooldownTimerGore = cooldownGore * Random.Range(0.4f, 0.6f);
                currentBehavior = StartCoroutine(AggressiveMove());
                goto endFlag;
                //yield break;
            }

            initialTargetPos = PlayerBase.instance.transform.position;
            forward = (initialTargetPos - transform.position).normalized;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(forward, Vector3.up), Time.deltaTime * 360f);
            yield return null;
        }

        // Set vars
        _agent.speed = _moveSpeed * goreSpeedMultiplier;
        _agent.SetDestination(initialTargetPos);
        while(_agent.pathPending)
            yield return null;
        //Debug.DrawRay(transform.position, _agent.destination + (forward * goreSkidDistance) - transform.position, Color.red, 2);
        //Debug.DrawRay(transform.position, _agent.destination - transform.position, Color.green, 2);

        // Can no longer interrupt via exiting room
        inGore = true;

        // TODO - begin charge animation

        // Charge
        _scurrierFX.GoreAttack.Invoke();
        crashDetector.gameObject.SetActive(true);
        _scurrierFX.goreTrail.SetActive(true);
        //_animator.SetBool("Gore", true);
        _animator.SetTrigger("Gore Start");
        GetComponentInChildren<EnemyAnimationController>().EnableHitbox(1);
        _agent.isStopped = false;
        float timeout = 0; // Failsafe to prevent infinite gore
        while(_agent.remainingDistance > 1) {
            // Check wall collision
            if(crashDetector.crash) {
                currentBehavior = StartCoroutine(GoreCrash());
                yield break;
            }

            yield return null;

            // Premature end (player exit room OR scurrier left its parent room)
            if(!hyperseed && (!PlayerInRoom() || !EnemyInRoom()))
                break;

            // Check timeout
            timeout += Time.deltaTime;
            if(timeout > 3f) {
                Debug.LogError(gameObject.name + "'s gore attack timed out.");
                break;
            }
        }

        // Did not crash - begin skidding
        _animator.SetTrigger("Gore Finish");
        currentBehavior = StartCoroutine(GoreSkid());

        endFlag:
        yield return null;
    }

    /// <summary>
    /// Function for if gore finishes successfully, and Scurrier starts skidding
    /// </summary>
    private IEnumerator GoreSkid() {
        // If stalled/stopped, exit
        if(_agent.velocity.magnitude <= 0.05f) {
            Debug.Log(gameObject.name + " got stalled during gore.");
            goto endGoreSkid;
        }

        goreHitbox.damage /= 2;

        // TODO - start skid animation
        _scurrierFX.GoreSkid.Invoke();

        // Δx = (v + v_o)t/2 => t = 2(Δx)/(v + v_o) => v = 0, so t = 2(Δx)/v_0
        Vector3 baseVelocity = _agent.velocity;
        float skidTime = Mathf.Clamp(2 * goreSkidDistance / baseVelocity.magnitude, 0.5f, 5f);

        _agent.updatePosition = false;
        _agent.ResetPath();

        // Skid
        for(float i = 0; i < skidTime; i += Time.deltaTime) {
            // Check wall collision
            if(crashDetector.crash) {
                currentBehavior = StartCoroutine(GoreCrash());
                yield break;
            }

            // Slow velocity
            _agent.velocity = Vector3.Lerp(baseVelocity, Vector3.zero, i / skidTime);
            transform.position = _agent.nextPosition;

            yield return null;
        }
        _agent.velocity = Vector3.zero;
        GetComponentInChildren<EnemyAnimationController>().DisableHitbox(1);
        yield return new WaitForSeconds(0.5f);

        endGoreSkid:
        // Set cooldown & return to movement
        inGore = false;
        _animator.SetBool("Gore", false);
        cooldownTimer = _cooldown;
        cooldownTimerGore = cooldownGore;
        currentBehavior = StartCoroutine(AggressiveMove());
    }

    /// <summary>
    /// Function for if Scurrier crashes into a wall during gore
    /// </summary>
    private IEnumerator GoreCrash() {
        Debug.Log("Gore crashed into wall");
        _agent.isStopped = true;
        _agent.velocity = Vector3.zero;
        GetComponentInChildren<EnemyAnimationController>().DisableHitbox(1);

        SpawnGoreHitVFX();

        yield return new WaitForSeconds(1f);

        // Set cooldown & return to movement
        inGore = false;
        _animator.SetBool("Gore", false);
        cooldownTimer = _cooldown;
        cooldownTimerGore = cooldownGore;
        currentBehavior = StartCoroutine(AggressiveMove());
    }

    /// <summary>
    /// Reset scurrier stats after gore in case of interruption
    /// </summary>
    private void GoreReset() {
        GetComponentInChildren<EnemyAnimationController>().DisableHitbox(1);
        crashDetector.gameObject.SetActive(false);
        _scurrierFX.goreTrail.SetActive(false);
        _animator.SetBool("Gore", false);

        _agent.isStopped = false;
        _agent.autoBraking = true;
        _agent.speed = _moveSpeed;
        _agent.updatePosition = true;
        inGore = false;
        attemptingGore = false;
    }

    // -----

    /// <summary>
    /// Swat attack
    /// </summary>
    private IEnumerator AttackSwat() {
        currentState = EnemyState.Attacking;
        attackDone = false;
        _animator.SetTrigger("Swat");

        while(!attackDone) {
            yield return null;
        }

        cooldownTimer = _cooldown;
        currentBehavior = StartCoroutine(AggressiveMove());
    }

    // -------------------------------------------------------------------------------------------

    public override void ResetEnemy() {
        if(inGore) {
            return;
        }
        GoreReset();
        base.ResetEnemy();
    }

    public override void ForceIdle() {
        if(inGore) {
            return;
        }
        base.ForceIdle();
    }

    // -------------------------------------------------------------------------------------------

    public override void PlayAttackSound() {
        _scurrierFX.SwatAttack.Invoke();
    }

    /// <summary>
    /// Spawns the gore hit VFX in the correct position
    /// </summary>
    private void SpawnGoreHitVFX() {
        VFXSpawner.vfx.SpawnVFX(_scurrierFX.goreImpact, 1, transform.position + Vector3.up + (transform.forward * 0.5f), transform.rotation);
    }

}
