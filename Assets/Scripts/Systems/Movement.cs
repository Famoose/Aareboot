using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    // Update is called once per frame
    public float speed = 1;
    public float rotationSpeed = 20;

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        var rotation = 0f;

        if (Input.GetKey(KeyCode.A))
        {
            rotation += rotationSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.D))
        {
            rotation -= rotationSpeed * Time.deltaTime;
        }

        rb.angularVelocity += rotation;
        rb.MovePosition( rb.position + new Vector2(-transform.right.y, 0) * (speed * Time.deltaTime));
    }
}