using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public float sensitivity = 5.0f;            //The speed at which the camera/character turns
    public float smoothing = 2.0f;              //The value the camera is smoothed by

    Vector2 mouseLook;                          //Vector2 to store where the mouse is currently looking
    Vector2 smoothV;                            //Vector2 to store where the camera is currently looking

    GameObject player;                          //Reference to the player's Game Object

    void Start()
    {
        //Initializes the reference to the player Game Object
        player = this.transform.parent.gameObject;
    }

    void Update()
    {
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
        transform.localRotation = Quaternion.AngleAxis(-mouseLook.y, Vector3.right);
        //Rotates the player based on the mouseLook Vector2's x axis
        player.transform.localRotation = Quaternion.AngleAxis(mouseLook.x, player.transform.up);
    }

}
