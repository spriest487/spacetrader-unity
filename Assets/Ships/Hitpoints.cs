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

        public HitpointValue()
        {
        }
        
        public HitpointValue(int val)
        {
            this.current = this.max = val;
        }
    }

    [Serializable]
    public class ShieldValue : HitpointValue
    {
        public float weight = 1;

        public ShieldValue()
        {
        }

        public ShieldValue(int val, float weight) : base(val)
        {
            this.weight = weight;
        }
    }

    [SerializeField]
    private ShieldValue[] shieldSectors;

    [SerializeField]
	private HitpointValue armor;

    [SerializeField]
    private float lastHealTick;
        
    public void Reset(int armor, int[] shieldSectors)
    {
        this.armor = new HitpointValue(armor);

		this.shieldSectors = new ShieldValue[shieldSectors.Length];

        int shieldTotal = 0;
        foreach (int sector in shieldSectors) 
        {
            shieldTotal += sector;
        }

        for (int sectorNo = 0; sectorNo < shieldSectors.Length; ++sectorNo)
        {
            int sectorVal = shieldSectors[sectorNo];
            float weight = shieldTotal / (float) sectorVal;

            this.shieldSectors[sectorNo] = new ShieldValue(sectorVal, weight);
        }

        lastHealTick = 0;
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
        if (amount == 0)
        {
            return;
        }

        //for now... uniform distribution across shields before any armor damage
        var sectorCount = shieldSectors.Length;
	    var amountPerSector = Math.Max(1, amount / sectorCount);
        	
	    for (int sector = 0; sector < sectorCount; ++sector) {
		    TakeDamageInSector(amountPerSector, sector);
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

    private bool IsSectorHealed(int[] healed, int sectorIt)
    {
        return shieldSectors[sectorIt].current + healed[sectorIt] >= shieldSectors[sectorIt].max;
    }

    private bool IsFullyHealed(int[] healed)
    {
        bool result = true;

        for (int sectorIt = 0; sectorIt < shieldSectors.Length; ++sectorIt)
        {
            result &= IsSectorHealed(healed, sectorIt);
        }

        return result;
    }

    private void RecurseHealShield(int previousRemainder, int[] healed, int amount)
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
        var healed = new int[shieldSectors.Length];
        for (int sector = 0; sector < shieldSectors.Length; ++sector)
        {
            healed[sector] = 0;
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
		TakeDamage(damage.Amount);
	}

    void Update()
    {
        const float HEAL_TICK_RATE = 1.0f;
        
        lastHealTick += Time.deltaTime;

        if (lastHealTick > HEAL_TICK_RATE)
        {
            lastHealTick -= HEAL_TICK_RATE;

            var healPerShield = 1;
            var healAmount = shieldSectors.Length * healPerShield;

            HealShield(healAmount);
        }
    }
}
