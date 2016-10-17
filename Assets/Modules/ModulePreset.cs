#pragma warning disable 0649

using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "SpaceTrader/Modules/Module Preset")]
public class ModulePreset : ScriptableObject
{
    [SerializeField]
    private List<ModuleItemType> slots;

    [SerializeField]
    private List<CargoItemType> cargoItems;

    public IEnumerable<ModuleItemType> FrontModules { get { return slots; } }
    public IEnumerable<CargoItemType> CargoItems { get { return cargoItems; } }

    public void Apply(Ship ship)
    {
        var moduleLoadout = ship.ModuleLoadout;
        moduleLoadout.SlotCount = slots.Count;
        
        for (int slot = 0; slot < slots.Count; ++slot)
        {
            var moduleType = slots[slot];
            moduleLoadout.Equip(slot, moduleType);
        }

        ship.Cargo = CreateInstance<CargoHold>();
        ship.Cargo.Size = cargoItems.Count;
        for (int cargoIt = 0; cargoIt < cargoItems.Count; ++cargoIt)
        {
            ship.Cargo[cargoIt] = cargoItems[cargoIt];
        }
    }
}
