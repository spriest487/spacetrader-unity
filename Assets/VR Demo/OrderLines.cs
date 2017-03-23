using UnityEngine;
using System.Collections;

public class OrderLines : MonoBehaviour
{
    [SerializeField]
    private LineRenderer moveLine;

    [SerializeField]
    private LineRenderer targetLine;

    private AITaskFollower owner;

    private void Awake()
    {
        owner = GetComponentInParent<AITaskFollower>();
        Debug.Assert(owner, "OrderLines must belong to an AI or one of its children");
    }

    private void SetLineTo(LineRenderer lineRenderer, Vector3 targetPos)
    {
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(lineRenderer.numPositions - 1, targetPos);

        for (int pos = 1; pos < moveLine.numPositions - 1; ++pos)
        {
            var middleVert = Vector3.Lerp(transform.position, targetPos, 1f / pos);
            moveLine.SetPosition(pos, middleVert);
        }
    }

    private void Update()
    {
        var current = owner.CurrentTask;
        if (current)
        {
            var targetPos = current.TargetLocation;
            if (targetPos.HasValue)
            {
                moveLine.gameObject.SetActive(true);
                SetLineTo(moveLine, targetPos.Value);
            }
            else
            {
                moveLine.gameObject.SetActive(false);
            }
        }

        if (owner.Ship.Target)
        {
            var targetPos = owner.Ship.Target.transform.position;
            targetLine.gameObject.SetActive(true);
            SetLineTo(targetLine, targetPos);
        }
        else
        {
            targetLine.gameObject.SetActive(false);
        }
    }
}
