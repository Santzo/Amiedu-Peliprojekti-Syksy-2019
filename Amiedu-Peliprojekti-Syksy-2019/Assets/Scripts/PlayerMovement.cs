using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D rb;
    Transform light;
    Camera mainCam;
    Vector2 movement;
    float moveSpeed = 5f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        light = transform.GetChild(0);
    }
    void Start()
    {
        mainCam = Camera.main;
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.deltaTime);
    }
    // Update is called once per frame
    void Update()
    {
        Vector2 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        light.right = new Vector2(mousePos.x - light.position.x, mousePos.y - light.position.y);
        movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }
}
