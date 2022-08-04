using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : Inventory {

    [Tooltip("The text object which displays the sorting order")]
    public Text sortedBy;
    [Tooltip("The text object that just says to flip order.  Used for controlling the UI")]
    public Text flipOrder;
    [Tooltip("The inventory slot the head is contained in.  Used for controlling the UI")]
    public GameObject headSlot;
    [Tooltip("The inventory slot the chest is contained in.  Used for controlling the UI")]
    public GameObject chestSlot;
    [Tooltip("The inventory slot the pants are contained in.  Used for controlling the UI")]
    public GameObject pantsSlot;
    [Tooltip("The inventory slot the boots are contained in.  Used for controlling the UI")]
    public GameObject bootsSlot;
    [Tooltip("The inventory slot the glove is contained in.  Used for controlling the UI")]
    public GameObject gloveSlot;
    [Tooltip("The inventory slot the melee weapon is contained in.  Used for controlling the UI")]
    public GameObject meleeSlot;
    [Tooltip("The inventory slot the ranged weapon is contained in.  Used for controlling the UI")]
    public GameObject rangedSlot;
    [Tooltip("The inventory slot the gem is contained in.  Used for controlling the UI")]
    public GameObject gemSlot;

    /// <summary>
    /// Reverses the list of the inventory then updates the UI
    /// </summary>
    public void FlipListOrder()
    {
        _newInventory = new List<Item>();
        inventory.Reverse();
        UpdateUI();
    }

    /// <summary>
    /// Sorts the inventory by the order it has been obtained
    /// </summary>
    public void OrderObtained()
    {
        _newInventory = new List<Item>();
        int currentCount = 0;
        int safety = 0;
        Item[] itemArray = inventory.ToArray();
        while ( currentCount <= itemArray.Length ) {
            if ( safety >= 100 ) {
                currentCount++;
                safety = 0;
                //throw new System.Exception("Safety Check reached Limit.  Please contact Paprika with how you recieved this message");
            }
            for ( int i = 0; i < itemArray.Length; i++ ) {
                if ( itemArray[i].obtainedWhen == currentCount + 1 ) {
                    _newInventory.Add(itemArray[i]);
                    inventory.Remove(itemArray[i]);
                    currentCount++;
                    safety = 0;
                }
            }
            safety++;
        }
        inventory = _newInventory;
        UpdateUI();
        sortedBy.text = "Order Obtained";
    }

    /// <summary>
    /// Sorts the inventory by name alphabetically
    /// </summary>
    public void NameSort()
    {
        _newInventory = new List<Item>();
        Item[] itemArray = inventory.ToArray();
        if(itemArray.Length == 0) {
            return;
        }
        Item[] sorted = new Item[itemArray.Length - 1];
        for ( int i = 0; i < itemArray.Length; i++ ) {
            sorted = _newInventory.ToArray();
            if ( _newInventory.Count == 0 ) {
                _newInventory.Add(itemArray[i]);
            }
            else {
                for ( int j = 0; j < sorted.Length; j++ ) {
                    if ( sorted[j].itemName.CompareTo(itemArray[i].itemName) > 0 ) {
                        _newInventory.Insert(j, itemArray[i]);
                        break;
                    }
                    else if ( j == sorted.Length - 1 ) {
                        _newInventory.Add(itemArray[i]);
                    }
                }
            }
        }
        inventory = _newInventory;
        UpdateUI();
        sortedBy.text = "Name";
    }

    /// <summary>
    /// Weapons are 0, Armor is 1, Consumables is 2, and Key Items is 3
    /// Sorts the inventory by the type of item.
    /// </summary>
    public void ItemTypeSort()
    {
        if ( currentSlot == 4 ) {
            currentSlot = 0;
        }
        _newInventory = new List<Item>();
        List<Item> weapons = new List<Item>();
        List<Item> armor = new List<Item>();
        List<Item> consumables = new List<Item>();
        List<Item> keyItems = new List<Item>();

        foreach ( Item item in inventory ) {
            if ( item.itemType == Item.ItemType.Consumable ) {
                consumables.Add(item);
                continue;
            }
            if ( item.itemType == Item.ItemType.KeyItem ) {
                keyItems.Add(item);
                continue;
            }
            if ( item.itemType == Item.ItemType.Firearm || item.itemType == Item.ItemType.Gem || item.itemType == Item.ItemType.Melee ) {
                weapons.Add(item);
                continue;
            }
            if ( item.itemType == Item.ItemType.Boots || item.itemType == Item.ItemType.Chest || item.itemType == Item.ItemType.Gloves || item.itemType == Item.ItemType.Head || item.itemType == Item.ItemType.Pants ) {
                armor.Add(item);
                continue;
            }
            throw new System.Exception(item.name + "Does not have an Item Type");
        }
        for ( int i = 0; i < 4; i++ ) {
            switch ( currentSlot ) {
                case 0:
                    _newInventory.AddRange(weapons);
                    sortedBy.text = "Armor";
                    break;
                case 1:
                    _newInventory.AddRange(armor);
                    sortedBy.text = "Consuambles";
                    break;
                case 2:
                    _newInventory.AddRange(consumables);
                    sortedBy.text = "KeyItems";
                    break;
                case 3:
                    _newInventory.AddRange(keyItems);
                    sortedBy.text = "Weapons";
                    break;
            }
            currentSlot++;
            if ( currentSlot == 4 ) {
                currentSlot = 0;
            }
        }
        inventory = _newInventory;
        UpdateUI();
    }
}
