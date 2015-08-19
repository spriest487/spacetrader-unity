using UnityEngine;

public class WeaponHardpoint : MonoBehaviour
{
    [SerializeField]
    [Range(0, 180)]
    private float arc;

    public float Arc { get { return arc; } }

    private float GetArcAngleTo(Vector3 pos)
    {
        var forward = transform.forward;
        var toTarget = (pos - transform.position).normalized;

        float dot = Vector3.Dot(toTarget, forward);
        
        float angle = Mathf.Rad2Deg * Mathf.Acos(dot);
        return angle;
    }

    public bool CanAimAt(Vector3 pos)
    {
        if (arc < float.Epsilon || arc >= 180)
        {
            return true;
        }
        
        float angle = GetArcAngleTo(pos);

        return angle < arc;
    }
}
