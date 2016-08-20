#pragma warning disable 0649

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class MooringTrigger : MonoBehaviour
{
    [SerializeField]
    private SpaceStation spaceStation;
    
    public Collider Collider { get; private set; }
    public SpaceStation SpaceStation { get { return spaceStation; } }

    void Start()
    {
        Collider = GetComponent<Collider>();

        Debug.Assert(spaceStation);
        Debug.Assert(Collider.isTrigger);
    }
}
