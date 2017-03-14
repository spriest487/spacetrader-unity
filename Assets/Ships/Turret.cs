using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Turret : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("SpaceTrader/Detect Turrets")]
    public static void Turretify()
    {
        var obj = Selection.activeGameObject;
        if (!obj)
        {
            return;
        }

        obj.WalkChildren(child =>
        {
            if (child.name.StartsWith("Turret") && !child.GetComponent<Turret>())
            {
                var turret = child.AddComponent<Turret>();

                turret.gameObject.WalkChildren(turretChild =>
                {
                    if (!turret.gun && turretChild.name.StartsWith("Gun"))
                    {
                        turret.gun = turretChild.transform;
                    }
                });
            }
        });
    }
#endif

    [SerializeField]
    private Transform aim;

    [SerializeField]
    private Transform gun;
    
    private Matrix4x4 gunLocalToWorld;

    void Start()
    {
        gunLocalToWorld = gun.localToWorldMatrix;
    }

    void Update()
    {
        var aimOnYzPlane = aim.position;
        aimOnYzPlane.x = transform.position.x;
        
        var between = (aimOnYzPlane - transform.position).normalized;
        transform.rotation = Quaternion.FromToRotation(Vector3.forward, between);

        //Debug.DrawLine(transform.position,
        //    aimOnYzPlane,
        //    Color.magenta,
        //    Time.deltaTime);

        /* guns rotate on their y axis */
        var aimInGunSpace = gun.parent.transform.worldToLocalMatrix.MultiplyPoint(aim.position);
        aimInGunSpace.y = 0;

        //Debug.DrawLine(gun.position,
        //    gun.parent.transform.localToWorldMatrix.MultiplyPoint(aimInGunSpace), 
        //    Color.cyan, 
        //    Time.deltaTime);

        var initialGunForward = gunLocalToWorld.MultiplyVector(Vector3.forward);

        gun.localRotation = Quaternion.FromToRotation(initialGunForward, aimInGunSpace.normalized);
    }
}
