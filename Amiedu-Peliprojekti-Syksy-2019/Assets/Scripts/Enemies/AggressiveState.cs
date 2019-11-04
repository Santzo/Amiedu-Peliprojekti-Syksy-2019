using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AggressiveState : IEnemyState
{
    public BaseEnemy enemy;
    public AggressiveState(BaseEnemy enemy) { this.enemy = enemy; }
    public void OnStateEnter()
    {
    }

    public virtual void OnStateExecute()
    {
    }

    public void OnStateExit()
    {
    }
}
