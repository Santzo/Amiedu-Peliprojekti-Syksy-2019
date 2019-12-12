using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AggressiveState : IEnemyState
{
    public BaseEnemy enemy;
    public AggressiveState(BaseEnemy enemy) { this.enemy = enemy; }
    private int index;
    private float timer, attackTimer;

    public void OnStateEnter()
    {
        index = 0;
        UpdatePath(true);
        RandomizeAttack();
    }

    public void OnStateExit()
    {
    }

    public void OnStateFixedUpdate()
    {
        timer += Time.fixedDeltaTime;
        
        if (timer > enemy.stats.minPathUpdateTime)
        {
            UpdatePath();
        }

        if (enemy.path == null)
            return;

        if (enemy.targetIndex < enemy.path.Length - 1 || enemy.targetIndex == enemy.path.Length - 1 && enemy.rb.position != enemy.destination)
        {
            enemy.rb.position = enemy.ReturnNextPoint();
            enemy.sGroup.sortingOrder = Info.SortingOrder(enemy.sortingTransform.position.y);
        }

        else if (enemy.targetIndex == enemy.path.Length - 1 && enemy.rb.position == enemy.destination)
            UpdatePath(true);

        attackTimer += Time.fixedDeltaTime;
        if (attackTimer > enemy.currentAttack.attackInterval)
        {
            if (enemy.currentAttack.attackRange != 0)
            {
                if (enemy.CheckRangeToPlayer(enemy.currentAttack.attackRange))
                {
                    enemy.state.ChangeState(enemy.attackState);
                }
            }
        }
    }

    public void OnStateUpdate()
    {

    }

    private void UpdatePath(bool nullPath = false)
    {
        if (nullPath) enemy.path = null;
        RandomizeAttack();
        PathRequestManager.RequestPath(new PathRequest(false, enemy.rb.position, References.rf.playerEquipment.transform.position, enemy.OnPathFound));
        index = index + 1 < enemy.patrolPoints.Length ? index + 1 : 0;
        timer = 0f;
    }
    private void RandomizeAttack()
    {
        if (enemy.currentAttack.attackRange != 0) return;
        Debug.Log("New attack selected");
        int atk = Random.Range(0, enemy.stats.attacks.Length);
        enemy.currentAttack = enemy.stats.attacks[atk];
        enemy.currentAttack.attackInterval += Random.Range(-0.15f, 0.15f);
        enemy.overrideController["BaseAttack"] = enemy.currentAttack.animation;
        attackTimer = 0f;
    }
}
