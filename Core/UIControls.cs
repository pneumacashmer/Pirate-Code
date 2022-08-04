using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ProjectAlpha.Core
{
    public class UIControls : MonoBehaviour
    {
        public Controls controls;
        private InventoryControls _controls;
        private InputAction _up;
        private InputAction _down;
        private InputAction _left;
        private InputAction _right;
        private InputAction _enter;
        private InputAction _escape;

        public InputAction Up {
            get {
                return _up;
            }
        }
        public InputAction Down {
            get {
                return _down;
            }
        }
        public InputAction Left {
            get {
                return _left;
            }
        }
        public InputAction Right {
            get {
                return _right;
            }
        }
        public InputAction Enter {
            get {
                return _enter;
            }
        }
        public InputAction Escape {
            get {
                return _escape;
            }
        }

        private bool assignedVariables;
        // Start is called before the first frame update
        void Start()
        {
            _controls = new InventoryControls();
            _up = _controls.Inventory.Up;
            _down = _controls.Inventory.Down;
            _left = _controls.Inventory.Left;
            _right = _controls.Inventory.Right;
            _enter = _controls.Inventory.Enter;
            _escape = _controls.Inventory.Escape;
            assignedVariables = true;
        }

        private void OnEnable()
        {
            if (assignedVariables) {
                InventoryEnable();
            }
        }

        public void InventoryEnable()
        {
            _up.Enable();
            _down.Enable();
            _left.Enable();
            _right.Enable();
            _enter.Enable();
            _escape.Enable();
            controls.PlayerDisable();
        }

        private void OnDisable()
        {
            InventoryDisable();
        }

        public void InventoryDisable()
        {
            _up.Disable();
            _down.Disable();
            _left.Disable();
            _right.Disable();
            _enter.Disable();
            _escape.Disable();
        }
    }
}
