using UnityEngine;

public class Crosshair : MonoBehaviour
{
	public Texture crosshairTexture;

	private void DrawCrosshair(Vector3 worldPos, float alpha)
	{
        if (!FollowCamera.Current)
        {
            return;
        }

        var screenPos = FollowCamera.Current.Camera.WorldToScreenPoint(worldPos);

        var xhairRect = new Rect
        {
            width = crosshairTexture.width,
            height = crosshairTexture.height,
            x = screenPos.x - (crosshairTexture.width / 2),
            y = Screen.height - (screenPos.y + (crosshairTexture.height / 2)),
        };
        
		var guiColor = GUI.color;
		GUI.color = new Color(guiColor.r, guiColor.g, guiColor.b, alpha);
		GUI.DrawTexture(xhairRect, crosshairTexture);
		GUI.color = guiColor;
	}

	void OnGUI()
	{
        var player = Universe.LocalPlayer;
        if (!player)
        {
            return;
        }

		var ship = player.GetComponent<Ship>();
        var loadout = ship.ModuleLoadout;

        if (!ship)
		{
			return;
		}

        var moduleCount = loadout.SlotCount;
        foreach (var module in loadout)
        {
               DrawCrosshair(module.Aim, 1);
        }

		int layerMask = ~LayerMask.GetMask("Bullets and Effects", "Ignore Raycast");
                        
        for (var moduleIndex = 0; moduleIndex < moduleCount; ++moduleIndex)
        {
            var module = loadout.GetSlot(moduleIndex);

            Vector3 aimFromPoint;
            if (loadout.SlotCount > 0)
            {
                aimFromPoint = ship.GetHardpointAt(moduleIndex).transform.position;
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
