#pragma warning disable 0649

using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Linq;

public class BracketManager : MonoBehaviour
{
    private class DistanceComparator : IComparer<Targetable>
    {
        public int Compare(Targetable t1, Targetable t2)
        {
            float dist = t1.transform.position.z - t2.transform.position.z;

            if (dist > Mathf.Epsilon)
            {
                return 1;
            }
            else if (dist < -Mathf.Epsilon)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }
    }

    [SerializeField]
    private Bracket bracket;

    [SerializeField]
    private Color friendlyColor = Color.green;
    [SerializeField]
    private Color hostileColor = Color.red;
    [SerializeField]
    private Color fleetMemberColor = Color.magenta;
    [SerializeField]
    private Color resourceColor = Color.yellow;

    //[SerializeField]
    //private Color unselectedTint = new Color(1, 1, 1, 0.5f);

    [SerializeField]
    private Sprite corner;
    [SerializeField]
    private Sprite selectedCorner;
    [SerializeField]
    private Sprite edgeMarker;
    [SerializeField]
    private Sprite selectedEdgeMarker;

    [SerializeField]
    private int defaultWidth = 64;
    [SerializeField]
    private int defaultHeight = 64;
    [SerializeField]
    private float selectedExpand = 1.25f;

    private float clickOffTargetTime;

    private IComparer<Targetable> bracketOrder = new DistanceComparator();

    private PooledList<Bracket, Targetable> brackets;
    
    public Color FriendlyColor { get { return friendlyColor; } }
    public Color HostileColor { get { return hostileColor; } }
    public Color FleetMemberColor { get { return fleetMemberColor; } }
    public Color ResourceColor { get { return resourceColor; } }
    public Sprite Corner { get { return corner; } }
    public Sprite SelectedCorner { get { return selectedCorner; } }
    public Sprite EdgeMarker { get { return edgeMarker; } }
    public Sprite SelectedEdgeMarker { get { return selectedEdgeMarker; } }

    public int DefaultWidth { get { return defaultWidth; } }
    public int DefaultHeight { get { return defaultHeight; } }
    public float SelectedExpand { get { return selectedExpand; } }

    void LateUpdate()
	{
        if (brackets == null)
        {
            brackets = new PooledList<Bracket, Targetable>(transform, bracket);
        }

        if (!Camera.main)
        {
            brackets.Clear();
            return;
        }

        var allTargetables = FindObjectsOfType<Targetable>().ToList();
        allTargetables.Sort(bracketOrder);

        brackets.Refresh(allTargetables, (i, bracket, targetable) =>
        {
            bracket.Assign(this, targetable);
        });
        
        if (!EventSystem.current.IsPointerOverGameObject())
        { 
            if (Input.GetButtonDown("turn"))
            {
                clickOffTargetTime = Time.time;
            }
            else if (Input.GetButtonUp("turn"))
            {
                var player = PlayerShip.LocalPlayer;

                if (player && Time.time - clickOffTargetTime < FollowCamera.UI_DRAG_DELAY)
                {
                    /* the select button was un-pressed, but no bracket was under the cursor
                     (they block raycasts). do a world raycast to see if we hit any 3d objects*/
                    var mousePos = Input.mousePosition;
                    var mouseRay = Camera.main.ScreenPointToRay(new Vector3(mousePos.x, mousePos.y, 0));
                    RaycastHit mouseHit;
                    if (Physics.Raycast(mouseRay, out mouseHit))
                    {
                        for (int bracket = 0; bracket < brackets.Count; ++bracket)
                        {
                            if (brackets[bracket].Target.transform == mouseHit.transform)
                            {
                                brackets[bracket].SetPlayerTarget();
                                break;
                            }
                        }
                    }
                    else
                    {
                        player.Ship.Target = null;
                    }                    
                }
            }
        }
	}

    void OnLevelWasLoaded()
    {
        if (brackets != null)
        {
            brackets.Clear();
        }
    }

    public Bracket FindBracket(GameObject obj)
    {   
        foreach (var bracket in brackets)
        {
            if (bracket.Target && bracket.Target.gameObject == obj)
            {
                return bracket;
            }
        }

        return null;
    }

    public Color GetBracketColor(Targetable bracketOwner, Targetable bracketTarget)
    {
        TargetRelationship relationship;
        if (!bracketOwner || !bracketTarget)
        {
            relationship = TargetRelationship.Neutral;
        }
        else
        {
            relationship = bracketOwner.RelationshipTo(bracketTarget);
        }

        switch (relationship)
        {
            case TargetRelationship.FleetMember:
                return FleetMemberColor;
            case TargetRelationship.Friendly:
                return FriendlyColor;
            case TargetRelationship.Hostile:
                return HostileColor;
            case TargetRelationship.Resource:
                return ResourceColor;
            default:
                return Color.white;
        }
    }
}
