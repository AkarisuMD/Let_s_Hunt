using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class HealthBarManager : Singleton<HealthBarManager>
{
    private int _indexOwner = 0;
    public int IndexOwner 
    { set 
        { 
            _indexOwner = value;
            IntHealthBar();
        } 
    }
    [SerializeField] private HealthBar healthBar1, healthBar2, healthBar3, healthBar4;

    private void IntHealthBar()
    {
        List<int> _indexPlayers = new() { 1, 2, 3, 4 };

        // get player owner index and set it to healthbar1

        if(_indexOwner == 0)
        {
            return;
        }

        healthBar1.SetIndexPlayer(_indexOwner);
        _indexPlayers.Remove(_indexOwner);

        healthBar2.SetIndexPlayer(_indexPlayers[0]);
        _indexPlayers.Remove(_indexPlayers[0]);

        healthBar3.SetIndexPlayer(_indexPlayers[0]);
        _indexPlayers.Remove(_indexPlayers[0]);

        healthBar4.SetIndexPlayer(_indexPlayers[0]);
        _indexPlayers.Remove(_indexPlayers[0]);
    }

    public void ChangeHealthBar(int indexPlayer, int value) 
    {
        HealthBar _healthBar = GetHealthFromIndex(indexPlayer);
        int newHp = Mathf.Clamp(_healthBar.GetHpValue() + value, 0, 10);
        _healthBar.ChangeValueHealthBar(newHp);

        if (_indexOwner != indexPlayer) return;
        if (_healthBar.GetHpValue() == 0) Tps_PlayerController.Instance.playerData.ChangeHp(-10);
    }

    private HealthBar GetHealthFromIndex(int indexPlayer)
    {
        if (healthBar1.GetIndexPlayer() == indexPlayer) return healthBar1;
        else if (healthBar2.GetIndexPlayer() == indexPlayer) return healthBar2;
        else if (healthBar3.GetIndexPlayer() == indexPlayer) return healthBar3;
        else if (healthBar4.GetIndexPlayer() == indexPlayer) return healthBar4;
        else return null;
    }
}
