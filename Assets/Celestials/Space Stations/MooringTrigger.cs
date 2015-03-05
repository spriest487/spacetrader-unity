using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class MooringTrigger : MonoBehaviour
{
    public SpaceStation spaceStation;

    void Start()
    {
        if (!spaceStation)
        {
            throw new UnityException("mooring trigger has no station");
        }

        if (!GetComponent<Collider>().isTrigger)
        {
            throw new UnityException("Mooring trigger set up with a non-trigger collider");
        }
    }
}
