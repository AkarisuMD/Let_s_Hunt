using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using NaughtyAttributes;
using DG.Tweening;
using UnityEditor;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;
public class Player_Animator : NetworkBehaviour
{
    public bool debug = false;
    public void SetDebug(bool value) => debug = value;
    [SerializeField] private List<Animator> animatorToSendSpeed;
    [SerializeField] private Monster_StateMachine monster_Statemachine;
    [SerializeField] private float buffer = 0.2f;
    [SerializeField] private string alphaMaterialParameterName = "_Alpha";

    private List<SpriteRenderer> allTheSpriteRender = new();
    private List<Material> spritesMaterials = new();
    private Tween onHitChange;
    private NavMeshAgent navMesh => monster_Statemachine.monster_Movement.navMeshAgent;
    private int numberOfParrelIsHitIsPlayed = 0;
    private void OnEnable()
    {
        //MonsterHitCollider.onMonsterHit += HitFeedback;

    }
    private void OnDisable()
    {
        //MonsterHitCollider.onMonsterHit -= HitFeedback;
    }
    private void Awake()
    {
        allTheSpriteRender = gameObject.GetComponentsInChildren<SpriteRenderer>().ToList();
        foreach(SpriteRenderer spriteRenderer in allTheSpriteRender)
        {
            spritesMaterials.Add(spriteRenderer.material);
        }
    }
    [Button]
    public async void HitFeedback(int damage = 0)
    {
        numberOfParrelIsHitIsPlayed++;

        HitFeedback(true);

        await System.Threading.Tasks.Task.Delay(500);

        if (numberOfParrelIsHitIsPlayed <= 1)
        {
            HitFeedback(false);
        }
        numberOfParrelIsHitIsPlayed--;
    }
    public void HitFeedback(bool value)
    {
        if(value)
        {
            foreach (Material material in spritesMaterials)
            {
                material.SetFloat("_IsHit", 1);
            }
        }
        else
        {
            foreach (Material material in spritesMaterials)
            {
                material.SetFloat("_IsHit", 0);
            }
        }
    }
    public void SendSpeedToAnimator(float speed)
    {
        speed = Mathf.Clamp(speed, 0, 1);
        foreach (Animator animator in animatorToSendSpeed)
        {
            animator.SetFloat("speed", speed);
        }
    }
    public void AttackAnimator()
    {
        foreach (Animator animator in animatorToSendSpeed)
        {
            animator.GetComponent<ClientNetworkAnimator>().SetTrigger("whenAttack");
            if(debug) animator.SetTrigger("whenAttack");
        }
    }
    public void DashAnimator()
    {
        foreach (Animator animator in animatorToSendSpeed)
        {
            animator.GetComponent<ClientNetworkAnimator>().SetTrigger("whenDodge");
            if (debug) animator.SetTrigger("whenDodge");
        }
    }
    public void DeathAnimator()
    {
        foreach (Animator animator in animatorToSendSpeed)
        {
            animator.GetComponent<ClientNetworkAnimator>().SetTrigger("whenDied");
        }
    }
    public Animator GetPlayerAnimator(int index)
    {
        if (animatorToSendSpeed.Count > index)
        {
            return animatorToSendSpeed[index];
        }
        else return animatorToSendSpeed[0];
    }

    [ClientRpc]
    public void SetHunterColorViaIdClientRpc(int idPlayer)
    {
        switch(idPlayer)
        {
            case 1:
                foreach(Material material in spritesMaterials)
                {
                    material.SetFloat("_IsFirstplayer", 1);
                }
                break;
            case 2:
                foreach (Material material in spritesMaterials)
                {
                    material.SetFloat("_IsSecondPlayer", 1);
                }
                break;
            case 3:
                foreach (Material material in spritesMaterials)
                {
                    material.SetFloat("_IsThirdPlayer", 1);
                }
                break;
            case 4:
                foreach (Material material in spritesMaterials)
                {
                    material.SetFloat("_IsFourthPlayer", 1);
                }
                break;
            default:
                return;
        }
    }
    public void SetUpdateTime(float value)
    {
        foreach (Animator animator in animatorToSendSpeed)
        {
            animator.speed = value;
        }
    }
}
