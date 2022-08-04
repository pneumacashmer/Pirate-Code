using UnityEngine;

public abstract class Item : Interactable {
    [Tooltip("The Name of the Item")]
    public string itemName;
    [Tooltip("The Description of the Item")]
    public string itemDescription;
    [Tooltip("The current about of the item. 0 is player inventory, 1 is shop inventory")]
    [Range(1, 99)]
    public int[] currentQuantity = new int[2];
    [Tooltip("The maximum amount an item can stack")]
    [Range(1, 99)]
    public int maxQuantity;
    public enum ItemType { Consumable, KeyItem, Head, Chest, Pants, Boots, Gloves, Melee, Firearm, Gem };
    [Tooltip("What the item type is.  Used for equipping, sorting, and other functions")]
    public ItemType itemType;
    [Tooltip("The amount an item is bought for.  Leave at 0 for if it cannot be bought or sold.")]
    public int itemPrice;
    [Tooltip("Whether or not the item has a use function.")]
    public bool canUse;
    [Tooltip("The sprite the object has in the inventory")]
    public Sprite sprite;
    // Used for a Sort Function
    [Tooltip("Currently not static for debug purposes.")]
    public int obtainedWhen;
    // Used to know the total amount of items collected.  Used for a sort function
    [Tooltip("Currently not static for debug purposes.")]
    public static int currentItemCount;

    private GameObject _inventory;

    private void Start()
    {
        _inventory = GameObject.FindGameObjectWithTag("Inventory");
    }
    /// <summary>
    /// The special effect the specific item has.
    /// </summary>
    public abstract void UseItem();

    /// <summary>
    /// When an item is picked up, it gets added to the player's inventory.  (The inventory this needs to be added to needs to be passed though)
    /// </summary>
    /// <param name="inven">The inventory this is added to</param>
    public void AddItem(Inventory inven, int ID)
    {
        if ( inven.inventory.Contains(this) ) {
            currentQuantity[ID]++;
        }
        else {
            inven.inventory.Insert(0, this);
            currentQuantity[ID] = 1;
        }
    }

    public override void Interact()
    {
        gameObject.transform.parent = _inventory.gameObject.transform;
        FindObjectOfType<PlayerInventory>().inventory.Add(this);
        gameObject.GetComponent<Renderer>().enabled = false;
    }
}
