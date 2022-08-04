using UnityEngine;

/// <summary>
/// Holds Information for the AIs to handle.
/// </summary>
public class AIManager : MonoBehaviour {
    [SerializeField]
    public static GameObject player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
}
