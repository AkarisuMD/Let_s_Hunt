using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapManager : Singleton<MiniMapManager>
{
    public bool mapTuto;
    
    // Reference
    [SerializeField] public GameObject canvas; 
    [SerializeField] private RectTransform _MM_UI;
    [SerializeField] private Camera _camera;
    [SerializeField] private GameObject _monster;
    [SerializeField] private Transform _monsterCamera;
    [SerializeField] private bool isHunter = false;
    [SerializeField] private RectTransform _MM_MonsterCamera;
    [SerializeField] private GameObject monsterZone_Top_Left, monsterZone_Top_Right, monsterZone_Bottom_Left, monsterZone_Bottom_Right;
    
    [SerializeField] private RectTransform _fightBroke;
    [SerializeField] private RectTransform playerIcon;

    // Private var
    [SerializeField] private float mapSizeX, mapSizeY;
    [SerializeField] private float mapRatio;
    private float midMapX, midMapY;

    [SerializeField] private float distanceCamera = 100;
    [SerializeField] private float sizeCamera = 50;

    private GameObject currentActifZone;


    [SerializeField] private float fightBrokeTimer = 10;

    private Transform playerPos;


    // Monobehaviour
    private void OnValidate()
    {
        _camera.transform.position = Vector3.up * distanceCamera;
        _camera.orthographicSize = sizeCamera;

        _MM_UI.sizeDelta = new Vector2(mapSizeX, mapSizeY) * mapRatio;

        _MM_MonsterCamera.sizeDelta = new Vector2(mapSizeX * 16 / (sizeCamera * 2), mapSizeY * 9 / (sizeCamera * 2)) * mapRatio ;
    }
    private void Reset()
    {
        _camera = gameObject.GetComponentInChildren<Camera>();
    }

    private void Start()
    {
        SetUpZone();
        SetUpVariableMap();
    }
    private void Update()
    {
        MM_Updater();
    }

    // Public fonction

    public void SetForMonster()
    {
        isHunter = false;
    }

    [Button]
    public void SetForHunter()
    {
        isHunter = true; 
        _MM_MonsterCamera.gameObject.SetActive(false);
        playerIcon.gameObject.SetActive(true);
        playerPos = Tps_PlayerController.instance.transform;
    }

    [Button]
    public void FightBroke()
    {
        if (fightBroke_Coroutine != null) StopCoroutine(fightBroke_Coroutine);

        fightBroke_Coroutine = StartCoroutine(_FightBroke());
    }

    // Private fonction
    private void SetUpZone()
    {
        monsterZone_Top_Left.SetActive(false);
        monsterZone_Top_Right.SetActive(false);
        monsterZone_Bottom_Left.SetActive(false);
        monsterZone_Bottom_Right.SetActive(false);
        if (mapTuto || isHunter) return;
        currentActifZone = monsterZone_Top_Left;
    }
    private void SetUpVariableMap()
    {
        midMapX = mapSizeX * mapRatio / 2;
        midMapY = mapSizeY * mapRatio / 2;
    }
    private void MM_Updater()
    {
        if (!isHunter && !mapTuto)
        {
            MonsterPosition();
            CameraPosition();
        }

        if (isHunter)
        {
            PlayerPosition();
        }
    }

    private void PlayerPosition()
    {
        Vector3 _playerPos = playerPos.position;
        playerIcon.localPosition = new Vector3(_playerPos.x * mapRatio + midMapX, _playerPos.z * mapRatio + midMapY, 0);
    }

    private void MonsterPosition()
    {
        Vector3 _monsterPos = _monster.transform.position;

        // monster on Right
        if (_monsterPos.x > 0)
        {
            // monster on Top
            if (_monsterPos.z > 0)
            {
                if (monsterZone_Top_Right != currentActifZone) 
                {
                    currentActifZone.SetActive(false);
                    monsterZone_Top_Right.SetActive(true);
                    currentActifZone = monsterZone_Top_Right;
                }
            }
            // monster on Bottom
            else
            {
                if (monsterZone_Bottom_Right != currentActifZone)
                {
                    currentActifZone.SetActive(false);
                    monsterZone_Bottom_Right.SetActive(true);
                    currentActifZone = monsterZone_Bottom_Right;
                }
            }
        }

        // monster on Left
        else
        {
            // monster on Top
            if (_monsterPos.z > 0)
            {
                if (monsterZone_Top_Left != currentActifZone)
                {
                    currentActifZone.SetActive(false);
                    monsterZone_Top_Left.SetActive(true);
                    currentActifZone = monsterZone_Top_Left;
                }
            }
            // monster on Bottom
            else
            {
                if (monsterZone_Bottom_Left != currentActifZone)
                {
                    currentActifZone.SetActive(false);
                    monsterZone_Bottom_Left.SetActive(true);
                    currentActifZone = monsterZone_Bottom_Left;
                }
            }
        }
    }
    private void CameraPosition()
    {
        Vector3 _camPos = _monsterCamera.transform.position;
        _MM_MonsterCamera.localPosition = new Vector3(_camPos.x * mapRatio + midMapX, _camPos.z * mapRatio + midMapY, 0);
    }

    private Coroutine fightBroke_Coroutine;
    private IEnumerator _FightBroke()
    {
        _fightBroke.gameObject.SetActive(true);

        Vector3 _Pos = _monster.transform.position;
        _fightBroke.localPosition = new Vector3(_Pos.x * mapRatio + midMapX, _Pos.z * mapRatio + midMapY, 0);

        yield return new WaitForSeconds(10);
        _fightBroke.gameObject.SetActive(false);
    }
    

}
