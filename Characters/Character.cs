using ProjectAlpha.Core.Weapons;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class Character : MonoBehaviour {
    public enum AreaToLookAt { Stats, Weapons, Gems, Investigation, Fighting };

    public bool LookAtStats;

    #region Player Stats
    [Header("Player Stats")]
    public float maxHealth = 400;
    public float currentHealth;
    [Space(10)]
    public float maxStamina = 20;
    public float currentStamina;
    [Space(5)]
    public int staminaOnDodge = 5;
    [Space(5)]
    public float StaminaDecreasePerFrame = 1.0f;
    public float StaminaIncreasePerFrame = 5.0f;
    public float StaminaTimeToRegen = 3.0f;
    [NonSerialized] public float StaminaRegenTimer = 0.0f;

    [Space(10)]
    public float maxStrength = 30;
    public float currentStrength;
    [Space(10)]
    public float maxSpeed = 15;
    public float currentSpeed;

    [NonSerialized]
    public bool _isDead;

    [NonSerialized] public Animator anim;
    [NonSerialized] public CombatHandler combatHandler;
    [NonSerialized] public StatsSystem statsSystem;
    #endregion

    public bool LookAtWeaponData;
    #region Weapon Handling
    [Header("Weapon Handling")]
    [Tooltip("Default starter weapon")]
    public WeaponObject defaultWeapon;
    [Tooltip("current weapon equipped")]
    public WeaponObject currentWeapon;

    [Space(10)]
    [Tooltip("weapon is a left handed weapon thats the spawn point")]
    public Transform leftHandTransform;
    [Tooltip("weapon is a right handed weapon thats the spawn point")]
    public Transform rightHandTransform;
    [Tooltip("holstered transform for spawn point")]
    public Transform holsteredTransform;

    // 0 LIGHT 1 HEAVY
    [NonSerialized] public int lightOrHeavy;

    // references to weapons currently spawned, so for example we can disable it, get the stats from it etc.
    public GameObject _currentWeapon;
    public GameObject _currentHolsteredWeapon;

    public AIStyles style;

    public enum Action { Equip, Unequip }

    #endregion

    public bool LookAtGems;
    #region Gems
    [Header("Current Gem Info")]
    public GemObject currentGem;
    [Space(10)]
    public List<GemShardObject> GemShardsObtained;
    public List<GameObject> PowersObtained;

    [Space(10)]
    [NonSerialized]
    public GemHandler gemHandler;
    [NonSerialized]
    public bool canChangePower = true;
    #endregion

    #region Animation Variables
    //Animation Variable
    private string _isTriggeringFirstPower = "isTriggeringFirstPower";
    private string _isTriggeringSecondPower = "isTriggeringSecondPower";

    [HideInInspector]
    public bool isAttackTriggering = false;
    [HideInInspector]
    public bool isKickAttackTriggering = false;
    [HideInInspector]
    public bool immune = false;
    // a bool to check if a parry is allowed to the enemy
    [HideInInspector]
    public bool _isParryable = false;
    // A variable that gets set in animation events, it lets us know which ability to spawn
    [HideInInspector]
    public int powerIndex;
    #endregion


    #region Tribes
    /// <summary>
    /// Player Crew is the crew that is with the player, including the player themselves.  This is used for both crewmates and guards.
    /// YellowTail Crew is the crew that is aligned with Captain Yellowtail, and will attack other tribes that aren't a part of Yellowtail.
    /// Not Attackable means that the AI will never focus attacks on them.  Reserve these for important NPCs such as shopkeepers.
    /// </summary>
    public enum Tribe { playerCrew, yellowTailCrew, notAttackable };
    [Tooltip("Look Inside Character Script for details on each Tribe")]
    public Tribe currentTribe;

    #endregion


    #region UI
    public Slider healthBar;
    #endregion

    private PlayerHandler player;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        combatHandler = GetComponent<CombatHandler>();
        gemHandler = GetComponent<GemHandler>();
        statsSystem = GetComponent<StatsSystem>();
        player = GetComponent<PlayerHandler>();
    }


    /// <summary>
    ///  Updating stats on start along with getting references and equipping weapon
    /// </summary>
    private void Start()
    {

        //NONE OF THIS STARTS AS THIS IS NOT ON PLAYER
        currentWeapon = defaultWeapon;
        SpawnWeapon(currentWeapon);

        currentHealth = maxHealth;
        currentSpeed = maxSpeed;
        currentStrength = maxStrength;
        currentStamina = maxStamina;

        style = GetComponent<AIStyles>();

        //UI
        if ( healthBar == null ) { return; }
        healthBar.maxValue = maxHealth;
        healthBar.value = currentHealth;
    }

    /// <summary>
    ///  Set the players health
    /// </summary>
    public void SetHealth(float value)
    {
        if ( value > 0 ) {
            currentHealth += value;
        }

        // only doing this so i can have control of animators
        if ( value < 0 ) {
            if ( immune ) { return; }
            currentHealth += value;

            //combatHandler.ResetAnimParams();

            isAttackTriggering = false;

            if ( currentHealth <= 0 ) {
                HandleDeath();

                //Stopping sword collision after doing a hit reaction
                isAttackTriggering = false;

                if ( currentHealth <= 0 ) {
                    HandleDeath();
                }
            }
        }

        if ( healthBar != null )
            healthBar.maxValue = maxHealth;
        if ( healthBar != null )
            healthBar.value = currentHealth;

    }


    /// <summary>
    ///  When the player dies
    /// </summary>
    public virtual void HandleDeath()
    {
        anim.SetBool("IsDead", true);
        //disable movement
        _isDead = true;
        Invoke("DestroyObject", 3f);
    }

    private void DestroyObject()
    {
        Destroy(this.gameObject);
    }



    #region Weapon Handling

    /// <summary>
    ///  Spawning weapon on player
    /// </summary>
    public GameObject SpawnWeapon(WeaponObject weapon)
    {
        // Spawning Weapon 
        if ( weapon == null ) { weapon = defaultWeapon; }

        if(weapon == null ) { return null; }

        if ( weapon.Prefab != null ) {

            //Checking which hand weapon spawns in
            Transform handTransform = weapon.IsRightHanded ? rightHandTransform : leftHandTransform;

            //spawning weapon
            _currentWeapon = Instantiate(weapon.Prefab, handTransform);
        }

        //Spawning Holstered Weapon
        if ( weapon.HolsteredPrefab != null ) {
            _currentHolsteredWeapon = Instantiate(weapon.HolsteredPrefab, holsteredTransform);
        }

        return _currentWeapon;

    }

    /// <summary>
    ///  On equip animation this is being called to enable and disable weaposn if holstered or equipped
    ///  called on a script on animation
    /// </summary>
    public void ResetWeapon(Action action)
    {
        if ( action == Action.Equip ) {
            _currentWeapon.SetActive(true);
            _currentHolsteredWeapon.SetActive(false);
        }
        else {
            _currentWeapon.SetActive(false);
            _currentHolsteredWeapon.SetActive(true);

        }

    }
    #endregion


    //NEED TO UPDATE



    /// <summary>
    ///  If the player is hit, or damaged
    ///  it also handles where the collision was marked to trigger the right reaction
    /// </summary>
    public void HitReaction(int limbID, Vector3 hitPoint, Character chara, bool isKick)
    {
        if ( immune || chara.currentTribe == currentTribe ) { return; }


        transform.rotation = Quaternion.LookRotation(new Vector3(chara.transform.position.x, transform.position.y, chara.transform.position.z) - transform.position);

        if(player != null) {
            player.hitLookAt = chara.gameObject;
            player.canControl = false;
        }


        if ( !isKick ) {
            anim.SetInteger("LimbID", limbID);
            anim.SetFloat("HitX", hitPoint.x);
            anim.SetBool("IsHit", true);
            Invoke("ResetState", 1.3f);
            if ( style != null ) {
                style.currentTarget = chara.gameObject.GetComponent<AIStyles>();
            }
        }
        else if ( isKick ) {
            anim.SetBool("IsKicked", true);
            Invoke("ResetState", 2f);
        }
        if ( !combatHandler.isPlayer ) {
            _currentWeapon.GetComponentInChildren<BoxCollider>().enabled = false;
        }

    }

    /// <summary>
    ///  resetting the hit state variable to stop the reaction from a loop
    /// </summary>
    public void ResetState()
    {
        anim.SetBool("IsHit", false);
        anim.SetBool("IsKicked", false);
        isAttackTriggering = false;
        isKickAttackTriggering = false;
        if ( player != null ) {
            player.canControl = true;
        }
    }
}

