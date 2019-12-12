using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : IEnemyState
{
    public BaseEnemy enemy;
    private Vector2 target;
    private ContactFilter2D filter, attackFilter;
    public bool applyForce = false;
    bool hasHit;
    Collider2D collider, attackCol;
    public AttackState(BaseEnemy enemy, Collider2D collider, Collider2D attackCol)
    {   this.enemy = enemy; 
        this.collider = collider;
        this.attackCol = attackCol;
        filter.SetLayerMask(LayerMask.GetMask("StaticWall") | LayerMask.GetMask("StaticObject") | LayerMask.GetMask("DynamicObject"));
        attackFilter.SetLayerMask(LayerMask.GetMask("PlayerHitbox"));
    }

    public void OnStateEnter()
    {
        hasHit = false;
        SetTarget();
        applyForce = false;
        enemy.anim.SetFloat(enemy._animAttackSpeed, enemy.currentAttack.animationMultiplier);

        enemy.anim.SetTrigger("Attack");
    }

    public void OnStateExit()
    {

    }

    public void OnStateFixedUpdate()
    {
        if (applyForce)
        {
            enemy.sGroup.sortingOrder = Info.SortingOrder(enemy.sortingTransform.position.y);
            if (!enemy.anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
            {
                enemy.currentAttack.attackRange = 0;
                enemy.state.ChangeState(enemy.aggressiveState);
                return;
            }
            if (!hasHit)
            {
                RaycastHit2D[] hitResults = new RaycastHit2D[1];
                attackCol.Cast(Vector2.zero, attackFilter, hitResults);
                if (hitResults[0].collider != null)
                {
                    hasHit = true;
                    References.rf.playerMovement.OnGetHit(Info.CalculateEnemyDamage(enemy.currentAttack));
                }
            }
            RaycastHit2D[] results = new RaycastHit2D[1];
            collider.Cast(Vector2.zero, filter, results);
            if (results[0].collider != null) return;
        
            enemy.rb.position = Vector2.MoveTowards(enemy.rb.position, target, enemy.stats.moveSpeed * enemy.currentAttack.forwardForce * Time.deltaTime);
        }
    }

    public void OnStateUpdate()
    {

    }
    public void SetTarget()
    {
        var player = References.rf.playerEquipment.transform.position;
        var node = PathRequestManager.instance.grid.NodeFromWorldPoint(player);
        node = PathRequestManager.instance.grid.GetWalkableNeighbor(node);
        target = node.worldPosition;
        enemy.TurnEnemy(target.x);
    }

}
