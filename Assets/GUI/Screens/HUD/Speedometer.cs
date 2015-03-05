using System;
using System.Collections.Generic;
using UnityEngine;

public class Speedometer : MonoBehaviour
{
	public GUIStyle style;

	void OnGUI()
	{
		var playerObjs = GameObject.FindGameObjectsWithTag("Player");
		if (playerObjs == null || playerObjs.Length != 1)
		{
			return;
		}

		var player = playerObjs[0];
		var playerShip = player.GetComponent<Ship>();

		if (playerShip == null) {
			return;
		}

		var thrust = Mathf.Max(
			Mathf.Abs(playerShip.thrust),
			Mathf.Abs(playerShip.lift),
			Mathf.Abs(playerShip.strafe));
		var maneuverThrust = Mathf.Max(
			Mathf.Abs(playerShip.pitch),
			Mathf.Abs(playerShip.yaw),
			Mathf.Abs(playerShip.roll));

		var text = string.Format("Speed: {0:F2}m/s"
			+"\nThrust: {1:P2}"
			+"\nManeuver speed: {2:F2}deg/sec"
			+"\nManeuver thrust: {3:P2}",
			playerShip.GetComponent<Rigidbody>().velocity.magnitude,
			thrust,
			playerShip.GetComponent<Rigidbody>().angularVelocity.magnitude * Mathf.Rad2Deg,
			maneuverThrust);

		var ai = playerShip.GetComponent<AICaptain>();		
		if (ai)
		{
			var dest = ai.destination;

			var dist = (dest - playerShip.GetComponent<Rigidbody>().position).magnitude;
			text += "\ndistance to target : " + dist;
		}
		

		var content = new GUIContent(text);

		var size = style.CalcSize(content);

		var rect = new Rect(0, Screen.height - size.y, size.x, size.y);

		GUI.Box(rect, content, style);
	}
}