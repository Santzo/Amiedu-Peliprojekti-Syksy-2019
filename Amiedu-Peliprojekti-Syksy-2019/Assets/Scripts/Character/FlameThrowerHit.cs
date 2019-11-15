using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameThrowerHit : MonoBehaviour
{
    ParticleSystem particles;
    LayerMask layer;

    private void Awake()
    {
        layer = LayerMask.GetMask("EnemyHitbox");
        particles = GetComponent<ParticleSystem>();
    }
    private void OnEnable()
    {
        Events.onEnemyHitboxesUpdated += UpdateEnemyList;
        UpdateEnemyList(Info.enemyHitboxes);
    }
    private void OnDisable()
    {
        Events.onEnemyHitboxesUpdated -= UpdateEnemyList;
    }

    private void UpdateEnemyList(List<Collider2D> obj)
    {
        for (int i = 0; i < obj.Count; i++)
        {
            particles.trigger.SetCollider(i, obj[i]);
        }
    }
    private void OnParticleTrigger()
    {
        if (!References.rf.playerMovement.activeAttackFrames) return;
       
        var hits = new List<ParticleSystem.Particle>();
        particles.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, hits);
        if (hits.Count == 0) return;
        foreach (var hit in hits)
        {
            var collision = Physics2D.OverlapCircle(hit.position, 0.4f,layer);
            if (collision)
            {
                BaseEnemy enemy = collision.GetComponentInParent<BaseEnemy>();
                if (enemy != null)
                {
                    enemy.OnGetHit(Info.CalculateDamage(enemy.stats));
                    StartCoroutine(References.rf.playerMovement.WaitForFlameThrowerAttack());
                    break;
                }
            }

        }
    }
}
