using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;

public class HandController : MonoBehaviour
{
    [SerializeField]
    private VRNode node;
    
	private void Update ()
    {
        var pos = InputTracking.GetLocalPosition(node);
        var rot = InputTracking.GetLocalRotation(node);

        transform.position = pos;
        transform.rotation = rot;
	}
}
