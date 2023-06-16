using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cac_Competance_Monstre : BaseCompetance_Monster
{
    [SerializeField] private Animator animator;

    protected override async void SkillFonction()
    {
        isAttacking = true;
        animator.SetTrigger("OnAttack");

        await Task.Delay(timeOfTheAttack);

        isAttacking = false;
    }
}
