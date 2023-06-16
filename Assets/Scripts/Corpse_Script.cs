using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Unity.Netcode;
using NaughtyAttributes;

public class Corpse_Script : NetworkBehaviour
{

    //=========
    //VARIABLE
    //=========

    [SerializeField, Required] private GameObject objectToActivateWhenMonsterNear;
    [SerializeField, Required] private GameObject objectToActivateWhenMonsterUseCorpse;
    [SerializeField, Required] private GameObject objectToDeactivateWhenMonsterUseCorpse;
    [SerializeField] private float secondToRemoveWhenUsed = 5;
    [SerializeField] private float maxDistanceBeforeUse = 5;
    [SerializeField] private bool debug = false;
    private bool isMonsterNear => 
        Vector3.Distance(transform.position, GameObject.FindAnyObjectByType<MonsterHitCollider>().transform.position) < maxDistanceBeforeUse;

    //========
    //MONOBEHAVIOUR
    //========

    private void OnMouseEnter()
    {
        if (!IsHost) return;
        objectToActivateWhenMonsterNear.SetActive(true);
    }
    private void OnMouseExit()
    {
        if (!IsHost) return;
        objectToActivateWhenMonsterNear.SetActive(false);
    }
    private void OnMouseDown()
    {
        if (!IsHost) return;
        TryToUse();
    }

    //=========
    //FONCTION
    //=========
    [Button]
    public void TryToUse()
    {
        if (isMonsterNear)
            OnUse();
        else
        {
            print("Can't Use");
            UI_Message_Manager.Instance.ShowMessage(Color.red, "Too far away");
        }
    }

    [Button]
    public void OnUse()
    {
        TimeManager timeManager = GameObject.FindAnyObjectByType<TimeManager>();
        timeManager.RemoveSecondOfTimeClientRpc(secondToRemoveWhenUsed);
        objectToActivateWhenMonsterUseCorpse.SetActive(true);
        objectToDeactivateWhenMonsterUseCorpse.SetActive(false);
        Destroy(this);
        objectToActivateWhenMonsterNear.SetActive(false);
    }
}
