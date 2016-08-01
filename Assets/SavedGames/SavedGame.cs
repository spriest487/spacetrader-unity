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
            Z = quat.z;
            W = quat.w;
        }
    }

    [Serializable]
    class SavedGame
    {
        //loaded level scene id
        private int level;

        private List<ShipInfo> fleetShips;
        private int playerMoney;

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
                    result.fleetShips.AddRange(fleet.Followers.Select(ship => new ShipInfo(ship)));
                }

                result.playerMoney = player.Money;
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

                if (player)
                {
                    var localPlayer = SpaceTraderConfig.LocalPlayer = player.gameObject.AddComponent<PlayerShip>();
                    localPlayer.AddMoney(playerMoney);
                }

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
