using System.Collections.Generic;
using UnityEngine;

public class GemHandler : MonoBehaviour {
    //references
    [Tooltip("Character Script Reference")]
    public Character character;
   
    /// <summary>
    /// Initializing a gem, basically when swapping a gem or equiping a gem
    /// </summary>
    private void InitializeGem(GemObject newGem)
    {
        if ( character.currentGem == newGem ) { return; }

        character.currentGem = newGem;

        character.GemShardsObtained = new List<GemShardObject>();
        character.PowersObtained = new List<GameObject>();

    }

    /// <summary>
    ///When obtaining a shard this function will trigger, basically adding the shards to the list
    /// </summary>
    public void ObtainShard(GemObject gemType, GemShardObject shard)
    {
        if ( character.currentGem != gemType || character.currentGem == null ) {
            InitializeGem(gemType);
        }

        character.GemShardsObtained.Add(shard);

        //Adding Power Prefab
        character.PowersObtained.Add(shard.gShardPowerPrefab[0]);
        character.PowersObtained.Add(shard.gShardPowerPrefab[1]);

    }

}