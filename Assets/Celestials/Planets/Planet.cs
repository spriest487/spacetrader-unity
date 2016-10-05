#pragma warning disable 0649

using UnityEngine;

public class Planet : MonoBehaviour
{
    [SerializeField]
    private float spinSpeed;

    private void Update()
    {
        float frameSpin = Time.deltaTime * spinSpeed;
        transform.rotation *= Quaternion.AngleAxis(frameSpin, Vector3.up);
    }
}