using System;
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
    public Rigidbody2D rb;
    protected Coroutine co;
    protected SortingGroup sgroup;
    protected Vector3 shadowOffset;
    protected float spriteBoundsY;

    private bool objectIsMoving = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sgroup = GetComponent<SortingGroup>();
        spriteBoundsY = GetComponent<SpriteRenderer>().bounds.extents.y * 0.5f;
        CreateStaticCollider();
    }
    private void Start()
    {
        FloorObjectManager.instance.Add(this);
        sgroup.sortingOrder = Info.SortingOrder(transform.position.y);
    }
    private void CreateStaticCollider()
    {
        GameObject staticStats = new GameObject("StaticCollider");
        staticStats.transform.SetParent(transform, false);
        var bc = staticStats.AddComponent<BoxCollider2D>();
        bc.size = GetComponent<BoxCollider2D>().size;
        bc.offset = GetComponent<BoxCollider2D>().offset;
        staticStats.layer = LayerMask.NameToLayer("StaticObject");
    }

    public void UpdateSortLayer()
    {
        if (objectIsMoving) return;
        if (co != null) StopCoroutine(co);
        co = StartCoroutine(UpdateLayer());
    }

    private IEnumerator UpdateLayer()
    {
        objectIsMoving = true;
        while (rb.velocity.magnitude > 0.1f)
        {
            sgroup.sortingOrder = Info.SortingOrder(transform.position.y);
            yield return null;
        }
        sgroup.sortingOrder = Info.SortingOrder(transform.position.y);
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
