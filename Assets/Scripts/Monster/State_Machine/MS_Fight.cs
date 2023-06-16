using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MS_Fight : Monster_State
{
    public MS_Fight(Monster_StateMachine _stateMachine, Monster_StateFactory _factory) : base(_stateMachine, _factory)
    {

    }
    public override void EnterState()
    {
        stateMachine.monster_Camera.ChangeCameraState(MonsterCameraState.LockedCam);
        stateMachine.monster_Movement.ChangeSpeed(MonsterSpeed.Fight);
        stateMachine.monster_Skills.CanMonsterUseSkill(true);
        stateMachine.monster_Hider.alphaOnHide = 0.15f;
        stateMachine.monster_Hider.RefreshHide();
    }
    public override void UpdateState()
    {
        if (!stateMachine.IsMonsterCloseToHunter(stateMachine.maxDistanceForExitingTheFight))
        {
            SwitchState(factory.GetAnyState(MonsterState.Invisible));
        }
    }
    public override void ExitState()
    {
        Debug.Log("End Fight State");
    }
}
