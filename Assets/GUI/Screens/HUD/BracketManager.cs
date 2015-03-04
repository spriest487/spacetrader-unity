using UnityEngine;
using System.Collections.Generic;
using System;

public class BracketManager : MonoBehaviour
{
	public Bracket bracket;

    public Color friendlyColor;
    public Color hostileColor;
    public Color unselectedTint;

	public Sprite corner;
	public Sprite selectedCorner;

	private Dictionary<int, Bracket> brackets;

	void LateUpdate()
	{
		if (brackets == null)
		{
			brackets = new Dictionary<int, Bracket>();
		}

        var ships = FindObjectsOfType(typeof(Targetable)) as Targetable[];

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
				brackets.TryGetValue(shipId, out existingBracket);

				if (existingBracket)
				{
					newBrackets.Add(shipId, existingBracket);
				}
				else
				{
					var newBracket = (Bracket) Instantiate(bracket);
					newBracket.transform.SetParent(transform, false);
					newBracket.bracketManager = this;
					newBracket.name = "Bracket for " + ship.name;
					newBracket.target = ship;
					newBrackets.Add(shipId, newBracket);
				}
			}
		}

		//remove all brackets that don't appear in the new list
		foreach (var bracket in brackets)
		{
			if (!newBrackets.ContainsKey(bracket.Key))
			{
				Bracket.Destroy(bracket.Value.gameObject);
			}
		}
        
		brackets = newBrackets;
	}
}
