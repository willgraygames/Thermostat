using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TemperatureItem : NetworkBehaviour
{
    [SyncVar]
    public bool isOn;                           //Bool to determine whether or not the object is on
    public string objectName;
    public bool hot;                            //
    public bool neither;
    [SyncVar]
    public bool onlyOn;                         //Bool to determine if this object only dispense one type of temperature change. For example, it will be true if the object is a space heater that only dispenses heat, but false for the postcard that has two states, both of which affect that temperature
    public int positiveTempChange;
    public int negativeTempChange;
    [SyncVar]
    public ItemState myState;                   //Enum to determine if the item is in Hot mode, Cold mode, or Off

    private void Awake()
    {
    }

    private void Update()
    {
        
    }
}
