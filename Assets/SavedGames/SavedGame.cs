using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

namespace SavedGames
{
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
