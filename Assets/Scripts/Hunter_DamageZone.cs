using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Hunter_DamageZone : NetworkBehaviour
{
    public int damage;
    public GameObject hitParticule;
    public Vector2 offsetHit = new(1, -2);
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
            Vector3 positionOfHit = other.ClosestPoint(transform.position);

            positionOfHit = new(positionOfHit.x, offsetHit.x, other.transform.position.z + offsetHit.y);
            GameObject hitFeedback = Instantiate(hitParticule, positionOfHit, Quaternion.identity);
            Destroy(hitFeedback, 3f);
        }
    }
}
