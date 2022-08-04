using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Quests {
    public class CutsceneManager : MonoBehaviour {
        #region variables
        [Tooltip("The list of to be passed through")]
        public List<CutsceneInformation> eventList;
        [Tooltip("The text object for the character name")]
        public Text characterName;
        [Tooltip("The text object for the character dialogue")]
        public Text characterDialogue;
        [Tooltip("The UI gameObject for dialogue")]
        public GameObject dialogueUI;
        private Transform _originalCameraTransform;
        private GameObject _playerCamera;
        private bool _scenePlaying;
        private float _speed = 1;
        private Vector3 _targetPosition;
        private Vector3 _targetRotation;
        private CinemachineFreeLook _camera;
        private Transform _follow;
        private Transform _lookAt;
        #endregion

        /// <summary>
        /// This gets called with the list of all CutsceneInformation to start the cutscene
        /// </summary>
        /// <param name="events">Full list of events needed for the scene</param>
        public void StartSequence(List<CutsceneInformation> events)
        {
            _camera = FindObjectOfType<CinemachineFreeLook>();
            _follow = _camera.Follow;
            _lookAt = _camera.LookAt;
            //_inputs = FindObjectOfType<GameManager>().UIInput;
            //Gets the camera
            _playerCamera = _camera.gameObject;
            eventList = events;
            StartCoroutine(StartEventChain());
        }

        /// <summary>
        /// This is the start of the event chain for the whole cutscene.  Goes through each event and plays it.
        /// </summary>
        /// <returns></returns>
        IEnumerator StartEventChain()
        {
            // Gets the initial camera angle to return to after the cutscene
            _originalCameraTransform = _playerCamera.transform;
            foreach ( CutsceneInformation events in eventList ) {
                // Does the events of the cutscene
                NextScene(events);
                // Allows the camera to move
                _scenePlaying = true;
                // Forces a wait time if an animation or something happens that needs to play through
                yield return new WaitForSeconds(events.forcedWaitTime);
                // Determines if the player can choose to continue or not
                if ( !events.autoContinue ) {
                    // Allows the player to control when they move on to the next event
                    yield return new WaitUntil(() => PlayerHandler.submitted == true);
                    PlayerHandler.submitted = false;
                }
                //Deactivates UI if needed
                dialogueUI.SetActive(false);
            }
            _camera.Follow = _follow;
            _camera.LookAt = _lookAt;
            _camera.GetRig(0).LookAt = _follow;
            _camera.GetRig(1).LookAt = _follow;
            _camera.GetRig(2).LookAt = _follow;
        }

        /// <summary>
        /// Handles Camera Movement
        /// </summary>
        private void Update()
        {
            if(eventList.Count == 0) {
                return;
            }
            float step = _speed * Time.deltaTime;
            // Adjusts the camera position and rotation to what is needed for the scene
            if ( _scenePlaying ) {
                _playerCamera.transform.position = Vector3.MoveTowards(_playerCamera.transform.position, _targetPosition, step);
                Vector3 rotation = Vector3.MoveTowards(new Vector3(_playerCamera.transform.rotation.x, _playerCamera.transform.rotation.y,
                    _playerCamera.transform.rotation.z), _targetRotation, step);
                _playerCamera.transform.rotation = new Quaternion(rotation.x, rotation.y, rotation.z, _playerCamera.transform.rotation.w);
            }
            // Returns the camera to the position it was in before
            else {
                _playerCamera.transform.position = Vector3.MoveTowards(_playerCamera.transform.position, _originalCameraTransform.position, step);
                Vector3 rotation = Vector3.MoveTowards(new Vector3(_playerCamera.transform.rotation.x, _playerCamera.transform.rotation.y,
                    _playerCamera.transform.rotation.z), new Vector3(_originalCameraTransform.transform.rotation.x,
                    _originalCameraTransform.transform.rotation.y, _originalCameraTransform.transform.rotation.z), step);
                _playerCamera.transform.rotation = new Quaternion(rotation.x, rotation.y, rotation.z, _playerCamera.transform.rotation.w);
            }
        }

        /// <summary>
        /// Plays the events of the scene
        /// </summary>
        /// <param name="currentEvent">The event played</param>
        private void NextScene(CutsceneInformation currentEvent)
        {
            Debug.Log(currentEvent.gameObject);
            // Manages Camera Events
            if ( currentEvent.usingCamera ) {
                _camera.Follow = null;
                _camera.LookAt = null;
                _camera.GetRig(0).LookAt = currentEvent.cameraFocus;
                _camera.GetRig(1).LookAt = currentEvent.cameraFocus;
                _camera.GetRig(2).LookAt = currentEvent.cameraFocus;
                _speed = currentEvent.transitionTime;
                _targetPosition = currentEvent.cameraPosition;
                _targetRotation = currentEvent.cameraRotation;
                float dist = Vector3.Distance(transform.position, _targetPosition);
                _speed = dist / _speed;
            }
            // Plays the animations
            if ( currentEvent.animsToPlay != null && currentEvent.charactersToAnimate != null ) {
                for ( int i = 0; i < currentEvent.animsToPlay.Length; i++ ) {
                    currentEvent.charactersToAnimate[i].GetComponent<Animator>().Play(currentEvent.animsToPlay[i].name);
                }
            }
            // Updates dialogue said
            if ( currentEvent.characterName != null && currentEvent.dialogue != null ) {
                dialogueUI.SetActive(true);
                characterName.text = currentEvent.characterName;
                characterDialogue.text = currentEvent.dialogue;
            }
        }
    }
}
