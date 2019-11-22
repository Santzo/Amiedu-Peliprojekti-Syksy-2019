﻿
using UnityEngine;
using UnityEngine.Rendering;

public class InteractableObject : MonoBehaviour
{
    InteractableObjectText io;
    GameObject obj;
    float spriteX, spriteY;
    Animator anim;
    int triggerId;
    bool activated = false;
    Transform actionTrigger;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        var sr = GetComponent<SpriteRenderer>();
        spriteY = sr.sprite.bounds.size.y * 1.5f * transform.localScale.x;
        spriteX = sr.sprite.bounds.extents.x * transform.localScale.x;
        triggerId = Animator.StringToHash("Action");
        actionTrigger = transform.Find("ActionTrigger");
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerCollider"))
        {
            if (obj == null || !obj.activeSelf)
            {
                obj = ObjectPooler.op.Spawn("InteractableObjectText", new Vector2(transform.position.x + spriteX, transform.position.y + spriteY));
            }
            References.rf.currentInteractableObject = this;
            io = obj.GetComponent<InteractableObjectText>();
            io.text.text = $"Press {TextColor.Return("green")}{KeyboardConfig.ReturnKeyName(KeyboardConfig.action[0].ToString())} {TextColor.Return()} to open the {name}.";
            io.ToggleTextActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerCollider"))
        {
            if (io != null) io.ToggleTextActive(false);
            References.rf.currentInteractableObject = null;
        }
    }

    public void Interact()
    {
        if (io != null) io.ToggleTextActive(false);
        anim.SetTrigger(triggerId);
        Destroy(actionTrigger.gameObject);
    }
}
