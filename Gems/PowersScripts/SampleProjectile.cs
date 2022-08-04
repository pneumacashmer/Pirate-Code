using System.Collections.Generic;
using UnityEngine;

public class SampleProjectile : MonoBehaviour {
    [Tooltip("The damage of the projectile")]
    public float damage;
    [Tooltip("Who cast the spell (Leave Blank)")]
    public string casterTag;
    [Tooltip("Any tag that this projectile ignores")]
    public List<string> exceptionList;

    public bool hasHandEffect;
    public GameObject handEffectPrefab;
}
