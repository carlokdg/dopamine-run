using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using System;

// This is the script that controls the player's forward movement and speed.
public class PlayerMovement : MonoBehaviour
{

    //Player Speed and Player Horizontal Speed
    public float playerSpeed = 2;
    public float horizontalSpeed = 3;

    public float rightlimit = 5.5f;
    public float leftlimit = -5.5f;

    // Velocità con cui aumenta la velocità del player nel tempo
    public float speedIncreaseRate = 0.1f; 

    void Update()
    {

        // Aumenta gradualmente la velocità del player nel tempo
        playerSpeed += speedIncreaseRate * Time.deltaTime;

        // This part makes the player move forward constantly and allows for left and right movement using A/D or Left/Right Arrow keys.
        transform.Translate(Vector3.forward * Time.deltaTime * playerSpeed, Space.World);

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            // Check to make sure the player doesn't go out of bounds
            if (this.gameObject.transform.position.x > leftlimit)
            {
                transform.Translate(Vector3.left * Time.deltaTime * horizontalSpeed);
            }
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            if (this.gameObject.transform.position.x < rightlimit)
            {
                transform.Translate(Vector3.left * Time.deltaTime * horizontalSpeed * -1);
            }
        }
    }
}
