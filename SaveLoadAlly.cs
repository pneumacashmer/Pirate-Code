using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SaveLoadAlly : MonoBehaviour {
    public GameObject aiTest;
    public GameObject basePrefab;
    public string aiName;
    public bool save = true;
    [Tooltip("The distance away the AI spawns in from")]
    public float spawnDistance = 10f;
    GameObject player;
    private void Start()
    {
        player = FindObjectOfType<PlayerCommand>().gameObject;
    }

    private void Update()
    {
        if ( Input.GetKeyDown(KeyCode.O) ) {
            OnClick();
        }
    }

    public void OnClick()
    {
        if(save) {
            ES3.Save(aiName, aiTest);
            ES3.Save(aiName + "1", aiTest.GetComponent<Animator>());
            ES3.Save(aiName + "2", aiTest.GetComponent<NavMeshAgent>());
            ES3.Save(aiName + "3", aiTest.GetComponent<DefaultStyle>());
            ES3.Save(aiName + "4", aiTest.GetComponent<CombatHandler>());
            ES3.Save(aiName + "5", aiTest.GetComponent<AnimationEvent>());
            //Destroy(aiTest.GetComponent<AIStyles>()._currentWeapon);
            //Destroy(aiTest.GetComponent<AIStyles>()._currentHolsteredWeapon);
            Destroy(aiTest);
            save = false;
        }
        else {
            save = true;
            StartCoroutine(LoadAI());
        }
    }

    public IEnumerator LoadAI()
    {
        float x = Random.Range(0, spawnDistance);
        float z = spawnDistance - x;
        if ( Random.Range(0, 2) == 0 ) {
            x *= -1;
        }
        if ( Random.Range(0, 2) == 0 ) {
            z *= -1;
        }

        aiTest = Instantiate(basePrefab, new Vector3(player.transform.position.x + x, player.transform.position.y, player.transform.position.z + z), transform.rotation);
        yield return new WaitUntil(() => aiTest != null);
        ES3.LoadInto(aiName + "1", aiTest.GetComponent<Animator>());
        ES3.LoadInto(aiName + "2", aiTest.GetComponent<NavMeshAgent>());
        ES3.LoadInto(aiName + "3", aiTest.GetComponent<DefaultStyle>());
        ES3.LoadInto(aiName + "4", aiTest.GetComponent<CombatHandler>());
        //ES3.LoadInto(aiName + "5", aiTest.GetComponent<AnimationEvent>());
        player.GetComponent<PlayerCommand>().StartSetup();
        aiTest.GetComponent<AIStyles>().currentState = AIStyles.AIStates.follow;
        aiTest.name = aiName;
    }
}
