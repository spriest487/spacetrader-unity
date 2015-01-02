using UnityEngine;
using System.Collections.Generic;
using System;

public class Brackets : MonoBehaviour
{
	[Serializable]
	public class Bracket
	{
		public Targetable ship;
	}

    public Color friendlyColor;
    public Color hostileColor;

	public Bracket[] brackets = new Bracket[] { };

	public Texture bracketTexture;
	
	public GUIStyle textStyle;

	void LateUpdate()
	{
        var ships = FindObjectsOfType(typeof(Targetable)) as Targetable[];

		if (ships != null)
		{
			var bracketList = new List<Bracket>(ships.Length);

			for (int shipIt = 0; shipIt < ships.Length; ++shipIt)
			{
				var ship = ships[shipIt];

				var bracket = new Bracket();
				bracket.ship = ship;
				bracketList.Add(bracket);
			}

			brackets = bracketList.ToArray();
		}
		else
		{
			brackets = new Bracket[] { };
		}
	}

	void OnGUI()
	{
        GUI.depth = -1;

        var player = PlayerManager.Player;
        var playerTargetable = player.GetComponent<Targetable>();

		if (brackets == null || !Camera.main)
		{
			return;
		}

		foreach (var bracket in brackets)
		{
			var pos = Camera.main.WorldToScreenPoint(bracket.ship.transform.position);
			//var pos = new Vector3(10, 10, 10);

			var bracketSize = bracketTexture.width;

			if (pos.z > 0)
			{
				pos.y = Screen.height - pos.y;

				var topLeft = new Rect(pos.x - bracketSize, pos.y - bracketSize, bracketSize, bracketSize);

				var topRight = new Rect(pos.x + bracketSize, pos.y - bracketSize, bracketSize, bracketSize);
				var topRightUv = new Rect(1, 0, -1, 1);

				var bottomLeft = new Rect(pos.x - bracketSize, pos.y + bracketSize, bracketSize, bracketSize);
				var bottomLeftUv = new Rect(0, 1, 1, -1);

				var bottomRight = new Rect(pos.x + bracketSize, pos.y + bracketSize, bracketSize, bracketSize);
				var bottomRightUv = new Rect(1, 1, -1, -1);

                var captionPos = new Rect();
                var captionContent = new GUIContent(bracket.ship.name);
                var captionSize = textStyle.CalcSize(captionContent);
                var bracketWidth = topRight.xMax - topLeft.x;
                captionPos.width = captionSize.x;
                captionPos.height = captionSize.y;
                captionPos.x = topLeft.x + (bracketWidth / 2 - captionSize.x / 2);
                captionPos.y = topLeft.y - captionPos.height;

                GuiUtils.DrawOutlined(delegate(bool outlinePass)
                {
                    if (outlinePass)
                    {
                        GUI.color = Color.black;
                    }
                    else if (playerTargetable && string.Equals(playerTargetable.faction, bracket.ship.faction))
                    {
                        GUI.color = friendlyColor;
                    }
                    else
                    {
                        GUI.color = hostileColor;
                    }

                    GUI.DrawTexture(topLeft, bracketTexture);
                    GUI.DrawTextureWithTexCoords(topRight, bracketTexture, topRightUv);
                    GUI.DrawTextureWithTexCoords(bottomLeft, bracketTexture, bottomLeftUv);
                    GUI.DrawTextureWithTexCoords(bottomRight, bracketTexture, bottomRightUv);

                    GUI.Label(captionPos, captionContent, textStyle);
                }, 1);
			}			
		}
	}
}
