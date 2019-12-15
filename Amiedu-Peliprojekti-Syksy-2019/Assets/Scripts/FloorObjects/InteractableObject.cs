using UnityEngine;
using UnityEngine.Rendering;

public class InteractableObject : MonoBehaviour
{
    protected InteractableObjectText io;
    protected GameObject obj;
    protected bool inRange;
    protected bool interacting;
    protected ParticleSystem ps;
    float spriteX, spriteY;
    Animator anim;
    int triggerId;
    protected Transform actionTrigger;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        var sr = GetComponent<SpriteRenderer>();
        ps = GetComponentInChildren<ParticleSystem>();
        spriteY = sr.bounds.size.y * 1.25f;
        spriteX = sr.bounds.extents.x;
        triggerId = Animator.StringToHash("Action");
        actionTrigger = transform.Find("ActionTrigger");
        interacting = false;
        ps?.Stop();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerCollider"))
        {
            if (obj == null || !obj.activeSelf)
            {
                EnterRange();
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerCollider"))
        {
            LeaveRange();
        }
    }
    protected virtual void EnterRange()
    {
        inRange = true;
        obj = ObjectPooler.op.Spawn("InteractableObjectText", new Vector2(transform.position.x + spriteX, transform.position.y + spriteY));
        References.rf.currentInteractableObject = this;
    }
    protected virtual void LeaveRange()
    {
        inRange = false;
        if (io != null) io.ToggleTextActive(false);
        References.rf.currentInteractableObject = null;
        interacting = false;
    }
    public virtual void Interact()
    {
        if (interacting) return;
        if (io != null) io.ToggleTextActive(false);
        obj = null;
        io = null;
        anim.SetTrigger(triggerId);
        interacting = true;
        //Destroy(actionTrigger.gameObject);
    }
}
