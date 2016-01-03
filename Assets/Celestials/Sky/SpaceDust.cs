﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ParticleSystem))]
public class SpaceDust : MonoBehaviour {
	public int count = 100;
	public int radius = 10;
	public int fadeDistance = 10;

	public float size = 0.05f;
	
	private ParticleSystem.Particle[] particles;
	
	private ParticleSystem.Particle NewPoint(Vector3 center)
	{
		var result = new ParticleSystem.Particle();
		result.position = Random.insideUnitSphere * radius;
	
		var colorVal = 0.8f + 0.2f * Random.value;
		result.startColor = new Color(colorVal, colorVal, colorVal);
		result.startSize = size;

		result.position += center;

		return result;
	}

	public void Update()
	{
		Vector3 center = transform.position;

		if (particles == null)
		{
			particles = new ParticleSystem.Particle[count];
			for (int index = 0; index < count; ++index)
			{
				particles[index] = NewPoint(center);
			}
		}		

		var maxSqrDist = Mathf.Pow(radius, 2);

		for (int posIt = 0; posIt < count; ++posIt)
		{
			var sqrDist = (particles[posIt].position - center).sqrMagnitude;
			if (sqrDist > maxSqrDist)
			{
				particles[posIt] = NewPoint(center);
			}
		}

		var fadeDistSqr = Mathf.Pow(fadeDistance, 2);
		for (int particleIt = 0; particleIt < particles.Length; ++particleIt)
		{
			var sqrDist = (particles[particleIt].position - center).sqrMagnitude;
			float fadeAmt = 1-(sqrDist / fadeDistSqr);
			fadeAmt = Mathf.Clamp(fadeAmt, 0, 1);

			var color = particles[particleIt].startColor;
			particles[particleIt].startColor = new Color(
				color.r,
				color.g,
				color.b,
				fadeAmt);
		}

		GetComponent<ParticleSystem>().maxParticles = particles.Length;
		GetComponent<ParticleSystem>().SetParticles(particles, particles.Length);
	}
}
