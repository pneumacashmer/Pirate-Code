using UnityEngine;

public enum GemType { Fire, Water, Ice, Undead }


[CreateAssetMenu(menuName = "Gem/Gem")]
public class GemObject : ScriptableObject {
    [Tooltip("What gem is this?")]
    public GemType gemType;
    [Tooltip("The Prefab Model of this gem")]
    public GameObject gemObject;
    [Tooltip("The Icon for this gem")]
    public Sprite gemSprite;

    [Space(10)]
    [Tooltip("All shards made for this gem must be put here")]
    public GemShardObject[] gemShards;

}
