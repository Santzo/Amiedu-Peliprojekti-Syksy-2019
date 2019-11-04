using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerMovement : MonoBehaviour
{
    public class PlayerAnimations
    {
        private float moveAnim;
        private bool sprinting;
        PlayerMovement pm;
        public PlayerAnimations(PlayerMovement pm)
        {
            this.pm = pm;
        }
        public float MoveAnim
        {
            get
            {
                return moveAnim;
            }
            set
            {
                if (value == moveAnim) return;
                moveAnim = value;
                pm.UpdateMoveAnimation();
            }
        }
        public bool Sprinting
        {
            get
            {
                return sprinting;
            }
            set
            {
                if (value == sprinting) return;
                sprinting = value;
                pm.UpdateMoveAnimation();
            }
        }
    }
    Rigidbody2D rb;
    [HideInInspector]
    public Rigidbody2D weaponRb;
    Camera mainCam;
    Animator anim;
    Vector2 movement;
    Vector3 oriScale;
    public MeleeWeaponHit meleeWeapon;
    SortingGroup sortingGroup;
    private bool movementPossible = true;
    PlayerAnimations pa;
    private bool activeAttackFrames, attacking;
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
        pa = new PlayerAnimations(this);
    }
    void Start()
    {
        mainCam = Camera.main;
        TempStuff(); // VÄLIAIKAINEN METODI - MUISTA POISTAA KUN PELI ON VALMIS
        References.rf.healthBar.ChangeValues(CharacterStats.health, CharacterStats.maxHealth);
        References.rf.staminaBar.ChangeValues(CharacterStats.stamina, CharacterStats.maxStamina);

    }

    private void FixedUpdate()
    {
        if (!movementPossible)
            return;
        rb.MovePosition(rb.position + movement * CharacterStats.moveSpeed * Time.deltaTime);

    }
    // Update is called once per frame
    void Update()
    {
        if (activeAttackFrames)
        {
            if (meleeWeapon != null)
            {
                meleeWeapon.CheckForCollision();
            }
        }

        HandleInput();
        if (!movementPossible)
            return;
        Vector2 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        transform.localScale = mousePos.x > transform.position.x ? oriScale : new Vector3(-oriScale.x, oriScale.y, oriScale.z);
        movement = pa.Sprinting ? new Vector2(HandleHorizontal, HandleVertical) * CharacterStats.movementSpeedMultiplier : new Vector2(HandleHorizontal, HandleVertical);
        pa.MoveAnim = movement != Vector2.zero ? pa.MoveAnim >= 1f ? 1f : pa.MoveAnim += 0.05f : 0f;
        sortingGroup.sortingOrder = Info.SortingOrder(transform.position.y);
    }

    void UpdateMoveAnimation()
    {
        anim.SetFloat("Movement", pa.MoveAnim);
        anim.SetFloat("MovementMultiplier", pa.Sprinting ? CharacterStats.movementSpeedMultiplier : 1f);
    }

    void TempStuff() // VÄLIAIKAINEN METODI - MUISTA POISTAA KUN PELI ON VALMIS
    {
        CharacterStats.ResetStats();
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyboardConfig.inventory[0]) || Input.GetKeyDown(KeyboardConfig.inventory[1]))
        {
            ResetAnimations();
            Events.inventoryKey();
            movementPossible = !movementPossible;
        }

        if (!movementPossible) return;

        if (!Input.GetKey(KeyboardConfig.sprint[0]) && !Input.GetKey(KeyboardConfig.sprint[1]))
        {
            pa.Sprinting = false;
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
            if (!attacking && CharacterStats.characterEquipment.weapon != null)
            {
                attacking = true;
                StartCoroutine("WaitForAttack");
                anim.SetTrigger("Attack");
            }
        }

        if (Input.GetKey(KeyboardConfig.sprint[0]) || Input.GetKey(KeyboardConfig.sprint[1]))
        {
            if (CharacterStats.stamina <= 0f)
            {
                pa.Sprinting = false;
                return;
            }
            CharacterStats.stamina -= 0.5f;
            pa.Sprinting = true;
            References.rf.staminaBar.UpdateValue(CharacterStats.stamina);

        }
    }

    public void MeleeWeaponHit(Collider2D[] collisions, Collider2D position)
    {
        if (!activeAttackFrames) return;
        Vector2? hitPosition = null;
        foreach (var col in collisions)
        {
            if (col != null)
            {
                Vector2 colBounds = new Vector2(col.transform.position.y - col.bounds.extents.y * 3f, col.transform.position.y + 0.15f);
                if (transform.position.y > colBounds.x && transform.position.y < colBounds.y)
                {
                    hitPosition = position.ClosestPoint(col.transform.position);
                }
            }
        }
        if (hitPosition == null) return;
        ObjectPooler.op.Spawn("ObjectMeleeHit", hitPosition);
        activeAttackFrames = false;
    }

    public void ActivateAttackFrames(int activate)
    {
        activeAttackFrames = activate == 1;
    }
    private void ResetAnimations()
    {
        anim.speed = 1f;
        anim.SetFloat("Movement", 0f);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
    }
    private IEnumerator WaitForAttack()
    {
        yield return new WaitForSeconds(1f / CharacterStats.characterEquipment.weapon.fireRate + 0.02f);
        activeAttackFrames = false;
        attacking = false;
    }
}
