using UnityEngine;
using System.Collections.Generic;

public class SpaceStation : MonoBehaviour
{
    public MooringTrigger mooringTrigger;

    private List<Moorable> mooredShips;

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
            if (moorable.rigidbody)
            {
                moorable.rigidbody.angularVelocity = Vector3.zero;
                moorable.rigidbody.velocity = Vector3.zero;
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
