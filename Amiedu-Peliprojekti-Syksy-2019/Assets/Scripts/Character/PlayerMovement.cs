using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D rb;
    Camera mainCam;
    Camera fogOfWarCam;
    Animator anim;
    Vector2 movement;
    Vector3 oriScale;
    SortingGroup sortingGroup;
    private bool movementPossible = true;
    float moveSpeed = 5f;
    float moveAnim;
    bool sprinting;

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
        oriScale = transform.localScale;
        sortingGroup = GetComponent<SortingGroup>();
        anim = GetComponent<Animator>();
        //anim.SetFloat("Movement", 0.5f);

    }
    void Start()
    {
        mainCam = Camera.main;
        fogOfWarCam = GameObject.Find("FogOfWarCamera").GetComponent<Camera>();
        fogOfWarCam.clearFlags = CameraClearFlags.Nothing;

        TempStuff(); // VÄLIAIKAINEN METODI - MUISTA POISTAA KUN PELI ON VALMIS
        References.rf.healthBar.ChangeValues(CharacterStats.health, CharacterStats.maxHealth);
        References.rf.staminaBar.ChangeValues(CharacterStats.stamina, CharacterStats.maxStamina);
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
        transform.localScale = mousePos.x > transform.position.x ? oriScale : new Vector3(-oriScale.x, oriScale.y, oriScale.z);
        movement = sprinting ? new Vector2(HandleHorizontal, HandleVertical) * CharacterStats.movementSpeedMultiplier : new Vector2(HandleHorizontal, HandleVertical);
        moveAnim = movement != Vector2.zero ? moveAnim >= 1f ? 1f : moveAnim += 0.05f : 0f;
        anim.SetFloat("Movement", moveAnim);
        anim.speed = sprinting ? CharacterStats.movementSpeedMultiplier : 1f;
        sortingGroup.sortingOrder = Info.SortingOrder(transform.position.y);
    }

    void TempStuff() // VÄLIAIKAINEN METODI - MUISTA POISTAA KUN PELI ON VALMIS
    {
        CharacterStats.ResetStats();
    }

    void HandleInput()
    {
        if (!Input.GetKey(KeyboardConfig.sprint[0]) && !Input.GetKey(KeyboardConfig.sprint[1]))
        {
            sprinting = false;
            if (CharacterStats.stamina < CharacterStats.maxStamina)
            {
                CharacterStats.stamina += 0.1f;
                if (CharacterStats.stamina > CharacterStats.maxStamina)
                    CharacterStats.stamina = CharacterStats.maxStamina;
                References.rf.staminaBar.UpdateValue(CharacterStats.stamina);
            }
        }

        if (!Input.anyKey) return;

        if (Input.GetKeyDown(KeyboardConfig.attack[0]) || Input.GetKeyDown(KeyboardConfig.attack[1]))
        {
            anim.SetTrigger("Attack");
        }
        if (Input.GetKey(KeyboardConfig.sprint[0]) || Input.GetKey(KeyboardConfig.sprint[1]))
        {
            if (CharacterStats.stamina <= 0f)
            {
                sprinting = false;
                return;
            }
            CharacterStats.stamina -= 0.5f;
            sprinting = true;
            References.rf.staminaBar.UpdateValue(CharacterStats.stamina);

        }


        if (Input.GetKeyDown(KeyboardConfig.inventory[0]) || Input.GetKeyDown(KeyboardConfig.inventory[1]))
        {
            ResetAnimations();
            Events.inventoryKey();
            movementPossible = !movementPossible;
        }
    }


    private void ResetAnimations()
    {
        anim.speed = 1f;
        anim.SetFloat("Movement", 0f);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
       //DynamicObject _do = collision.collider.GetComponent<DynamicObject>();
       // if (_do != null) _do.UpdateSortLayer(movement);
    }
}
