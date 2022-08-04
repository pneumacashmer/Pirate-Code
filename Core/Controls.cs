using ProjectAlpha.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ProjectAlpha.Core
{
    public class Controls : MonoBehaviour
    {
        public UIControls uiControls;
        private InputMaster _input;
        #region Character InputActions
        private InputAction _move;
        private InputAction _look;
        private InputAction _lightAttack;
        private InputAction _heavyAttack;
        private InputAction _sprint;
        private InputAction _equip;
        private InputAction _parry;
        private InputAction _jump;
        private InputAction _dodge;
        private InputAction _interact;
        private InputAction _ability1;
        private InputAction _ability2;
        private InputAction _lockOn;
        private InputAction _swapTarget;
        private InputAction _switchPreviousCharacter;
        private InputAction _switchNextCharacter;
        private InputAction _inventoryToggle;
        private InputAction _continue;

        public InputAction Move {
            get {
                return _move;
            }
        }
        public InputAction Look {
            get {
                return _look;
            }
        }
        public InputAction LightAttack {
            get {
                return _lightAttack;
            }
        }
        public InputAction HeavyAttack {
            get {
                return _heavyAttack;
            }
        }
        public InputAction Sprint {
            get {
                return _sprint;
            }
        }
        public InputAction Equip {
            get {
                return _equip;
            }
        }
        public InputAction Parry {
            get {
                return _parry;
            }
        }
        public InputAction Jump {
            get {
                return _jump;
            }
        }
        public InputAction Dodge {
            get {
                return _dodge;
            }
        }
        public InputAction Interact {
            get {
                return _interact;
            }
        }
        public InputAction Ability1 {
            get {
                return _ability1;
            }
        }
        public InputAction Ability2 {
            get {
                return _ability2;
            }

        }
        public InputAction LockOn {
            get {
                return _lockOn;
            }
        }
        public InputAction SwapTarget {
            get {
                return _swapTarget;
            }
        }
        public InputAction SwitchPreviousCharacter {
            get {
                return _switchPreviousCharacter;
            }
        }
        public InputAction SwitchNextCharacter {
            get {
                return _switchNextCharacter;
            }
        }
        public InputAction InventoryToggle {
            get {
                return _inventoryToggle;
            }
        }
        public InputAction Continue {
            get {
                return _continue;
            }
        }
        #endregion

        private bool _assignedVariables;
        /// <summary>
        /// Sets up and assigns controls to variables.

        /// </summary>
        void Start()
        {
            _input = new InputMaster();
            _move = _input.Character.Move;
            _look = _input.Character.Look;
            _lightAttack = _input.Character.LightAttack;
            _heavyAttack = _input.Character.HeavyAttack;
            _sprint = _input.Character.Sprint;
            _equip = _input.Character.Equip;
            _parry = _input.Character.Parry;
            _jump = _input.Character.Jump;
            _dodge = _input.Character.Dodge;
            _interact = _input.Character.Interact;
            _ability1 = _input.Character.Ability1;
            _ability2 = _input.Character.Ability2;
            _lockOn = _input.Character.LockOn;
            _swapTarget = _input.Character.SwapTarget;
            _switchPreviousCharacter = _input.Character.SwitchPreviousCharacter;
            _switchNextCharacter = _input.Character.SwitchNextCharacter;
            _inventoryToggle = _input.Character.InventoryToggle;
            _continue = _input.Character.Continue;
            _assignedVariables = true;
            PlayerEnable();
        }

        private void OnEnable()
        {
            if(_assignedVariables) {
                PlayerEnable();
            }
        }

        /// <summary>
        /// Enables of the Player Controls
        /// </summary>
        public void PlayerEnable()
        {
            uiControls = FindObjectOfType<UIControls>();
            _move.Enable();
            _look.Enable();
            _lightAttack.Enable();
            _heavyAttack.Enable();
            _sprint.Enable();
            _equip.Enable();
            _parry.Enable();
            _jump.Enable();
            _dodge.Enable();
            _interact.Enable();
            _ability1.Enable();
            _ability2.Enable();
            _lockOn.Enable();
            _swapTarget.Enable();
            _switchNextCharacter.Enable();
            _switchPreviousCharacter.Enable();
            _inventoryToggle.Enable();
            _continue.Enable();
            uiControls.InventoryDisable();
        }

        /// <summary>
        /// Disable all the player controls
        /// </summary>
        public void PlayerDisable()
        {
            _move.Disable();
            _look.Disable();
            _lightAttack.Disable();
            _heavyAttack.Disable();
            _sprint.Disable();
            _equip.Disable();
            _parry.Disable();
            _jump.Disable();
            _dodge.Disable();
            _interact.Disable();
            _ability1.Disable();
            _ability2.Disable();
            _lockOn.Disable();
            _swapTarget.Disable();
            _switchNextCharacter.Disable();
            _switchPreviousCharacter.Disable();
            //_inventoryToggle.Disable();
            _continue.Disable();
        }

        private void OnDisable()
        {
            PlayerDisable();
        }
        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
