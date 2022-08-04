using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RusherStyle : AIStyles {
    [Tooltip("The distance where the rusher attacks from when running towards a foe")]
    public float rushdownDistance = 3f;

    [Tooltip("The amount of time to equip their weapon.  When this is done, they start combat.")]
    public float equipTime;
    private bool _startEquip = false;

    #region Y Lerp Stuff
    float _YlerpTime = 1f;
    bool _changingY = false;
    float _newY = 0f;
    float _YScaleTime = 1f;
    #endregion

    #region X Lerp Stuff
    //CODE REVIEW: Needs to be up top
    float _XlerpTime = 0f;
    bool _changingX = false;
    float _newX = 0f;
    float _XScaleTime = 1f;
    #endregion

    #region Combat Variables
    private bool _attemptedDodge = false;
    private bool _canStrafe = true;
    public float cycleStrafeMin = .5f;
    public float cycleStrafeMax = 3f;
    float xLoco = 0;
    #endregion
    private List<int> _combo = new List<int>();
    /// <summary>
    /// Handles movement and changes that the AI needs to do every frame
    /// </summary>
    public override void Update()
    {
        if ( combatHandler.isPlayer ) {
            return;
        }
        SwitchStates();
        if ( currentTarget != null ) {
            transform.LookAt(currentTarget.transform);
        }
        if ( _changingY ) {
            if ( _YlerpTime >= 1f ) {
                _changingY = false;
            }
            anim.SetFloat("InputY_Locomotion", Mathf.Lerp(anim.GetFloat("InputY_Locomotion"), _newY, _YlerpTime));
            _YlerpTime += Time.deltaTime / _YScaleTime;

        }
        if ( _changingX ) {
            if ( _XlerpTime >= 1f ) {
                _changingX = false;
            }
            anim.SetFloat("InputX_Locomotion", Mathf.Lerp(anim.GetFloat("InputX_Locomotion"), _newX, _XlerpTime));
            _XlerpTime += Time.deltaTime;
        }
    }
    /// <summary>
    /// Upon having a target, the AI will rush towards them into rushdown Distance.
    /// </summary>
    public override void CombatStyle()
    {
        if ( !_startEquip ) {
            anim.SetTrigger("IsEquipping");
            GetComponent<NavMeshAgent>().enabled = false;
            _startEquip = true;
            Invoke("Equiped", equipTime);
        }

        if ( currentTarget == null ) {
            return;
        }

        if ( Vector3.Distance(transform.position, currentTarget.transform.position) < rushdownDistance ) {
            anim.SetBool("HeavyAttack", true);
            ChangeY(0f, 1f);
            _navMesh.destination = transform.position;
            Invoke("SetWaiting", heavyAttackTime);
            Invoke("WaitFrame", Time.deltaTime);
        }
    }

    /// <summary>
    /// Helps set up lerps for moving foward and backwards
    /// </summary>
    /// <param name="towards">The Value that needs to be headed to</param>
    /// <param name="timeToFinish">The amount of time it should take to get to the value</param>
    void ChangeY(float towards, float timeToFinish)
    {
        if ( !_changingY ) {
            if ( timeToFinish == 0 ) {
                _YScaleTime = 1;
            }
            else {
                _YScaleTime = timeToFinish;
            }
            _newY = towards;
            _YlerpTime = 0f;
            _changingY = true;

        }

    }

    /// <summary>
    /// Helps set up lerps for moving left and right
    /// </summary>
    /// <param name="towards">The Value that needs to be headed to</param>
    /// <param name="timeToFinish">The amount of time it should take to get to the value</param>
    void ChangeX(float towards, float timeToFinish)
    {
        if ( !_changingX ) {
            if ( timeToFinish == 0 ) {
                _XScaleTime = 1;
            }
            else {
                _XScaleTime = timeToFinish;
            }
            _newX = towards;
            _XlerpTime = 0f;
            _changingX = true;
        }

    }

    void WaitFrame()
    {
        anim.SetBool("StartAttack", true);
    }

    void SetWaiting()
    {
        currentState = AIStates.waiting;
        anim.SetBool("HeavyAttack", false);
        anim.SetBool("StartAttack", false);
    }

    void Equiped()
    {
        anim.SetBool("IsEquipped", true);
        ChangeY(1f, 1f);
    }

    /// <summary>
    /// What happens when the AI is not performing an action.
    /// </summary>
    public override void Waiting()
    {
        if ( Vector3.Distance(transform.position, currentTarget.transform.position) < keepAwayDistance ) {
            ChangeY(-.75f, .4f);
        }
        else if ( Vector3.Distance(transform.position, currentTarget.transform.position) > keepAwayDistance + 1f ) {
            ChangeY(1f, .4f);
        }
        else {
            ChangeY(0f, .4f);
        }
        /*if ( Vector3.Distance(transform.position, currentTarget.transform.position) <= keepAwayDistance ) {
            _navMesh.destination = transform.position;
        }
        else {
            _navMesh.destination = currentTarget.transform.position;
        }*/

        if ( _canStrafe ) {
            transform.LookAt(currentTarget.transform);
            _canStrafe = false;
            _navMesh.enabled = false;
            Invoke("TurnOnStrafe", Random.Range(cycleStrafeMin, cycleStrafeMax));
            xLoco = Random.Range(-1f, 1f);
            while ( Mathf.Abs(xLoco) < .5f ) {
                xLoco = Random.Range(-1f, 1f);
            }
            //float yLoco = Random.Range(-1f, 1f);
            ChangeY(0, .1f);
        }
        else {
            ChangeX(xLoco, 1f);
        }


        if ( !_preppingCombo ) {
            _preppingCombo = true;
            Invoke("BuildCombo", Random.Range(minTimeBeforeCombo, maxTimeBeforeCombo));
        }
    }

    void EnableDodge()
    {
        currentState = AIStates.dodging;
        _attemptedDodge = false;

    }

    void TurnOnStrafe()
    {
        _canStrafe = true;
    }

    private void BuildCombo()
    {
        _combo = new List<int>();
        if ( Vector3.Distance(transform.position, currentTarget.transform.position) > keepAwayDistance + 1f ) {
            _preppingCombo = false;
            return;
        }
        if ( _combo.Count > 0 ) {
            _combo.Clear();
        }

        int comboLength = Random.Range(minCombo, maxCombo);
        for ( int i = 0; i < comboLength; i++ ) {
            float randomNumber = Random.Range(0, 100f);
            if ( randomNumber <= lightHeavyChance ) {
                _combo.Add(1);
            }
            else {
                _combo.Add(2);
            }
        }

        StartCoroutine(ExecuteCombo());
    }

    [Tooltip("The amount of time it takes to do a light attack")]
    public float lightAttackTime = 4f;
    [Tooltip("The amount of time it takes to do a heavy attack")]
    public float heavyAttackTime = 7f;
    IEnumerator ExecuteCombo()
    {
        foreach ( int move in _combo ) {
            if ( Vector3.Distance(transform.position, currentTarget.transform.position) < keepAwayDistance ) {
                anim.SetFloat("InputY_Locomotion", -1f);
            }
            else {
                anim.SetFloat("InputY_Locomotion", 0f);
            }
            switch ( move ) {
                case 1: {
                        anim.SetBool("LightAttack", true);
                        anim.SetBool("HeavyAttack", false);
                        anim.SetBool("StartAttack", true);
                        yield return new WaitForSeconds(lightAttackTime);
                        break;
                    }
                case 2: {
                        anim.SetBool("LightAttack", false);
                        anim.SetBool("HeavyAttack", true);
                        anim.SetBool("StartAttack", true);
                        yield return new WaitForSeconds(heavyAttackTime);
                        break;
                    }
            }
        }
        currentState = AIStates.waiting;
        anim.SetBool("StartAttack", false);
        _isAttacking = false;
        _preppingCombo = false;
    }
    public override void Parry()
    {
        //DEBUG PURPOSES ONLY 
        currentState = AIStates.waiting;
        //throw new System.NotImplementedException();
    }
}
