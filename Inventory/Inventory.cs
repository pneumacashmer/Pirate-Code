using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour {
    [Tooltip("The inventory that said character has")]
    public List<Item> inventory;
    // Used for sorting before updating inventory
    protected List<Item> _newInventory = new List<Item>();
    //The Gameobjects with UI Slots.  They need an image component
    public GameObject[] UISlots;

    [Tooltip("The current position in the UI, public for debugging and testing.")]
    public int currentSlot = 0;

    public int scrollMod = 0;

    private void Start()
    {
        GatherInventorySlots();
        UpdateUI();
    }

    /// <summary>
    /// Gathers every inventory slot and collects them.  Then sorts them to be in order
    /// </summary>
    public void GatherInventorySlots()
    {
        //UISlots = GameObject.FindGameObjectsWithTag("InventorySlot");
        //temp is the original gameobject collection and is used with sorted to sort the inventory by name.
        List<GameObject> temp = new List<GameObject>();
        GameObject[] sorted = new GameObject[1];
        for ( int i = 0; i < UISlots.Length; i++ ) {
            sorted = temp.ToArray();
            if ( temp.Count == 0 ) {
                temp.Add(UISlots[i]);
            }
            else {
                for ( int j = 0; j < sorted.Length; j++ ) {
                    if ( sorted[j].gameObject.name.CompareTo(UISlots[i].name) > 0 ) {
                        temp.Insert(j, UISlots[i]);
                        break;
                    }
                    else if ( j == sorted.Length - 1 ) {
                        temp.Add(UISlots[i]);
                    }
                }
            }
        }
        sorted = temp.ToArray();
        UISlots = sorted;
    }

    /// <summary>
    /// Updates the inventory UI
    /// </summary>
    public void UpdateUI()
    {
        int count = 0;
        foreach(Item item in inventory) {
            if(item != null) {
                count++;
            }
        }
        Item[] items = inventory.ToArray();

        for ( int i = 0;  i < UISlots.Length; i++ ) {
            if ( i >= count + scrollMod || i + scrollMod >= items.Length) {
                UISlots[i].GetComponent<Image>().sprite = null;
                UISlots[i].GetComponent<Image>().color = new Color(0f, 0f, 0f, 0f);
            }
            else {
                UISlots[i].GetComponent<Image>().sprite = items[i + scrollMod].sprite;
                UISlots[i].GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
            }
        }
    }

}
