using System;
using System.Collections.Generic;
using UnityEngine;
public class CombatHandler : MonoBehaviour
{

    [Header("Enemies In Range")]

    [Tooltip("Radius in which the player can camera lock")]
    public float noticeRange = 10f;
    [Tooltip("All enemies in the notice range radius, this is done in runtime")]
    public List<GameObject> _enemiesInRange = new List<GameObject>();
    [NonSerialized] public AIStyles nearestEnemy;
    private List<AIStyles> _enemies = new List<AIStyles>();

    [Space(10)]
    [Tooltip("Checking if is player")]
    public bool isPlayer;

    private Character _character;
    private Animator _characterAnimator;
    Camera _mainCamera;


    //Weaponn
    public static bool isWeaponEquipped = true;

    /// <summary>
    ///  Getting references and looking for nearest enemies
    /// </summary>
    private void Awake()
    {
        try
        {
            _characterAnimator = GetComponentInChildren<Animator>();
        }
        catch (Exception e)
        {
            Debug.LogException(e, this);
        }

        _mainCamera = Camera.main;
        // InvokeRepeating("FindObjectsInRange", 1f, 1f);

        _character = GetComponent<Character>();
    }

    private void FixedUpdate()
    {
        FindObjectsInRange();
        //InvokeRepeating("FindObjectsInRange", 1f, 1f);

    }


    private void OnEquip()
    {
        if (!_characterAnimator.GetBool("IsAttacking"))
        {
            _characterAnimator.SetBool("IsWeaponEquipped", !isWeaponEquipped);
            isWeaponEquipped = !isWeaponEquipped;

            if (isWeaponEquipped == false)
            {


            }
        }
    }


    /// <summary>
    ///  Checking input, to perform a light attack
    /// </summary>
    private void OnLightAttack()
    {
        SetAttack(0);


    }

    /// <summary>
    ///  Checking input, to perform a heavy attack
    /// </summary    
    private void OnHeavyAttack()
    {
        SetAttack(1);

    }

    private void SetAttack(int attackType)
    {
        if (_characterAnimator.GetBool("IsWeaponEquipped"))
        {
            _character.lightOrHeavy = attackType;

            switch (attackType)
            {
                case 0:
                    //_character.statsSystem.StartCoroutine(_character.statsSystem.SetStamina(_character.currentWeapon.LightAttackStaminaCost));
                    _characterAnimator.SetTrigger("Attack");
                    _characterAnimator.SetInteger("AttackType", attackType);

                    break;
                case 1:
                    //_character.statsSystem.StartCoroutine(_character.statsSystem.SetStamina(_character.currentWeapon.HeavyAttackStaminaCost));
                    _characterAnimator.SetTrigger("Attack");
                    _characterAnimator.SetInteger("AttackType", attackType);
                    break;
            }
        }
    }


    /// <summary>
    ///  Checking input, to perfrom a dodge
    /// </summary>
    private void OnDodge()
    {
        Debug.Log("Horizontal: " + GetComponent<Animator>().GetFloat("Horizontal") + " Vertical: " + GetComponent<Animator>().GetFloat("Vertical"));
        if (_characterAnimator.GetBool("CanDodge"))
        {
            _characterAnimator.SetTrigger("Dodge");
            // character.statsSystem.StartCoroutine(_character.statsSystem.SetStamina(_character.staminaOnDodge));
        }
    }


    /// <summary>
    ///  Checking input, to perform a parry
    /// </summary>
    private void OnParry()
    {
        print("Parry");

        for (int i = 0; i < _enemiesInRange.Count; i++)
        {
            var enemy = _enemiesInRange[i].GetComponent<Character>();
            if (enemy._isParryable)
            {
                //changing targetlock index to enemy to parry


                //changing the param to the enemys attack to perform the right parry
                _characterAnimator.SetInteger("ParryParam", enemy.anim.GetInteger("ParryParam"));
                _characterAnimator.SetBool("Parry", true);
            }
        }
    }

    /// <summary>
    ///   Stopping parry param to stop a loop
    /// </summary>
    private void OnReleaseParry()
    {
        _characterAnimator.SetBool("Parry", false);
    }

    #region Enemies In Range

    /// <summary>
    /// Collecting enemies in the notice range radius which stores them in the enemies array
    /// </summary>
    public void CollectEnemies()
    {
        AIStyles[] allAIs = FindObjectsOfType<AIStyles>();
        foreach (AIStyles enemy in allAIs)
        {
            if (enemy.currentTribe != GetComponent<AIStyles>().currentTribe)
            {
                _enemies.Add(enemy);
            }
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
        //CODE REVIEW: Inefficient, use AIStyles method instead
        Collider[] objectsInRange = Physics.OverlapSphere(transform.position, noticeRange);

        for (int i = 0; i < objectsInRange.Length; i++)
        {
            if (_enemiesInRange.Contains(objectsInRange[i].gameObject))
            {
                continue;

            }
            else
            {
                Character current = objectsInRange[i].gameObject.GetComponent<Character>();

                if (current == null || current.currentTribe == _character.currentTribe || current.currentTribe == Character.Tribe.notAttackable)
                {
                    continue;
                }
                else
                {
                    _enemiesInRange.Add(objectsInRange[i].gameObject);

                }
            }
        }
    }

    /// <summary>
    /// Finds closest enemies in a given distance
    /// </summary>
    public void FindClosestEnemy()
    {
        float dist = float.MaxValue;
        foreach (AIStyles enemy in _enemies)
        {
            float currentDist = Vector3.Distance(enemy.transform.position, this.transform.position);
            if (currentDist < dist)
            {
                nearestEnemy = enemy;
                dist = currentDist;
            }
        }
    }
    #endregion





#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, noticeRange);
    }
#endif
















    //THESE NEED TO BE UPDATED TO NEW SYSTEM




    /*
        /// <summary>
        ///  Checking input, to perfrom ability 1
        /// </summary>
        private void OnAbility1()
        {
            if ( character.currentGem == null ) { return; }
            characterAnimator.SetBool(_isTriggeringFirstPower, true);
            if ( character.canChangePower ) {
                character.powerIndex = 0;
                character.canChangePower = false;
            }
        }

        /// <summary>
        ///  Checking input, to perfrom ability 2
        /// </summary>
        private void OnAbility2()
        {
            if ( character.currentGem == null ) { return; }
            characterAnimator.SetBool(_isTriggeringSecondPower, true);
            if ( character.canChangePower ) {
                character.powerIndex = 1;
                character.canChangePower = false;
            }
        }






        /// <summary>
        ///  Once Damaged reset all params to not have any glitch or after collisions
        /// </summary>
        public void ResetAnimParams()
        {
            characterAnimator.SetBool(_isTriggeringFirstPower, false);
            characterAnimator.SetBool(_isTriggeringSecondPower, false);
            characterAnimator.SetBool(_isRolling, false);
            //character.TriggeringAttackColliderEvent(1);
        }*/



}

