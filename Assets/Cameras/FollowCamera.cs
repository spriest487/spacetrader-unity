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

    void DockedCam(PlayerShip player, SpaceStation station)
    {
        transform.rotation = Quaternion.identity;
        transform.position = station.transform.position;
        transform.position -= new Vector3(0, 0, 100);
    }

    void FlightCam(PlayerShip player)
    {
        transform.position = offset;
        if (player)
        {
            if (!ignoreTranslation)
            {
                var newPos = player.transform.TransformPoint(offset);
                newPos -= currentSpeedOffset;

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
            var rotOffsetAmt = Quaternion.Euler(player.GetComponent<Rigidbody>().angularVelocity.x * rotationOffsetAmt,
                player.GetComponent<Rigidbody>().angularVelocity.y * rotationOffsetAmt,
                player.GetComponent<Rigidbody>().angularVelocity.z * rotationOffsetAmt);
            transform.rotation = rotOffsetAmt * transform.rotation;
        }
        else
        {
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
        }		
    }

    void CutsceneCam(CutsceneCameraRig cutsceneCamRig)
    {
        transform.position = cutsceneCamRig.View;

        var lookDirection = (cutsceneCamRig.Focus - cutsceneCamRig.View).normalized;
        transform.rotation = Quaternion.LookRotation(lookDirection);
    }

    void LateUpdate()
    {
        var cutsceneCamRig = ScreenManager.Instance.CurrentCutsceneCameraRig;
        if (cutsceneCamRig)
        {
            CutsceneCam(cutsceneCamRig);
        }
        else
        {
            var player = PlayerShip.LocalPlayer;

            if (player)
            {
                var moorable = player.GetComponent<Moorable>();
                if (moorable && moorable.Moored)
                {
                    DockedCam(player, moorable.SpaceStation);
                }
                else
                {
                    FlightCam(player);
                }
            }
            else
            {
                transform.position = Vector3.zero;
                transform.rotation = Quaternion.identity;
            }
        }        
	}

	void FixedUpdate()
	{
        var player = PlayerShip.LocalPlayer;
		if (!player)
		{
			return;
		}

		shake = Mathf.Lerp(shake, 0, 0.5f);

		var playerRb = player.GetComponent<Rigidbody>();
		var ship = player.GetComponent<Ship>();
		
		if (playerRb && ship)
		{
			var speed = playerRb.velocity / Mathf.Max(1, ship.BaseStats.maxSpeed);
			var targetSpeedOffset = speed * thrustOffset;

			currentSpeedOffset = Vector3.Lerp(currentSpeedOffset, targetSpeedOffset, 0.1f);
		}
	}
}
