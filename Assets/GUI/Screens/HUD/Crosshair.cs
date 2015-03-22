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
        var player = PlayerShip.LocalPlayer;
        if (!player)
        {
            return;
        }

		var ship = player.GetComponent<Ship>();
        var loadout = player.GetComponent<ModuleLoadout>();

		if (!ship)
		{
			return;
		}

		DrawCrosshair(ship.aim, 1);

		int layerMask = ~LayerMask.GetMask("Bullets and Effects", "Ignore Raycast");

        Vector3[] aimPoints;
        if (loadout)
        {
            aimPoints = new Vector3[ loadout.Hardpoints.Length ];
            for (int hardpoint = 0; hardpoint < loadout.Hardpoints.Length; ++hardpoint)
            {
                aimPoints[hardpoint] = loadout.Hardpoints[hardpoint].transform.position;
            }
        }
        else
        {
            aimPoints = new Vector3[] { player.transform.position };
        }

        foreach (var aimPoint in aimPoints)
        {
            var between = ship.aim - aimPoint;

            RaycastHit rayHit;
            if (Physics.Raycast(aimPoint, between.normalized, out rayHit, between.magnitude, layerMask))
            {
                if (!player.transform.IsChildOf(rayHit.transform))
                {
                    DrawCrosshair(rayHit.point, 0.5f);
                }
            }
        }
	}
}
