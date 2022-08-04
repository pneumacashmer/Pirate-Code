using System.Collections.Generic;
using UnityEngine;

namespace Quests {
    public class EventList : MonoBehaviour {
        [Tooltip("The full list of information needed for the scene")]
        public List<CutsceneInformation> information;
    }
}
