using System;
using System.Collections.Generic;
using UnityEngine;

public class Hitpoints : MonoBehaviour
{
    [Serializable]
    public class HitpointValue
    {
        public int current = 1;
        public int max = 1;
    }

    [Serializable]
    public class ShieldValue : HitpointValue
    {
        public float weight;
    }

    public ShieldValue[] shieldSectors;
	public HitpointValue armor;

    public Hitpoints(int shieldSectorCount, int maxArmor, int maxShields)
    {
		if (shieldSectorCount < 1)
		{
			throw new ArgumentException("Must have at least one sector");
		}

		armor = new HitpointValue();
		armor.current = maxArmor;
		armor.max = maxArmor;

		int shieldPerSector = maxShields / shieldSectorCount;

        shieldSectors = new ShieldValue[shieldSectorCount];  
        for (int sector = 0; sector < shieldSectorCount; ++sector)
        {
            var shieldValue = new ShieldValue();
			shieldValue.current = shieldPerSector;
			shieldValue.max = shieldPerSector;
			shieldValue.weight = 1;
            shieldSectors[sector] = shieldValue;
        }
    }

    public Hitpoints(int maxArmor, int maxShields):
		this(1, maxArmor, maxShields)
    {
    }

    public void SetShield(int sector, int value)
    {
	    var shieldSector = shieldSectors[sector];
	
	    shieldSector.max = value;
	    shieldSector.current = Math.Min(shieldSector.max, shieldSector.current);
    }

	public void SetSectorWeight(int sector, float value)
	{
		var shieldSector = shieldSectors[sector];
		shieldSector.weight = value;
	}

    public void SetArmor(int value)
    {
	    armor.max = value;
	    armor.current = Math.Min(armor.max, armor.current);
    }

    public void TakeDamage(int amount) {
        var sectorCount = shieldSectors.Length;
	    var amountPerSector = Math.Max(0, amount / sectorCount);
	    
        var damage = new List<int>();
        int sectorIt = 0;
        for (sectorIt = 0; sectorIt < sectorCount; ++sectorIt)
        {
            damage.Add(amountPerSector);
        }

	    int remainder = amount % sectorCount;
	
        sectorIt = 0;
	    while (remainder > 0) {
		    ++damage[sectorIt];
            --remainder;

		    sectorIt = (int)((sectorIt + 1) % sectorCount);
	    }

	    for (sectorIt = 0; sectorIt < damage.Count; ++sectorIt) {
		    TakeDamageInSector(damage[sectorIt], sectorIt);
	    }
    }

    public void TakeDamageInSector(int amount, int sector) {
		var sectorValue = shieldSectors[sector];

		var remaining = sectorValue.current - amount;
		sectorValue.current = Math.Max(0, remaining);

	    if (remaining <= 0) {
		    TakeDamageToArmor(-remaining);
	    }
    }

    public void TakeDamageToArmor(int amount) {
	    armor.current = Math.Max(0, armor.current - amount);
    }

    public void HealArmor(int amount) {
	    armor.current = Math.Max(armor.max, armor.current + amount);
    }

    private bool IsSectorHealed(List<int> healed, int sectorIt)
    {
        return shieldSectors[sectorIt].current + healed[sectorIt] >= shieldSectors[sectorIt].max;
    }

    private bool IsFullyHealed(List<int> healed)
    {
        bool result = true;

        for (int sectorIt = 0; sectorIt < shieldSectors.Length; ++sectorIt)
        {
            result &= IsSectorHealed(healed, sectorIt);
        }

        return result;
    }

    private void RecurseHealShield(int previousRemainder, List<int> healed, int amount)
    {
        float weightTotal = 0;
		foreach (var sector in shieldSectors) {
			weightTotal += sector.weight;
		}

		int newRemainder = 0;

		int sectorIt;
		for (sectorIt = 0; sectorIt < shieldSectors.Length; ++sectorIt) {
			var shieldSector = shieldSectors[sectorIt];

			healed[sectorIt] = Mathf.FloorToInt(amount * (shieldSectors[sectorIt].weight / weightTotal));
			
			int total = healed[sectorIt] + shieldSector.current;
			if (total > shieldSector.max) {
				int extra = total - shieldSector.max;
				newRemainder += extra;
				healed[sectorIt] -= extra;
			}
		}

		if (newRemainder != previousRemainder) {
			RecurseHealShield(newRemainder, healed, amount);
		}
		else {
			sectorIt = 0;
			while (!(IsFullyHealed(healed) || newRemainder < 1)) {
				if (!IsSectorHealed(healed, sectorIt)) {
					++shieldSectors[sectorIt].current;
				}

				sectorIt = (sectorIt + 1) % shieldSectors.Length;
			}
		}
    }

    public void HealShield(int amount) {
	    var healed = new List<int>(shieldSectors.Length);
        foreach (var shieldSector in shieldSectors)
        {
            healed.Add(0);
        }

	    RecurseHealShield(0, healed, amount);

	    for (int sectorIt = 0; sectorIt < shieldSectors.Length; ++sectorIt) {
		    shieldSectors[sectorIt].current += healed[sectorIt];
	    }
    }

	public void DistributeShield()
	{
	    int totalShields = 0;
	    foreach (var shieldSector in shieldSectors) {
		    totalShields += shieldSector.current;
		    shieldSector.current = 0;
	    }

	    HealShield(totalShields);
    }

	public void ResetShield()
	{
	    foreach (var shieldSector in shieldSectors) {
		    shieldSector.current = shieldSector.max;
	    }
    }

	public void ResetArmor()
	{
	    armor.current = armor.max;
    }

	public int GetArmor()
	{
	    return armor.current;
    }

	public int GetShield(int sector)
	{
		if (sector < 0 || sector >= shieldSectors.Length)
		{
			throw new ArgumentException(string.Format("Invalid sector {0}, sector count is {1}", sector, shieldSectors.Length));
		}

	    return shieldSectors[sector].current;
    }

	public int GetMaxArmor()
	{
	    return armor.max;
    }

    public int GetMaxShields(int sector) {
		if (sector < 0 || sector >= shieldSectors.Length)
		{
			throw new ArgumentException(string.Format("Invalid sector {0}, sector count is {1}", sector, shieldSectors.Length));
		}

	    return shieldSectors[sector].max;
    }

	void OnTakeDamage(HitDamage damage)
	{
		TakeDamageToArmor(damage.amount);
	}
}
