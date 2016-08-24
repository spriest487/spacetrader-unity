#pragma warning disable 0649

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public abstract class QuestBehaviour : ScriptableObject
{
    public abstract int XPReward { get; }
    public abstract int MoneyReward { get; }

    public abstract bool Done(Quest quest);
    public abstract string Describe(Quest quest);

    public abstract void OnFinish(Quest quest);
    public abstract void OnAbandon(Quest quest);
}
