using ProjectAlpha.Core;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryGUI : MonoBehaviour {

    [Tooltip("The current position the icon is in")]
    public int position = 0;
    [Tooltip("The inventory attached to this GUI")]
    public Inventory inventory;

    // The transform that is changed throughout moving around
    protected RectTransform _rect;



    protected bool _menuOpen;
    public Color color = new Color(.8f, .8f, .8f);
    public Color unSelectedColor = new Color(1f, 1f, 1f);

    protected int menuPos;

    [SerializeField] protected GameObject _parent;
    private UIControls _controls;


    public virtual void DelayedStart()
    {
        _parent = gameObject.transform.parent.gameObject;
        inventory = FindObjectOfType<Inventory>();
        _rect = GetComponent<RectTransform>();
        inventory.UpdateUI();
        _rect.position = inventory.UISlots[position].GetComponent<RectTransform>().position;
        GetComponent<PlayerInput>().enabled = true;
        _controls = FindObjectOfType<UIControls>();
        _controls.Up.started += ctx => OnUp();
        _controls.Down.started += ctx => OnDown();
        _controls.Left.started += ctx => OnLeft();
        _controls.Right.started += ctx => OnRight();
        _controls.Enter.started += ctx => OnEnter();
        _controls.Escape.started += ctx => OnEscape();
    }

    /// <summary>
    /// Handles Controls for the UI
    /// </summary>
    public virtual void OnUp()
    {
        if ( !_menuOpen ) {
            ChangeInventoryPosition(-5);
        }
        else {
            ChangeMenuPos(-1);
        }
    }

    public virtual void OnRight()
    {
        if ( !_menuOpen )
            ChangeInventoryPosition(1);
    }

    public virtual void OnLeft()
    {
        if ( !_menuOpen )
            ChangeInventoryPosition(-1);
    }

    public virtual void OnDown()
    {
        if ( !_menuOpen )
            ChangeInventoryPosition(5);
        else {
            ChangeMenuPos(1);
        }
    }

    public virtual void OnEnter()
    {
        if ( !_menuOpen ) {
            OpenMenu();
        }
        else {
            Item[] items = inventory.inventory.ToArray();
            Item item = items[position];
            switch ( menuPos ) {
                case 1:
                    Debug.Log(item.itemName);
                    break;
                case 2:
                    Debug.Log(item.itemDescription);
                    break;
            }
        }
    }

    public virtual void OnEscape()
    {
        if ( _menuOpen ) {
            CloseMenu();
        }
        else {
            PlayerHandler.canMove = true;
            PlayerHandler.canOpenMenus = true;
            _parent.SetActive(false);
        }
    }

    public virtual void ChangeInventoryPosition(int countChange)
    {
        position += countChange;
        _rect = GetComponent<RectTransform>();
        _rect.position = inventory.UISlots[position].GetComponent<RectTransform>().position;
    }

    public virtual void OpenMenu()
    {
        if ( inventory.UISlots[position].GetComponent<Image>().color == new Color(0, 0, 0, 0) ) {
            return;
        }
        Item[] items = inventory.inventory.ToArray();
        Item item = items[position];
        _menuOpen = true;
        menuPos = 1;
    }
    public virtual void ChangeMenuPos(int change)
    {
        menuPos += change;
        if ( menuPos == 0 ) {
            menuPos = 3;
        }
        if ( menuPos == 4 ) {
            menuPos = 1;
        }
    }



    void CloseMenu()
    {
        _menuOpen = false;
    }
}
