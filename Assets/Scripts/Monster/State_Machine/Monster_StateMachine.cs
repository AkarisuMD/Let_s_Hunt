using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class Monster_StateMachine : MonoBehaviour
{
    //========
    //VARIABLES
    //========

    [Header("State Machine")]
    public Monster_State currentState;
    private Monster_StateFactory factory;
    public string nameOfTheCurrentState;

    [Header("Ref")]
    public Monster_Movement monster_Movement;
    public Monster_Camera monster_Camera;
    public Monster_Skills monster_Skills;
    public PlayerInput monster_Input;
    public Animator monster_Animator;
    public MonsterHitCollider monsterHitCollider;
    public TimeManager timeManager;
    public Player_Animator player_Animator;
    public Monster_Hider monster_Hider;
    public Transform MonsterTransform => monster_Movement.transform; 
    public NavMeshAgent Navmesh => monster_Movement.navMeshAgent;

    [Header("Stun Variable")]
    public int stunTimeInMillisecond = 10;

    [Header("Hunter Dectetion Fight Mode")]
    public float maxDistanceForEnteringInFight = 10;
    public float maxDistanceForExitingTheFight = 15;
    [SerializeField] private LayerMask playerLayer;

    [Header("Attack Variable")]
    public int timeOfTheAttackInMillisecond;

    //========
    //MONOBEHAVIOUR
    //========

    private void Start()
    {
        factory = new Monster_StateFactory(this);

        currentState = factory.GetAnyState(MonsterState.BeforeGame);
        currentState.EnterState();

        //Dead
        MonsterHealth.whenTheMonsterDied += SetMonsterStateToDead;
    }

    private void Update()
    {
        if(currentState != null)
        {
            currentState.UpdateState();
        }
        UpdateSpeedAnimator();
    }

    private void OnDestroy()
    {
        MonsterHealth.whenTheMonsterDied -= SetMonsterStateToDead;
    }

    //========
    //FONCTION
    //========
    public Component GetComponent(Component component)
    {
        return GetComponentInChildren(component.GetType());
    }
    public void SetMonsterStateToActive()
    {
        currentState.SwitchState(factory.GetAnyState(MonsterState.OnStartGame));
    }
    public bool IsMonsterCloseToHunter(float distance)
    {
        Collider[] colliders = Physics.OverlapSphere(MonsterTransform.position, distance, playerLayer);
        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent<HunterHitCollider>(out _))
            {
                return true;
            }
        }
        return false;
    }
    private void SetMonsterStateToDead()
    {
        currentState.SwitchState(factory.GetAnyState(MonsterState.Dead));
    }
    private void UpdateSpeedAnimator()
    {
        player_Animator.SendSpeedToAnimator(Navmesh.velocity.magnitude);
        print(Navmesh.velocity.magnitude + "Navmesh");
    }
}
