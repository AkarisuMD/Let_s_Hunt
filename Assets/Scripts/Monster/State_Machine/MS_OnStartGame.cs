using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class MS_OnStartGame : Monster_State
{
    public MS_OnStartGame(Monster_StateMachine _stateMachine, Monster_StateFactory _factory) : base(_stateMachine, _factory)
    {

    }
    public override async void EnterState()
    {
        stateMachine.monster_Camera.ChangeCameraState(MonsterCameraState.LockedCam);
        await Task.Delay(0);
        stateMachine.monster_Input.enabled = true;
        if (!stateMachine.IsMonsterCloseToHunter(stateMachine.maxDistanceForExitingTheFight))
        {
            SwitchState(factory.GetAnyState(MonsterState.Invisible));
        }
        else SwitchState(factory.GetAnyState(MonsterState.Fight));

        stateMachine.timeManager.StartTimeClientRpc();
    }
    public override void ExitState()
    {
        Debug.Log("Quiting Starting State");
    }
}
