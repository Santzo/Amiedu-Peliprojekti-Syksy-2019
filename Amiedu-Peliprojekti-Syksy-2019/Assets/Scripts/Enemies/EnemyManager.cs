using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public float deltaFixedTimer;
    public static EnemyManager instance;

    private void Awake()
    {
        instance = this;
    }

    private void FixedUpdate()
    {
        deltaFixedTimer += Time.fixedDeltaTime;
    }
 

}
