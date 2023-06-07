using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;
using BrunoMikoski.AnimationSequencer;

public class Monster_Skills : MonoBehaviour
{
    //========
    //VARIABLES
    //========
    [SerializeField] private List<BaseCompetance_Monster> listOfCompetance = new();
    [SerializeField] private AnimationSequencerController skillUIAnimationController;
    public static Action<int> whenASkillIsUsed;

    //========
    //MONOBEHAVIOUR
    //========

    //========
    //FONCTION
    //========
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
