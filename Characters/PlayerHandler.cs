using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHandler : MonoBehaviour
{

    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float dodgeSpeed = 5f;
    [SerializeField] private float gravity = 30f;


    [Range(20f, 80f)]
    [Tooltip("Spped at which rotation is being smoothed")]
    public float rotationSpeed = 20f;

    [Range(0f, 30f)]
    [Tooltip("Speed at which target lock rotation is being smoothed")]
    public float targetLockRotationSpeed = 20f;
    [Tooltip("speed at which input is being smoothed")]
    public float axisSmoothSpeed = 10f;

    private Camera mainCamera;
    private Animator anim;

    private Vector3 stickDirection;
    private bool isWeaponEquipped = false;

    public Transform currentTargetLock;
    public GameObject targetLockLookAt;
    private bool _isTargetLocked = false;
    private int _targetIndex = 0;
    private bool _canRotate = false;
    private bool _isSprinting = false;
    //Input variables
    Vector2 moveAxis;
    Vector2 lookAxis;
    private Character character;
    private CharacterController characterController;
    private InteractionChecker interactionChecker;

    public GameObject playerInventoryUI;

    public bool canControl = true;
    public static bool canMove = true;
    public static bool canOpenMenus = true;
    public static bool submitted = false;
    public GameObject hitLookAt;
    /// <summary>
    ///  Start initializing needed variables
    /// </summary>
    private void Start()
    {
        canControl = true;
        Cursor.lockState = CursorLockMode.Locked;
        mainCamera = Camera.main;
        anim = transform.GetComponent<Animator>();
        character = GetComponent<Character>();
        characterController = GetComponent<CharacterController>();
        interactionChecker = FindObjectOfType<InteractionChecker>();
        Invoke("DelayedStart", Time.deltaTime);
    }

    void DelayedStart()
    {
        playerInventoryUI.SetActive(false);
    }
    /// <summary>
    ///  handling all input / target lock
    /// </summary>
    private void Update()
    {
        if (!canControl)
        {
            //transform.rotation = Quaternion.LookRotation(new Vector3(hitLookAt.transform.position.x, transform.position.y, hitLookAt.transform.position.z) - transform.position);
        }
        else
        {
            HandleInputData();
        }


        if (currentTargetLock != null && _isTargetLocked)
        {
            HandTargetLocomotionRotation();
        }
        else
            HandLocomotionRotation();

        //HandleMovementInput();
    }


    /// <summary>
    /// Input for more length, for sprint or dodge to make it go farther, using both rootmotion / transform movement
    /// </summary>
    private Vector3 moveDirection;
    private Vector2 currentInput;
    /*private void HandleMovementInput()
    {
        //(_isSprinting ? sprintSpeed : dodgeSpeed) * dodgeSpeed *
        currentInput = new Vector2( Input.GetAxis("Vertical"),  Input.GetAxis("Horizontal"));
        float moveDirectionY = moveDirection.y;
        if(_isSprinting) {
            currentInput *= sprintSpeed;
        }
        moveDirection = (transform.TransformDirection(Vector3.forward) * currentInput.x) + (transform.TransformDirection(Vector3.right * currentInput.y));
        moveDirection.y = moveDirectionY;

        //gravity
        if ( !characterController.isGrounded )
            moveDirection.y -= gravity * Time.deltaTime;

        //if (_isSprinting || anim.GetBool("IsDodging") )
        characterController.Move(moveDirection * Time.deltaTime);
        
    }*/

    /// <summary>
    /// input for moving 
    /// </summary>
    private void OnMove(InputValue input)
    {
        if (!canMove)
        {
            stickDirection = Vector3.zero;
            return;
        }
        moveAxis = input.Get<Vector2>();
        stickDirection = new Vector3(moveAxis.x, 0, moveAxis.y);
    }

    /// <summary>
    ///  input for rotation 
    /// </summary>
    protected void OnLook(InputValue input)
    {
        if (!canMove)
        {
            return;
        }
        lookAxis = input.Get<Vector2>();

        var X = lookAxis.x * .1f;
        var Y = lookAxis.y * .1f;

        //tpCamera.RotateCamera(X, Y);
    }

    /// <summary>
    ///  handling normal locomotion
    /// </summary>
    private void HandLocomotionRotation()
    {
        if (!_canRotate)
            return;

        Vector3 rotationOffset = mainCamera.transform.TransformDirection(stickDirection);
        rotationOffset.y = 0;

        transform.forward += Vector3.Lerp(transform.forward, rotationOffset, Time.deltaTime * rotationSpeed);
    }

    /// <summary>
    /// handling targetlock locomotion
    /// </summary>
    private void HandTargetLocomotionRotation()
    {

        Vector3 rotationOffset = currentTargetLock.position - transform.position;
        rotationOffset.y = 0;

        float lookDirection = Vector3.SignedAngle(transform.forward, rotationOffset, Vector3.up);
        anim.SetFloat("LookDirection", lookDirection);

        if (anim.GetFloat("Speed") > 0.1f)
        {
            transform.forward += Vector3.Slerp(transform.forward, rotationOffset, Time.deltaTime * targetLockRotationSpeed);
        }

    }

    /// <summary>
    ///  handling all data needed to get character working, mainly animator params
    /// </summary>
    private void HandleInputData()
    {
        if (_isSprinting)
        {

            anim.SetFloat("Speed", Vector3.ClampMagnitude(stickDirection, 1.0f).magnitude + 0.5f, axisSmoothSpeed, Time.deltaTime);

        }
        else
        {
            anim.SetFloat("Speed", Vector3.ClampMagnitude(stickDirection, 1.0f).magnitude, axisSmoothSpeed, Time.deltaTime);
        }


        anim.SetFloat("Horizontal", stickDirection.x, axisSmoothSpeed, Time.deltaTime);
        anim.SetFloat("Vertical", stickDirection.z, axisSmoothSpeed, Time.deltaTime);

        isWeaponEquipped = anim.GetBool("IsWeaponEquipped");
        _isTargetLocked = anim.GetBool("IsTargetLock");
        _canRotate = anim.GetBool("CanRotate");

    }

    /// <summary>
    /// input for targetlock
    /// </summary>
    private void OnLockOn()
    {
        if (!canControl)
        {
            return;
        }
        if (isWeaponEquipped)
        {
            GameObject currentTarget = character.combatHandler._enemiesInRange[_targetIndex];
            if(currentTarget == null) {
                return;
            }
            currentTargetLock = currentTarget.transform;
            GetComponent<AIStyles>().currentTarget = currentTarget.GetComponent<AIStyles>();
            anim.SetBool("IsTargetLock", !_isTargetLocked);
            _isTargetLocked = !_isTargetLocked;
        }
    }

    /// <summary>
    ///  input for target swapping
    /// </summary>
    private void OnSwapTarget()
    {
        if (!canControl)
        {
            return;
        }
        if (isWeaponEquipped)
        {
            IncrementIndex();
        }
    }

    /// <summary>
    /// input for equipping
    /// </summary>
    private void OnEquip()
    {
        if (!canControl)
        {
            return;
        }
        if (!anim.GetBool("IsAttacking"))
        {
            anim.SetBool("IsWeaponEquipped", !isWeaponEquipped);
            isWeaponEquipped = !isWeaponEquipped;

            if (isWeaponEquipped == false)
            {
                anim.SetBool("IsTargetLock", false);
                _isTargetLocked = false;
            }
        }
    }

    /// <summary>
    /// input for sprinting
    /// </summary>
    private void OnSprint()
    {
        _isSprinting = true;
    }

    /// <summary>
    ///  sprint release input
    /// </summary>
    private void OnSprintRelease()
    {
        _isSprinting = false;
    }

    /// <summary>
    /// Interacts with the object
    /// </summary>
    private void OnInteract()
    {
        if (!canControl)
        {
            return;
        }
        if (interactionChecker.closestObject != null)
        {
            interactionChecker.closestObject.GetComponent<Interactable>().Interact();
        }
    }

    private void OnInventoryToggle()
    {
        if (!canOpenMenus)
        {
            return;
        }
        playerInventoryUI.SetActive(!playerInventoryUI.activeInHierarchy);
        canControl = !playerInventoryUI.activeInHierarchy;
        playerInventoryUI.GetComponent<PlayerInventory>().UpdateUI();
    }

    private void OnContinue()
    {
        submitted = true;
    }

    /// <summary>
    /// handl;ing target swap index
    /// </summary>
    private void IncrementIndex()
    {
        if (_targetIndex + 1 <= character.combatHandler._enemiesInRange.Count - 1)
        {

            _targetIndex++;
        }
        else
        {
            _targetIndex = 0;
        }

        currentTargetLock = character.combatHandler._enemiesInRange[_targetIndex].transform;

    }

}
