using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using System;

// This is the script that controls the player's forward movement and speed.
public class PlayerMovement : MonoBehaviour
{


    [SerializeField] GameObject playerAnimJump;
    //Player Speed and Player Horizontal Speed
    public float playerSpeed = 2;
    public float horizontalSpeed = 3;

    public float rightlimit = 5.5f;
    public float leftlimit = -5.5f;

    // Velocità con cui aumenta la velocità del player nel tempo
    public float speedIncreaseRate = 0.5f; 

    public float jumpForce = 5f;
    private bool isGrounded = true;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {

        // Increase player speed and horizontal speed over time
        playerSpeed += speedIncreaseRate * Time.deltaTime;
        horizontalSpeed += speedIncreaseRate * Time.deltaTime;

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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        playerAnimJump.GetComponent<Animator>().Play("Running Forward Flip");
        {
            isGrounded = true;
        }
    }
}
