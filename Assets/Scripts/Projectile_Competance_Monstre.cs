using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Projectile_Competance_Monstre : BaseCompetance_Monster
{
    [SerializeField] private ParticleSystem startParticule;
    [SerializeField] private Collider colliderToActivate;
    [SerializeField] private GameObject mainObject;
    [SerializeField] private int numberOfAttack = 10;
    protected override async void SkillFonction()
    {
        isAttacking = true;
        startParticule.Play();

        Transform lastParent = mainObject.transform.parent;
        transform.parent = transform.parent.parent.parent;

        float lastY = transform.position.y;

        transform.position = PositionToMouse.GetMouseWorldPosition(-1);

        transform.position = new(transform.position.x, lastY, transform.position.z);

        await Task.Delay(timeBeforeTheAttack);

        for (int i = 0; i <= numberOfAttack; i++)
        {
            colliderToActivate.enabled = true;
            await Task.Delay(timeOfTheAttack / numberOfAttack / 2);
            colliderToActivate.enabled = false;
            await Task.Delay(timeOfTheAttack / numberOfAttack / 2);
        }


        transform.parent = lastParent;
        isAttacking = false;
    }
}
