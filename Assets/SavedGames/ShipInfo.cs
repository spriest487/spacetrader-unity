using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;

namespace SavedGames
{
    [Serializable]
    class ShipInfo
    {
        private int transientId;
        public int TransientID { get { return transientId; } }

        private string name;

        private string shipType;

        private SerializedVector3 position;
        private SerializedQuaternion rotation;
        private SerializedVector3 linearVelocity;
        private SerializedVector3 angularVelocity;

        private bool targetable;

        private List<float> abilityCooldowns;
        private List<ModuleInfo> equippedModules;

        private CharacterInfo captain;
        
        public ShipInfo()
        {
        }

        public ShipInfo(Ship fromShip, CharacterInfo captain, int transientId) : this()
        {
            this.transientId = transientId;

            var rb = fromShip.GetComponent<Rigidbody>();

            name = fromShip.name;

            position = new SerializedVector3(rb.position);
            rotation = new SerializedQuaternion(rb.rotation);
            linearVelocity = new SerializedVector3(rb.velocity);
            angularVelocity = new SerializedVector3(rb.angularVelocity);

            targetable = fromShip.GetComponent<Targetable>();

            shipType = fromShip.ShipType.name;

            abilityCooldowns = fromShip.Abilities.Select(a => a.Cooldown).ToList();

            equippedModules = fromShip.ModuleLoadout
                .Select(m => m != null? new ModuleInfo(m) : null)
                .ToList();

            this.captain = captain;
        }

        public Ship RestoreShip(IDictionary<int, CrewMember> charactersByTransientId)
        {
            var type = SpaceTraderConfig.Market.BuyableShipTypes.Where(st => st.name == shipType).FirstOrDefault();
            if (!type)
            {
                Debug.Log("no shiptype matches " + shipType);
                return null;
            }

            var ship = type.CreateShip(position.AsVector(), rotation.AsQuaternion());
            var rb = ship.GetComponent<Rigidbody>();
            rb.velocity = linearVelocity.AsVector();
            rb.angularVelocity = angularVelocity.AsVector();

            ship.name = name;

            if (captain != null)
            {
                CrewMember captainCharacter;
                if (charactersByTransientId.TryGetValue(captain.TransientID, out captainCharacter))
                {
                    captainCharacter.Assign(ship, CrewAssignment.Captain);
                }
            }

            if (targetable)
            {
                ship.gameObject.AddComponent<Targetable>();
            }

            if (abilityCooldowns == null || ship.Abilities.Count != abilityCooldowns.Count)
            {
                Debug.LogWarning("invalid ability count in save");
            }

            if (equippedModules != null)
            {
                equippedModules.Select((m, slot) => new { Slot = slot, Module = m })
                    .ToList()
                    .ForEach(em => em.Module.Restore(ship, em.Slot));
            }

            for (int ac = 0; ac < abilityCooldowns.Count; ++ac)
            {
                var cooldown = abilityCooldowns[ac];
                ship.Abilities[ac].Cooldown = cooldown;
            }

            return ship;
        }
    }
}
