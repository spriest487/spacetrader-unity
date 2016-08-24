#pragma warning disable 0649

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class QuestBoard : ScriptableObject
{
    [Serializable]
    public class QuestAssignment
    {
        public PlayerShip Player;
        public Quest Quest;
    }

    [SerializeField]
    private List<QuestBehaviour> questTypes;
    
    [Header("Runtime")]

    [SerializeField]
    private List<QuestAssignment> quests;

    public IEnumerable<QuestBehaviour> QuestTypes
    {
        get { return questTypes; }
    }

    public static QuestBoard Create(QuestBoard prefab)
    {
        var instance = Instantiate(prefab);
        return instance;
    }

    public IEnumerable<Quest> QuestsForPlayer(PlayerShip player)
    {
        return quests.Where(q => q.Player == player)
            .Select(q => q.Quest);
    }

    public IEnumerable<Quest> QuestsAtStation(SpaceStation station)
    {
        return quests.Where(q => q.Quest.Station == station)
            .Select(q => q.Quest);
    }

    public PlayerShip OwnerOf(Quest quest)
    {
        return quests.Where(a => a.Quest == quest)
            .Select(a => a.Player).FirstOrDefault();
    }

    public void NewQuest(Quest quest)
    {
        Debug.Assert(!quests.Any(a => a.Quest == quest), "quest should not already be registered");

        quests.Add(new QuestAssignment()
        {
            Quest = quest
        });
    }

    public void AcceptQuest(PlayerShip player, Quest quest)
    {
        Debug.Assert(!quests.Any(a => a.Quest == quest && a.Player), "quest should not already be assigned");

        var assignment = quests.Where(a => a.Quest).First();
        assignment.Player = player;
    }

    public void FinishQuest(Quest quest)
    {
        var owner = OwnerOf(quest);

        Debug.Assert(owner);
        Debug.Assert(quest.Behaviour.Done(quest));
        quest.Behaviour.OnFinish(quest);

        owner.AddMoney(quest.Behaviour.MoneyReward);
        
        foreach (var crew in owner.Ship.GetAllCrew())
        {
            crew.GrantXP(quest.Behaviour.XPReward);
        }

        DestroyQuest(quest);
    }

    public void CancelQuest(Quest quest)
    {
        quest.Behaviour.OnAbandon(quest);
        DestroyQuest(quest);
    }

    private void DestroyQuest(Quest quest)
    {
        quests.RemoveAll(a => a.Quest == quest);

        Destroy(quest);
    }
}
