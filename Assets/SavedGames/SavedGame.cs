using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

namespace SavedGames
{
    [Serializable]
    struct SerializedVector3
    {
        public float X, Y, Z;

        public Vector3 AsVector()
        {
            return new Vector3(X, Y, Z);
        }

        public SerializedVector3(Vector3 vec3)
        {
            X = vec3.x;
            Y = vec3.y;
            Z = vec3.z;
        }
    }

    [Serializable]
    struct SerializedQuaternion
    {
        public float X, Y, Z, W;

        public Quaternion AsQuaternion()
        {
            return new Quaternion(X, Y, Z, W);
        }

        public SerializedQuaternion(Quaternion quat)
        {
            X = quat.x;
            Y = quat.y;
            Z = quat.y;
            W = quat.w;
        }
    }

    [Serializable]
    class ShipInfo
    {
        private string shipType;

        private SerializedVector3 position;
        private SerializedQuaternion rotation;
        private SerializedVector3 linearVelocity;
        private SerializedVector3 angularVelocity;

        public ShipInfo()
        {
        }

        public ShipInfo(Ship fromShip) : this()
        {
            var rb = fromShip.GetComponent<Rigidbody>();
            
            position = new SerializedVector3(rb.position);
            rotation = new SerializedQuaternion(rb.rotation);
            linearVelocity = new SerializedVector3(rb.velocity);
            angularVelocity = new SerializedVector3(rb.angularVelocity);

            shipType = fromShip.ShipType.name;
        }

        public Ship RestoreShip()
        {
            var type = SpaceTraderConfig.Market.BuyableShipTypes.Where(st => st.name == shipType).FirstOrDefault();
            if (!type)
            {
                return null;
            }

            var ship = type.CreateShip(position.AsVector(), rotation.AsQuaternion());
            var rb = ship.GetComponent<Rigidbody>();
            rb.velocity = linearVelocity.AsVector();
            rb.angularVelocity = angularVelocity.AsVector();

            return ship;
        }
    }

    [Serializable]
    class SavedGame
    {
        [SerializeField]
        private int level;

        [SerializeField]
        private List<ShipInfo> fleetShips;

        public static SavedGame CaptureFromCurrentState()
        {
            var result = new SavedGame();

            result.level = SceneManager.GetActiveScene().buildIndex;

            result.fleetShips = new List<ShipInfo>();

            var player = PlayerShip.LocalPlayer;
            if (player)
            {
                result.fleetShips.Add(new ShipInfo(player.Ship));

                var fleet = SpaceTraderConfig.FleetManager.GetFleetOf(player.Ship);
                if (fleet)
                {
                    result.fleetShips.AddRange(fleet.Members.Select(ship => new ShipInfo(ship)));
                }
            }

            return result;
        }

        public IEnumerator RestoreState()
        {
            yield return SceneManager.LoadSceneAsync(level);

            //TODO: to prevent dupes, simply delete all pre-existing ships!
            var ships = UnityEngine.Object.FindObjectsOfType<Ship>();
            foreach (var ship in ships)
            {
                UnityEngine.Object.Destroy(ship.gameObject);
            }

            if (fleetShips != null)
            {
                var player = fleetShips.Select(s => s.RestoreShip())
                    .FirstOrDefault();

                var fleetMembers = fleetShips.Skip(1)
                    .Select(s => s.RestoreShip())
                    .Where(ship => ship != null)
                    .ToList();

                fleetMembers.ForEach(ship => 
                    SpaceTraderConfig.FleetManager.AddToFleet(player, ship));
            }
        }
    }
}
