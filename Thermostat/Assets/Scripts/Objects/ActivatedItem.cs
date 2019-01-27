using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ActivatedItem : NetworkBehaviour
{
    public GameObject[] activatedItems;
    public TemperatureItem myTempItem;

    bool meIsOn;
    public bool neitherHotNorCold;

    private void Awake()
    {
        myTempItem = GetComponent<TemperatureItem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (myTempItem.isOn == true && meIsOn == false && !neitherHotNorCold)
        {
            for (int i = 0; i < activatedItems.Length; i++)
            {
                activatedItems[i].SetActive(true);
            }
            meIsOn = true;
        } else if (myTempItem.isOn == false && meIsOn == true && !neitherHotNorCold)
        {
            meIsOn = false;
            for (int i = 0; i < activatedItems.Length; i++)
            {
                activatedItems[i].SetActive(false);
            }
        } else if (myTempItem.isOn == true && meIsOn == false && neitherHotNorCold)
        {
            meIsOn = true;
            for (int i = 0; i < activatedItems.Length; i++)
            {
                if(activatedItems[i].activeInHierarchy == true)
                {
                    activatedItems[i].SetActive(false);
                } else
                {
                    activatedItems[i].SetActive(true);
                }
            }
        }
        else if (myTempItem.isOn == false && meIsOn == true && neitherHotNorCold)
        {
            meIsOn = false;
            for (int i = 0; i < activatedItems.Length; i++)
            {
                if (activatedItems[i].activeInHierarchy == true)
                {
                    activatedItems[i].SetActive(false);
                }
                else
                {
                    activatedItems[i].SetActive(true);
                }
            }
        }
    }
}
