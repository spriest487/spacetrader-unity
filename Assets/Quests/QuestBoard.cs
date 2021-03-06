﻿#pragma warning disable 0649

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

[CreateAssetMenu(menuName = "SpaceTrader/Quests/Quest Board")]
public class QuestBoard : ScriptableObject
{
    [Serializable]
    public class QuestAssignment
    {
        public PlayerShip Player;
        public Quest Quest;
    }

    [SerializeField]
    private List<Quest> questTypes;
    
    [Header("Runtime")]

    [SerializeField]
    private List<QuestAssignment> quests;

    public IEnumerable<Quest> QuestTypes
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
        var area = Universe.WorldMap.GetCurrentArea().name;

        if (station == null)
        {
            return Enumerable.Empty<Quest>();
        }

        return quests.Where(q => q.Quest.Location.Area == area
                && q.Quest.Location.Station == station.name)
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
        Debug.Assert(quests.Any(a => a.Quest == quest && !a.Player), "quest should not already be assigned");

        var assignment = quests.Where(a => a.Quest == quest).First();
        assignment.Player = player;

        quest.OnAccepted();
    }

    public void FinishQuest(Quest quest)
    {
        var owner = OwnerOf(quest);

        Debug.Assert(owner);
        Debug.Assert(quest.Status == QuestStatus.Completed, "Only call FinishQuest on quests that are definitely finished");
        quest.OnFinish();

        owner.AddMoney(quest.MoneyReward);
        owner.Ship.GrantCrewXP(quest.XPReward);

        DestroyQuest(quest);
    }

    public void CancelQuest(Quest quest)
    {
        quest.OnAbandon();
        DestroyQuest(quest);
    }

    private void DestroyQuest(Quest quest)
    {
        quests.RemoveAll(a => a.Quest == quest);

        Destroy(quest);
    }

    public void NotifyDeath(Ship ship, Ship killer)
    {
        for (int questIt = 0; questIt < quests.Count; ++questIt)
        {
            quests[questIt].Quest.NotifyDeath(ship, killer);
        }
    }
}
