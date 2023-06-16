using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Events;
using NaughtyAttributes;
using System;
using System.Threading.Tasks;

public class MonsterHitCollider : NetworkBehaviour

{
    private bool canGetHit = true;
    public static event Action<int> onMonsterHit;
    public Player_Animator player_Animator;

    [ServerRpc(RequireOwnership = false)]
    public void MonsterGetHitServerRpc(int damage)
    {
        if(canGetHit)
        onMonsterHit?.Invoke(damage);
    }
    public void FeedbackMonsterHit()
    {
        player_Animator.HitFeedback();
        print("yess");
    }
    public void GetStun()
    {

    }
    public async void GetMonsterInvincibleForXMiliseconds(int milliseconds)
    {
        if(canGetHit)
        {
            Debug.Log("Monster is Invicible for " + milliseconds * 100 + "seconds");
            canGetHit = false;
            await Task.Delay(milliseconds);
            Debug.Log("Monster end of invicible time");
            canGetHit = true;
        }
    }
}
