using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MS_Stun : Monster_State
{
    public MS_Stun(Monster_StateMachine _stateMachine, Monster_StateFactory _factory) : base(_stateMachine, _factory)
    {

    }
    public override async void EnterState()
    {
        Debug.Log("Monster is Stun");
        stateMachine.monster_Input.enabled = false;
        stateMachine.Navmesh.destination = stateMachine.monster_Movement.transform.position;
        stateMachine.Navmesh.speed = 0;

        await Task.Delay(stateMachine.stunTimeInMillisecond);
        SwitchState(factory.GetAnyState(MonsterState.Fight));
    }
    public override void ExitState()
    {
        stateMachine.monster_Input.enabled = true;
        stateMachine.Navmesh.speed = 1;
        Debug.Log("Monster Is Un-Stun");
    }
}
