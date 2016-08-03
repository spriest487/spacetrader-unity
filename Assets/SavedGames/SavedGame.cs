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

        private List<CharacterInfo> characters;
        
        private List<ShipInfo> ships;
        private List<FleetInfo> fleets;

        private ShipInfo playerShip;
        
        private int playerMoney;
        
        public static SavedGame CaptureFromCurrentState()
        {
            var result = new SavedGame();

            result.level = SceneManager.GetActiveScene().buildIndex;

            var charactersByInstanceId = new Dictionary<int, CharacterInfo>();

            int nextCharacterId = 0;
            foreach (var character in SpaceTraderConfig.CrewConfiguration.Characters)
            {
                charactersByInstanceId[character.GetInstanceID()] = new CharacterInfo(character, nextCharacterId++);
            }

            result.characters = charactersByInstanceId.Values.ToList();
            
            var sceneShips = UnityEngine.Object.FindObjectsOfType<Ship>();

            var shipsByInstanceId = new Dictionary<int, ShipInfo>();
            int nextTransientId = 0;
            foreach (var ship in sceneShips)
            {
                var captain = ship.GetCaptain();

                CharacterInfo captainInfo;
                if (!captain || !charactersByInstanceId.TryGetValue(captain.GetInstanceID(), out captainInfo))
                {
                    captainInfo = null;
                }

                shipsByInstanceId[ship.GetInstanceID()] = new ShipInfo(ship, captainInfo, nextTransientId++);
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

            //TODO: need a better way of storing non-scene game session state!
            SpaceTraderConfig.CrewConfiguration.Characters.ToList().ForEach(SpaceTraderConfig.CrewConfiguration.DestroyCharacter);

            //wait for the nice clean scene next frame
            yield return null;

            var charactersByTransientId = new Dictionary<int, CrewMember>();
            if (characters != null)
            {
                characters.ForEach(c => charactersByTransientId.Add(c.TransientID, c.Restore()));
            }

            if (ships != null)
            {
                var shipsByTransientId = ships.ToDictionary(s => s.TransientID, s => s.RestoreShip(charactersByTransientId));

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
