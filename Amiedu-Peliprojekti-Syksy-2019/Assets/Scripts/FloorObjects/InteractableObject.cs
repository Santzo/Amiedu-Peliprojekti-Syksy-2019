using UnityEngine;
using UnityEngine.Rendering;

public class InteractableObject : MonoBehaviour
{
    protected InteractableObjectText io;
    protected GameObject obj;
    float spriteX, spriteY;
    Animator anim;
    int triggerId;
    Transform actionTrigger;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        var sr = GetComponent<SpriteRenderer>();
        spriteY = sr.bounds.size.y * 1.25f;
        spriteX = sr.bounds.extents.x;
        triggerId = Animator.StringToHash("Action");
        actionTrigger = transform.Find("ActionTrigger");
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerCollider"))
        {
            if (obj == null || !obj.activeSelf)
            {
                InRange();
            }
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
    protected virtual void InRange()
    {
        obj = ObjectPooler.op.Spawn("InteractableObjectText", new Vector2(transform.position.x + spriteX, transform.position.y + spriteY));
        References.rf.currentInteractableObject = this;
      
    }
    public virtual void Interact()
    {
        if (io != null) io.ToggleTextActive(false);
        obj = null;
        io = null;
        anim.SetTrigger(triggerId);
        //Destroy(actionTrigger.gameObject);
    }
}
