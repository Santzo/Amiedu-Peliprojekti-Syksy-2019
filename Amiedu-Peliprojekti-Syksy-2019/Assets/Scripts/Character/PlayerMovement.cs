using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D rb;
    Camera mainCam;
    Vector2 movement;
    private bool movementPossible = true;
    float moveSpeed = 5f;


    private int HandleVertical
    {
        get
        {
            if (Input.GetKey(KeyboardConfig.up[0]) || Input.GetKey(KeyboardConfig.up[1])) return 1;
            if (Input.GetKey(KeyboardConfig.down[0]) || Input.GetKey(KeyboardConfig.down[1])) return -1;
            return 0;
        }
    }

    private int HandleHorizontal
    {
        get
        {
            if (Input.GetKey(KeyboardConfig.right[0]) || Input.GetKey(KeyboardConfig.right[1])) return 1;
            if (Input.GetKey(KeyboardConfig.left[0]) || Input.GetKey(KeyboardConfig.left[1])) return -1;
            return 0;
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

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
        

        movement = new Vector2(HandleHorizontal, HandleVertical);
    }

    void TempStuff() // VÄLIAIKAINEN METODI - MUISTA POISTAA KUN PELI ON VALMIS
    {
        CharacterStats.ResetStats();
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyboardConfig.inventory[0]) || Input.GetKeyDown(KeyboardConfig.inventory[1]))
        {
            Events.inventoryKey();
            movementPossible = !movementPossible;
        }
    }


}
