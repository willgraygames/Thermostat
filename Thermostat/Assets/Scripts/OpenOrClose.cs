using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenOrClose : MonoBehaviour
{
   
    public float range = 100f;
    

    public GameObject RaycastPoint;

   


    void Shoot()
    {
       
        {

            
            RaycastHit hit;

            if (Physics.Raycast(RaycastPoint.transform.position, RaycastPoint.transform.forward, out hit, range))
            {
                

                Debug.Log(hit.transform.name);
                
            }

            
        }
       
    }
}

