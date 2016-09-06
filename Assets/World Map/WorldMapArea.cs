using UnityEngine;
using System.Collections;
using System;

public partial class WorldMapArea : ActionOnActivate
{
    [SerializeField]
    private TextMesh label;

    [SerializeField]
    private Transform icon;

    [SerializeField]
    private Transform tailBase;

    [SerializeField]
    private LineRenderer tail;

    public override string ActionName
    {
        get
        {
            return "Jump to " + name;
        }
    }

    public override void Activate(Ship activator)
    {
        activator.JumpTo(this);
    }

    public override bool CanBeActivatedBy(Ship activator)
    {
        return true;
    }

    private void Update()
    {
        var cam = SpaceTraderConfig.WorldMap.Camera;

        icon.transform.rotation = cam.transform.rotation;
        label.transform.rotation = cam.transform.rotation;

        ForceUpdateLayout();
    }

#if UNITY_EDITOR
    public
#else
    private
#endif
    void ForceUpdateLayout()
    {
        var basePos = tailBase.position;
        basePos.y = 0;
        tailBase.position = basePos;

        tail.SetPosition(0, label.transform.position);
        tail.SetPosition(1, basePos);
    }
}
