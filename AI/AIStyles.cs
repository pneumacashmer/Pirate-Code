using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class AIStyles : Character
{

    public enum AIStates { combat, lightAttack, heavyAttack, casting, firing, reloading, patrol, follow, guardArea, dodging, waiting, parry, dead, player};
    [Header("Debug Variables")]
    [Tooltip("The current state the AI is in")]
    public AIStates currentState;
    /// <summary>
    /// I don't like this being used in the same script that it is referencing.  But this is to keep track of what the enemy is doing.
    /// </summary>
    public AIStyles currentTarget;
    [HideInInspector]
    public bool _isAttacking;
    protected bool _isFinishedAttacking;
    [HideInInspector]
    public NavMeshAgent _navMesh;
    [HideInInspector]
    public AIManager _manager;
    protected bool _canSeeEnemy;

    [Space]
    public bool LookAtInvestigation;
    [Header("Patrol/Investigation Variables")]
    [Tooltip("The amount of Unity Units away the AIs will keep distance when set to follow.")]
    public float followDistance = 4f;
    public float fieldOfView = 135;
    public float noticeRange = 10f;
    public float meleeAttackDistance = 4f;
    [Tooltip("The Patrol Route the AI will follow")]
    public Vector3[] patrolRoute;
    //The current patrol position
    private int _currentPosition = 0;

    [Space]
    public bool LookAtCombat;
    [Header("Combat Variables")]
    [Tooltip("The distance the AI will stay away from the target when in combat")]
    public float keepAwayDistance = 3f;
    [HideInInspector]
    public bool _preppingCombo = false;
    [Tooltip("The minimum amount of time needed before attacking")]
    public float minTimeBeforeCombo = 4f;
    [Tooltip("The maximum amount of time needed before attacking")]
    public float maxTimeBeforeCombo = 10f;
    [Tooltip("When the enemy is attacking, the minimum amount of times the AI will attack.  THIS HAS TO BE AT LEAST 1")]
    public int minCombo = 1;
    [Tooltip("When the enemy is attacking, the maximum amount of times the AI will attack in a row.")]
    public int maxCombo = 3;
    [Range(0, 100)]
    [Tooltip("The weight percentage between light and heavy attacks.  If this was at 30, there would be a 30% chance to do light attacks, and 70% for heavy")]
    public float lightHeavyChance = 50f;
    private bool _closeEnough = false;

    private List<AIStyles> _enemies = new List<AIStyles>();

    private bool isChecking;

    Vector2 smoothDeltaPosition = Vector2.zero;
    Vector2 velocity = Vector2.zero;
    public bool isPlayer;
    [HideInInspector] public bool ignoreAnim = false;
    protected Quaternion endRotation;
    public float rotateSpeed = 1f;

    private void Start()
    {
        if (!isPlayer)
        {
            _navMesh = GetComponent<NavMeshAgent>();
            _navMesh.updatePosition = false;
        }
        _manager = FindObjectOfType<AIManager>();
        anim = GetComponent<Animator>();
        combatHandler = GetComponent<CombatHandler>();

        currentWeapon = defaultWeapon;
        //need to do this here for character.
        SpawnWeapon(currentWeapon);
        //temporary
        if (!combatHandler.isPlayer)
            _currentWeapon.SetActive(true);
        else if(_currentWeapon != null)
            _currentWeapon.SetActive(false);

        // Don’t update position automatically

        anim.applyRootMotion = true;
        Invoke("LateStart", 1f);
    }

    /// <summary>
    /// Collects the enemies after start has been completed for everything
    /// </summary>
    private void LateStart()
    {

        CollectEnemies();
    }

    /// <summary>
    /// Finds out which entities to track for potential fights
    /// </summary>
    public void CollectEnemies()
    {
        AIStyles[] allAIs = FindObjectsOfType<AIStyles>();
        foreach (AIStyles enemy in allAIs)
        {
            if (enemy.currentTribe != currentTribe)
            {
                _enemies.Add(enemy);
            }
        }
    }

    /// <summary>
    /// Handles what mode the AI is in
    /// </summary>
    public void SwitchStates()
    {
        switch (currentState)
        {
            case AIStates.combat:
                {
                    CombatStyle();
                    return;
                }
            case AIStates.lightAttack:
                {
                    if (!_isAttacking)
                    {
                        LightAttack();
                    }
                    return;
                }
            case AIStates.heavyAttack:
                {
                    if (!_isAttacking)
                    {
                        HeavyAttack();
                    }
                    return;
                }
            case AIStates.casting:
                {
                    if (!_isAttacking)
                    {
                        Casting();
                    }

                    return;
                }
            case AIStates.firing:
                {
                    if (!_isAttacking)
                    {
                        Firing();
                    }

                    return;
                }
            case AIStates.reloading:
                {
                    Reloading();
                    return;
                }
            case AIStates.patrol:
                {
                    Patroling();
                    StartLooking();
                    return;
                }
            case AIStates.follow:
                {
                    Follow();
                    StartLooking();
                    return;
                }
            case AIStates.guardArea:
                {
                    GuardArea();
                    StartLooking();
                    return;
                }
            case AIStates.dodging:
                {
                    Dodging();
                    return;
                }
            case AIStates.waiting:
                {
                    Waiting();
                    return;
                }

            case AIStates.parry:
                {
                    Parry();
                    return;
                }
            case AIStates.dead:
                {
                    _navMesh.enabled = false;

                    return;
                }
        }
    }

    void OnAnimatorMove()
    {
        if (isPlayer)
        {
            transform.position += anim.deltaPosition;
            return;
        }
        if (ignoreAnim)
        {
            transform.position += anim.deltaPosition;
            _navMesh.SetDestination(transform.position);
            return;
        }

        // Update position to agent position
        transform.position = _navMesh.nextPosition;
    }

    /// <summary>
    /// Lines up anim with navmesh
    /// </summary>
    public virtual void Update()
    {
        if (currentTarget == null)
        {
            if (currentTribe != Tribe.playerCrew)
            {
                if (patrolRoute.Length > 0)
                {
                    currentState = AIStates.patrol;
                }
                else
                {
                    currentState = AIStates.guardArea;
                }
            }
        }
        if (isPlayer)
        {
            return;
        }
        SwitchStates();
        if (ignoreAnim)
        {
            return;
        }
        Vector3 worldDeltaPosition = _navMesh.nextPosition - transform.position;

        // Map 'worldDeltaPosition' to local space
        float dx = Vector3.Dot(transform.right, worldDeltaPosition);
        float dy = Vector3.Dot(transform.forward, worldDeltaPosition);
        Vector2 deltaPosition = new Vector2(dx, dy);

        // Low-pass filter the deltaMove
        float smooth = Mathf.Min(1.0f, Time.deltaTime / 0.15f);
        smoothDeltaPosition = Vector2.Lerp(smoothDeltaPosition, deltaPosition, smooth);

        // Update velocity if time advances
        if (Time.deltaTime > 1e-5f)
            velocity = smoothDeltaPosition / Time.deltaTime;

        bool shouldMove = velocity.magnitude > 0.5f && _navMesh.remainingDistance > _navMesh.radius;

        // Update animation parameters
        anim.SetBool("move", shouldMove);
        anim.SetFloat("InputX_Locomotion", velocity.x);
        anim.SetFloat("InputY_Locomotion", velocity.y);


    }

    private void LateUpdate()
    {
        if (!isPlayer && endRotation != new Quaternion() && !_navMesh.updateRotation)
            transform.rotation = Quaternion.Lerp(transform.rotation, endRotation, Time.deltaTime * rotateSpeed);

    }

    //CODE REVIEW: Needs XML Comment
    private void StartLooking()
    {
        if (!isChecking)
        {
            Invoke("FindObjectsInRange", 1f);
            isChecking = true;
        }
    }

    /// <summary>
    /// When in a non combat state, the AI checks what is in it's detection range.
    /// If there are objects with Colliders on them, it checks to see if they have a Character Component
    /// From there, it cycles out which ones are enemies.
    /// If the enemy exists within the field of view, it switches to a combat state.  Noting which enemy it spotted.
    /// </summary>
    public void FindObjectsInRange()
    {
        bool continueSearch = false;
        foreach (AIStyles enemy in _enemies)
        {
            if (enemy == null)
            {
                _enemies.Remove(enemy);
            }

            if (Vector3.Distance(this.transform.position, enemy.gameObject.transform.position) < noticeRange)
            {
                continueSearch = true;
            }
        }
        isChecking = false;
        if (!continueSearch)
        {
            return;
        }
        if (_enemies.Count > 0)
        {
            foreach (AIStyles enemy in _enemies)
            {
                Vector3 target = enemy.transform.position - transform.position;
                if (IsInPOV(target))
                {
                    currentTarget = enemy;
                    currentState = AIStates.combat;
                    _navMesh.SetDestination(transform.position);
                    return;
                }
            }
        }
        if (currentState != AIStates.patrol)
            currentState = AIStates.patrol;
    }

    /// <summary>
    /// Checks whether a target is within the field of view
    /// </summary>
    /// <param name="target">The target's transform.position</param>
    /// <returns>True if in Range</returns>
    public bool IsInPOV(Vector3 target)
    {
        bool isIn = false;
        float angle = Vector3.Angle(target, transform.forward);
        if (angle <= fieldOfView / 2)
        {
            isIn = true;
        }

        return isIn;
    }

    /// <summary>
    /// This is the primary Script to be used the next level up.  This is to handle what combat pattern the AI follows.
    /// </summary>
    public abstract void CombatStyle();
    public virtual void LightAttack()
    {
        _isAttacking = true;
    }
    public virtual void HeavyAttack()
    {
        _isAttacking = true;
    }
    public virtual void Casting()
    {
        _isAttacking = true;
        throw new NotImplementedException();
    }
    public virtual void Firing()
    {
        _isAttacking = true;
        throw new NotImplementedException();
    }
    public virtual void Reloading()
    {
        throw new NotImplementedException();
    }
    /// <summary>
    /// When the AI reaches the current position, it'll update the next position after WaitTime
    /// </summary>
    private void Patroling()
    {
        if (patrolRoute != null && patrolRoute.Length > 0)
        {
            if (_navMesh.destination != patrolRoute[_currentPosition])
            {
                _navMesh.destination = patrolRoute[_currentPosition];
            }
            if (Vector3.Distance(transform.position, patrolRoute[_currentPosition]) <= 2f && !_closeEnough)
            {
                _closeEnough = true;
                Invoke("SetNextPosition", UnityEngine.Random.Range(1, 3));
            }
        }
    }
    //CODE REVIEW: Needs XML Comment
    private void SetNextPosition()
    {
        _closeEnough = false;
        _currentPosition++;
        if (_currentPosition >= patrolRoute.Length)
        {
            _currentPosition = 0;
        }

        _navMesh.SetDestination(patrolRoute[_currentPosition]);
    }
    /// <summary>
    /// The AI moves towards the player and follows them from a short distance.
    /// </summary>
    private void Follow()
    {
        if (Vector3.Distance(transform.position, AIManager.player.transform.position) <= followDistance)
        {
            _navMesh.SetDestination(transform.position);
            transform.LookAt(AIManager.player.transform);
            return;
        }
        else
        {
            transform.LookAt(AIManager.player.transform);
            _navMesh.SetDestination(AIManager.player.transform.position);
        }
    }
    /// <summary>
    /// AI stays in place.
    /// </summary>
    private void GuardArea()
    {
        _navMesh.SetDestination(transform.position);
    }

    //CODE REVIEW: Needs XML Comment
    public virtual void Dodging()
    {

    }

    //CODE REVIEW: Needs XML Comment
    private void DeactivateDodge()
    {

    }

    //CODE REVIEW: Needs XML Comment
    public override void HandleDeath()
    {
        currentState = AIStates.dead;

        base.HandleDeath();
    }
    public abstract void Waiting();
    public abstract void Parry();

    public void FinishAttack()
    {
        _isFinishedAttacking = true;
        anim.SetBool("StartAttack", false);
    }

    /// <summary>
    /// Handles the movement of the Ai, information fed into it handles movement.
    /// </summary>
    public void Movement()
    {

    }

    private void OnCollisionStay(Collision collision)
    {
        if (isPlayer)
            return;
        GameObject other = collision.gameObject;
        Vector3 difference = transform.position - other.transform.position;
        difference = new Vector3(difference.x, 0, difference.z);
        transform.position += (difference * .05f);
    }


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Quaternion leftRayRotation = Quaternion.AngleAxis(fieldOfView / 2, Vector3.up);
        Quaternion rightRayRotation = Quaternion.AngleAxis(-fieldOfView / 2, Vector3.up);
        Vector3 leftRayDirection = leftRayRotation * transform.forward * 5;
        Vector3 rightRayDirection = rightRayRotation * transform.forward * 5;
        Gizmos.DrawRay(transform.position, leftRayDirection);
        Gizmos.DrawRay(transform.position, rightRayDirection);
        Gizmos.DrawWireSphere(transform.position, noticeRange);
        for(int i = 1; i < patrolRoute.Length; i++ ) {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(patrolRoute[i], patrolRoute[i - 1]);
        }
    }
#endif
}
