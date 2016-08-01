using UnityEngine;
using System.Linq;
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

        public ShipInfo()
        {
        }

        public ShipInfo(Ship fromShip) : this()
        {
            var rb = fromShip.GetComponent<Rigidbody>();

            name = fromShip.name;

            position = new SerializedVector3(rb.position);
            rotation = new SerializedQuaternion(rb.rotation);
            linearVelocity = new SerializedVector3(rb.velocity);
            angularVelocity = new SerializedVector3(rb.angularVelocity);

            targetable = fromShip.GetComponent<Targetable>();

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

            ship.name = name;

            if (targetable)
            {
                ship.gameObject.AddComponent<Targetable>();
            }

            return ship;
        }
    }
}
