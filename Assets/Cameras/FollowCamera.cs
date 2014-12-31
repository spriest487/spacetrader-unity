using UnityEngine;
using System.Collections;

public class FollowCamera : MonoBehaviour
{
	public bool ignoreTranslation;

	public Vector3 offset;
	public float thrustOffset;
	public float rotationOffset;
	
	public float shakeMax;
	public float shakeSpeed;

	///world speeds multiplied by this to get the added amount of shake
	public float shakeCollisionConversion; 

	private Vector3 currentSpeedOffset;

	private float shake;
	private Vector3 shakeAngles;

	private void AddShake(float amount)
	{
		shake = Mathf.Clamp(shakeMax, 0, amount + shake);
	}

	public void NotifyPlayerCollision(Collision collision)
	{
		AddShake(collision.relativeVelocity.magnitude);
	}

	void Start()
	{
		currentSpeedOffset = Vector3.zero;
		shakeAngles = new Vector3(Random.value * 2 - 1,
			Random.value * 2 - 1,
			Random.value * 2 - 1);

		shake = shakeCollisionConversion * 6;
	}

	void LateUpdate()
	{
		var player = GameObject.FindWithTag("Player");
		
		transform.position = offset;
		if (player)
		{
			if (!ignoreTranslation)
			{
				var newPos = player.transform.TransformPoint(offset);
				newPos -= currentSpeedOffset;

				var dilutedShake = shake * 0.05f;
				var shakePhase = shakeSpeed * Time.frameCount;

				var shakeAmount = new Vector3(
					shake * Mathf.Sin(shakePhase * shakeAngles.x),
					shake * Mathf.Sin(shakePhase * shakeAngles.y),
					shake * Mathf.Sin(shakePhase * shakeAngles.z));
				newPos += transform.TransformDirection(shakeAmount);
							
				transform.position = newPos;

				transform.LookAt(player.transform.position + (player.transform.forward * 1000), player.transform.up);
			}
			else
			{
				transform.position = Vector3.zero;
				transform.LookAt(player.transform.forward * 1000, player.transform.up);
			}

			float rotationOffsetAmt = rotationOffset * Mathf.Rad2Deg;
			var rotOffsetAmt = Quaternion.Euler(player.rigidbody.angularVelocity.x * rotationOffsetAmt,
				player.rigidbody.angularVelocity.y * rotationOffsetAmt,
				player.rigidbody.angularVelocity.z * rotationOffsetAmt);
			transform.rotation = rotOffsetAmt * transform.rotation;
		}
		else
		{
			transform.position = Vector3.zero;
			transform.rotation = Quaternion.identity;
		}		
	}

	void FixedUpdate()
	{		
		var player = GameObject.FindWithTag("Player");
		if (!player)
		{
			return;
		}

		shake = Mathf.Lerp(shake, 0, 0.5f);

		var playerRb = player.GetComponent<Rigidbody>();
		var ship = player.GetComponent<Ship>();
		
		if (playerRb && ship)
		{
			var speed = playerRb.velocity / ship.stats.maxSpeed;
			var targetSpeedOffset = speed * thrustOffset;

			currentSpeedOffset = Vector3.Lerp(currentSpeedOffset, targetSpeedOffset, 0.1f);
		}
	}
}
