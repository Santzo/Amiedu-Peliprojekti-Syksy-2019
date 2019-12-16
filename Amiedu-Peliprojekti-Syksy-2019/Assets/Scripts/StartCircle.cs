using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartCircle : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("Left the starting circle");
        References.rf.enemyManager.ActivateEnemies();
        Destroy(gameObject);
    }
}
