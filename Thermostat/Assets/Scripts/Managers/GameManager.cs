using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; protected set; }

    [SyncVar]
    public int currentTemp = 70;

    [SyncVar]
    public bool end;

    public List<GameObject> playerList = new List<GameObject>();

    private void Awake()
    {
        if (Instance != null)
        {
            print("There should only be one GameManager");
        } else
        {
            Instance = this;
        }

        StartCoroutine(EndGame());
    }

    private void Update()
    {
        
    }

    
    public void ChangeTemp(int tempChange)
    {
        currentTemp += tempChange;
    }

    IEnumerator EndGame ()
    {
        print("coroutine is running");
        yield return new WaitUntil(() => end == true);
        print("got it");
        if (currentTemp > 80)
        {
            for (int i = 0; i < playerList.Count; i++)
            {
                print("should be ending");
                playerList[i].GetComponent<PlayerMaster>().CmdEndTheGame(true);
            }
        } else if (currentTemp < 60)
        {
            for (int i = 0; i < playerList.Count; i++)
            {
                playerList[i].GetComponent<PlayerMaster>().CmdEndTheGame(false);
            }
        }
        yield return new WaitForSeconds(5);
    }
}
