using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : BaseEnemy
{
    float growlFrequency;
    float timer;

    protected override void Start()
    {
        base.Start();
        growlFrequency = Random.Range(3f, 5f);
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (state.currentState == patrolState || state.currentState == aggressiveState)
        {
            if (EnemyManager.instance.deltaFixedTimer - timer > growlFrequency)
            {
                if (DistanceToPlayer() > 40f) return;
                audioSource.PlaySound(stats.audio.growl, Mathf.Clamp((12f - DistanceToPlayer()) * 0.1f, 0.15f, 1f));
                timer = EnemyManager.instance.deltaFixedTimer;
            }
        }
    }
  
}
