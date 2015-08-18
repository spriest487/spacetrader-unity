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

        foreach (var target in targets)
        {
            remaining = true;
            break;
        }

        if (!remaining)
        {
            objective.Complete = true;
        }
    }
}