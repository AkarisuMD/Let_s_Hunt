using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AirdropManager : NetworkBehaviour
{
    [SerializeField] private List<SpawnAirdrop> airdropList = new();
    [SerializeField] private int airdropCount = 2;


    [ServerRpc(RequireOwnership = true)]
    public void CallAirdropServerRpc()
    {
        List<SpawnAirdrop> _list = new();

        int y = airdropCount;
        for (int i = 0; i < y; i++)
        {
            int rng = Random.Range(0, airdropList.Count);

            if (!_list.Contains(airdropList[rng]))
            {
                _list.Add(airdropList[rng]);
            }
            else 
            {
                y++;

                if (y >= 100) break;
            }
        }

        foreach (SpawnAirdrop airdrop in _list)
        {
            airdrop.Spawn_AirdropServerRpc();
        }
    }
}
