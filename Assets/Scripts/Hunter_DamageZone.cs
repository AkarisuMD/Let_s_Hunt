using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Hunter_DamageZone : NetworkBehaviour
{
    public int damage;
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<HunterHitCollider>(out HunterHitCollider hunterCollider))
        {
            /*if(hunterCollider.isThePlayerDodging())
            {
                print("player dodge");
                return;
            }*/
            if(IsOwner)
            hunterCollider.HunterGetHitClientRpc(damage);
            hunterCollider.HitFeedback();
        }
    }
}
