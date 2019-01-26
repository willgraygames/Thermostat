using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
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
    public List<GameObject> myInventory = new List<GameObject>();
    GameObject currentHoverObject;

    int pitObjects;
    int lastPitObjects;
    public AudioSource footsteps;
    public AudioSource pickup;
    bool isWalking;
    public Animator itemMessage;
    public Animator itemCount;
    bool interacting;


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
    }

    void Update()
    {
        if (!isLocalPlayer)
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
        if (Input.GetButtonDown("Cancel"))
        {
            Application.Quit();
        }

        if (Physics.Raycast(myRay, out hit, interactRange))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward));
            if (hit.collider.gameObject.tag == "Pit" && myInventory.Count > 0)
            {
                print("hit something");
                interactText.text = "Feed the Pit";
                cursor.SetActive(true);
                myInventory.RemoveAll(item => item == null);
                if (Input.GetMouseButtonDown(0) && myInventory.Count > 0 && interacting == false)
                {
                    interacting = true;
                }
            }
            else if (hit.collider.tag == "Fire" && myInventory.Count > 0)
            {  // for fire effective heat range increase, have two objects one without an image one with, after clicking on the one with image it will increase collider size of emptyobject. Empty collider will be the one that increases and the one that is basis for healing
                interactText.text = "Feed the Fire";
                cursor.SetActive(true);
                myInventory.RemoveAll(item => item == null);
                if (Input.GetMouseButtonDown(0) && myInventory.Count > 0 && interacting == false)
                {
                    interacting = true;
                    print("shit yea feed that fire boi");
                }
            }
            else if (hit.collider.gameObject.tag == "Item")
            {
                

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
        GetComponent<MeshRenderer>().material.color = Color.blue;
    }
}
