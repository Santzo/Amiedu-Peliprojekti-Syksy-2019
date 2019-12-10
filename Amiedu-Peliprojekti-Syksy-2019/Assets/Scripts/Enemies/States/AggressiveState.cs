using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AggressiveState : IEnemyState
{
    public BaseEnemy enemy;
    public AggressiveState(BaseEnemy enemy) { this.enemy = enemy; }
    private int index;
    private float timer;

    public void OnStateEnter()
    {
        index = 0;
        UpdatePath(true);
    }

    public void OnStateExit()
    {
    }

    public void OnStateFixedUpdate()
    {
        timer += Time.fixedDeltaTime;
        if (timer > enemy.minPathUpdateTime)
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
    }

    public void OnStateUpdate()
    {

    }

    private void UpdatePath(bool nullPath = false)
    {
        if (nullPath) enemy.path = null;
        PathRequestManager.RequestPath(new PathRequest(false, enemy.rb.position, References.rf.playerEquipment.transform.position, enemy.OnPathFound));
        index = index + 1 < enemy.patrolPoints.Length ? index + 1 : 0;
        timer = 0f;
    }
}
