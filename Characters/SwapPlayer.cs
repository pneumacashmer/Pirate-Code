using Cinemachine;
using ProjectAlpha.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class SwapPlayer : MonoBehaviour {
    public List<GameObject> _allies;
    public GameObject[] _alliesInOrder;
    public int _position;
    private CinemachineFreeLook _cinemachine;
    public Controls _controls;
    // Start is called before the first frame update
    void Start()
    {
        AIStyles[] all = GameObject.FindObjectsOfType<AIStyles>();
        foreach ( AIStyles check in all ) {
            if ( check.currentTribe == Character.Tribe.playerCrew ) {
                _allies.Add(check.gameObject);
            }
        }
        _alliesInOrder = _allies.ToArray();
        _cinemachine = FindObjectOfType<CinemachineFreeLook>();
        _controls = FindObjectOfType<Controls>();
        _controls.SwitchNextCharacter.started += ctx => OnIncreasePosition();
        _controls.SwitchPreviousCharacter.started += ctx => OnDecreasePosition();
    }

    void OnIncreasePosition()
    {
        _alliesInOrder[_position].GetComponent<CombatHandler>().isPlayer = false;
        _alliesInOrder[_position].GetComponent<NavMeshAgent>().enabled = true;
        _alliesInOrder[_position].GetComponent<AIStyles>().currentState = AIStyles.AIStates.follow;
        _alliesInOrder[_position].GetComponent<PlayerHandler>().enabled = false;
        _alliesInOrder[_position].tag = "Untagged";
        _position++;
        if ( _position >= _alliesInOrder.Length ) {
            _position = 0;
        }
        _alliesInOrder[_position].GetComponent<CombatHandler>().isPlayer = true;
        AIManager.player = _alliesInOrder[_position];
        _alliesInOrder[_position].GetComponent<AIStyles>().currentState = AIStyles.AIStates.player;
        _alliesInOrder[_position].GetComponent<PlayerHandler>().enabled = true;
        _alliesInOrder[_position].GetComponent<NavMeshAgent>().enabled = false;
        _cinemachine.Follow = _alliesInOrder[_position].transform;
        _cinemachine.LookAt = _alliesInOrder[_position].GetComponentInChildren<CinemachineTargetGroup>().transform;
        _alliesInOrder[_position].tag = "Player";
    }

    void OnDecreasePosition()
    {
        _alliesInOrder[_position].GetComponent<CombatHandler>().isPlayer = false;
        _alliesInOrder[_position].GetComponent<AIStyles>().currentState = AIStyles.AIStates.follow;
        _alliesInOrder[_position].GetComponent<PlayerHandler>().enabled = false;
        _alliesInOrder[_position].GetComponent<NavMeshAgent>().enabled = true;
        _alliesInOrder[_position].tag = "Untagged";
        _position--;
        if ( _position <= 0 ) {
            _position = _alliesInOrder.Length - 1;
        }
        _alliesInOrder[_position].GetComponent<CombatHandler>().isPlayer = true;
        AIManager.player = _alliesInOrder[_position];
        _alliesInOrder[_position].GetComponent<AIStyles>().currentState = AIStyles.AIStates.player;
        _alliesInOrder[_position].GetComponent<PlayerHandler>().enabled = true;
        _cinemachine.Follow = _alliesInOrder[_position].transform;
        _alliesInOrder[_position].GetComponent<NavMeshAgent>().enabled = false;
        _cinemachine.LookAt = _alliesInOrder[_position].GetComponentInChildren<CinemachineTargetGroup>().transform;
        _alliesInOrder[_position].tag = "Player";
    }
}
