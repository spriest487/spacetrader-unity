#pragma warning disable 0649

using UnityEngine;

[RequireComponent(typeof(MissionObjective))]
public class DestroyAllObjective : MonoBehaviour
{
    [SerializeField]
    private Transform[] targets;

    private MissionObjective objective;

    void Start()
    {
        objective = GetComponent<MissionObjective>();
    }

    void Update()
    {
        bool remaining = false;

        for (int target = 0; target < targets.Length; ++target)
        {
            if (targets[target])
            {
                remaining = true;
                break;
            }
        }

        if (!remaining)
        {
            objective.Complete = true;
        }
    }
}