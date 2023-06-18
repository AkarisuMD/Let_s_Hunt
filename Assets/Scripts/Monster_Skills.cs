using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;
using UnityEngine.InputSystem;
using BrunoMikoski.AnimationSequencer;

public class Monster_Skills : MonoBehaviour
{
    //========
    //VARIABLES
    //========
    [SerializeField] private List<BaseCompetance_Monster> listOfCompetance = new();
    [SerializeField] private AnimationSequencerController skillUIAnimationController;
    public static Action<int, AttackAnim> whenASkillIsUsed;

    //========
    //MONOBEHAVIOUR
    //========

    //========
    //FONCTION
    //========
    public void InputMousePosition(InputAction.CallbackContext context)
    {
        foreach(BaseCompetance_Monster competance_Monster in listOfCompetance)
        {
            competance_Monster.MousePosition(context);
        }
    }
    public void CanMonsterUseSkill(bool value)
    {
        if(value)
        {
            ActivateSkill();
            //ActivateSkillUI();
        }
        else
        {
            DeactivateAllSkill();
            //DeactivateSkillUI();
        }
    }
    private void ActivateSkill()
    {
        foreach (BaseCompetance_Monster skill in listOfCompetance)
        {
            skill.enabled = true;
        }
    }
    private void DeactivateAllSkill()
    {
        foreach(BaseCompetance_Monster skill in listOfCompetance)
        {
            skill.enabled = false;
        }
    }
    private void ActivateSkillUI()
    {
        skillUIAnimationController.SetPlayType(AnimationSequencerController.PlayType.Forward);
        skillUIAnimationController.Play();
    }
    private void DeactivateSkillUI()
    {
        skillUIAnimationController.SetPlayType(AnimationSequencerController.PlayType.Backward);
        skillUIAnimationController.Play();
    }
}
public enum AttackAnim
{
    Bite, Dash
}
