using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;

namespace SavedGames
{
    [Serializable]
    class ShipInfo
    {
        private string name;

        private string shipType;

        private SerializedVector3 position;
        private SerializedQuaternion rotation;
        private SerializedVector3 linearVelocity;
        private SerializedVector3 angularVelocity;

        private bool targetable;

        private List<float> abilityCooldowns;
        private List<ModuleInfo> equippedModules;

        private int shield;
        private int armor;

        public int TransientID { get; private set; }

        public CharacterInfo Captain { get; private set; }

        public ShipInfo()
        {
        }

        public ShipInfo(Ship fromShip, CharacterInfo captain, int transientId) : this()
        {
            TransientID = transientId;

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

            var hp = fromShip.GetComponent<Hitpoints>();
            if (hp)
            {
                armor = hp.GetArmor();
                shield = hp.GetShield();
            }
            else
            {
                armor = -1;
                shield = -1;
            }

            Captain = captain;
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

            //doesn't have HP? values should be -1
            if (armor < 0 || shield < 0)
            {
                var hp = ship.GetComponent<Hitpoints>();
                UnityEngine.Object.Destroy(hp);
            }

            ship.name = name;

            if (Captain != null)
            {
                CrewMember captainCharacter;
                if (charactersByTransientId.TryGetValue(Captain.TransientID, out captainCharacter))
                {
                    captainCharacter.Assign(ship, CrewAssignment.Captain);
                }
            }

            if (targetable)
            {
                ship.gameObject.AddComponent<Targetable>();
            }

            if (abilityCooldowns == null || ship.Abilities.Count() != abilityCooldowns.Count)
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
                ship.GetAbility(ac).Cooldown = cooldown;
            }

            return ship;
        }
    }
}
