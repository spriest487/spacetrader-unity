#pragma warning disable 0649

using UnityEngine;

public class ShipThrusterPoint : MonoBehaviour
{
	const float CENTER_MARGIN = 0.1f;
    const float MIN_VISIBLE_THRUST = 0.01f;
	
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

    [SerializeField]
    private Transform effectRoot;

    [HideInInspector]
    [SerializeField]
    private Renderer[] thrusterParticles;

    [SerializeField]
    private bool miniThruster;

    [SerializeField]
    private string tintShaderProperty = "_TintColor";

    [SerializeField]
    private int fadeTimeMs = 100;
        
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
        Debug.Assert(ship, "ShipThrusterPoint must be a child of a Ship");

        if (!effectRoot && ship.ShipType)
        {
            var baseEffect = miniThruster ? ship.ShipType.MiniThrusterEffect : ship.ShipType.ThrusterEffect;
            if (baseEffect)
            {
                effectRoot = Instantiate(baseEffect, transform, false);
                effectRoot.localPosition = Vector3.zero;
                effectRoot.localRotation = Quaternion.identity;
            }
        }

        if (effectRoot)
        {
            thrusterParticles = effectRoot.GetComponentsInChildren<Renderer>();
        }
        else
        {
            thrusterParticles = new Renderer[0];
        }

		intensity = 0;
        
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
            case ThrusterDirection.FORWARD:
                directionRot.eulerAngles = new Vector3(0, 0, 0);
                break;
            case ThrusterDirection.BACK:
                directionRot.eulerAngles = new Vector3(0, 180, 0);
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
        
		var nextIntensity = Mathf.Clamp01(ship.Thrust * thrust);
		nextIntensity += Mathf.Clamp01(ship.Strafe * strafe);
		nextIntensity += Mathf.Clamp01(ship.Lift * lift);
		nextIntensity += Mathf.Clamp01(ship.Roll * roll);
		nextIntensity += Mathf.Clamp01(ship.Yaw * yaw);
		nextIntensity += Mathf.Clamp01(ship.Pitch * pitch);

		nextIntensity = Mathf.Clamp01(nextIntensity);

        var smoothIntensity = Mathf.Lerp(intensity, nextIntensity, Time.deltaTime * (1000f / fadeTimeMs));
        SetTint(smoothIntensity);

        intensity = smoothIntensity;

        if (effectRoot)
        {
            effectRoot.localRotation = directionRot;
        }
    }

    private void SetTint(float intensity)
    {
        bool thrusterVisible = intensity > MIN_VISIBLE_THRUST;

        foreach (var renderer in thrusterParticles)
        {
            renderer.enabled = thrusterVisible;

            if (thrusterVisible)
            {
                var material = renderer.material;
                if (material)
                {
                    var color = material.GetColor(tintShaderProperty);
                    var newColor = new Color(color.r, color.g, color.b, Mathf.Clamp01(intensity));
                    material.SetColor(tintShaderProperty, newColor);
                }
            }
        }
    }
}
