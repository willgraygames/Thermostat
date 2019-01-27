using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; protected set; }

    [SyncVar]
    public int currentTemp = 70;

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
    }

    private void Update()
    {
        
    }

    
    public void ChangeTemp(int tempChange)
    {
        currentTemp += tempChange;
    }
}
