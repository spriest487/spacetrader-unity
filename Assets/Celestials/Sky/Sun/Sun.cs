using UnityEngine;
using System.Collections;

public class Sun : MonoBehaviour {
    void Update()
    {
        transform.rotation = Quaternion.LookRotation((Vector3.zero - transform.position).normalized);
    }
}
