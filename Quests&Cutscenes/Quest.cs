
using UnityEngine;

namespace Quests {
    [CreateAssetMenu(menuName = "Quests/Quest")]
    [System.Serializable]
    public class Quest : ScriptableObject {
        public string questName;
        public string questDescription;
        public bool isActive;
        public bool isComplete;
    }
}
