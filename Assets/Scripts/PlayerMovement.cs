using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using System;

public class PlayerMovement : MonoBehaviour
{
    public float playerSpeed = 2;
    void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * playerSpeed, Space.World);
    }
}
