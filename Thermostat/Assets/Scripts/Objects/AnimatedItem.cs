using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AnimatedItem : NetworkBehaviour
{
    public Animator[] animatedItems;
    public TemperatureItem myTempItem;

    bool meIsOn;

    // Start is called before the first frame update
    void Awake()
    {
        myTempItem = GetComponent<TemperatureItem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (myTempItem.isOn == true && meIsOn == false)
        {
            meIsOn = true;
            for (int i = 0; i < animatedItems.Length; i++)
            {
                print("activate Animation");
                animatedItems[i].SetBool("activateOn", true);
            }
        } else if (myTempItem.isOn == false && meIsOn == true)
        {
            meIsOn = false;
            for (int i = 0; i < animatedItems.Length; i++)
            {
                print("deactivate Animation");
                animatedItems[i].SetBool("activateOn", false);
            }
        } 

        if (myTempItem.neither)
        {
            for (int i = 0; i < animatedItems.Length; i++)
            {
                if (myTempItem.myState == ItemState.Off)
                {
                    animatedItems[i].SetBool("activateOn", false);
                } else if (myTempItem.myState == ItemState.Hot)
                {
                    animatedItems[i].SetBool("activateOn", true);
                } else if (myTempItem.myState == ItemState.Cold)
                {
                    animatedItems[i].SetBool("activateOn", false);
                }
            }
        }
    }
}
