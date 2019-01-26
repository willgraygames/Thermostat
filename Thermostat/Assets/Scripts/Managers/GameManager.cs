using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; protected set; }

    public int currentTemp;

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

    public void ChangeTemp(int tempChange)
    {
        currentTemp += tempChange;
    }
}
