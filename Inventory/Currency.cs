using UnityEngine;

public class Currency : MonoBehaviour {
    // The amount of Gold the player has
    public static int gold;
    // Whether or not the initial adjustment for gold has been done.
    private static bool adjustedGold;
    [Tooltip("The amount of Gold the player starts with")]
    public int startingGold;
    public void Awake()
    {
        gold = startingGold;
        if ( !adjustedGold ) {
            gold = startingGold;
            adjustedGold = true;
        }
    }
}
