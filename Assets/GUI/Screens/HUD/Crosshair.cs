using UnityEngine;

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

        if (!ship || !loadout)
		{
			return;
		}

        var moduleCount = loadout.FrontModules.Size;
        foreach (var module in loadout.FrontModules)
        {
            DrawCrosshair(module.Aim, 1);
        }

		int layerMask = ~LayerMask.GetMask("Bullets and Effects", "Ignore Raycast");
                        
        for (var moduleIndex = 0; moduleIndex < moduleCount; ++moduleIndex)
        {
            var module = loadout.FrontModules[moduleIndex];

            Vector3 aimFromPoint;
            if (loadout.Hardpoints != null && loadout.Hardpoints.Length > 0)
            {
                var hardpointIndex = moduleIndex & loadout.Hardpoints.Length;
                aimFromPoint = loadout.Hardpoints[hardpointIndex].transform.position;
            }
            else
            {
                aimFromPoint = ship.transform.position;
            }
            
            var between = module.Aim - aimFromPoint;

            RaycastHit rayHit;
            if (Physics.Raycast(aimFromPoint, between.normalized, out rayHit, between.magnitude, layerMask))
            {
                if (!player.transform.IsChildOf(rayHit.transform))
                {
                    DrawCrosshair(rayHit.point, 0.5f);
                }
            }
            else {
                DrawCrosshair(module.Aim, 1.0f);
            }
        }
	}
}
