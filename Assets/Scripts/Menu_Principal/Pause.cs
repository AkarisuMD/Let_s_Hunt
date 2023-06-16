using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{
    public Controls _controls;
    [SerializeField] private GameObject pauseObj;
    [SerializeField]
    private JoinGame_Manager joinGame_Manager;
    private bool canOpen => joinGame_Manager.isTheGameStarted.Value;

    private void Start()
    {
        _controls = new();
        _controls.Enable();
        _controls.Menu.Pause.started += ctx => OpenPause();
    }

    private void OpenPause()
    {
        if (!canOpen) return;

        pauseObj.SetActive(!pauseObj.active);
    }
}
