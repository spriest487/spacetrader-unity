using UnityEngine;

public class MissionObjective : MonoBehaviour
{
    [SerializeField]
    private bool complete;

    [SerializeField]
    private string[] teams;

    [SerializeField]
    private string description;

    public bool Complete
    {
        get { return complete; }
        set { complete = value; }
    }
    public string[] Teams { get { return teams; } }
    public string Description { get { return description; } }
}
