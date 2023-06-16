using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster_StateFactory
{
    private Dictionary<MonsterState, Monster_State> _statesList = new();
    protected Monster_StateMachine stateMachine;
    public Monster_StateFactory(Monster_StateMachine _stateMachine)
    {
        stateMachine = _stateMachine;

        _statesList.Add(MonsterState.BeforeGame, new MS_BeforeGame(stateMachine, this));
        _statesList.Add(MonsterState.OnStartGame, new MS_OnStartGame(stateMachine, this));
        _statesList.Add(MonsterState.Invisible, new MS_Invisible(stateMachine, this));
        _statesList.Add(MonsterState.Fight, new MS_Fight(stateMachine, this));
        _statesList.Add(MonsterState.Stun, new MS_Stun(stateMachine, this));
        _statesList.Add(MonsterState.Attack, new MS_Stun(stateMachine, this));
        _statesList.Add(MonsterState.Dead, new MS_BeforeGame(stateMachine, this));
    }
    public Monster_State GetAnyState(MonsterState stateToGet)
    {
        if(_statesList.TryGetValue(stateToGet, out Monster_State monster_State))
        {
            return monster_State;
        }
        Debug.LogError("Missing, State");
        return null;
    }
}
public enum MonsterState
{
    BeforeGame, OnStartGame, Invisible, Fight, Attack, Stun, Dead
}
