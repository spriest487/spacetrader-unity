using UnityEngine;

public class MissionObjective : MonoBehaviour
{
    [SerializeField]
    private bool complete;

    [SerializeField]
    private string[] teams;

    [SerializeField]
    private string description;

    public string[] Teams
    {
        get { return teams; }
        set { teams = value; }
    }

    public bool Complete
    {
        get { return complete; }
        set { complete = value; }
    }

    public string Description
    {
        get { return description; }
        set{ description = value; }
    }
}
