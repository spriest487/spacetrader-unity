using UnityEngine;
using System.Collections;

public class Crosshair : MonoBehaviour
{
	public Texture crosshairTexture;

	private void DrawCrosshair(Vector3 worldPos, float alpha)
	{
        if (!Camera.main)
        {
            return;
        }

		var screenPos = Camera.main.WorldToScreenPoint(worldPos);
		
		Rect xhairRect = new Rect();
		xhairRect.width = crosshairTexture.width;
		xhairRect.height = crosshairTexture.height;
		xhairRect.x = screenPos.x - (xhairRect.width / 2);
		xhairRect.y = screenPos.y + (xhairRect.height / 2);

		xhairRect.y = Screen.height - xhairRect.y;

		var guiColor = GUI.color;
		GUI.color = new Color(guiColor.r, guiColor.g, guiColor.b, alpha);
		GUI.DrawTexture(xhairRect, crosshairTexture);
		GUI.color = guiColor;
	}

	void OnGUI()
	{
		var player = PlayerManager.Player;
        if (!player)
        {
            return;
        }

		var ship = player.GetComponent<Ship>();

		if (!ship)
		{
			return;
		}

		DrawCrosshair(ship.aim, 1);

		int layerMask = ~LayerMask.GetMask("Bullets and Effects", "Ignore Raycast");
		var between = ship.aim - player.transform.position;

		RaycastHit rayHit;
		if (Physics.Raycast(player.transform.position, between.normalized, out rayHit, between.magnitude, layerMask))
		{
			if (!player.transform.IsChildOf(rayHit.transform))
			{
				DrawCrosshair(rayHit.point, 0.5f);
			}
		}
	}
}
