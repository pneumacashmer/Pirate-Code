using ProjectAlpha.Core.Weapons;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class DefaultStyle : AIStyles
{

    [Tooltip("The amount of time to equip their weapon.  When this is done, they start combat.")]
    public float equipTime;
    private bool _startEquip = false;

    /// <summary>
    /// 1 is light attack, 2 is heavy, 3 is finish
    /// </summary>
    private List<int> _combo = new List<int>();

    #region Y Lerp Stuff
    private float _startTimeY;
    private float _startY;
    private float _endY;
    private bool _adjust;
    #endregion

    #region X Lerp Stuff
    private float _strafeAmount;
    private float _startTimeX;
    private float _startX;
    #endregion

    #region Combat Variables
    private bool _canStrafe = true;
    public float cycleStrafeMin = .5f;
    public float cycleStrafeMax = 3f;
    #endregion

    [SerializeField]
    private float _minCastTime = 3f;
    [SerializeField]
    private float _maxCastTime = 8f;

    /// <summary>
    /// Handles movement and changes that the AI needs to do every frame
    /// </summary>
    public override void Update()
    {
        if (_isDead || isPlayer)
        {
            return;
        }
        base.Update();
    }

    /// <summary>
    /// Upon having a target, the foe will equip their weapon and prepare movement towards it
    /// </summary>
    public override void CombatStyle()
    {
        if (!_startEquip)
        {
            anim.SetTrigger("IsEquipping");
            _startEquip = true;
            Invoke("Equiped", equipTime);
        }

        if (currentTarget == null)
        {
            return;
        }

        SetWaiting();
    }


    #region Handles controling animation bools 
    void SetWaiting()
    {
        currentState = AIStates.waiting;
        anim.SetBool("HeavyAttack", false);
        anim.SetBool("StartAttack", false);
    }

    void Equiped()
    {
        anim.SetBool("IsEquipped", true);
    }
    #endregion

    float endTime;
    /// <summary>
    /// What happens when the AI is not performing an action.
    /// </summary>
    public override void Waiting()
    {
        _navMesh.updateRotation = false;
        ignoreAnim = true;
        endRotation = Quaternion.LookRotation(new Vector3(currentTarget.transform.position.x, transform.position.y, currentTarget.transform.position.z) - transform.position);
        if ( Vector3.Distance(currentTarget.transform.position, transform.position) > meleeAttackDistance && !_isAttacking && !_adjust ) {
            _endY = 1f;
            _startTimeY = Time.time;
            _startTimeY = Time.time;
            _adjust = true;
            _startY = anim.GetFloat("InputY_Locomotion");
        }
        else if ( Vector3.Distance(currentTarget.transform.position, transform.position) < keepAwayDistance && !_isAttacking && !_adjust ) {
            _endY = -.5f;
            _startTimeY = Time.time;
            _adjust = true;
            _startY = anim.GetFloat("InputY_Locomotion");
        }
        else if ( !_adjust ) {
            _endY = 0;
            _startTimeY = Time.time;
            _adjust = true;
            _startY = anim.GetFloat("InputY_Locomotion");
        }
        else {
            anim.SetFloat("InputY_Locomotion", Mathf.Lerp(_startY, _endY, (Time.time - _startTimeY) * 2));
            if ( anim.GetFloat("InputY_Locomotion") == _endY ) {
                _adjust = false;
            }
        }
        if ( _canStrafe ) {
            _startX = anim.GetFloat("InputX_Locomotion");
            _strafeAmount = Random.Range(.5f, 1f);
            if ( Random.Range(1, 3) == 2 ) {
                _strafeAmount *= -1;
            }
            float strafeTime = Random.Range(cycleStrafeMin, cycleStrafeMax);
            Invoke("TurnOffStrafe", strafeTime);
            _canStrafe = false;
            _startTimeX = Time.time;
        }
        else {
            anim.SetFloat("InputX_Locomotion", Mathf.Lerp(_startX, _strafeAmount, Time.time - _startTimeX));
        }
        _navMesh.SetDestination(transform.position);
        transform.position = new Vector3(transform.position.x, _navMesh.destination.y, transform.position.z);

        if ( !_preppingCombo ) {
            _preppingCombo = true;
            endTime = Random.Range(minTimeBeforeCombo, maxTimeBeforeCombo) + Time.time;
        }

        if(Time.time >= endTime) { BuildCombo(); }
    }

    /// <summary>
    /// Turns off the current stafe direction, setting up to roll a new one
    /// </summary>
    void TurnOffStrafe()
    {
        _canStrafe = true;
    }

    /// <summary>
    /// Builds the combo to be executed
    /// </summary>
    private void BuildCombo()
    {
        _combo = new List<int>();
        if (_combo.Count > 0)
        {
            _combo.Clear();
        }
        int comboLength = Random.Range(minCombo, maxCombo);
        for (int i = 0; i < comboLength; i++)
        {
            float randomNumber = Random.Range(0, 100f);
            if (randomNumber <= lightHeavyChance)
            {
                _combo.Add(1);
            }
            else
            {
                _combo.Add(2);
            }
        }

        StartCoroutine(ExecuteCombo());
    }

    /// <summary>
    /// Takes the steps in the combo and plays them
    /// </summary>
    /// <returns></returns>
    IEnumerator ExecuteCombo()
    {
        _navMesh.updateRotation = false;
        _isAttacking = true;
        ignoreAnim = true;
        foreach (int move in _combo)
        {
            switch (move)
            {
                case 1:
                    {
                        yield return new WaitUntil(new System.Func<bool>(() => IsInPOV(currentTarget.transform.position)));
                        currentState = AIStates.lightAttack;
                        _isFinishedAttacking = false;
                        anim.SetBool("LightAttack", true);
                        anim.SetBool("HeavyAttack", false);
                        anim.SetBool("StartAttack", true);
                        yield return new WaitUntil(() => _isFinishedAttacking);
                        anim.SetBool("LightAttack", false);
                        //currentState = AIStates.waiting;
                        break;
                    }
                case 2:
                    {
                        yield return new WaitUntil(new System.Func<bool>(() => IsInPOV(currentTarget.transform.position)));
                        currentState = AIStates.heavyAttack;
                        _isFinishedAttacking = false;
                        anim.SetBool("LightAttack", false);
                        anim.SetBool("HeavyAttack", true);
                        anim.SetBool("StartAttack", true);
                        yield return new WaitUntil(() => _isFinishedAttacking);
                        anim.SetBool("HeavyAttack", false);
                        //currentState = AIStates.waiting;
                        break;
                    }

            }
            endRotation = Quaternion.LookRotation(new Vector3(currentTarget.transform.position.x, transform.position.y, currentTarget.transform.position.z) - transform.position).normalized;
        }
        currentState = AIStates.waiting;
        anim.SetBool("StartAttack", false);
        _isAttacking = false;
        _preppingCombo = false;
        _canStrafe = true;
    }

    /// <summary>
    /// Trigger casting in animator controller to spawn ability 
    /// </summary>
    public override void Casting()
    {
        powerIndex = Random.Range(0, PowersObtained.Count);

        if (powerIndex == 0)
        {
            anim.SetBool("IsTriggeringPower", true);
        }
        else
        {
            anim.SetBool("IsTriggeringHandPower", true);
        }
    }

    /// <summary>
    /// Spawns the ability on the right hand
    /// </summary>
    public void SpawnAbility()
    {
        //CODE REVIEW:  powerEffect is created but not used
        var powerEffect = Instantiate(PowersObtained[powerIndex], rightHandTransform.position, gameObject.transform.rotation);

        anim.SetBool("IsTriggeringPower", false);
        anim.SetBool("IsTriggeringHandPower", false);

        SetWaiting();
    }

    /// <summary>
    /// Spawns the effect on the hands when using the power
    /// </summary>
    public void SpawnHandEffect()
    {
        //CHosing which ability and checking if it has a hand effect
        if (PowersObtained[powerIndex].GetComponent<SampleProjectile>().hasHandEffect)
        {
            GameObject rightHandEffect = null;
            //spawning hand effect on right hand
            if (rightHandTransform != null)
            {
                rightHandEffect = Instantiate(PowersObtained[powerIndex].GetComponent<SampleProjectile>().handEffectPrefab,
                   rightHandTransform.position, rightHandTransform.rotation);
                Destroy(rightHandEffect, 2f);
            }
            GameObject leftHandEffect = null;
            //spawning hand effect on left hand
            if (leftHandEffect != null)
            {
                leftHandEffect = Instantiate(PowersObtained[powerIndex].GetComponent<SampleProjectile>().handEffectPrefab,
      leftHandTransform.position, leftHandTransform.rotation);
                Destroy(leftHandEffect, 2f);
            }
        }

        anim.SetBool("IsTriggeringPower", false);
        anim.SetBool("IsTriggeringHandPower", false);

        SetWaiting();
    }

    /// <summary>
    /// Use paragon power if character has any
    /// </summary>
    private void UsePower()
    {
        if (PowersObtained.Count > 0)
        {
            currentState = AIStates.casting;
        }
    }

    public override void Parry()
    {
        throw new System.NotImplementedException();
    }
}

[CustomEditor(typeof(DefaultStyle))]
public class CharacterEditor : Editor {
    override public void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();

        var chara = target as DefaultStyle;

        chara.LookAtStats = EditorGUILayout.Toggle("Look At Stats", chara.LookAtStats);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        using ( var group = new EditorGUILayout.FadeGroupScope(System.Convert.ToSingle(chara.LookAtStats)) ) {
            if ( group.visible == true ) {
                EditorGUILayout.PrefixLabel("Max Health");
                chara.maxHealth = EditorGUILayout.FloatField(chara.maxHealth);
                EditorGUILayout.PrefixLabel("Current Health");
                chara.currentHealth = EditorGUILayout.FloatField(chara.currentHealth);
                EditorGUILayout.PrefixLabel("Max Stamina");
                chara.maxStamina = EditorGUILayout.FloatField(chara.maxStamina);
                EditorGUILayout.PrefixLabel("Current Stamina");
                chara.currentStamina = EditorGUILayout.FloatField(chara.currentStamina);
                EditorGUILayout.PrefixLabel("Stamina On Dodge");
                chara.staminaOnDodge = EditorGUILayout.IntField(chara.staminaOnDodge);
                EditorGUILayout.PrefixLabel("Stamina Decrease Per Frame");
                chara.StaminaDecreasePerFrame = EditorGUILayout.FloatField(chara.StaminaDecreasePerFrame);
                EditorGUILayout.PrefixLabel("Stamina Increase Per Frame");
                chara.StaminaIncreasePerFrame = EditorGUILayout.FloatField(chara.StaminaIncreasePerFrame);
                EditorGUILayout.PrefixLabel("Stamina Time to Regen");
                chara.StaminaTimeToRegen = EditorGUILayout.FloatField(chara.StaminaTimeToRegen);
                EditorGUILayout.PrefixLabel("Max Strength");
                chara.maxStrength = EditorGUILayout.FloatField(chara.maxStrength);
                EditorGUILayout.PrefixLabel("Current Strength");
                chara.currentStrength = EditorGUILayout.FloatField(chara.currentStrength);
                EditorGUILayout.PrefixLabel("Max Speed");
                chara.maxSpeed = EditorGUILayout.FloatField(chara.maxSpeed);
                EditorGUILayout.PrefixLabel("Current Speed");
                chara.currentSpeed = EditorGUILayout.FloatField(chara.currentSpeed);
            }

        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        chara.LookAtWeaponData = EditorGUILayout.Toggle("Look At WeaponData", chara.LookAtWeaponData);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        using ( var group = new EditorGUILayout.FadeGroupScope(System.Convert.ToSingle(chara.LookAtWeaponData)) ) {
            if ( group.visible == true ) {
                EditorGUILayout.PrefixLabel("Default Weapon");
                chara.defaultWeapon = (WeaponObject)EditorGUILayout.ObjectField(chara.defaultWeapon, typeof(WeaponObject), true);
                EditorGUILayout.PrefixLabel("Current Weapon");
                chara.currentWeapon = (WeaponObject)EditorGUILayout.ObjectField(chara.currentWeapon, typeof(WeaponObject), true);
                EditorGUILayout.PrefixLabel("Left Hand Transform");
                chara.leftHandTransform = (Transform)EditorGUILayout.ObjectField(chara.leftHandTransform, typeof(Transform), true);
                EditorGUILayout.PrefixLabel("Right Hand Transform");
                chara.rightHandTransform = (Transform)EditorGUILayout.ObjectField(chara.rightHandTransform, typeof(Transform), true);
                EditorGUILayout.PrefixLabel("Holstered Transform");
                chara.holsteredTransform = (Transform)EditorGUILayout.ObjectField(chara.holsteredTransform, typeof(Transform), true);
                EditorGUILayout.PrefixLabel("Current Weapon Spawned");
                chara._currentWeapon = (GameObject)EditorGUILayout.ObjectField(chara._currentWeapon, typeof(Transform), true);
            }
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        chara.LookAtGems = EditorGUILayout.Toggle("Look At Gems", chara.LookAtGems);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        using ( var group = new EditorGUILayout.FadeGroupScope(System.Convert.ToSingle(chara.LookAtGems)) ) {
            if ( group.visible == true ) {
                EditorGUILayout.PrefixLabel("Gem Object");
                chara.currentGem = (GemObject)EditorGUILayout.ObjectField(chara.currentGem, typeof(GemObject), true);
                EditorGUILayout.PrefixLabel("Gem Shards Obtained");
                int obtainedCount = Mathf.Max(0, EditorGUILayout.IntField("size", chara.GemShardsObtained.Count));
                while(obtainedCount < chara.GemShardsObtained.Count) {
                    chara.GemShardsObtained.RemoveAt(chara.GemShardsObtained.Count - 1);
                }
                while ( obtainedCount < chara.GemShardsObtained.Count ) {
                    chara.GemShardsObtained.Add(null);
                }

                for(int i = 0; i < chara.GemShardsObtained.Count; i++ ) {
                    chara.GemShardsObtained[i] = (GemShardObject)EditorGUILayout.ObjectField(chara.GemShardsObtained[i], typeof(GemShardObject), true);
                }

                EditorGUILayout.PrefixLabel("Powers Obtained");
                int powersCount = Mathf.Max(0, EditorGUILayout.IntField("size", chara.PowersObtained.Count));
                while ( powersCount < chara.PowersObtained.Count ) {
                    chara.PowersObtained.RemoveAt(chara.PowersObtained.Count - 1);
                }
                while ( powersCount < chara.PowersObtained.Count ) {
                    chara.PowersObtained.Add(null);
                }

                for ( int i = 0; i < chara.PowersObtained.Count; i++ ) {
                    chara.PowersObtained[i] = (GameObject)EditorGUILayout.ObjectField(chara.PowersObtained[i], typeof(GameObject), true);
                }
            }
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        chara.LookAtInvestigation = EditorGUILayout.Toggle("Look At Investigation", chara.LookAtInvestigation);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        using ( var group = new EditorGUILayout.FadeGroupScope(System.Convert.ToSingle(chara.LookAtInvestigation)) ) {
            if ( group.visible == true ) {
                EditorGUILayout.PrefixLabel("Follow Distance");
                chara.followDistance = EditorGUILayout.FloatField(chara.followDistance);
                EditorGUILayout.PrefixLabel("Field Of View");
                chara.fieldOfView = EditorGUILayout.FloatField(chara.fieldOfView);
                EditorGUILayout.PrefixLabel("Notice Range");
                chara.noticeRange = EditorGUILayout.FloatField(chara.noticeRange);
                EditorGUILayout.PrefixLabel("Melee Attack Distance");
                chara.meleeAttackDistance = EditorGUILayout.FloatField(chara.meleeAttackDistance);
                EditorGUILayout.PrefixLabel("Patrol Route");
                int patrol = Mathf.Max(0, EditorGUILayout.IntField("size", chara.patrolRoute.Length));
                if ( patrol != chara.patrolRoute.Length) {
                    Vector3[] oldRoute = chara.patrolRoute;
                    chara.patrolRoute = new Vector3[patrol];
                    for(int i = 0; i < oldRoute.Length; i++) {
                        if(i == patrol) {
                            continue;
                        }
                        chara.patrolRoute[i] = oldRoute[i];
                    }
                }

                for ( int i = 0; i < chara.patrolRoute.Length; i++ ) {
                    chara.patrolRoute[i] = EditorGUILayout.Vector3Field("Position:", chara.patrolRoute[i]);
                }

            }
        }

        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        chara.LookAtCombat = EditorGUILayout.Toggle("Look At Combat", chara.LookAtCombat);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        using ( var group = new EditorGUILayout.FadeGroupScope(System.Convert.ToSingle(chara.LookAtCombat)) ) {
            if ( group.visible == true ) {
                EditorGUILayout.PrefixLabel("Keep Away Distance");
                chara.keepAwayDistance = EditorGUILayout.FloatField(chara.keepAwayDistance);
                EditorGUILayout.PrefixLabel("Min Time Before Combo");
                chara.minTimeBeforeCombo = EditorGUILayout.FloatField(chara.minTimeBeforeCombo);
                EditorGUILayout.PrefixLabel("Max Time Before Combo");
                chara.maxTimeBeforeCombo = EditorGUILayout.FloatField(chara.maxTimeBeforeCombo);
                EditorGUILayout.PrefixLabel("Min Combo Size");
                chara.minCombo = EditorGUILayout.IntField(chara.minCombo);
                EditorGUILayout.PrefixLabel("Max Combo Size");
                chara.maxCombo = EditorGUILayout.IntField(chara.maxCombo);
                EditorGUILayout.PrefixLabel("Light Heavy Attack Chance");
                chara.lightHeavyChance = EditorGUILayout.IntSlider((int)chara.lightHeavyChance, 0, 100);
                chara.isPlayer = EditorGUILayout.Toggle("Is Player", chara.isPlayer);
                EditorGUILayout.PrefixLabel("Rotating Speed");
                chara.rotateSpeed = EditorGUILayout.FloatField(chara.rotateSpeed);
            }
        }
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
    }
}

