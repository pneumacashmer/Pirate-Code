using UnityEngine;

public class ShopKeeper : Interactable {
    public GameObject shop;
    private GameObject player;

    public override void Interact()
    {
        shop.SetActive(true);
        PlayerHandler.canMove = false;
        PlayerHandler.canOpenMenus = false;
    }
}
