using UnityEngine;
using System.Collections;

public partial class Ship
{
    const float JUMP_TIME = 5.0f; //TODO: maybe a stat for this
    const float JUMP_DIST = 1000.0f; //how far do we "fake fly" into the distance when jumping

    [Header("Jumpdrive")]

    [SerializeField]
    private WorldMapArea jumpTarget;
    
    public WorldMapArea JumpTarget { get { return jumpTarget; } }

    private Coroutine jumpRoutine;

    private IEnumerator JumpRoutine()
    {
        Debug.Assert(Moorable.State == DockingState.InSpace);
        Debug.Assert(SpaceTraderConfig.WorldMap && SpaceTraderConfig.WorldMap.JumpEffectCurve != null);

        yield return null;

        //disable physics
        RigidBody.isKinematic = true;
        RigidBody.angularVelocity = Vector3.zero;
        RigidBody.velocity = Vector3.zero;

        ResetControls();

        Collider.enabled = false;

        if (!jumpTarget)
        {
            yield break;
        }

        //TODO: don't just instantly go to correct rot
        var jumpDir = jumpTarget.transform.position.normalized;

        transform.rotation = Quaternion.LookRotation(jumpDir);

        var jumpOrigin = transform.position;
        var jumpEnd = transform.position + (jumpDir * JUMP_DIST);
        var jumpCurve = SpaceTraderConfig.WorldMap.JumpEffectCurve;

        float jumpProgress = 0;
        float increment = 1 / JUMP_TIME;
        while (jumpProgress < 1)
        {
            float effectPos = jumpCurve.Evaluate(jumpProgress);

            transform.position = Vector3.LerpUnclamped(jumpOrigin, jumpEnd, effectPos);
            jumpProgress += Time.deltaTime * increment;

            yield return null;
        }

        /* this is likely to destroy or reset us, so do this last,
         but before JumpTarget is unset so receivers can figure out
         where we jumped to */
        SendMessage("OnCompletedJump");

        jumpTarget = null;
        jumpRoutine = null;
    }

    public void JumpTo(WorldMapArea area)
    {
        Debug.Assert(jumpRoutine == null, "jump routine must not already be in progress");

        jumpTarget = area;
        jumpRoutine = StartCoroutine(JumpRoutine());
    }
}
