using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    public IEnemyState currentState;

    public void ChangeState(IEnemyState newState)
    {
        if (currentState != null)
            currentState.OnStateExit();
        currentState = newState;
        currentState.OnStateEnter();
    }

}