using System.Collections.Generic;
using UnityEngine;

public class InteractionChecker : MonoBehaviour {
    public List<GameObject> interactions;
    public GameObject player;
    public GameObject closestObject;

    public void OnTriggerEnter(Collider other)
    {
        if ( other.gameObject.GetComponent<Interactable>() ) {
            interactions.Add(other.gameObject);
            FindNearestObject();
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if ( other.gameObject.GetComponent<Interactable>() ) {
            interactions.Remove(other.gameObject);
            FindNearestObject();
        }
    }

    private void FindNearestObject()
    {
        GameObject nearest = null;
        float minDistance = 0f;
        foreach ( GameObject obj in interactions ) {
            float distance = Vector3.Distance(obj.transform.position, player.transform.position);
            if ( nearest == null ) {
                nearest = obj;
                minDistance = distance;
            }
            else {
                if ( minDistance > distance ) {
                    nearest = obj;
                    minDistance = distance;
                }
            }
        }
        closestObject = nearest;
    }
}
