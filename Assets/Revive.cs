using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Revive : InteractableObject
{
    
    [SerializeField] private GameObject onCanPickUp;
    private Tps_PlayerController playerController;
    [SerializeField] private SphereCollider sphereCollider;
    [SerializeField] private float timerRevive = 1f;
    [SerializeField] private Slider slider;
    private Coroutine coroutine;
    private bool revive = false;
    void OnEnable()
    {
        isInteractable = false;
        sphereCollider.enabled = false;
        onCanPickUp.SetActive(false);
        slider.gameObject.SetActive(false);
    }

    [ClientRpc]
    public void IsActiveClientRpc()
    {
        playerController.GetComponentInParent<Tps_PlayerController>();
        isInteractable = true;
        sphereCollider.enabled = true;
    }

    [ClientRpc]
    public void IsInactiveClientRpc()
    {
        isInteractable = false;
        sphereCollider.enabled = false;

        if (Tps_PlayerController.Instance.interactableObjects.Contains(this))
            Tps_PlayerController.Instance.interactableObjects.Remove(this);

    }

    public override void Interact()
    {
        Tps_PlayerController.Instance.ReviveSomeone();
        if (coroutine != null) StopCoroutine(coroutine);
        coroutine = StartCoroutine(CheckRevive());
    }

    private IEnumerator CheckRevive()
    {
        slider.gameObject.SetActive(true);
        _GetReviveClientRpc();
        while (playerController.isInteracting)
        {
            slider.value += 1 / timerRevive * Time.deltaTime;
            yield return Time.deltaTime;

            if (slider.value == slider.maxValue)
            {
                _InteractClientRpc();
                revive = true;
                break;
            }
        }
        slider.value = 0;
        slider.gameObject.SetActive(false);
        Tps_PlayerController.Instance.ChangeStateToIdle();

        if (!revive) _StopGetReviveClientRpc();
        revive = false;
    }

    private void OnDisable()
    {
        if (coroutine != null) StopCoroutine(coroutine);
    }

    [ClientRpc]
    public void _InteractClientRpc()
    {
        if (playerController == null) return;

        playerController.Revive();
    }
    [ClientRpc]
    public void _GetReviveClientRpc()
    {
        if (playerController == null) return;

        //playerController.ReviveAnim();
    }
    [ClientRpc]
    public void _StopGetReviveClientRpc()
    {
        if (playerController == null) return;

        //playerController.StopReviveAnim();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isInteractable) return;

        if (other.TryGetComponent<HunterHitCollider>(out HunterHitCollider _hunterCollider))
        {
            if (!_hunterCollider.IsOwner) return;

            Tps_PlayerController.Instance.interactableObjects.Add(this);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<HunterHitCollider>(out HunterHitCollider _hunterCollider))
        {
            if (!_hunterCollider.IsOwner) return;

            if (Tps_PlayerController.Instance.interactableObjects.Contains(this))
                Tps_PlayerController.Instance.interactableObjects.Remove(this);

        }
    }
}
