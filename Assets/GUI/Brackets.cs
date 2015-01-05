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
    public Color unselectedTint;

	public Bracket[] brackets = new Bracket[] { };

	public Texture bracketTexture;
    public Texture bracketSelectedTexture;
	
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

                if (ship.hideBracket)
                {
                    continue;
                }

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
        var playerShip = player.GetComponent<Ship>();

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
                //flip posY to gui coord space
                pos.y = Screen.height - pos.y;
                
                //TODO: calculate screen size of object
                float sizeX = 72;
                float sizeY = 72;
                
                float halfSizeX = sizeX/2;
                float halfSizeY = sizeY/2;

				var topLeft = new Rect(pos.x - (halfSizeX),
                    pos.y - halfSizeY,
                    bracketSize,
                    bracketSize);
				var topRight = new Rect(pos.x + (halfSizeX - bracketSize),
                    pos.y - halfSizeY,
                    bracketSize,
                    bracketSize);
                var bottomLeft = new Rect(pos.x - halfSizeX,
                    pos.y + (halfSizeY - bracketSize),
                    bracketSize,
                    bracketSize);                
				var bottomRight = new Rect(pos.x + (halfSizeX - bracketSize),
                    pos.y + (halfSizeY - bracketSize),
                    bracketSize,
                    bracketSize);

                var topRightUv = new Rect(1, 0, -1, 1);
                var bottomLeftUv = new Rect(0, 1, 1, -1);
                var bottomRightUv = new Rect(1, 1, -1, -1);

                var captionPos = new Rect();
                var captionContent = new GUIContent(bracket.ship.name);
                var captionSize = textStyle.CalcSize(captionContent);
                var bracketWidth = topRight.xMax - topLeft.x;
                captionPos.width = captionSize.x;
                captionPos.height = captionSize.y;
                captionPos.x = topLeft.x + (bracketWidth / 2 - captionSize.x / 2);
                captionPos.y = topLeft.y - captionPos.height;

                bool isTarget = playerShip && playerShip.target == bracket.ship;
                bool sameFaction  =playerTargetable && string.Equals(playerTargetable.faction, bracket.ship.faction);

                Color reactionColor = sameFaction ? friendlyColor : hostileColor;

                if (isTarget)
                {
                    GuiUtils.DrawOutlined(delegate(bool outlinePass)
                    {
                        if (outlinePass)
                        {
                            GUI.color = Color.black;
                        }
                        else
                        {
                            GUI.color = reactionColor;
                        }

                        GUI.DrawTexture(topLeft, bracketSelectedTexture);
                        GUI.DrawTextureWithTexCoords(topRight, bracketSelectedTexture, topRightUv);
                        GUI.DrawTextureWithTexCoords(bottomLeft, bracketSelectedTexture, bottomLeftUv);
                        GUI.DrawTextureWithTexCoords(bottomRight, bracketSelectedTexture, bottomRightUv);

                        GUI.Label(captionPos, captionContent, textStyle);
                    }, 1);
                }
                else
                {
                    GUI.color = reactionColor * unselectedTint;

                    GUI.DrawTexture(topLeft, bracketTexture);
                    GUI.DrawTextureWithTexCoords(topRight, bracketTexture, topRightUv);
                    GUI.DrawTextureWithTexCoords(bottomLeft, bracketTexture, bottomLeftUv);
                    GUI.DrawTextureWithTexCoords(bottomRight, bracketTexture, bottomRightUv);
                }
			}			
		}
	}
}
