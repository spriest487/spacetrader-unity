﻿using UnityEngine;
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

        public string UniqueName
        {
            get
            {
                if (playerShip != null && playerShip.Captain != null)
                {
                    return playerShip.Captain.Name;
                }

                Debug.LogWarning("no unique name available for save, using default name (don't allow game save in this state!)");
                return "Save";
            }
        }

        public static SavedGame CaptureFromCurrentState()
        {
            var result = new SavedGame
            {
                level = SceneManager.GetActiveScene().buildIndex
            };
            var charactersByInstanceId = new Dictionary<int, CharacterInfo>();

            int nextCharacterId = 0;
            foreach (var character in Universe.CrewConfiguration.Characters)
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

            var allFleets = sceneShips.Select<Ship, Fleet>(Universe.FleetManager.GetFleetOf)
                .Where(f => !!f)
                .Distinct();

            result.fleets = allFleets.Select<Fleet, FleetInfo>(f => new FleetInfo(f, shipsByInstanceId)).ToList();
            result.ships = shipsByInstanceId.Values.ToList();

            var player = Universe.LocalPlayer;
            if (player)
            {
                result.playerMoney = player.Money;
                result.playerShip = shipsByInstanceId[player.Ship.GetInstanceID()];
            }

            return result;
        }

        public SaveHeader CreateHeader()
        {
            var pc = playerShip.Captain;
            return new SaveHeader(pc,
                playerMoney,
                SceneManager.GetSceneByBuildIndex(level).name);
        }

        private class RestoreOperation : LoadValueOperation<bool>
        {
            IEnumerator Restore(SavedGame save)
            {
                if (MissionManager.Instance.Mission)
                {
                    yield return MissionManager.Instance.CancelMission();
                }
                else if (Universe.WorldMap.IsWorldSceneActive)
                {
                    yield return Universe.WorldMap.LoadArea(null);
                }

                yield return SceneManager.LoadSceneAsync(save.level, LoadSceneMode.Additive);
                SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(save.level));

                //TODO: to prevent dupes, simply delete all pre-existing ships!
                var oldShips = UnityEngine.Object.FindObjectsOfType<Ship>();
                foreach (var ship in oldShips)
                {
                    UnityEngine.Object.Destroy(ship.gameObject);
                }

                //TODO: need a better way of storing non-scene game session state!
                Universe.CrewConfiguration.Characters.ToList().ForEach(Universe.CrewConfiguration.DestroyCharacter);

                //wait for the nice clean scene next frame
                yield return null;

                var charactersByTransientId = new Dictionary<int, CrewMember>();
                if (save.characters != null)
                {
                    save.characters.ForEach(c => charactersByTransientId.Add(c.TransientID, c.Restore()));
                }

                if (save.ships != null)
                {
                    var shipsByTransientId = save.ships.ToDictionary(s => s.TransientID, s => s.RestoreShip(charactersByTransientId));

                    if (save.fleets != null)
                    {
                        save.fleets.ForEach(f => f.Restore(shipsByTransientId));
                    }

                    if (save.playerShip != null)
                    {
                        var ship = shipsByTransientId[save.playerShip.TransientID];
                        var newLocalPlayer = ship.gameObject.AddComponent<PlayerShip>();
                        newLocalPlayer.AddMoney(save.playerMoney);

                        Universe.LocalPlayer = newLocalPlayer;
                    }
                }

                Result = true;
            }

            public RestoreOperation(SavedGame save)
            {
                Universe.Instance.StartCoroutine(Restore(save));
            }
        }

        public LoadValueOperation<bool> RestoreState()
        {
            return new RestoreOperation(this);
        }
    }
}
