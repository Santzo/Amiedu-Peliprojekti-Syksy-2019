
using UnityEngine;
using UnityEngine.Rendering;

public class InteractableObject : MonoBehaviour
{
    InteractableObjectText io;
    GameObject obj;
    float spriteY;

    private void Awake()
    {
        spriteY = GetComponent<SpriteRenderer>().sprite.bounds.size.y;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerCollider"))
        {
            if (obj == null || !obj.activeSelf) obj = ObjectPooler.op.Spawn("InteractableObjectText", new Vector2(transform.position.x, transform.position.y + spriteY));
            io = obj.GetComponent<InteractableObjectText>();
            io.text.text = $"Press {TextColor.Return("green")}{KeyboardConfig.ReturnKeyName(KeyboardConfig.action[0].ToString())} {TextColor.Return()} to open the {name}.";
            io.ToggleTextActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerCollider"))
            if (io != null) io.ToggleTextActive(false);
    }
}
