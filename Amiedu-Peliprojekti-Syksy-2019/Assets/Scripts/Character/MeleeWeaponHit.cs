using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeaponHit : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        References.rf.playerMovement.MeleeWeaponHit(collision.GetContact(0).point);
    }
}
