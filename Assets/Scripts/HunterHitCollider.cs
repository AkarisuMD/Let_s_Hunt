using System.Collections;
using System.Collections.Generic;
using System;
using Unity.Netcode;
using UnityEngine;
using NaughtyAttributes;

public class HunterHitCollider : NetworkBehaviour
{
    [SerializeField] private Player_Animator player_Animator;
    private NetworkVariable<int> indexPlayer = new NetworkVariable<int>(0);
    
    /// <summary>
    /// If collider got hit, transfert info to player.
    /// </summary>
    [ClientRpc]
    public void HunterGetHitClientRpc(int Damage)
    {
        // change the healthbar
        if (IsHost) return; // Monster don't have this.


        HealthBarManager.Instance.ChangeHealthBar(indexPlayer.Value, Damage);
    }
    public void HitFeedback()
    {
        player_Animator.HitFeedback();
    }
    public void StunHunter()
    {
        Debug.Log(indexPlayer.Value + "is Stun");
    }
    public void DeactivateFlashLightForXMillisecondSecond(int milliseconds)
    {
        Debug.Log(indexPlayer.Value + "has no flashlight");
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetPlayerIdServerRpc(int newPlayerId)
    {
        indexPlayer.Value = newPlayerId;
        SetColorToHunter();
    }
    public bool isThePlayerDodging()
    {
        Animator animator = player_Animator.GetPlayerAnimator(0);
        return animator.GetCurrentAnimatorStateInfo(0).IsName("Dodge");
    }
    public int GetPlayerId()
    {
        return indexPlayer.Value;
    }
    public void SetColorToHunter()
    {
        player_Animator.SetHunterColorViaIdClientRpc(indexPlayer.Value);
    }
}
