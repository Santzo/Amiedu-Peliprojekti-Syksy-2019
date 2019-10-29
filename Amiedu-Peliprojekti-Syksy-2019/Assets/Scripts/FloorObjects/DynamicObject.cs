using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SortingGroup))]
[RequireComponent(typeof(SpriteRenderer))]
public class DynamicObject : MonoBehaviour
{
    protected Rigidbody2D rb;
    protected Coroutine co;
    protected SortingGroup sgroup;
    protected Transform shadow;
    protected Vector3 shadowOffset;
    protected float spriteBoundsY;

    private bool objectIsMoving = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sgroup = GetComponent<SortingGroup>();
        shadow = transform.GetChild(0);
        //shadow.transform.SetParent(null);
        //shadowOffset = transform.position - shadow.position;
        spriteBoundsY = GetComponent<SpriteRenderer>().bounds.extents.y;
    }

    private void Update()
    {
        if (Time.frameCount % 5 != 0 || objectIsMoving)
            return;
        if (rb.velocity.magnitude > 0.1f)
            UpdateSortLayer();
    }

    public void UpdateSortLayer()
    {
        if (co != null) StopCoroutine(co);
        co = StartCoroutine(UpdateLayer());
    }

    private IEnumerator UpdateLayer()
    {
        objectIsMoving = true;
        while (rb.velocity.magnitude > 0.1f)
        {
            sgroup.sortingOrder = Info.SortingOrder(transform.position.y - spriteBoundsY);
            yield return null;
        }
        sgroup.sortingOrder = Info.SortingOrder(transform.position.y - spriteBoundsY);
        objectIsMoving = false;
    }

    private IEnumerator ResetRotation()
    {
     
        while (rb.rotation != 0f)
        {
            rb.rotation = 0f;
        }
        Debug.Log("Rotation reset");
        yield return null;
    }

 
}
