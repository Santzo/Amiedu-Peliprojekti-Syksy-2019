using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeaponHit : MonoBehaviour
{
    private BoxCollider2D co2d;
    private Collider2D[] results;
    private ContactFilter2D filter;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        co2d = GetComponent<BoxCollider2D>();
        results = new Collider2D[3];
        results.Populate();
        filter = new ContactFilter2D();
        filter.useTriggers = false;
    }

    public void CheckForCollision()
    {
        int a = GetComponent<Rigidbody2D>().OverlapCollider(filter, results);
        Debug.Log(name + " " + a);
        if (a > 0 && results != null)
        {
            Debug.Log(results[0].name);
        }
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        References.rf.playerMovement.MeleeWeaponHit(collision.GetContact(0).point);
    }
}
