using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : IEnemyState
{
    public BaseEnemy enemy;
    public IdleState(BaseEnemy enemy) { this.enemy = enemy; }
    public void OnStateEnter()
    {
    }

    public void OnStateExecute()
    {
    }

    public void OnStateExit()
    {
    }
}
