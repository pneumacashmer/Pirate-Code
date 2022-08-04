using ProjectAlpha.Core;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerInventoryGUI : InventoryGUI {

    /// <summary>
    /// Determines if the inventory is on the side of Inventory Slots or Equipment Slots
    /// </summary>
    private bool _inventorySide = true;

    /// <summary>
    /// 1 = Head Slot
    /// 2 = Chest Slot
    /// 3 = Pants Slot
    /// 4 = Boots Slot
    /// 5 = Glove Slot
    /// 6 = Melee Slot
    /// 7 = Firearm Slot
    /// 8 = Gem Slot
    /// </summary>
    private int _equipmentPosition;

    [SerializeField] private Button _nameButton;
    [SerializeField] private Button _descriptionButton;
    [SerializeField] private Button _equipButton;

    // The sort type, refer to inventory sort by type
    private int _currentSortType = -1;

    private PlayerInventory _inventory;

    public GameObject downArrow;
    public GameObject upArrow;
    int rangeMin = 0;
    int rangeMax = 20;
    public void Awake()
    {
        _parent = gameObject.transform.parent.gameObject;
        _inventory = FindObjectOfType<PlayerInventory>();
        _rect = GetComponent<RectTransform>();
        _inventory.UpdateUI();
        _rect.position = _inventory.UISlots[position].GetComponent<RectTransform>().position;
        GetComponent<PlayerInput>().enabled = true;
        if(_inventory.inventory.Count > _inventory.UISlots.Length) {
            downArrow.SetActive(true);
        }
    }

    /// <summary>
    /// Handles Controls for the UI
    /// </summary>
    public override void OnUp()
    {
        if ( !_menuOpen ) {
            ChangeInventoryPosition(-5);
        }
        else {
            ChangeMenuPos(-1);
        }
    }

    public override void OnRight()
    {
        if ( !_menuOpen )
            ChangeInventoryPosition(1);
    }

    public override void OnLeft()
    {
        if ( !_menuOpen )
            ChangeInventoryPosition(-1);
    }

    public override void OnDown()
    {
        if ( !_menuOpen )
            ChangeInventoryPosition(5);
        else {
            ChangeMenuPos(1);
        }
    }

    public override void OnEnter()
    {
        if ( !_menuOpen ) {
            if ( position == -4 ) {
                _currentSortType++;
                if ( _currentSortType == 6 ) {
                    _currentSortType = 0;
                }
                switch ( _currentSortType ) {
                    case 0:
                        _inventory.OrderObtained();
                        break;
                    case 1:
                        _inventory.NameSort();
                        break;
                    case 2:
                        _inventory.currentSlot = 0;
                        _inventory.ItemTypeSort();
                        break;
                    case 3:
                        _inventory.currentSlot = 1;
                        _inventory.ItemTypeSort();
                        break;
                    case 4:
                        _inventory.currentSlot = 2;
                        _inventory.ItemTypeSort();
                        break;
                    case 5:
                        _inventory.currentSlot = 3;
                        _inventory.ItemTypeSort();
                        break;
                }
            }
            else if ( position == -5 ) {
                _inventory.FlipListOrder();
            }
            else {
                OpenMenu();
            }
        }
        else {
            Item[] items = _inventory.inventory.ToArray();
            Item item = items[position];
            switch ( menuPos ) {
                case 1:
                    Debug.Log(item.itemName);
                    break;
                case 2:
                    Debug.Log(item.itemDescription);
                    break;
                case 3:
                    if ( item.canUse ) {
                        item.UseItem();
                    }
                    else {
                        EquipItem();
                    }
                    break;
            }
        }
    }

    public override void OnEscape()
    {
        if ( _menuOpen ) {
            CloseMenu();
        }
        else {
            _parent.SetActive(false);
        }
    }

    public override void ChangeInventoryPosition(int countChange)
    {
        if ( _inventorySide ) {
            if ( position == -4 ) {
                if ( countChange == 1 || countChange == -1 ) {
                    position = -5;
                }
                else if ( countChange == 5 ) {
                    position = 4;
                }
                else if ( countChange == -5 ) {
                    position = _inventory.UISlots.Length - 1;
                }

            }
            else {
                position += countChange;
            }
            if ( _rect.position == _inventory.sortedBy.GetComponent<RectTransform>().position && (countChange == 1 || countChange == -1) ) {
                _rect.position = _inventory.flipOrder.GetComponent<RectTransform>().position;
                position = -5;
            }
            else if ( _rect.position == _inventory.flipOrder.GetComponent<RectTransform>().position && (countChange == 1 || countChange == -1) ) {
                _rect.position = _inventory.sortedBy.GetComponent<RectTransform>().position;
                position = -4;
            }
            else if ( position >= _inventory.UISlots.Length ) {
                if(downArrow.activeInHierarchy) {
                    position -= countChange;
                    ScrollDown();
                }
                else {
                    int difference = position - _inventory.UISlots.Length;
                    position = difference;
                    _rect.position = _inventory.UISlots[position].GetComponent<RectTransform>().position;
                }

            }
            else if(upArrow.activeInHierarchy && position < 0) {
                position -= countChange;
                ScrollUp();
            }
            else if ( position < 0 && position >= -4 ) {
                position = -4;
                _rect.position = _inventory.sortedBy.GetComponent<RectTransform>().position;
            }
            else if ( position == -5 ) {
                _rect.position = _inventory.flipOrder.GetComponent<RectTransform>().position;
            }
            else if ( position < -5 ) {
                position = _inventory.UISlots.Length + position + 5;
                _rect.position = _inventory.UISlots[position].GetComponent<RectTransform>().position;
            }
            else if ( position >= 0 ) {
                _rect.position = _inventory.UISlots[position].GetComponent<RectTransform>().position;
            }


            if ( countChange == 1 ) {
                switch ( position ) {
                    case 5:
                        _rect.position = _inventory.headSlot.transform.position;
                        _equipmentPosition = 1;
                        _inventorySide = false;
                        break;
                    case 10:
                        _rect.position = _inventory.chestSlot.transform.position;
                        _equipmentPosition = 2;
                        _inventorySide = false;
                        break;
                    case 15:
                        _rect.position = _inventory.pantsSlot.transform.position;
                        _equipmentPosition = 3;
                        _inventorySide = false;
                        break;
                    case 0:
                        _rect.position = _inventory.bootsSlot.transform.position;
                        _equipmentPosition = 4;
                        _inventorySide = false;
                        break;
                }
            }
            else if ( countChange == -1 ) {
                switch ( position ) {
                    case 4:
                        _rect.position = _inventory.rangedSlot.transform.position;
                        _equipmentPosition = 7;
                        _inventorySide = false;
                        break;
                    case 9:
                        _rect.position = _inventory.gemSlot.transform.position;
                        _equipmentPosition = 8;
                        _inventorySide = false;
                        break;
                    case 14:
                        _rect.position = _inventory.bootsSlot.transform.position;
                        _equipmentPosition = 4;
                        _inventorySide = false;
                        break;
                    case -1:
                        _rect.position = _inventory.meleeSlot.transform.position;
                        _equipmentPosition = 6;
                        _inventorySide = false;
                        break;
                }
            }

        }
        else {
            if ( countChange == 5 ) {
                switch ( _equipmentPosition ) {
                    case 1:
                        _equipmentPosition = 2;
                        _rect.position = _inventory.chestSlot.transform.position;
                        break;
                    case 2:
                        _equipmentPosition = 3;
                        _rect.position = _inventory.pantsSlot.transform.position;
                        break;
                    case 3:
                        _equipmentPosition = 4;
                        _rect.position = _inventory.bootsSlot.transform.position;
                        break;
                    case 4:
                        _equipmentPosition = 1;
                        _rect.position = _inventory.headSlot.transform.position;
                        break;
                    case 6:
                        _equipmentPosition = 7;
                        _rect.position = _inventory.rangedSlot.transform.position;
                        break;
                    case 7:
                        _equipmentPosition = 8;
                        _rect.position = _inventory.gemSlot.transform.position;
                        break;
                    case 8:
                        _equipmentPosition = 6;
                        _rect.position = _inventory.meleeSlot.transform.position;
                        break;

                }
            }
            else if ( countChange == -5 ) {
                switch ( _equipmentPosition ) {
                    case 1:
                        _equipmentPosition = 4;
                        _rect.position = _inventory.bootsSlot.transform.position;
                        break;
                    case 2:
                        _equipmentPosition = 1;
                        _rect.position = _inventory.headSlot.transform.position;
                        break;
                    case 3:
                        _equipmentPosition = 2;
                        _rect.position = _inventory.chestSlot.transform.position;
                        break;
                    case 4:
                        _equipmentPosition = 3;
                        _rect.position = _inventory.pantsSlot.transform.position;
                        break;
                    case 6:
                        _equipmentPosition = 8;
                        _rect.position = _inventory.gemSlot.transform.position;
                        break;
                    case 7:
                        _equipmentPosition = 6;
                        _rect.position = _inventory.meleeSlot.transform.position;
                        break;
                    case 8:
                        _equipmentPosition = 7;
                        _rect.position = _inventory.rangedSlot.transform.position;
                        break;
                }
            }
            else if ( countChange == 1 ) {
                switch ( _equipmentPosition ) {
                    case 1:
                        _equipmentPosition = 6;
                        _rect.position = _inventory.meleeSlot.transform.position;
                        break;
                    case 2:
                        _equipmentPosition = 7;
                        _rect.position = _inventory.rangedSlot.transform.position;
                        break;
                    case 3:
                        _equipmentPosition = 5;
                        _rect.position = _inventory.gloveSlot.transform.position;
                        break;
                    case 4:
                        position = 15;
                        _rect.position = _inventory.UISlots[position].GetComponent<RectTransform>().position;
                        _inventorySide = true;
                        break;
                    case 5:
                        _equipmentPosition = 8;
                        _rect.position = _inventory.gemSlot.transform.position;
                        break;
                    case 6:
                        position = 0;
                        _rect.position = _inventory.UISlots[position].GetComponent<RectTransform>().position;
                        _inventorySide = true;
                        break;
                    case 7:
                        position = 5;
                        _rect.position = _inventory.UISlots[position].GetComponent<RectTransform>().position;
                        _inventorySide = true;
                        break;
                    case 8:
                        position = 10;
                        _rect.position = _inventory.UISlots[position].GetComponent<RectTransform>().position;
                        _inventorySide = true;
                        break;
                }
            }
            else {
                switch ( _equipmentPosition ) {
                    case 1:
                        position = 4;
                        _rect.position = _inventory.UISlots[position].GetComponent<RectTransform>().position;
                        _inventorySide = true;
                        break;
                    case 2:
                        position = 9;
                        _rect.position = _inventory.UISlots[position].GetComponent<RectTransform>().position;
                        _inventorySide = true;
                        break;
                    case 3:
                        position = 14;
                        _rect.position = _inventory.UISlots[position].GetComponent<RectTransform>().position;
                        _inventorySide = true;
                        break;
                    case 4:
                        position = 19;
                        _rect.position = _inventory.UISlots[position].GetComponent<RectTransform>().position;
                        _inventorySide = true;
                        break;
                    case 5:
                        _equipmentPosition = 3;
                        _rect.position = _inventory.pantsSlot.transform.position;
                        break;
                    case 6:
                        _equipmentPosition = 1;
                        _rect.position = _inventory.headSlot.transform.position;
                        break;
                    case 7:
                        _equipmentPosition = 2;
                        _rect.position = _inventory.chestSlot.transform.position;
                        break;
                    case 8:
                        _equipmentPosition = 5;
                        _rect.position = _inventory.gloveSlot.transform.position;
                        break;
                }
            }
        }
    }

    public override void OpenMenu()
    {
        if ( _inventory.UISlots[position].GetComponent<Image>().color == new Color(0, 0, 0, 0) ) {
            return;
        }
        Item[] items = _inventory.inventory.ToArray();
        Item item = items[position];
        if ( item.canUse ) {
            _equipButton.GetComponentInChildren<Text>().text = "Use Item";
        }
        else {
            _equipButton.GetComponentInChildren<Text>().text = "Equip Item";
        }
        _nameButton.gameObject.SetActive(true);
        _descriptionButton.gameObject.SetActive(true);
        _equipButton.gameObject.SetActive(true);
        _menuOpen = true;
        _nameButton.GetComponent<Image>().color = color;
        menuPos = 1;
    }
    public override void ChangeMenuPos(int change)
    {
        menuPos += change;
        if ( menuPos == 0 ) {
            menuPos = 3;
        }
        if ( menuPos == 4 ) {
            menuPos = 1;
        }

        switch ( menuPos ) {
            case 1:
                _nameButton.GetComponent<Image>().color = color;
                _descriptionButton.GetComponent<Image>().color = unSelectedColor;
                _equipButton.GetComponent<Image>().color = unSelectedColor;
                break;
            case 2:
                _nameButton.GetComponent<Image>().color = unSelectedColor;
                _descriptionButton.GetComponent<Image>().color = color;
                _equipButton.GetComponent<Image>().color = unSelectedColor;
                break;
            case 3:
                _nameButton.GetComponent<Image>().color = unSelectedColor;
                _descriptionButton.GetComponent<Image>().color = unSelectedColor;
                _equipButton.GetComponent<Image>().color = color;
                break;
        }
    }

    void EquipItem()
    {

        Item[] items = _inventory.inventory.ToArray();
        if ( position < items.Length ) {
            Item item = items[position];
            if ( item != null ) {
                Item.ItemType type = item.itemType;
                switch ( type ) {
                    case Item.ItemType.Head:
                        if ( _inventory.headSlot.GetComponentInChildren<Item>() != null ) {
                            _inventory.inventory.Add(_inventory.headSlot.GetComponentInChildren<Item>());
                            _inventory.headSlot.GetComponentInChildren<Item>().gameObject.transform.parent = _inventory.transform;
                        }
                        item.gameObject.transform.parent = _inventory.headSlot.transform;
                        _inventory.headSlot.GetComponent<Image>().sprite = item.sprite;
                        _inventory.headSlot.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                        _inventory.inventory.Remove(item);
                        _inventory.UpdateUI();
                        break;
                    case Item.ItemType.Chest:
                        if ( _inventory.chestSlot.GetComponentInChildren<Item>() != null ) {
                            _inventory.inventory.Add(_inventory.chestSlot.GetComponentInChildren<Item>());
                            _inventory.chestSlot.GetComponentInChildren<Item>().gameObject.transform.parent = _inventory.transform;
                        }
                        item.gameObject.transform.parent = _inventory.chestSlot.transform;
                        _inventory.chestSlot.GetComponent<Image>().sprite = item.sprite;
                        _inventory.chestSlot.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                        _inventory.inventory.Remove(item);
                        _inventory.UpdateUI();
                        break;
                    case Item.ItemType.Pants:
                        if ( _inventory.pantsSlot.GetComponentInChildren<Item>() != null ) {
                            _inventory.inventory.Add(_inventory.pantsSlot.GetComponentInChildren<Item>());
                            _inventory.pantsSlot.GetComponentInChildren<Item>().gameObject.transform.parent = _inventory.transform;
                        }
                        item.gameObject.transform.parent = _inventory.pantsSlot.transform;
                        _inventory.pantsSlot.GetComponent<Image>().sprite = item.sprite;
                        _inventory.pantsSlot.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                        _inventory.inventory.Remove(item);
                        _inventory.UpdateUI();
                        break;
                    case Item.ItemType.Boots:
                        if ( _inventory.bootsSlot.GetComponentInChildren<Item>() != null ) {
                            _inventory.inventory.Add(_inventory.bootsSlot.GetComponentInChildren<Item>());
                            _inventory.bootsSlot.GetComponentInChildren<Item>().gameObject.transform.parent = _inventory.transform;
                        }
                        item.gameObject.transform.parent = _inventory.bootsSlot.transform;
                        _inventory.bootsSlot.GetComponent<Image>().sprite = item.sprite;
                        _inventory.bootsSlot.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                        _inventory.inventory.Remove(item);
                        _inventory.UpdateUI();
                        break;
                    case Item.ItemType.Gloves:
                        if ( _inventory.gloveSlot.GetComponentInChildren<Item>() != null ) {
                            _inventory.inventory.Add(_inventory.gloveSlot.GetComponentInChildren<Item>());
                            _inventory.gloveSlot.GetComponentInChildren<Item>().gameObject.transform.parent = _inventory.transform;
                        }
                        item.gameObject.transform.parent = _inventory.gloveSlot.transform;
                        _inventory.gloveSlot.GetComponent<Image>().sprite = item.sprite;
                        _inventory.gloveSlot.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                        _inventory.inventory.Remove(item);
                        _inventory.UpdateUI();
                        break;
                    case Item.ItemType.Firearm:
                        if ( _inventory.rangedSlot.GetComponentInChildren<Item>() != null ) {
                            _inventory.inventory.Add(_inventory.rangedSlot.GetComponentInChildren<Item>());
                            _inventory.rangedSlot.GetComponentInChildren<Item>().gameObject.transform.parent = _inventory.transform;
                        }
                        item.gameObject.transform.parent = _inventory.rangedSlot.transform;
                        _inventory.rangedSlot.GetComponent<Image>().sprite = item.sprite;
                        _inventory.rangedSlot.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                        _inventory.inventory.Remove(item);
                        _inventory.UpdateUI();
                        break;
                    case Item.ItemType.Melee:
                        if ( _inventory.meleeSlot.GetComponentInChildren<Item>() != null ) {
                            _inventory.inventory.Add(_inventory.meleeSlot.GetComponentInChildren<Item>());
                            _inventory.meleeSlot.GetComponentInChildren<Item>().gameObject.transform.parent = _inventory.transform;
                        }
                        item.gameObject.transform.parent = _inventory.meleeSlot.transform;
                        _inventory.meleeSlot.GetComponent<Image>().sprite = item.sprite;
                        _inventory.meleeSlot.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                        _inventory.inventory.Remove(item);
                        _inventory.UpdateUI();
                        break;
                    case Item.ItemType.Gem:
                        if ( _inventory.gemSlot.GetComponentInChildren<Item>() != null ) {
                            _inventory.inventory.Add(_inventory.gemSlot.GetComponentInChildren<Item>());
                            _inventory.gemSlot.GetComponentInChildren<Item>().gameObject.transform.parent = _inventory.transform;
                        }
                        item.gameObject.transform.parent = _inventory.gemSlot.transform;
                        _inventory.gemSlot.GetComponent<Image>().sprite = item.sprite;
                        _inventory.gemSlot.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                        _inventory.inventory.Remove(item);
                        _inventory.UpdateUI();
                        break;
                }
            }
        }
        CloseMenu();
    }

    void CloseMenu()
    {
        _nameButton.gameObject.SetActive(false);
        _descriptionButton.gameObject.SetActive(false);
        _equipButton.gameObject.SetActive(false);
        _nameButton.GetComponent<Image>().color = unSelectedColor;
        _descriptionButton.GetComponent<Image>().color = unSelectedColor;
        _equipButton.GetComponent<Image>().color = unSelectedColor;
        _menuOpen = false;
    }

    public void ScrollDown()
    {
        _inventory.scrollMod += 5;
        rangeMin += 5;
        rangeMax += 5;
        _inventory.UpdateUI();
        if ( _inventory.inventory.Count < 20 + _inventory.scrollMod ) {
            downArrow.SetActive(false);
        }
        upArrow.SetActive(true);
    }
    public void ScrollUp()
    {
        _inventory.scrollMod -= 5;
        rangeMin -= 5;
        rangeMax -= 5;
        if ( rangeMin == 0 ) {
            upArrow.SetActive(false);
        }
        downArrow.SetActive(true);
        _inventory.UpdateUI();
    }
}
