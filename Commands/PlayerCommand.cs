using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerCommand : MonoBehaviour
{
    public Image[] commandUI;
    public GameObject commandUICanvas;
    private bool isOpen;

    private int command = -1;

    
    public List<AIStyles> allies;
    [SerializeField]
    private AIStyles[] all;
    private float[] POV;

    PlayerHandler player;

    // Start is called before the first frame update

    private void Start()
    {
        StartSetup();
    }

    public void StartSetup()
    {
        StartCoroutine(Setup());
    }

    public IEnumerator Setup()
    {
        allies.Clear();
        player = GetComponent<PlayerHandler>();
        if ( commandUICanvas.activeInHierarchy ) {
            commandUICanvas.SetActive(false);
        }
        all = FindObjectsOfType<AIStyles>();
        yield return new WaitForSecondsRealtime(.2f);
        foreach ( AIStyles style in all ) {
            Debug.Log(style.currentTribe);
            if ( style.currentTribe == Character.Tribe.playerCrew ) {
                allies.Add(style);
            }
        }
        allies.Remove(player.GetComponent<AIStyles>());
        POV = new float[allies.Count];
        for ( int i = 0; i < allies.Count; i++ ) {
            POV[i] = allies[i].fieldOfView;
        }
    }

    void OnCommandOpen()
    {
        isOpen = true;
        commandUICanvas.SetActive(true);
        PlayerHandler.canMove = false;
    }

    void OnCommandClose()
    {
        isOpen = false;
        commandUICanvas.SetActive(false);
        RunCommand();
        command = -1;
        PlayerHandler.canMove = true;
        foreach ( Image image in commandUI ) {
            image.color = Color.white;
        }
    }

    private void OnMove(InputValue input)
    {
        if(!isOpen) {
            return;
        }

        Vector2 moveDirection = input.Get<Vector2>();
        int y = 0;
        int x = 0;
        if(moveDirection.x > .5f) {
            x = 1;
        }
        else if (moveDirection.x < -.5f) {
            x = -1;
        }

        if ( moveDirection.y > .5f ) {
            y = 1;
        }
        else if ( moveDirection.y < -.5f ) {
            y = -1;
        }

        /* CHART USED FOR ARRAY VALUE
         *              0
         *                  
         *      3               1
         *                 
         *              2
         */
        if ( x == 1 ) {
            command = 1;
        }
        else if ( x == -1 ) {
            command = 3;
        }
        else if ( y == 1 ) {
            command = 0;
        }
        else if ( y == -1 ) {
            command = 2;
        }
        else {
            command = -1;
        }
        foreach ( Image image in commandUI ) {
            image.color = Color.white;
        }

        if (command == -1) {
            return;
        }
        foreach ( Image image in commandUI ) {
            image.color = Color.white;
        }
        commandUI[command].color = Color.grey;
    }


    void RunCommand()
    {
        switch ( command ) {
            case -1:
                Debug.Log("No Command Selected, if this is a mistake, report to Paprika");
                break;
            case 0:
                Stop();
                break;
            case 1:
                Follow();
                break;
            case 2:
                ToggleAttacking();
                break;
            case 3:
                Charge();
                break;
        }
    }

    private void Stop()
    {
        foreach(AIStyles ally in allies) {
            ally.currentState = AIStyles.AIStates.guardArea;
        }
    }

    private void Follow()
    {
        foreach ( AIStyles ally in allies ) {
            ally.currentState = AIStyles.AIStates.follow;
        }
    }
    private bool willAttack = true;
    private void ToggleAttacking()
    {
        if ( willAttack ) {
            for ( int i = 0; i < allies.Count; i++ ) {
                allies[i].fieldOfView = 0;
            }
            willAttack = false;
        }
        else {
            for ( int i = 0; i < allies.Count; i++ ) {
                allies[i].fieldOfView = POV[i];
            }
            willAttack = true;
        }
    }

    private void Charge()
    {
        AIStyles playerAI = player.GetComponent<AIStyles>();
        if(playerAI.currentTarget != null) {
            for ( int i = 0; i < allies.Count; i++ ) {
                allies[i].currentTarget = playerAI.currentTarget;
                allies[i].currentState = AIStyles.AIStates.combat;
            }
        }
    }
}
