using UnityEngine;

[RequireComponent(typeof(ShipThrusterPoint))]
public class ShipThrusterTintController : MonoBehaviour
{
	public string tintShaderProperty = "_TintColor";
	public int fadeTimeMs = 100;

	private ParticleSystem[] particles;
	private ShipThrusterPoint thruster;

	private float currentIntensity;

	void Start()
	{
		currentIntensity = 0;

		thruster = GetComponent<ShipThrusterPoint>();
		particles = GetComponentsInChildren<ParticleSystem>();
	}
	
	void Update()
	{
		currentIntensity = Mathf.Lerp(currentIntensity, thruster.GetIntensity(), Time.deltaTime * (1000f / fadeTimeMs));
		
		transform.localRotation = thruster.GetDirection();
		SetTint(currentIntensity);
	}

	private void SetTint(float intensity)
	{
		bool enable = intensity > Mathf.Epsilon;

		foreach (var particle in particles)
		{
			particle.GetComponent<Renderer>().enabled = enable;

			if (enabled)
			{
				var material = particle.GetComponent<Renderer>().material;
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
