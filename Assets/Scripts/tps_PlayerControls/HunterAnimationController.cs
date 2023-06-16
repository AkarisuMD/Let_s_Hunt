using BrunoMikoski.AnimationSequencer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HunterAnimationController : MonoBehaviour
{
    [SerializeField] private Tps_PlayerController playerController;
    private bool isOwner 
    { 
        get 
        {
            if (playerController != null) return playerController.enabled;
            else return false;
        } 
    }

    [SerializeField] private GameObject anim_Healing;

    //==============================================================================================================
    #region PRIVATE FONCTION
    //==============================================================================================================


    private void ChangeStateToIdle() 
    {
        if(isOwner)
        playerController.ChangeStateToIdle(); 
    }
    private void StartDodge() 
    {
        if (isOwner)
            playerController.StartDodge(); 
    }
    private void EndDodge() 
    {
        if (isOwner)
            playerController.EndDodge(); 
    }
    private void ATK1() { if (isOwner) playerController.Atk1(); }
    private void ATK2() { if (isOwner) playerController.Atk2(); }
    private void StartHealing() { 
        anim_Healing.SetActive(true);
        AnimationSequencerController _asc = anim_Healing.GetComponentInChildren<AnimationSequencerController>();
        _asc.ResetToInitialState();
        _asc.Play();
    }
    private void EndHealing() {
        anim_Healing.GetComponentInChildren<AnimationSequencerController>().OnFinishedEvent.AddListener(EndAnimHealing);
        Debug.Log("Pass Heal");
        if (!isOwner) return; 

        playerController.playerData.ChangeHp(10);
        HealthBarManager.Instance.ChangeHealthBar(playerController.playerData.monitor.index, 10);
    }
    private void EndAnimHealing()
    {
        anim_Healing.GetComponentInChildren<AnimationSequencerController>().OnFinishedEvent.RemoveAllListeners();
        anim_Healing.SetActive(false);
    }


    // AUDIO

    private void AudioOnFootstep()
    {

    }

    private void AudioOnDodge()
    {

    }

    private void AudioOnATK1()
    {

    }

    private void AudioOnATK2()
    {

    }

    private void AudioOnGetHit()
    {

    }

    private void AudioOnDeath()
    {

    }

    private void AudioOnRevive()
    {

    }

    #endregion
    //==============================================================================================================
}
