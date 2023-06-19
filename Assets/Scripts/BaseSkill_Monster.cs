using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using NaughtyAttributes;
using TMPro;

public class BaseCompetance_Monster : MonoBehaviour
{
    //========
    //VARIABLES
    //========

    [SerializeField] private Slider UI_skillCooldown;
    [SerializeField] private TextMeshProUGUI UI_skillCooldownText;
    public Monster_StateMachine monster_StateMachine;

    [Header("In-Game ")]
    public bool isSkillOnCooldown = false;
    [SerializeField] private float cooldownMaxTimer = 10;
    [SerializeField] private GameObject previewSpell;

    [Header("Delay in Milliseconds")]
    [SerializeField] protected int timeBeforeTheAttack = 1000;
    [SerializeField] protected int timeOfTheAttack = 1000;
    [SerializeField] protected int timeOfMonsterStunWhenAttack = 1000;
    private float cooldownCurrentTimer = 0;
    private float _lookTargetRotation;
    protected bool isAttacking = false;
    [SerializeField] private AttackAnim animationTrigger;
    public bool canUsedSkill = true;

    public float CooldownTimer { get { return cooldownCurrentTimer; } set 
        { 
            cooldownCurrentTimer = value;
            UI_skillCooldown.value = cooldownCurrentTimer / 100;
            float secondsLeft = cooldownMaxTimer - (cooldownCurrentTimer / 10);

            if (secondsLeft > 0 && secondsLeft < cooldownMaxTimer)
                UI_skillCooldownText.text = secondsLeft.ToString().Substring(0, 3);
            else 
                UI_skillCooldownText.text = "";
        } 
    }

    //========
    //MONOBEHAVIOUR
    //========
    private void Update()
    {
        //if(!isAttacking) SetDirectionMouse();
        if (isSkillOnCooldown)
        {
            SkillRecharge();
        }
    }
    //========
    //FONCTION
    //========

    public void ShowPreviewOfSpell(bool value = true)
    {
        previewSpell.SetActive(value);
    }

    /// <summary>
    /// Input action of mouse position from player input in monster. (deliver a Vector2)
    /// </summary>
    /// <param name="ctx"></param>
    public void MousePosition(InputAction.CallbackContext ctx)
    {
        if(!isAttacking)
        SetDirectionMouse();
    }

    /// <summary>
    /// Set the rotation of the gameObject to the direction from him to mouse.
    /// </summary>
    private void SetDirectionMouse()
    {
        GetDirectionFromObjToMouse();
        Vector3 _direction = new Vector3(0, _lookTargetRotation, 0);
        transform.localRotation = Quaternion.Euler(_direction);
    }

    /// <summary>
    /// Set the lootTargetRotation used to make the Transform rotation of the gameObject.
    /// </summary>
    private void GetDirectionFromObjToMouse()
    {
        // target player to it's screen position.
        Vector3 objPosInViewport = Camera.main.WorldToScreenPoint(transform.position);


        // direction of the vector from player to mouse.
        Vector2 _direction = new Vector2(objPosInViewport.x - Input.mousePosition.x, objPosInViewport.y - Input.mousePosition.y);

        // calculate Y rotation from the direction.
        _lookTargetRotation = Mathf.Atan2(_direction.x, _direction.y) * Mathf.Rad2Deg + 180f;
    }


    public void StartingUsingSkill(InputAction.CallbackContext context)
    {
        if (isSkillOnCooldown) return;
        if (context.ReadValue<float>() == 1 && canUsedSkill)
        {
            if (monster_StateMachine.isSkillBeingCast) return;
            monster_StateMachine.isSkillBeingCast = true;
            Monster_StateMachine.whenSkillHaveToBeUsed += UseSkill;
            Monster_StateMachine.whenSkillHaveToBeUsed += CastingSkill;
            ShowPreviewOfSpell(true);
        }
        if (monster_StateMachine.isSkillBeingCast && context.ReadValue<float>() == 0)
        {
            Monster_StateMachine.whenSkillHaveToBeUsed?.Invoke();
        }

    }
    public void CastingSkill()
    {
        ShowPreviewOfSpell(false);
        monster_StateMachine.isSkillBeingCast = false;
        Monster_StateMachine.whenSkillHaveToBeUsed -= UseSkill;
        Monster_StateMachine.whenSkillHaveToBeUsed -= CastingSkill;
        isSkillOnCooldown = true;
    }

    [Button]
    public void UseSkill()
    {
        SkillFonction();
        Monster_Skills.whenASkillIsUsed?.Invoke(timeOfMonsterStunWhenAttack, animationTrigger);
    }
    protected virtual void SkillFonction()
    {
        Debug.LogWarning("No Skill Register");
    }
    private void SkillRecharge()
    {
        if (cooldownCurrentTimer >= 100)
        {
            isSkillOnCooldown = false;
            CooldownTimer = 0;
        }
        else
        {
            CooldownTimer += Time.deltaTime * 100 / cooldownMaxTimer;
        }
    }
}
