using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Ship))]
public class PlayerShip : MonoBehaviour
{
	private Ship ship;

	private Vector2? FindTouchPos()
	{
		//mouse input takes priority!
		if (Input.GetMouseButton(0))
		{
			return Input.mousePosition;
		}

		if (Input.touchCount > 0)
		{
			Touch touch = Input.GetTouch(0);
			if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
			{
				return touch.position;
			}
		}

		return null;
	}

	private void RotateTowardsAimAngle(float currentVal, ref float angle)
	{
		//Debug.Log(string.Format("current {0:F3}, angle {1:F3}", currentVal, angle));

		if (MathUtils.SameSign(currentVal, angle) && Mathf.Abs(currentVal) > Mathf.Abs(angle))
		{
			angle = 0;
		}
	}

	private void RotateTowardsAim(Vector2 screenAimPos)
	{
		var aimPitch = -Mathf.Clamp((2 * (screenAimPos.y / Screen.height)) - 1, -1, 1);
		var aimYaw = Mathf.Clamp((2 * (screenAimPos.x / Screen.width)) - 1, -1, 1);

		var currentRotAsAngles = transform.InverseTransformDirection(rigidbody.angularVelocity);
		var currentPitch = (Mathf.Rad2Deg * currentRotAsAngles.x) / ship.stats.maxTurnSpeed;
		var currentYaw = (Mathf.Rad2Deg * currentRotAsAngles.y) / ship.stats.maxTurnSpeed;

		/*RotateTowardsAimAngle(currentPitch, ref aimPitch);
		RotateTowardsAimAngle(currentYaw, ref aimYaw);*/

		ship.yaw = aimYaw;
		ship.pitch = aimPitch;
	}
	
	void FixedUpdate ()
	{	
		var touchPos = FindTouchPos();
		if (touchPos.HasValue)
		{
			RotateTowardsAim(touchPos.Value);
		}
		else if (ship)
		{
			ship.pitch = Input.GetAxis("pitch");
			ship.yaw = Input.GetAxis("yaw");				
		}

		//roll is manual only
		ship.roll = -Input.GetAxis("roll");

		ship.thrust = Input.GetAxis("thrust");
		ship.strafe = Input.GetAxis("strafe");
		ship.lift = Input.GetAxis("lift");

		var loadout = GetComponent<ModuleLoadout>();
		if (loadout)
		{
			if (Input.GetButton("fire"))
			{
				loadout.Activate(0);
			}
		}

		Vector2? mouseAimPos = null;

		if (touchPos.HasValue)
		{
			mouseAimPos = touchPos;
		}
		else if (Input.mousePresent)
		{
			mouseAimPos = Input.mousePosition;
		}

		if (mouseAimPos.HasValue)
		{
			Vector3 mousePos = new Vector3(mouseAimPos.Value.x, mouseAimPos.Value.y, 1000);
			var mouseRay = Camera.main.ScreenPointToRay(mousePos);

			RaycastHit rayHit;
			//Debug.Log(mouseRay);
			if (Physics.Raycast(mouseRay, out rayHit))
			{
				ship.aim = rayHit.point;
			}
			else
			{
				ship.aim = mouseRay.origin + mouseRay.direction * 1000;
			}
		}
		else
		{
			ship.aim = transform.position + transform.forward * 1000;
		}
	}

	void Start()
	{
		ship = GetComponent<Ship>();
	}

	void OnCollisionEnter(Collision collision)
	{
		var followCam = Camera.main ? Camera.main.GetComponent<FollowCamera>() : null;
		if (followCam)
		{
			followCam.NotifyPlayerCollision(collision);
		}
	}
}
