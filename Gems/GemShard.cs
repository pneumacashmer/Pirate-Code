using UnityEngine;
using UnityEngine.UI;
public class GemShard : Item {

    [Tooltip("What gem is this shard to")]
    public GemObject gemObject;
    [Tooltip("Which shard is this object?")]
    public GemShardObject gemShard;

    [Tooltip("Display text once inside area")]
    public Image objectUI;
    private GameObject playerObj;
    private bool isPlayerClose;

    private void OnTriggerEnter(Collider other)
    {
        objectUI.gameObject.SetActive(true);
        playerObj = other.gameObject;
        isPlayerClose = true;
    }

    private void OnInteract()
    {
        if ( playerObj == null ) { return; }
        playerObj.GetComponent<GemHandler>().ObtainShard(gemObject, gemShard);

        //add shard to inventory;
        Destroy(this.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        objectUI.gameObject.SetActive(false);
        isPlayerClose = false;
        playerObj = null;
    }

    public override void UseItem()
    {
        throw new System.NotImplementedException();
    }

}

