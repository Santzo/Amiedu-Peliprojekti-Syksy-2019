using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : IEnemyState
{
    public BaseEnemy enemy;
    public PatrolState(BaseEnemy enemy) { this.enemy = enemy; }

    public void OnStateEnter()
    {
    }

    public void OnStateExit()
    {
    }

    public void OnStateFixedUpdate()
    {

    }

    public void OnStateUpdate()
    {

    }
}
