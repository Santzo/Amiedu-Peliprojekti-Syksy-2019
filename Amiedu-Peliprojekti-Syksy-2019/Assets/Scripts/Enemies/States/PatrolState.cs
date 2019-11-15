using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : IEnemyState
{
    public BaseEnemy enemy;
    public PatrolState(BaseEnemy enemy) { this.enemy = enemy; }
    private int index;

    public void OnStateEnter()
    {
        index = 0;
        UpdatePatrolPath();
    }

    public void OnStateExit()
    {
    }

    public void OnStateFixedUpdate()
    {
        if (enemy.path == null)
            return;

        if (enemy.targetIndex < enemy.path.Length - 1 || enemy.targetIndex == enemy.path.Length - 1 && enemy.rb.position != enemy.destination)
        {
            enemy.rb.position = enemy.ReturnNextPoint();
            enemy.sGroup.sortingOrder = Info.SortingOrder(enemy.sortingTransform.position.y);
        }
        else if (enemy.targetIndex == enemy.path.Length - 1 && enemy.rb.position == enemy.destination)
            UpdatePatrolPath();
    }

    public void OnStateUpdate()
    {

    }

    private void UpdatePatrolPath()
    {
        enemy.path = null;
        PathRequestManager.RequestPath(new PathRequest(false, enemy.rb.position, enemy.patrolPoints[index], enemy.OnPathFound));
        index = index + 1 < enemy.patrolPoints.Length ? index + 1 : 0;   
    }
}
