using UnityEngine;
using System.Collections.Generic;
using System;

public class SpaceStation : MonoBehaviour
{
    [SerializeField]
    private MooringTrigger mooringTrigger;

    [SerializeField]
    private List<Moorable> mooredShips;

    [SerializeField]
    private List<CrewMember> availableCrew;

    public MooringTrigger MooringTrigger
    {
        get { return mooringTrigger; }
    }

    public List<CrewMember> AvailableCrew
    {
        get { return availableCrew; }
        set
        {
            availableCrew = value == null ?
                 new List<CrewMember>() :
                 new List<CrewMember>(value);
        }
    }

    public void RequestMooring(Moorable moorable)
    {
        if (mooredShips.Contains(moorable) || !moorable.gameObject.activeInHierarchy)
        {
            throw new UnityException("tried to moor an already moored or disabled ship");
        }

        moorable.gameObject.SendMessage("OnMoored", this);

        moorable.gameObject.SetActive(false);
        mooredShips.Add(moorable);
    }

    public void Unmoor(Moorable moorable)
    {
        if (!mooredShips.Contains(moorable))
        {
            Debug.LogError(string.Format("tried to unmoor {0} from {1} but it's not moored here",
                moorable,
                gameObject));
        }
        else
        {
            mooredShips.Remove(moorable);
            
            moorable.transform.position = mooringTrigger.transform.position;
            moorable.transform.rotation = mooringTrigger.transform.rotation;
            if (moorable.GetComponent<Rigidbody>())
            {
                moorable.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                moorable.GetComponent<Rigidbody>().velocity = Vector3.zero;
            }

            moorable.gameObject.SetActive(true);
            moorable.gameObject.SendMessage("OnUnmoored", this);
        }
    }

    void Start()
    {
        mooredShips = new List<Moorable>();
    }
}
