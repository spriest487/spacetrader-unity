#pragma warning disable 0649

using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Linq;

public class BracketManager : MonoBehaviour
{
    private class BracketOrderComparer : IComparer<Bracket>
    {
        public int Compare(Bracket b1, Bracket b2)
        {
            return (int) (b2.transform.position.z - b1.transform.position.z);
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

    private IComparer<Bracket> bracketOrder = new BracketOrderComparer();

    [HideInInspector]
    [SerializeField]
    private List<Bracket> brackets;
    
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

    private void Clear()
    {
        foreach (var bracket in brackets)
        {
            Destroy(bracket.gameObject);
        }

        brackets.Clear();
    }

    void LateUpdate()
	{
        if (!Camera.main)
        {
            Clear();
            return;
        }

        brackets.RemoveAll(b => !b);

        var targetables = FindObjectsOfType<Targetable>()
            .Where(t => t.BracketVisible)
            .ToList();

        brackets.Where(b => !targetables.Contains(b.Target))
            .ToList()
            .ForEach(removeBracket =>
            {
                Destroy(removeBracket.gameObject);
                brackets.Remove(removeBracket);
            });

        targetables.Except(brackets.Select(b => b.Target))
            .ToList()
            .ForEach(newTarget =>
            {
                var newBracket = Bracket.CreateFromPrefab(bracket, this, newTarget);
                newBracket.transform.SetParent(transform, false);
                brackets.Add(newBracket);
            });
        
        //z-sort (this is one frame behind but it shouldn't matter..?)
        brackets.Sort(bracketOrder);
        brackets.ForEach(b => b.transform.SetAsFirstSibling());

        if (!EventSystem.current.IsPointerOverGameObject())
        { 
            if (Input.GetButtonDown("turn"))
            {
                clickOffTargetTime = Time.time;
            }
            else if (Input.GetButtonUp("turn"))
            {
                if (Time.time - clickOffTargetTime < FollowCamera.UI_DRAG_DELAY)
                {
                    var player = PlayerShip.LocalPlayer;
                    if (player)
                    {
                        player.Ship.Target = null;
                    }
                }
            }
        }
	}

    void OnLevelWasLoaded()
    {
        Clear();
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
