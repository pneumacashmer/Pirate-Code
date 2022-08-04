using UnityEngine;

public class InventoryHolder : MonoBehaviour {
    [Tooltip("The Inventory Object")]
    public GameObject inventoryObject;
    private InventoryGUI _GUI;
    private Inventory _inventory;

    /// <summary>
    /// Collects Inventory Information, then disables it (needs to be enabled on start for controls to work)
    /// </summary>
    private void Start()
    {
        _GUI = inventoryObject.GetComponentInChildren<InventoryGUI>();
        _inventory = inventoryObject.GetComponent<Inventory>();
        if ( _GUI == null ) {
            throw new System.Exception("Inventory not collected");
        }
        inventoryObject.SetActive(false);
    }

    public void OnOpenInventory()
    {
        inventoryObject.SetActive(!inventoryObject.activeInHierarchy);
        if ( inventoryObject.activeInHierarchy ) {
            _GUI.position = 0;
            Invoke("DelayedChange", Time.deltaTime);
        }
    }

    /// <summary>
    /// Opens the inventory in a set order to prevent visual glitches
    /// </summary>
    void DelayedChange()
    {
        _inventory.GatherInventorySlots();
        _inventory.UpdateUI();
        _GUI.DelayedStart();
    }
}
