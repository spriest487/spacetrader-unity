using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;

namespace SavedGames
{
    [Serializable]
    class ModuleInfo
    {
        private string itemType;
        private float cooldown;

        public ModuleInfo()
        {
        }

        public ModuleInfo(HardpointModule fromModule)
        {
            itemType = fromModule.ModuleType ? fromModule.ModuleType.name : null;
            cooldown = fromModule.Cooldown;
        }

        public void Restore(Ship ship, int index)
        {
            ship.ModuleLoadout.Equip(index, null);

            ModuleItemType moduleType;
            if (itemType != null)
            {
                moduleType = SpaceTraderConfig.CargoItemConfiguration.FindType(itemType) as ModuleItemType;

                if (moduleType)
                {
                    ship.ModuleLoadout.Equip(index, moduleType);
                    ship.ModuleLoadout.GetSlot(index).Cooldown = cooldown;
                }
            }
        }
    }
}
