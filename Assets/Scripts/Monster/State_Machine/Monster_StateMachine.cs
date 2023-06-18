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
    [SerializeField] private LayerMask groundLayer;

    public bool isAOEBeingCast = false;

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
    public void OnMonsterClick(InputAction.CallbackContext context)
    {
        if (context.ReadValue<float>() == 0) return;

        (Vector3, GameObject) newDestination = GetMouseWorldPosition(groundLayer);
        
        Debug.Log("Monster Click Detected / Position = " + newDestination);
        if (newDestination.Item2 == null) return;

        if(isAOEBeingCast)
        {
            CastAOE(newDestination.Item1);
            isAOEBeingCast = false;
        }

        switch(newDestination.Item2.tag)
        {
            case "Corpse":
                return;
            default:
                //Movement
                monster_Movement.OnClickMovement(newDestination.Item1);
                break;
        }
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
    }
    private void CastAOE(Vector3 position)
    {

    }
    private static (Vector3, GameObject) GetMouseWorldPosition(LayerMask layerMask)
    {
        //using old Inputs System
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, layerMask))
        {
            return (raycastHit.point, raycastHit.collider.gameObject);
        }
        else
        {
            return (Vector3.zero, null);
        }
    }
}
