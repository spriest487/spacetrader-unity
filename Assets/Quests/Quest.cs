using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Quest : ScriptableObject
{
    [SerializeField]
    private QuestBehaviour behaviour;

    [SerializeField]
    private SpaceStation station;

    public QuestBehaviour Behaviour { get { return behaviour; } }
    public SpaceStation Station { get { return station; } }

    public static Quest Create(QuestBehaviour behaviour, SpaceStation station)
    {
        var quest = CreateInstance<Quest>();
        quest.name = behaviour.name;
        quest.behaviour = behaviour;
        quest.station = station;

        return quest;
    }

    public bool Done
    {
        get
        {
            return behaviour.Done(this);
        } 
    }

    public string Description
    {
        get
        {
            return behaviour.Describe(this);
        }
    }
}