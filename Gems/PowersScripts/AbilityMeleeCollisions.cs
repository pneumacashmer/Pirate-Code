using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityMeleeCollisions : MonoBehaviour
{
    [Tooltip("The damage of the projectile")]
    public int damage;

    public float aliveTime = 1.5f;
    private float timer;

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= aliveTime) {

            Destroy(this);

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if ( !other.gameObject.tag.Equals("Player") && other.GetComponent<Character>() ) 
            {

            Character target = other.GetComponent<Character>();

            target.SetHealth(-damage);
        }
    }
}
