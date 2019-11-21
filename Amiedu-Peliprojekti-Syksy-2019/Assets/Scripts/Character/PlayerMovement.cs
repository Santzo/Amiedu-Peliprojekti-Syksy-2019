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
    public ParticleSystem weaponTrailRenderer;
    [HideInInspector]
    public MeleeWeaponHit meleeWeapon;
    SortingGroup sortingGroup;
    private bool movementPossible = true;
    PlayerAnimations pa;
    public bool attacking, activeAttackFrames;
    public Transform mask;
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
        Events.onGameFieldCreated += RandomizePlayerPosition;
    }

    private void RandomizePlayerPosition()
    {
        var rooms = References.rf.levelGenerator.allRooms.ToArray();
        int x = 1000; int y = 1000;
        AllRooms startRoom = null;
        foreach (var room in rooms)
        {
            if (room.startX + room.startY < x + y)
            {
                startRoom = room;
                x = room.startX;
                y = room.startY;
            }
        }
        transform.position = new Vector2(x + 2, y + 2);
    }

    void Start()
    {
        mainCam = Camera.main;
        References.rf.healthBar.ChangeValues(CharacterStats.health, CharacterStats.maxHealth);
        References.rf.staminaBar.ChangeValues(CharacterStats.stamina, CharacterStats.maxStamina);
        ObjectPooler.op.SpawnDialogueBox(new Dialogue {talker =  "Testi", text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur." },
                                         new Dialogue {talker = "Testi 1", text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur." });
    }

    private void FixedUpdate()
    {
        if (!movementPossible)
            return;
        rb.MovePosition(rb.position + movement * CharacterStats.moveSpeed * Time.deltaTime);

    }

    void Update()
    {
        if (Events.onDiscard)
            return;

        HandleAttack();
        HandleInput();
        if (!movementPossible || Events.onDialogueBox)
            return;
        Vector2 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        transform.localScale = mousePos.x > transform.position.x ? oriScale : new Vector3(-oriScale.x, oriScale.y, oriScale.z);
        movement = pa.Sprinting ? new Vector2(HandleHorizontal, HandleVertical) * CharacterStats.movementSpeedMultiplier : new Vector2(HandleHorizontal, HandleVertical);
        HandleMovement();
    }

    private void HandleMovement()
    {
        pa.MoveAnim = movement != Vector2.zero ? pa.MoveAnim >= 1f ? 1f : pa.MoveAnim += 0.05f : 0f;
        sortingGroup.sortingOrder = Info.SortingOrder(transform.position.y);
        mask.transform.position = References.rf.playerEquipment.LOSCircle.position;
    }

    void HandleAttack()
    {
        if (!attacking || Events.onDialogueBox) return;
        if (!movementPossible)
        {
            attacking = false;
            activeAttackFrames = false;
            if (weaponTrailRenderer != null)
                weaponTrailRenderer.Stop();
        }

        switch (CharacterStats.characterEquipment.weapon.weaponType)
        {
            case WeaponType.Melee:
                if (activeAttackFrames)
                {
                    float frame = anim.GetCurrentAnimatorStateInfo(2).normalizedTime;
                    if (weaponTrailRenderer != null)
                    {
                        if (!weaponTrailRenderer.isPlaying && frame > 0.4f)
                            weaponTrailRenderer.Play();
                        if (!weaponTrailRenderer.isPlaying && frame > 0.9f)
                            weaponTrailRenderer.Stop();

                    }
                    if (meleeWeapon != null && frame > 0.7f)
                    {

                        meleeWeapon.CheckForCollision();
                    }
                }
                break;
            case WeaponType.Flamethrower:
                if (!Input.GetKey(KeyboardConfig.attack[0]) && !Input.GetKey(KeyboardConfig.attack[1]) || CharacterStats.gasAmmo <= 0)
                {
                    attacking = false;
                    activeAttackFrames = false;
                    weaponTrailRenderer.Stop();
                    anim.SetTrigger("StopAttack");
                    StopCoroutine("CalculateGasAmmo");
                }
                break;
        }
    }
    void UpdateMoveAnimation()
    {
        anim.SetFloat("Movement", pa.MoveAnim);
        anim.SetFloat("MovementMultiplier", pa.Sprinting ? CharacterStats.animationSprintMoveSpeed : CharacterStats.animationBaseMoveSpeed);
    }

    void HandleInput()
    {
        if (Events.onDialogueBox)
        {
            if (Input.GetKeyDown(KeyboardConfig.action[0]) || Input.GetKeyDown(KeyboardConfig.action[1]) || Input.GetKeyDown(KeyCode.Return))
            {
                References.rf.currentDialogueBox.SkipDialogue();
            }
            RegenerateStamina();
            return;
        }
     

        if (Input.GetKeyDown(KeyboardConfig.inventory[0]) || Input.GetKeyDown(KeyboardConfig.inventory[1]))
        {
            activeAttackFrames = false;
            attacking = false;
            ResetAnimations();
            Events.inventoryKey();
            movementPossible = !movementPossible;
        }

        if (!movementPossible)
        {
            RegenerateStamina();
            return;
        }

        if (!Input.GetKey(KeyboardConfig.sprint[0]) && !Input.GetKey(KeyboardConfig.sprint[1]))
        {
            pa.Sprinting = false;
            RegenerateStamina();
        }

        if (!Input.anyKey) return;

        if (Input.GetKeyDown(KeyboardConfig.attack[0]) || Input.GetKeyDown(KeyboardConfig.attack[1]))
        {
            if (!attacking && CharacterStats.characterEquipment.weapon != null && CharacterStats.stamina > 0f)
            {
                if (CharacterStats.characterEquipment.weapon.weaponType == WeaponType.Melee)
                {
                    CharacterStats.stamina -= CharacterStats.characterEquipment.weapon.staminaCost;
                    attacking = true;
                    activeAttackFrames = true;
                    StopCoroutine("WaitForMeleeAttack");
                    StartCoroutine("WaitForMeleeAttack");
                    anim.SetTrigger("Attack");
                    anim.SetTrigger("StopMeleeAttack");
                }
                else if (CharacterStats.characterEquipment.weapon.weaponType == WeaponType.Flamethrower && CharacterStats.gasAmmo > 0)
                {
                    attacking = true;
                    activeAttackFrames = true;
                    weaponTrailRenderer.Play();
                    anim.SetTrigger("Attack");
                    StartCoroutine("CalculateGasAmmo");
                }
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

    void RegenerateStamina()
    {
        if (CharacterStats.stamina >= CharacterStats.maxStamina) return;
        CharacterStats.stamina += CharacterStats.staminaRegenerationRate;
        if (CharacterStats.stamina > CharacterStats.maxStamina)
            CharacterStats.stamina = CharacterStats.maxStamina;
        References.rf.staminaBar.UpdateValue(CharacterStats.stamina);

    }
    public void MeleeWeaponHit(Collider2D[] collisions, Collider2D position)
    {
        Vector2? hitPosition = null;
        GameObject obj = null;
        foreach (var col in collisions)
        {
            if (col != null)
            {
                Vector2 colBounds = new Vector2(col.transform.position.y - col.bounds.extents.y * 3f, col.transform.position.y + 0.15f);
                if (transform.position.y > colBounds.x && transform.position.y < colBounds.y)
                {
                    hitPosition = position.ClosestPoint(col.transform.position);
                    obj = col.gameObject;
                    break;
                }
            }
        }
        if (hitPosition == null) return;
        string layer = LayerMask.LayerToName(obj.layer);
        switch (layer)
        {
            case "EnemyHitbox":
                var be = obj.GetComponentInParent<BaseEnemy>();
                be.OnGetHit(Info.CalculateDamage(be.stats));
                ObjectPooler.op.Spawn("BloodSplatter", hitPosition, Quaternion.Euler(0f, 0f, 205f));
                break;
            default:
                ObjectPooler.op.Spawn("ObjectMeleeHit", hitPosition);
                break;
        }
        activeAttackFrames = false;
    }

    private void ResetAnimations()
    {
        anim.speed = 1f;
        anim.SetFloat("Movement", 0f);
    }

    private IEnumerator WaitForMeleeAttack()
    {
        yield return new WaitForSeconds(Info.attackInterval + 0.05f);
        attacking = false;
        activeAttackFrames = false;
        if (weaponTrailRenderer != null && weaponTrailRenderer.isPlaying)
            weaponTrailRenderer.Stop();
    }
    private IEnumerator CalculateGasAmmo()
    {
        while (true)
        {
            CharacterStats.gasAmmo -= 1;
            References.rf.weaponSlot.UpdateAmmoText();
            yield return new WaitForSeconds(Info.attackInterval);
            if (CharacterStats.gasAmmo < 0) CharacterStats.gasAmmo = 0;
        }
    }
}
