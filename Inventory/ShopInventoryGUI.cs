using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ShopInventoryGUI : InventoryGUI {
    public GameObject buy;
    public GameObject sell;
    [Range(0, 1f)]
    public float sellBackRate;
    private bool _buySell;
    public Text itemName;
    public Text itemPrice;
    public Text itemDescription;
    public Inventory playerInventory;
    public Inventory shopInventory;
    public int shopID;
    public GameObject upArrow;
    public GameObject downArrow;
    public GameObject buyUI;
    public GameObject sellUI;
    public int rangeMin;
    public int rangeMax;

    private void Start()
    {
        Invoke("DelayedStart", Time.deltaTime);
    }

    public override void DelayedStart()
    {
        rangeMin = 0;
        rangeMax = 20;
        _parent = gameObject.transform.parent.gameObject;
        inventory = shopInventory;
        _rect = GetComponent<RectTransform>();
        inventory.UpdateUI();
        _rect.position = inventory.UISlots[position].GetComponent<RectTransform>().position;
        GetComponent<PlayerInput>().enabled = true;
        Item item = inventory.inventory[position];
        if(inventory.inventory.Count > rangeMax) {
            downArrow.SetActive(true);
        }
        itemName.text = item.itemName;
        itemDescription.text = item.itemDescription;
        itemPrice.text = item.itemPrice.ToString();
        buyUI.SetActive(true);
        sellUI.SetActive(false);
    }

    public override void ChangeInventoryPosition(int countChange)
    {
        _rect = GetComponent<RectTransform>();
        position += countChange;
        if ( _buySell ) {
            if ( countChange == 1 ) {
                _rect.transform.position = sell.transform.position;
                position = -1;
            }
            else if ( countChange == -1 ) {
                _rect.transform.position = buy.transform.position;
                position = -5;
            }
            else {
                _buySell = false;
            }
        }
        if ( position < -5 ) {
            position += 25;
        }
        if ( position >= 20 && downArrow.activeInHierarchy) {
            position -= countChange;
            ScrollDown();
        }
        else if ( position >= 20 ) {
                position -= 20;
            
        }

        if ( upArrow.activeInHierarchy && position < 0) {
            ScrollUp();
            position -= countChange;
        }
        else if ( !_buySell ) {

            if ( position < 0 && position >= -3 ) {
                _rect.transform.position = sell.transform.position;
                _buySell = true;
                position = -1;
            }
            else if ( position < -3 ) {
                _rect.transform.position = buy.transform.position;
                _buySell = true;
                position = -5;
            }
            else if (position == 23) {
                _rect.position = downArrow.transform.position;
            }
            else {
                _rect.position = inventory.UISlots[position].GetComponent<RectTransform>().position;
            }
        }
        UpdateShopDisplay();
    }

    public void UpdateShopDisplay()
    {
        if ( position < 0 || position >= inventory.inventory.Count ) {
            itemName.text = "";
            itemDescription.text = "";
            itemPrice.text = "";
            return;
        }
        Item item = inventory.inventory[position];

        itemName.text = item.itemName;
        itemDescription.text = item.itemDescription;
        float price = item.itemPrice;
        if ( inventory == playerInventory ) {
            price *= sellBackRate;
            price = (int)price;
        }
        itemPrice.text = price.ToString();
    }

    public override void OnEnter()
    {
        if ( position == -5 ) {
            inventory = shopInventory;
            inventory.UpdateUI();
            buyUI.SetActive(true);
            sellUI.SetActive(false);
            return;
        }
        if ( position == -1 ) {
            inventory = playerInventory;
            inventory.UISlots = shopInventory.UISlots;
            buyUI.SetActive(false);
            sellUI.SetActive(true);
            inventory.UpdateUI();
            return;
        }
        if ( position >= inventory.inventory.Count ) {
            return;
        }
        Item item = inventory.inventory[position];
        if ( inventory == shopInventory ) {
            if ( Currency.gold >= item.itemPrice ) {
                Currency.gold -= item.itemPrice;
                item.AddItem(playerInventory, 0);
                item.currentQuantity[shopID] -= 1;
                Debug.Log("Bought: " + Currency.gold + " Remaining");
                if ( item.currentQuantity[shopID] <= 0 ) {
                    shopInventory.inventory.Remove(item);
                    inventory.UpdateUI();
                }
            }
            else {
                Debug.Log("Not enough Gold");
            }
        }
        else {
            Currency.gold += (int)(item.itemPrice * sellBackRate);
            item.AddItem(shopInventory, shopID);
            item.currentQuantity[0] -= 1;
            Debug.Log("Sold: " + Currency.gold + " Remaining");
            if ( item.currentQuantity[0] <= 0 ) {
                playerInventory.inventory.Remove(item);
                inventory.UpdateUI();
            }
        }
    }

    public void ScrollDown()
    {
        inventory.scrollMod += 5;
        rangeMin += 5;
        rangeMax += 5;
        inventory.UpdateUI();
        if( inventory.inventory.Count < 20 + inventory.scrollMod) {
            downArrow.SetActive(false);
        }
        upArrow.SetActive(true);
    }
    public void ScrollUp()
    {
        inventory.scrollMod -= 5;
        rangeMin -= 5;
        rangeMax -= 5;
        if(rangeMin == 0) {
            upArrow.SetActive(false);
        }
        downArrow.SetActive(true);
        inventory.UpdateUI();
    }
}
