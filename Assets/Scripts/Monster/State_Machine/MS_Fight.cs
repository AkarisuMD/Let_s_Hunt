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

        Monster_Skills.whenASkillIsUsed += OnSkillUsed;
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
        try
        {
            Monster_Skills.whenASkillIsUsed -= OnSkillUsed;
        } catch { }
        Debug.Log("End Fight State");
    }
    private void OnSkillUsed(int timeOfStunOfTheAttack, AttackAnim attackAnim)
    {


        stateMachine.stunTimeInMillisecond = timeOfStunOfTheAttack;

        switch (attackAnim)
        {
            case AttackAnim.Bite:
                stateMachine.player_Animator.AttackAnimator();
                break;
            case AttackAnim.Dash:
                stateMachine.player_Animator.DashAnimator();
                break;
        }

        SwitchState(factory.GetAnyState(MonsterState.Stun));

    }
}
