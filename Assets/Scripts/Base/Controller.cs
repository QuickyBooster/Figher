using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{

    Rigidbody rigidBody;
    Camera viewCamera;
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        viewCamera = Camera.main;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
