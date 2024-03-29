﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]

public class MeleeWeaponHit : MonoBehaviour
{
    private BoxCollider2D co2d;
    private Collider2D[] results;
    private ContactFilter2D filter;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        co2d = GetComponent<BoxCollider2D>();
        results = new Collider2D[2];
        filter = new ContactFilter2D();
        filter.useTriggers = false;
        filter.SetLayerMask(LayerMask.GetMask("StaticObject") | LayerMask.GetMask("MeleeCollider") | LayerMask.GetMask("EnemyHitbox"));
    }

    public void CheckForCollision()
    {
        int a = rb.OverlapCollider(filter, results);
        if (a > 0 && results != null)
        {
            References.rf.playerMovement.MeleeWeaponHit(results, co2d);
        }
        
    }

}
