using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D rb;
    Transform light;
    Camera mainCam;
    Vector2 movement;
    private bool movementPossible = true;
    float moveSpeed = 5f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        light = transform.GetChild(0);
    }
    void Start()
    {
        mainCam = Camera.main;
        TempStuff(); // VÄLIAIKAINEN METODI - MUISTA POISTAA KUN PELI ON VALMIS
    }

    private void FixedUpdate()
    {
        if (!movementPossible)
            return;
        rb.MovePosition(rb.position + movement * moveSpeed * Time.deltaTime);
    }
    // Update is called once per frame
    void Update()
    {
        HandleInput();
        if (!movementPossible)
            return;
        Vector2 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        transform.localScale = mousePos.x > transform.position.x ? new Vector3(1f, 1f, 1f) : new Vector3(-1f, 1f, 1f);
        light.right = new Vector2(mousePos.x - light.position.x, mousePos.y - light.position.y) * transform.localScale.x;
        movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    void TempStuff() // VÄLIAIKAINEN METODI - MUISTA POISTAA KUN PELI ON VALMIS
    {
        CharacterStats.ResetStats();
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyboardConfig.inventory))
        {
            Events.inventoryKey();
            movementPossible = !movementPossible;
        }
    }
}
