using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;

public class ActivateObjectByDirection : NetworkBehaviour
{
    [SerializeField] private GameObject up_Object;
    [SerializeField] private GameObject left_Object;
    [SerializeField] private GameObject right_Object;
    [SerializeField] private GameObject down_Object;

    [SerializeField] private bool isMonster = true;
    [SerializeField] private NavMeshAgent navMesh;
    [SerializeField] private Tps_PlayerController tps_Controller;

    [SerializeField] private float buffer = 0.5f;
    [SerializeField] private bool debug = false;

    private List<GameObject> listOfDirection = new();
    public void SetDebug(bool value) => debug = value;
    private void Awake()
    {
        listOfDirection.Add(up_Object);
        listOfDirection.Add(left_Object);
        listOfDirection.Add(right_Object);
        listOfDirection.Add(down_Object);

        UpdateDirection(new Vector2 (0, -1));
    }
    private void Update()
    {
        if (!IsOwner && !debug) return;
        Vector3 direction;

        if (isMonster)
        { direction = navMesh.desiredVelocity;

            UpdateDirection(new(direction.x, direction.z));
        }
    }
    public void UpdateDirection(Vector2 newDirection)
    {
        if (!IsOwner && !debug) return;
        List<GameObject> objectToDisable = new() { up_Object, left_Object, right_Object, down_Object };
        bool isUpdated = false;
        GameObject directionToActivate = null;
        float absY = Mathf.Abs(newDirection.y);
        float absX = Mathf.Abs(newDirection.x);

        int idOfObjectToDiable = 0;

        if (debug) print(absY.ToString()+ " : " + absX.ToString() + "= Direction");
        if (absY > absX)
        {
            if(debug) print("Y is big");
            if (Mathf.Abs(newDirection.y) > buffer)
            {
                if (newDirection.y < 0)
                {
                    directionToActivate = down_Object;
                    idOfObjectToDiable = 3;
                    objectToDisable.Remove(down_Object);
                    isUpdated = true;
                }
                if (newDirection.y > 0)
                {
                    directionToActivate = up_Object;
                    idOfObjectToDiable = 0;
                    objectToDisable.Remove(up_Object);
                    isUpdated = true;
                }
            }

        }
        else
        {
            if (debug) print("X is big");
            if(Mathf.Abs(newDirection.x) > buffer)
            {
                if(newDirection.x < 0)
                {
                    directionToActivate = left_Object;
                    idOfObjectToDiable = 1;
                    objectToDisable.Remove(left_Object);
                    isUpdated = true;
                }
                if (newDirection.x > 0)
                {
                    directionToActivate = right_Object;
                    idOfObjectToDiable = 2;
                    objectToDisable.Remove(right_Object);
                    isUpdated = true;
                }
            }
        }

        if(isUpdated) 
        {
            SetActiveObjectInNetworkCServerRpc(0, false);
            SetActiveObjectInNetworkCServerRpc(1, false);
            SetActiveObjectInNetworkCServerRpc(2, false);
            SetActiveObjectInNetworkCServerRpc(3, false);

            SetActiveObjectInNetworkCServerRpc(idOfObjectToDiable, true);

            foreach(GameObject gameObject in listOfDirection)
            {
                SetEnableToAllRendererOfGameObject(gameObject, false);
            }
            SetEnableToAllRendererOfGameObject(listOfDirection[idOfObjectToDiable], true);
        }
    }
    [ServerRpc]
    private void SetActiveObjectInNetworkCServerRpc(int objectToDeactivate, bool value)
    {
        if (!IsOwner)
            SetEnableToAllRendererOfGameObject(listOfDirection[objectToDeactivate], value);
        //Set To Other Client
        SetActiveObjectInNetworkClientRpc(objectToDeactivate, value);
    }
    [ClientRpc]
    private void SetActiveObjectInNetworkClientRpc(int objectToDeactivate, bool value)
    {
        if (!IsOwner)
            SetEnableToAllRendererOfGameObject(listOfDirection[objectToDeactivate], value);
    }
    private static void SetEnableToAllRendererOfGameObject(GameObject gameObject, bool value)
    {
        List<SpriteRenderer> spriteRenderers = gameObject.GetComponentsInChildren<SpriteRenderer>().ToList();
        foreach(SpriteRenderer sprite in  spriteRenderers)
        {
            sprite.enabled = value;
        }
        List<TrailRenderer> trailRenderers = gameObject.GetComponentsInChildren<TrailRenderer>().ToList();
        foreach (TrailRenderer trail in trailRenderers)
        {
            trail.enabled = value;
        }
    }
}
