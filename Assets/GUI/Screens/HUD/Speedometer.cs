using UnityEngine;
using UnityEngine.UI;

public class Speedometer : MonoBehaviour
{
    [SerializeField]
    private Text throttleLabel;

    [SerializeField]
    private Text speedLabel;

    void LateUpdate()
    {
        var player = PlayerShip.LocalPlayer;

        if (player)
        {
            var throttle = player.Ship.Thrust;
            throttleLabel.text = string.Format("{0:P}", throttle);

            float speed;
            var rb = player.GetComponent<Rigidbody>();
            if (rb)
            {
                speed = rb.velocity.magnitude;
            }
            else
            {
                speed = 0;
            }

            speedLabel.text = string.Format("{0:n0} m/s", speed);
        }
    }
}