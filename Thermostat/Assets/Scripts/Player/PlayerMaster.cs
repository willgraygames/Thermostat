using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerMaster : NetworkBehaviour
{
    public float sensitivity = 5.0f;            //The speed at which the camera/character turns
    public float smoothing = 2.0f;              //The value the camera is smoothed by

    Vector2 mouseLook;                          //Vector2 to store where the mouse is currently looking
    Vector2 smoothV;                            //Vector2 to store where the camera is currently looking

    //Current target variables
    public GameObject cursor;                   //Reference to player's cursor
    public Text interactText;                   //Reference to the text naming the player's current target
    public float interactRange;                 //The maximum range from the player to a target to be able to interact with it
    RaycastHit hit;                             //RaycastHit for when the player hits something while attacking
    public Camera mainCamera;

    //Movement variables
    public float speed;                         //Speed at which the Player moves
    Vector3 movement;                           //Player's movement value stored in a Vector3
    CapsuleCollider myCollider;                 //Reference to the Player's capsule collider
    Rigidbody rb3d;                             //Reference to the Player's rigidbody
    public LayerMask dropsLayer;

    //Inventory
    GameObject currentHoverObject;

    public GameObject myCanvas;

    public int heatThreshold;
    public int coldThreshold;

    int pitObjects;
    int lastPitObjects;
    public AudioSource footsteps;
    public AudioSource pickup;
    bool isWalking;
    public Animator itemMessage;
    public Animator itemCount;
    public Animator animator;
    bool interacting;

    public Text tempText;
    public Text endText;

    private bool isOpen = false;
    public bool hotPlayer;

    void Awake()
    {
        //Sets the interact text to blank
        //interactText.text = "";
        //Initializes player's collider and rigidbody references
        myCollider = GetComponent<CapsuleCollider>();
        rb3d = GetComponent<Rigidbody>();
        //Locks cursor the center of the screen
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        mainCamera.enabled = false;
        myCanvas.SetActive(false);
        interactText.text = "";
        cursor.SetActive(false);
        if (GameManager.Instance.playerList.Count == 0)
        {
            GameManager.Instance.playerList.Add(this.gameObject);
            hotPlayer = true;
        } else
        {
            GameManager.Instance.playerList.Add(this.gameObject);
            hotPlayer = false;
        }
        
    }

    void Update()
    {
        if (!isLocalPlayer && GameManager.Instance.end)
        {
            return;
        }

        

        //Gets the input for the mouse's movement and stores it as a Vector2
        var mouseMovement = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        //Multiplies the mouseMovement Vector2 by sensitivity and smoothing for each axis
        mouseMovement = Vector2.Scale(mouseMovement, new Vector2(sensitivity * smoothing, sensitivity * smoothing));
        //Interpolates between the smoothV Vector2 and the mouseMovement Vector2 by a percentage of the smoothing float variable. It does this for each axis to smooth the camera's movement.
        smoothV.x = Mathf.Lerp(smoothV.x, mouseMovement.x, 1f / smoothing);
        smoothV.y = Mathf.Lerp(smoothV.y, mouseMovement.y, 1f / smoothing);
        mouseLook += smoothV;
        //Clamps the camera's movement along the y axis, limiting how far the player can look up and down
        mouseLook.y = Mathf.Clamp(mouseLook.y, -70f, 70f);
        //Rotates the camera based on the mouseLook Vector2's y axis
        mainCamera.transform.localRotation = Quaternion.AngleAxis(-mouseLook.y, Vector3.right);
        //Rotates the player based on the mouseLook Vector2's x axis
        transform.localRotation = Quaternion.AngleAxis(mouseLook.x, transform.up);

        Ray myRay = mainCamera.ScreenPointToRay(Input.mousePosition);

        //If ESC is pressed, unlock the cursor
        if (Input.GetButtonDown("Cancel") && Cursor.visible == false)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        } else if (Input.GetButtonDown("Cancel") && Cursor.visible == true)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (Physics.Raycast(myRay, out hit, interactRange))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward));
            if (hit.collider.gameObject.tag == "Openable")
            {
                //interactText.text = "Open";
                //cursor.SetActive(true);

                if (Input.GetMouseButtonDown(0))
                {
                    interacting = true;
                    animator.SetBool("open", isOpen);
                }
            }
            //Check if the object we just interacted with is a hot object
            else if (hit.collider.gameObject.tag == "Hot")
            {
                interactText.text = hit.collider.gameObject.GetComponent<TemperatureItem>().objectName;
                cursor.SetActive(true);
                if (Input.GetMouseButtonDown(0))
                {
                    print("We clicked that b");
                    interacting = true;
                    //If it is, are we the player who wants heat?
                    if (hotPlayer == true)
                    {
                        //If we are the player who wants heat, is the object on already?
                        if (hit.collider.gameObject.GetComponent<TemperatureItem>().isOn == false)
                        {
                            //If it isn't, let's turn it on and add its positiveTempChange value to the current temperature
                            CmdTurnObject(hit.collider.gameObject, true);
                            int tempChange = hit.collider.gameObject.GetComponent<TemperatureItem>().positiveTempChange;
                            CmdChangeTemp(tempChange);
                        }
                    }
                    else
                    {
                        //If we are NOT the player who wants heat, is the object on already?
                        if (hit.collider.gameObject.GetComponent<TemperatureItem>().isOn == true)
                        {
                            //If it is, let's turn it off and subtract its positiveTempChange value from the current temperature
                            CmdTurnObject(hit.collider.gameObject, false);
                            int tempChange = -hit.collider.gameObject.GetComponent<TemperatureItem>().positiveTempChange;
                            CmdChangeTemp(tempChange);
                        }
                    }

                }

            }
            else if (hit.collider.gameObject.tag == "Cold")
            {
                interactText.text = hit.collider.gameObject.GetComponent<TemperatureItem>().objectName;
                cursor.SetActive(true);
                if (Input.GetMouseButtonDown(0))
                {
                    //If the object is a cold object, are we the player who wants heat?
                    if (hotPlayer == false)
                    {
                        //If we ARE NOT the player who wants heat, is the object on already?
                        if (hit.collider.gameObject.GetComponent<TemperatureItem>().isOn == false)
                        {
                            //If the object isn't on, let's turn it on to make it colder and add the negativeTempChange value to the currentTemp
                            CmdTurnObject(hit.collider.gameObject, true);
                            int tempChange = hit.collider.gameObject.GetComponent<TemperatureItem>().negativeTempChange;

                            CmdChangeTemp(tempChange);
                        }
                    }
                    else
                    {
                        //If we ARE the player who wants heat, is the object on already?
                        if (hit.collider.gameObject.GetComponent<TemperatureItem>().isOn == true)
                        {
                            //If the object is on already, let's turn it off to make it warmer and subtract the negativeTempChange value from the currentTemp
                            CmdTurnObject(hit.collider.gameObject, false);
                            int tempChange = -hit.collider.gameObject.GetComponent<TemperatureItem>().negativeTempChange;
                            CmdChangeTemp(tempChange);
                        }
                    }
                }
            }
            else if (hit.collider.gameObject.tag == "Neither")
            {
                interactText.text = hit.collider.gameObject.GetComponent<TemperatureItem>().objectName;
                cursor.SetActive(true);
                if (Input.GetMouseButtonDown(0))
                {
                    if (hotPlayer)
                    {
                        //If we ARE the hot player
                        if (hit.collider.gameObject.GetComponent<TemperatureItem>().myState == ItemState.Off || hit.collider.gameObject.GetComponent<TemperatureItem>().myState == ItemState.Cold)
                        {
                            CmdChangeObjectState(hit.collider.gameObject, ItemState.Hot);
                            int tempChange = hit.collider.gameObject.GetComponent<TemperatureItem>().positiveTempChange;

                            CmdChangeTemp(tempChange);
                        }
                    } else
                    {
                        //If we ARE NOT the hot player
                        if (hit.collider.gameObject.GetComponent<TemperatureItem>().myState == ItemState.Off || hit.collider.gameObject.GetComponent<TemperatureItem>().myState == ItemState.Hot)
                        {
                            CmdChangeObjectState(hit.collider.gameObject, ItemState.Cold);
                            int tempChange = hit.collider.gameObject.GetComponent<TemperatureItem>().negativeTempChange;

                            CmdChangeTemp(tempChange);
                        }
                    }
                    
                    
                }
            }
            else
            {
                interactText.text = "";
                cursor.SetActive(false);
            }
        }
        else
        {
            interactText.text = "";
            cursor.SetActive(false);
            currentHoverObject = null;
        }

        

    }

    //Player movement is handled inside of FixedUpdate due to the use of Unity's rigidbody physics system
    void FixedUpdate()
    {
        if(!isLocalPlayer)
        {
            return;
        }
        //Variables to hold the player's movement inputs
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        //Calls the MovePlayer function with horizontal and vertical as parameters
        MovePlayer(horizontal, vertical);
    }

    //Function to handle moving the player. Takes two float values for the horizontal and vertical inputs as parameters
    void MovePlayer(float h, float v)
    {
        //Sets the movement Vector3 with the horizonal and vertical inputs
        movement.Set(h, 0f, v);
        //Calculates movement in relation to Time.deltaTime and the speed value
        movement = movement.normalized * speed * Time.deltaTime;
        //Sets the movement to be relative to the player's rotation
        movement = transform.TransformDirection(movement);
        //Moves the player's rigidbody in the direction of the movement Vector3
        rb3d.MovePosition(transform.position + movement);
        if (h >= .1f && isWalking == false || v >= .1f && isWalking == false || h <= -.1f && isWalking == false || v <= -.1f && isWalking == false)
        {
            isWalking = true;
            footsteps.Play();
            print("Im walking here");
        }
        else if (h < .1f && v < .1f && h > -.1f && v > -.1f)
        {
            isWalking = false;
            footsteps.Stop();
            print("Im not walking here");
        }
    }

    public override void OnStartLocalPlayer()
    {
        mainCamera.enabled = true;
        myCanvas.SetActive(true);
        
        if(hotPlayer == true)
        {
            GetComponent<MeshRenderer>().material.color = Color.red;
        } else
        {
            GetComponent<MeshRenderer>().material.color = Color.blue;
        }
    }

    [Command]
    void CmdChangeTemp(int change)
    {
        GameManager.Instance.currentTemp += change;
        if (GameManager.Instance.currentTemp > heatThreshold || GameManager.Instance.currentTemp < coldThreshold)
        {
            GameManager.Instance.end = true;
        }
        RpcUpdateTempValueClientSide(GameManager.Instance.currentTemp);
    }

    [ClientRpc]
    public void RpcUpdateTempValueClientSide (int tempValue)
    {
        if (GameManager.Instance.currentTemp > heatThreshold || GameManager.Instance.currentTemp < coldThreshold)
        {
            GameManager.Instance.end = true;
            if (GameManager.Instance.currentTemp > heatThreshold)
            {
                CmdEndTheGame(true);
            } else
            {
                CmdEndTheGame(false);
            }
        }
        for (int i = 0; i < GameManager.Instance.playerList.Count; i++)
        {
            GameManager.Instance.playerList[i].GetComponent<PlayerMaster>().tempText.text = tempValue + "°";
        }
    }

    [Command]
    void CmdTurnObject (GameObject item, bool state)
    {
        item.GetComponent<TemperatureItem>().isOn = state;
        RpcTurnObject(item, state);
    }

    [ClientRpc]
    public void RpcTurnObject (GameObject item, bool state)
    {
        item.GetComponent<TemperatureItem>().isOn = state;
    }

    [Command]
    void CmdChangeObjectState (GameObject item, ItemState myState)
    {
        item.GetComponent<TemperatureItem>().myState = myState;
    }

    [ClientRpc]
    public void RpcChangeObjectState (GameObject item, ItemState myState)
    {
        item.GetComponent<TemperatureItem>().myState = myState;
    }

    [Command]
    public void CmdEndTheGame (bool whoWon)
    {
        print("Command running");
        RpcEndTheGame(whoWon);
    }

    [ClientRpc]
    public void RpcEndTheGame (bool whoWon)
    {
        for (int i = 0; i < GameManager.Instance.playerList.Count; i++)
        {
            if (whoWon)
            {
                endText.text = "Heat Won!";
                
                Invoke("ThrowBackToTheLobby", 5f);
            }
            else if (!whoWon)
            {
                endText.text = "Cold Won!";
                Invoke("ThrowBackToTheLobby", 5f);
            }
        }
        
    }


    public void ThrowBackToTheLobby ()
    {
        print("Throwback");
        NetworkLobbyManager myLobby = GameObject.FindObjectOfType<NetworkLobbyManager>();
        myLobby.StopServer();
        myLobby.StopClient();
        
    }
}
