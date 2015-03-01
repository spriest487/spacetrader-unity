using UnityEngine;
using System.Collections;

public class AlwaysStartAtOrigin : MonoBehaviour
{
    void Start()
    {
        transform.position = Vector3.zero;
        transform.hasChanged = false;
    }
}
