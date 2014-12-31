using UnityEngine;
using System.Collections;

public class DestroyOnFinishParticle : MonoBehaviour
{
	private ParticleSystem[] particles;

	void Start()
	{
		particles = GetComponentsInChildren<ParticleSystem>();
	}

	void Update()
	{
		if (particles != null)
		{
			bool alive = false;

			foreach (var particle in particles)
			{
				if (particle.IsAlive())
				{
					alive = true;
				}
			}

			if (!alive)
			{
				Destroy(gameObject);
			}
		}
	}
}
