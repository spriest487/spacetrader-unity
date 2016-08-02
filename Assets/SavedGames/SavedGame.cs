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
    class FleetInfo
    {
        private ShipInfo Leader;
        private List<ShipInfo> Followers;

        public FleetInfo()
        {
        }

        public FleetInfo(Fleet fleet, Dictionary<int, ShipInfo> shipsByInstanceId)
        {
            Leader = shipsByInstanceId[fleet.Leader.GetInstanceID()];
            Followers = fleet.Followers.Select(f => 
                shipsByInstanceId[f.GetInstanceID()]).ToList();
        }

        public void Restore(Dictionary<int, Ship> shipsByTransientId)
        {
            var leader = shipsByTransientId[Leader.TransientID];
            Followers.ForEach(f => 
                SpaceTraderConfig.FleetManager.AddToFleet(leader, shipsByTransientId[f.TransientID]));
        }
    }
    
    [Serializable]
    class SavedGame
    {
        //loaded level scene id
        private int level;
        
        private List<ShipInfo> ships;
        private List<FleetInfo> fleets;

        private ShipInfo playerShip;
        
        private int playerMoney;

        public static SavedGame CaptureFromCurrentState()
        {
            var result = new SavedGame();

            result.level = SceneManager.GetActiveScene().buildIndex;

            result.ships = new List<ShipInfo>();

            var sceneShips = UnityEngine.Object.FindObjectsOfType<Ship>();

            var shipsByInstanceId = new Dictionary<int, ShipInfo>();
            int nextTransientId = 0;
            foreach (var ship in sceneShips)
            {
                shipsByInstanceId[ship.GetInstanceID()] = new ShipInfo(ship, nextTransientId++);
            }

            var allFleets = sceneShips.Select<Ship, Fleet>(SpaceTraderConfig.FleetManager.GetFleetOf)
                .Where(f => !!f)
                .Distinct();

            result.fleets = allFleets.Select<Fleet, FleetInfo>(f => new FleetInfo(f, shipsByInstanceId)).ToList();
            result.ships = shipsByInstanceId.Values.ToList();
            
            var player = PlayerShip.LocalPlayer;
            if (player)
            {
                result.playerMoney = player.Money;
                result.playerShip = shipsByInstanceId[player.Ship.GetInstanceID()];
            }

            return result;
        }

        public IEnumerator RestoreState()
        {
            yield return SceneManager.LoadSceneAsync(level);

            //TODO: to prevent dupes, simply delete all pre-existing ships!
            var oldShips = UnityEngine.Object.FindObjectsOfType<Ship>();
            foreach (var ship in oldShips)
            {
                UnityEngine.Object.Destroy(ship.gameObject);
            }

            //wait for the nice clean scene next frame
            yield return null;

            if (ships != null)
            {
                var shipsByTransientId = ships.ToDictionary(s => s.TransientID, s => s.RestoreShip());

                if (fleets != null)
                {
                    fleets.ForEach(f => f.Restore(shipsByTransientId));
                }

                if (playerShip != null)
                {
                    var ship = shipsByTransientId[playerShip.TransientID];
                    var newLocalPlayer = ship.gameObject.AddComponent<PlayerShip>();
                    newLocalPlayer.AddMoney(playerMoney);

                    SpaceTraderConfig.LocalPlayer = newLocalPlayer;
                }
            }
        }
    }
}
