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
        var hits = new List<ParticleSystem.Particle>();
        particles.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, hits);
        if (hits.Count == 0) return;
        foreach (var hit in hits)
        {
            var collision = Physics2D.OverlapCircle(new Vector2(hit.position.x, hit.position.y - 1f), 2f,layer);
            if (collision)
            {
                BaseEnemy enemy = collision.GetComponentInParent<BaseEnemy>();
                if (enemy != null && !enemy.hasBeenHit)
                {
                    enemy.OnGetHit(Info.CalculateDamage(enemy.stats));
                    StartCoroutine(enemy.HasBeenHit(Info.attackInterval));
                    break;
                }
            }

        }
    }
}
