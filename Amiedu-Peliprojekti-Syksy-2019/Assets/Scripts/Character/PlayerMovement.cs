﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

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
    [HideInInspector]
    public GameObject head;
    [HideInInspector]
    public ParticleSystem weaponTrailRenderer;
    [HideInInspector]
    public MeleeWeaponHit meleeWeapon;
    SortingGroup sortingGroup;
    private bool movementPossible = true;
    PlayerAnimations pa;
    public bool attacking, activeAttackFrames, activeObjectAttackFrames;
    public Transform mask;

    int _attack, _stopMeleeAttack, _movement, _movementMultiplier, _stopAttack;

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
        head = transform.GetFromAllChildren("Head").gameObject;
        pa = new PlayerAnimations(this);
        mainCam = Camera.main;

        _attack = Animator.StringToHash("Attack");
        _movement = Animator.StringToHash("Movement");
        _movementMultiplier = Animator.StringToHash("MovementMultiplier");
        _stopMeleeAttack = Animator.StringToHash("StopMeleeAttack");
        _stopAttack = Animator.StringToHash("StopAttack");
    }

    void Start()
    {
        Audio.PlayOnLoop("BackgroundMusic", 0f, 0.95f);
        Audio.VolumeFade("BackgroundMusic", 0f, 0.35f, 5f);
        sortingGroup.sortingOrder = Info.SortingOrder(transform.position.y);
        ObjectPooler.op.SpawnDialogueBox(head, new Dialogue { talker = CharacterStats.name, text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur."});
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
                    anim.SetTrigger(_stopAttack);
                    StopCoroutine("CalculateGasAmmo");
                }
                break;
        }
    }
    void UpdateMoveAnimation()
    {
        anim.SetFloat(_movement, pa.MoveAnim);
        anim.SetFloat(_movementMultiplier, pa.Sprinting ? CharacterStats.animationSprintMoveSpeed : CharacterStats.animationBaseMoveSpeed);
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

        if (Input.GetKeyDown(KeyboardConfig.action[0]) || Input.GetKeyDown(KeyboardConfig.action[1]))
        {
            if (References.rf.currentInteractableObject == null) return;
            References.rf.currentInteractableObject.Interact();
        }

        if (Input.GetKeyDown(KeyboardConfig.attack[0]) || Input.GetKeyDown(KeyboardConfig.attack[1]))
        {
            if (!attacking && CharacterStats.characterEquipment.weapon != null && CharacterStats.Stamina > 0f)
            {
                if (CharacterStats.characterEquipment.weapon.weaponType == WeaponType.Melee)
                {
                    CharacterStats.Stamina -= CharacterStats.characterEquipment.weapon.staminaCost;
                    attacking = true;
                    activeAttackFrames = true;
                    StopCoroutine("WaitForMeleeAttack");
                    StartCoroutine("WaitForMeleeAttack");
                    anim.SetTrigger(_attack);
                    anim.SetTrigger(_stopMeleeAttack);
                }
                else if (CharacterStats.characterEquipment.weapon.weaponType == WeaponType.Flamethrower && CharacterStats.gasAmmo > 0)
                {
                    attacking = true;
                    activeAttackFrames = true;
                    weaponTrailRenderer.Play();
                    anim.SetTrigger(_attack);
                    StartCoroutine("CalculateGasAmmo");
                }
            }
        }

        if (Input.GetKey(KeyboardConfig.sprint[0]) || Input.GetKey(KeyboardConfig.sprint[1]))
        {
            if (CharacterStats.Stamina <= 0f)
            {
                pa.Sprinting = false;
                return;
            }
            CharacterStats.Stamina -= 0.5f;
            pa.Sprinting = true;
        }
    }

    void RegenerateStamina()
    {
        if (CharacterStats.Stamina >= CharacterStats.MaxStamina) return;
        CharacterStats.Stamina += CharacterStats.staminaRegenerationRate * Time.deltaTime;
        if (CharacterStats.Stamina > CharacterStats.MaxStamina)
            CharacterStats.Stamina = CharacterStats.MaxStamina;

    }
    public void MeleeWeaponHit(Collider2D[] collisions, Collider2D position)
    {
        Vector2? hitPosition = null;
        GameObject obj = null;
        foreach (var col in collisions)
        {
            if (col != null && !col.CompareTag("EnemyAttackBox"))
            {
                Vector2 colBounds = new Vector2(col.transform.position.y - 0.4f, col.transform.position.y + col.bounds.size.y);
                Debug.Log(transform.position.y + " vs " + colBounds.x + ", " + colBounds.y);
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
                if (be.isDead) return;
                int dmg = Info.CalculateDamage(be.stats, out bool crit);
                be.OnGetHit(dmg, crit);
                ObjectPooler.op.Spawn("BloodSplatter", hitPosition, Quaternion.Euler(0f, 0f, 205f));
                activeAttackFrames = false;
                break;
            default:
                if (activeObjectAttackFrames) return;
                ObjectPooler.op.Spawn("ObjectMeleeHit", hitPosition);
                activeObjectAttackFrames = false;
                break;
        }
        
    }

    private void ResetAnimations()
    {
        anim.speed = 1f;
        anim.SetFloat(_movement, 0f);
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
    public void OnGetHit(int damage)
    {
        GameObject obj = ObjectPooler.op.Spawn("DamageText", new Vector2(head.transform.position.x, head.transform.position.y + 0.75f));
        DamageText dm = obj.GetComponent<DamageText>();
        dm.text.text = $"{TextColor.Red}{damage.ToString()}";
        CharacterStats.Health -= damage;
    }
    public void PlayFootStep()
    {
        Audio.PlaySound("FootStep", Random.Range(0.15f, 0.2f), Random.Range(0.95f, 1f));
    }
    public void PlayWeaponSwing()
    {
        Audio.PlaySound("WeaponSwing");
    }
}
