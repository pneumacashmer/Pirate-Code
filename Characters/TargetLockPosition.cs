using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetLockPosition : MonoBehaviour
{
    private PlayerHandler player;
    void Start()
    {
        player = FindObjectOfType<PlayerHandler>();
        player.targetLockLookAt = gameObject;
    }

    private void Update()
    {
        if ( player.currentTargetLock != null ) 
            {
            transform.position = Vector3.Lerp( transform.position,
            new Vector3(player.currentTargetLock.transform.position.x, 1f, player.currentTargetLock.transform.position.z)
            , Time.deltaTime * player.targetLockRotationSpeed);
        }
    }


}
