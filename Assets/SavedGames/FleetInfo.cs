using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SavedGames
{
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
}
