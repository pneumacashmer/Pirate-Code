using UnityEngine;

[CreateAssetMenu(menuName = "Gem/Shard")]
public class GemShardObject : ScriptableObject {
    
    [Tooltip("What Gem is this part of?")]
    public GemType gShardType;
    [Tooltip("The powers this shard contains")]
    public GameObject[] gShardPowerPrefab;
}
