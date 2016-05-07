using UnityEngine;
using System.Collections.Generic;
using System;

public class BracketManager : MonoBehaviour
{
    [SerializeField]
    private Bracket bracket;

    [SerializeField]
    private Color friendlyColor;
    [SerializeField]
    private Color hostileColor;
    [SerializeField]
    private Color unselectedTint;

    [SerializeField]
    private Sprite corner;
    [SerializeField]
    private Sprite selectedCorner;
    [SerializeField]
    private Sprite edgeMarker;

    [SerializeField]
    private int defaultWidth = 64;
    [SerializeField]
    private int defaultHeight = 64;
    [SerializeField]
    private float selectedExpand = 1.25f;

    [HideInInspector, SerializeField] Bracket[] brackets;

    public Color FriendlyColor { get { return friendlyColor; } }
    public Color HostileColor { get { return hostileColor; } }
    public Sprite Corner { get { return corner; } }
    public Sprite SelectedCorner { get { return selectedCorner; } }

    public int DefaultWidth { get { return defaultWidth; } }
    public int DefaultHeight { get { return defaultHeight; } }
    public float SelectedExpand { get { return selectedExpand; } }

    void LateUpdate()
	{
        var ships = FindObjectsOfType(typeof(Targetable)) as Targetable[];

        var existingBrackets = new Dictionary<int, Bracket>();
        if (brackets != null)
        {
            foreach (var bracket in brackets)
            {
                existingBrackets.Add(bracket.Target.GetInstanceID(), bracket);
            }
        }
        
		var newBrackets = new Dictionary<int, Bracket>();
		if (ships != null)
		{
			foreach (var ship in ships)
			{
				var shipId = ship.GetInstanceID();

                if (!ship.BracketVisible)
                {
                    continue;
                }

				Bracket existingBracket;
                existingBrackets.TryGetValue(shipId, out existingBracket);

				if (existingBracket)
				{
					newBrackets.Add(shipId, existingBracket);
				}
				else
				{
                    var newBracket = Bracket.CreateFromPrefab(bracket, this, ship);
                    newBracket.transform.SetParent(transform, false);
					newBrackets.Add(shipId, newBracket);
				}
			}
		}

		//remove all brackets that don't appear in the new list
		foreach (var bracket in existingBrackets)
		{
			if (!newBrackets.ContainsKey(bracket.Key))
			{
                Destroy(bracket.Value.gameObject);
			}
		}
        
		brackets = new Bracket[newBrackets.Count];
        newBrackets.Values.CopyTo(brackets, 0);
	}

    void OnLevelWasLoaded()
    {
        brackets = new Bracket[0];
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
        if (!bracketTarget || !bracketOwner)
        {
            return HostileColor;
        }

        bool sameFaction = bracketOwner.Faction == bracketTarget.Faction;        
        return sameFaction ? FriendlyColor : HostileColor;
    }
}
