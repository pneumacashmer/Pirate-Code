using UnityEngine;

namespace Quests {
    public class CutsceneInformation : MonoBehaviour {
        [Tooltip("Name of the character typing")]
        public string characterName;
        [Tooltip("What the character says")]
        public string dialogue;
        [Tooltip("Enable this to use a custom angle")]
        public bool usingCamera;
        [Tooltip("Position of the new camera")]
        public Vector3 cameraPosition;
        [Tooltip("Position of the old camera")]
        public Vector3 cameraRotation;
        [Tooltip("What the camera focuses on")]
        public Transform cameraFocus;
        [Tooltip("The amount of time it takes for the camera to transition to this position")]
        public float transitionTime;
        [Tooltip("Characters to animate, the anims are what animations play.  Match the anim to the gameobject in the same order")]
        public GameObject[] charactersToAnimate;
        public AnimationClip[] animsToPlay;
        [Tooltip("The amount of time to wait before allowing the player to continue")]
        public float forcedWaitTime;
        [Tooltip("Enable to auto continue after the forced wait time")]
        public bool autoContinue = false;
    }
}
