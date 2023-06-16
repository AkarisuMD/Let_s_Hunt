using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using NaughtyAttributes;
using Unity.Mathematics;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Data/Player")]
public class ScS_PlayerData : ScriptableObject
{
    #region singleton
    private static ScS_PlayerData instance;
    public static ScS_PlayerData Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<ScS_PlayerData>("ManagerSingleton/PlayerData");
            }
            return instance;
        }
    }
    #endregion

    /// <summary>
    /// All state active and used of the controller.
    /// </summary>
    public PlayerMonitor monitor;
    public void SetNewMonitor() { monitor = new(); }
    /// <summary>
    /// struct to control Player.
    /// </summary>
    [Serializable]
    public struct PlayerMonitor
    {
        /// <summary>
        /// Index of the player in the network.
        /// </summary>
        public int index;
        /// <summary>
        /// can Player start working.
        /// </summary>
        public bool isValid;
        /// <summary>
        /// State of player is changing.
        /// </summary>
        public bool isChangingState;
        /// <summary>
        /// is Player initialized.
        /// </summary>
        public bool isInit;
        /// <summary>
        /// is Player correctly working.
        /// </summary>
        public bool isActive;

        // check if player want to move and can move.
        public bool tryToMove;
        public bool canMove;
        public bool isMoving => tryToMove && canMove;

        // check if player want to dodge and can dodge.
        public bool tryToDodge;
        public bool canDodge;
        public bool isDodging => tryToDodge && canDodge;

        // check if player want to Attack1 and can Attack1.
        public bool tryToAtk1;
        public bool canAtk1;
        public bool isAtk1 => tryToAtk1 && canAtk1;

        // check if player want to Attack2 and can Attack2.
        public bool tryToAtk2;
        public bool canAtk2;
        public bool isAtk2 => tryToAtk2 && canAtk2;

        /// <summary>
        /// Used for hitCooldown to not get directly kill easily.
        /// </summary>
        public bool canGetHit;

        /// <summary>
        /// Is player stun (if true the can't do any action, if hit, return to false)
        /// </summary>
        public bool isStun;

        public bool canHeal;
        public bool canGetHealed;

        /// <summary>
        /// Someone is reviving player.
        /// </summary>
        public bool getRevive;

        /// <summary>
        /// is Player ded. obviously.
        /// </summary>
        public bool isDead;
    }

    /// <summary>
    /// All state active and used of the controller.
    /// </summary>
    public PlayerVariables variables;
    public void SetNewVariables() { variables = new(); }
    /// <summary>
    /// struct to control Player.
    /// </summary>
    [Serializable]
    public struct PlayerVariables
    {
        public Vector3 playerPosition;
        public Vector3 playerRotation;
        public float currentDashCooldownTimer;
        public float currentAttaqueTime;


        public float speed;
    }

    /// <summary>
    /// struct to Data of the controller.
    /// </summary>
    public InGamePlayerGameDataValue inGameDataValue;
    public void SetNewInGameDataValue() { inGameDataValue = new(); }
    /// <summary>
    /// struct to Data of the controller.
    /// </summary>
    [Serializable]
    public struct InGamePlayerGameDataValue
    {
        [Space(10)]
        [Tooltip("Move speed of the character in m/s (Default = 2.0f)")]
        public float speed;

        [Tooltip("Sprint speed of the character in m/s (Default = 5.335f)")]
        public float sprintSpeed;

        [Tooltip("Acceleration and deceleration (Default = 10.0f)")]
        public float speedChangeRate;

        [Tooltip("Dash Force")]
        public float dashForce;

        [Tooltip("Delay between 2 Dash")]
        public float dashCooldown;

        [Tooltip("Time to god mode cooldown when got hit")]
        public float hitCooldown;

        [Tooltip("Hp of player")]
        public int hp;

        [Tooltip("Time to get revived")]
        public int reviveTime;

        [Tooltip("maximum Hp of player")]
        public int maxHp;

        [Tooltip("The flow of the attack 1")]
        public int atk1Damage;

        [Tooltip("The flow of the attack 1")]
        public float atk1Speed;

        [Tooltip("The flow of the attack 1")]
        public float atk1MovementSpeed;

        [Tooltip("The flow of the attack 1")]
        public int atk1Delay;

        [Tooltip("The flow of the attack 1")]
        public int atk1DelayBeforeIdle;

        [Tooltip("The flow of the attack 1")]
        public int atk2Damage;

        [Tooltip("The flow of the attack 1")]
        public float atk2Speed;

        [Tooltip("The flow of the attack 1")]
        public float atk2MovementSpeed;

        [Tooltip("The flow of the attack 1")]
        public int atk2Delay;

        [Tooltip("The flow of the attack 1")]
        public int atk2DelayBeforeIdle;
    }

    /// <summary>
    /// Clear no importante data.
    /// </summary>
    public void ClearInGameData() { }

    /// <summary>
    /// Modify hp (positive value if heal, negative value if damage) and return the real modified value.
    /// </summary>
    /// <param name="value"></param>
    public int ChangeHp(int value)
    {
        int _hp = inGameDataValue.hp;

        inGameDataValue.hp = Mathf.Clamp(inGameDataValue.hp + value, 0, inGameDataValue.maxHp);
        if (inGameDataValue.hp == 0) PlayerDied();

        int hpToReturn = _hp - inGameDataValue.hp;
        return hpToReturn;
    }

    /// <summary>
    /// Player got 0hp left.
    /// </summary>
    public void PlayerDied()
    {
        monitor.isDead = true;
        Tps_PlayerController.Instance.Died();
    }

    // ==================================================
    // need cloud save here

    /// <summary>
    /// Persitant Value of the player.
    /// </summary>
    public PersistentPlayerGameDataValue persistentPlayerGameData;
    public void SetNewPersistentPlayerGameData() { persistentPlayerGameData = new(); }
    /// <summary>
    /// Persitant Value of the player.
    /// </summary>
    [Serializable]
    public struct PersistentPlayerGameDataValue
    {
        public float globalTimeMove;
    }

    /// <summary>
    /// WARNING, clear data definitly !!!
    /// </summary>
    public void ClearPersistantData() { }
}
