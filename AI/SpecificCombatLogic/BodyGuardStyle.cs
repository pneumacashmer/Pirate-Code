using System.Collections.Generic;
using UnityEngine;

public class BodyGuardStyle : AIStyles {
    //Code Review: This should be private (Unless this is just for testing)
    public Character currentlyProtecting;
    //Code Review: Give Tooltips
    public float protectivePosition;
    public float equipTime;
    //Code Review: This also should be private and have the _
    bool startEquip = false;
    public override void CombatStyle()
    {
        //Finds the ally with the lowest health
        List<Character> allies = new List<Character>();
        List<Character> enemies = new List<Character>();
        Character[] characters = FindObjectsOfType<Character>();
        foreach ( Character character in characters ) {
            if ( character.currentTribe == currentTribe ) {
                allies.Add(character);
            }
            else {
                enemies.Add(character);
            }
        }
        Character lowestAlly = null;
        float lowestHealth = float.MaxValue;
        foreach ( Character ally in allies ) {
            if ( ally == this.gameObject ) {
                continue;
            }
            if ( ally.currentHealth < lowestHealth ) {
                lowestAlly = ally;
                lowestHealth = currentHealth;
            }
        }
        //Goes and protects said ally
        foreach ( Character enemy in enemies ) {
            if ( enemy.GetComponent<AIStyles>().currentTarget == lowestAlly ) {
                enemy.GetComponent<AIStyles>().currentTarget = this;
                currentTarget = enemy.GetComponent<AIStyles>();
            }
        }

        anim.SetTrigger("IsEquipping");
        if ( !startEquip ) {
            startEquip = true;
            anim.SetFloat("InputY_Locomotion", 1f);
            Invoke("Equiped", equipTime);
        }

        currentState = AIStates.waiting;
    }

    void Equiped()
    {
        anim.SetBool("IsEquipped", true);
    }

    public override void Parry()
    {
        throw new System.NotImplementedException();
    }
    public float distanceRange = .2f;
    public override void Waiting()
    {
        if ( Vector3.Distance(transform.position, currentTarget.transform.position) < protectivePosition + distanceRange ) {
            anim.SetFloat("InputY_Locomotion", -1f);
        }
        else if ( Vector3.Distance(transform.position, currentTarget.transform.position) > protectivePosition - distanceRange ) {
            anim.SetFloat("InputY_Locomotion", 1f);
        }
    }
}
