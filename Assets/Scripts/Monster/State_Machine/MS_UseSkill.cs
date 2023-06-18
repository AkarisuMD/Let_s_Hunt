using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MS_UseSkill : Monster_State
{
    public MS_UseSkill(Monster_StateMachine _stateMachine, Monster_StateFactory _factory) : base(_stateMachine, _factory)
    {

    }
    public override async void EnterState()
    {
        Debug.Log("Monster is Attacking");
        stateMachine.monster_Input.enabled = false;

        await Task.Delay(stateMachine.timeOfTheAttackInMillisecond);
        SwitchState(factory.GetAnyState(MonsterState.Fight));
    }
    public override void ExitState()
    {
        Debug.Log("Monster Has Finish Attacking");
    }
}
