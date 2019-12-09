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
    [HideInInspector]
    public Rigidbody2D rb;
    protected Coroutine co;
    [HideInInspector]
    public SortingGroup sgroup;
    protected Vector3 shadowOffset;
    protected float spriteBoundsY;
    public Vector3 lastPos;

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
}
