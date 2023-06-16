using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

public class TwitchVoting_Manager : Singleton<TwitchVoting_Manager>
{
    //========
    //VARIABLES
    //========
    public static Action onStartingVote;
    public static Action<int> addingVote;

    public List<sc_TwitchVote> listOfAllPossibleVote = new();
    public List<VoteRef_UI> listOfVoteUI = new();
    public Dictionary<sc_TwitchVote, VoteRef_UI> listOfCurrentVote = new();
    [SerializeField] private GameObject voteUI;
    [SerializeField] private VerticalLayoutGroup voteGroupUI;
    [SerializeField] private int voteTime = 100000;
    private bool isVoteStarted = false;

    //========
    //MONOBEHAVIOUR
    //========

    private void OnEnable()
    {
        addingVote += OnVote;
    }
    private void OnDisable()
    {
        addingVote -= OnVote;
    }

    //========
    //FONCTION
    //========

    [Button]
    public void StartTwitchVote()
    {
        print("Strating Vote");
        if (isVoteStarted) return;
        ResetVoteCount();

        isVoteStarted = true;

        List<sc_TwitchVote> listOfChosenVote = listOfAllPossibleVote;

        foreach (sc_TwitchVote twitchVote in listOfChosenVote)
        {
            Transform newVoteUI = Instantiate(voteUI.transform, voteGroupUI.transform);
            if (newVoteUI.TryGetComponent<VoteRef_UI>(out VoteRef_UI voteRef_UI))
            {
                listOfCurrentVote.Add(twitchVote, voteRef_UI);
                voteRef_UI.InitVoteUI(twitchVote.nameOfTheVote, listOfAllPossibleVote.Count);
            }
        }
        onStartingVote?.Invoke();
    }
    public void OnVote(int voteNumber)
    {
        sc_TwitchVote key = listOfCurrentVote.Keys.ToList()[voteNumber];
        key.voteCount++;
        listOfCurrentVote.Values.ToList()[voteNumber].UpdatePourcentageOfVote(key.voteCount);
    }
    public void EndTwitchVote()
    {
        sc_TwitchVote winningVote = null;
        foreach (sc_TwitchVote sc_TwitchVote in listOfCurrentVote.Keys)
        {
            if (winningVote == null)
            {
                winningVote = sc_TwitchVote;
                continue;
            }
            if (sc_TwitchVote.voteCount > winningVote.voteCount)
            {
                winningVote = sc_TwitchVote;
                continue;
            }
        }
        isVoteStarted = false;
        //
        switch (winningVote.twitchVote)
        {
            case TwitchVote.Drop:

                break;
            case TwitchVote.Scream:

                break;
            case TwitchVote.Light:

                break;
            case TwitchVote.Revive:

                break;
        }
    }
    private void ResetVoteCount()
    {
        foreach(sc_TwitchVote twitchVote in listOfAllPossibleVote)
        {
            twitchVote.voteCount = 0;
        }
    }
    private List<sc_TwitchVote> ChoseXRandomFromList(List<sc_TwitchVote> listOfAllPossibility, int numberToTake)
    {
        List<sc_TwitchVote> chosenList = new(numberToTake);

        for(int i = 0; i < numberToTake; i++)
        {
            sc_TwitchVote randomFromList = listOfAllPossibility[UnityEngine.Random.Range(0, listOfAllPossibility.Count)];
            chosenList.Add(randomFromList);
        }

        return chosenList;
    }
}
public enum TwitchVote
{
    Drop, Scream, Light, Revive
}
