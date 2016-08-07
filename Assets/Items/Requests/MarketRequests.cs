using System;
using UnityEngine;

namespace MarketRequests
{
    public abstract class MarketRequest : CustomYieldInstruction
    {
        private bool done = false;

        public string Error { get; set; }
        public bool Done
        {
            get
            {
                if (Error != null)
                {
                    return true;
                }

                return done;
            }
            set
            {
                Error = null;
                done = value;
            }
        }

        public override bool keepWaiting
        {
            get
            {
                return !Done;
            }
        }
    }

    public class PlayerTakeLootRequest : MarketRequest
    {
        public int ItemIndex { get; private set; }
        public PlayerShip Player { get; private set; }
        public LootContainer Loot { get; private set; }

        public PlayerTakeLootRequest(PlayerShip player, LootContainer lootContainer, int itemIndex)
        {
            Player = player;
            Loot = lootContainer;
            ItemIndex = itemIndex;
        }
    }
}
