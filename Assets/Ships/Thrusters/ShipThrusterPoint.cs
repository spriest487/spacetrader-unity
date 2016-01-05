using UnityEngine;

public class ShipThrusterPoint : MonoBehaviour
{
	const float CENTER_MARGIN = 0.1f;
	
	public enum HullPos
	{
		MIDDLE,
		POSITIVE,
		NEGATIVE
	}

	public enum ThrusterDirection
	{
		LEFT,
		RIGHT,
		UP,
		DOWN,
		FORWARD,
		BACK
	}

	private HullPos xPos;
	private HullPos yPos;
	private HullPos zPos;
	
	public ThrusterDirection direction;
	private Quaternion directionRot;

	private float intensity;

	private Ship ship;

	public Quaternion GetDirection()
	{
		return directionRot;
	}

	public float GetIntensity()
	{
		return intensity;
	}

	public HullPos GetXPos()
	{
		return xPos;
	}

	public HullPos GetYPos()
	{
		return yPos;
	}

	public HullPos GetZPos()
	{
		return zPos;
	}

	void Start()
	{
		ship = GetComponentInParent<Ship>();
		if (!ship)
		{
			throw new UnityException("Requires a Ship parent");
		}

		intensity = 0;

		//var offset = transform.localPosition;
		var offset = ship.transform.InverseTransformPoint(transform.position);

		if (offset.x > CENTER_MARGIN)
		{
			xPos = HullPos.POSITIVE;
		}
		else if (offset.x < -CENTER_MARGIN)
		{
			xPos = HullPos.NEGATIVE;
		}
		else
		{
			xPos = HullPos.MIDDLE;
		}

		if (offset.y > CENTER_MARGIN)
		{
			yPos = HullPos.POSITIVE;
		}
		else if (offset.y < -CENTER_MARGIN)
		{
			yPos = HullPos.NEGATIVE;
		}
		else
		{
			yPos = HullPos.MIDDLE;
		}

		if (offset.z > CENTER_MARGIN)
		{
			zPos = HullPos.POSITIVE;
		}
		else if (offset.z < -CENTER_MARGIN)
		{
			zPos = HullPos.NEGATIVE;
		}
		else
		{
			zPos = HullPos.MIDDLE;
		}

		directionRot = new Quaternion();
		switch (direction)
		{
			case ThrusterDirection.UP:
				directionRot.eulerAngles = new Vector3(-90, 0, 0);
				break;
			case ThrusterDirection.DOWN:
				directionRot.eulerAngles = new Vector3(90, 0, 0);
				break;
			case ThrusterDirection.LEFT:
				directionRot.eulerAngles = new Vector3(0, -90, 0);
				break;
			case ThrusterDirection.RIGHT:
				directionRot.eulerAngles = new Vector3(0, 90, 0);
				break;
		}

		//Debug.Log(string.Format("Local pos is {0}, XPos is {1}, YPos is {2}, ZPos is {3}", offset.ToString("F2"), xPos, yPos, zPos));
	}

	void Update()
	{
		//weights
		float thrust = 0;
		float strafe = 0;
		float lift = 0;

		float roll = 0;
		float yaw = 0;
		float pitch = 0;

		switch (direction)
		{
			case ThrusterDirection.LEFT:
				strafe = 1;
				break;
			case ThrusterDirection.RIGHT:
				strafe = -1;
				break;
			case ThrusterDirection.UP:
				lift = -1;
				break;
			case ThrusterDirection.DOWN:
				lift = 1;
				break;
		}

		switch (xPos)
		{
			case HullPos.POSITIVE:
				switch (direction)
				{
					case ThrusterDirection.UP:
						roll = 1;
						break;
					case ThrusterDirection.DOWN:
						roll = -1;
						break;
				}
				break;
			case HullPos.NEGATIVE:
				switch (direction)
				{
					case ThrusterDirection.DOWN:
						roll = 1;
						break;
					case ThrusterDirection.UP:
						roll = -1;
						break;
				}
				break;
		}

		switch (zPos)
		{
			case HullPos.POSITIVE:
				switch (direction)
				{
					case ThrusterDirection.LEFT:
						yaw = 1;
						break;
					case ThrusterDirection.RIGHT:
						yaw = -1;
						break;
					case ThrusterDirection.UP:
						pitch = 1;
						break;
					case ThrusterDirection.DOWN:
						pitch = -1;
						break;
				}
				break;
			case HullPos.NEGATIVE:
				switch (direction)
				{
					case ThrusterDirection.RIGHT:
						yaw = 1;
						break;
					case ThrusterDirection.LEFT:
						yaw = -1;
						break;
					case ThrusterDirection.DOWN:
						pitch = 1;
						break;
					case ThrusterDirection.UP:
						pitch = -1;
						break;
				}
				break;
		}

		switch (direction)
		{
			case ThrusterDirection.BACK:
				thrust = 1;
				break;
			case ThrusterDirection.FORWARD:
				thrust = -1;
				break;
		}

		intensity = Mathf.Clamp01(ship.Thrust * thrust);
		intensity += Mathf.Clamp01(ship.Strafe * strafe);
		intensity += Mathf.Clamp01(ship.Lift * lift);
		intensity += Mathf.Clamp01(ship.Roll * roll);
		intensity += Mathf.Clamp01(ship.Yaw * yaw);
		intensity += Mathf.Clamp01(ship.Pitch * pitch);

		intensity = Mathf.Clamp01(intensity);
	}
}
