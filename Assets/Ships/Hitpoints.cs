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

    [SerializeField]
    private HitpointValue shield;

    [SerializeField]
	private HitpointValue armor;

    [SerializeField]
    private float lastHealTick;
        
    public void Reset(int armor, int shield)
    {
        this.armor = new HitpointValue(armor);
        this.shield = new HitpointValue(shield);
        
        lastHealTick = 0;
    }
    
    public void SetShield(int value)
    {
        shield.max = value;
        shield.current = Math.Min(shield.max, shield.current);
    }

    public void SetArmor(int value)
    {
	    armor.max = value;
	    armor.current = Math.Min(armor.max, armor.current);
    }

    public void TakeDamage(int amount)
    {
        if (PlayerShip.LocalPlayer)
        {
            GUIController.Current.BroadcastMessage("OnDamageNotification",
                new DamageNotification(amount, this),
                SendMessageOptions.DontRequireReceiver);
        }

        if (amount == 0)
        {
            return;
        }

        var remainingDamage = amount - shield.current;
        shield.current -= amount;

        if (remainingDamage > 0)
        {
            armor.current -= remainingDamage;
        }

        shield.current = Math.Max(0, shield.current);
        armor.current = Math.Max(0, armor.current);
    }
    
    public void HealArmor(int amount) {
	    armor.current = Math.Max(armor.max, armor.current + amount);
    }
    
    public void HealShield(int amount) {
        shield.current = Mathf.Min(shield.max, shield.current + amount);
    }
   
	public void ResetShield()
	{
        shield.current = shield.max;
    }

	public void ResetArmor()
	{
	    armor.current = armor.max;
    }

	public int GetArmor()
	{
	    return armor.current;
    }

	public int GetShield()
	{
        return shield.current;
    }

	public int GetMaxArmor()
	{
	    return armor.max;
    }

    public int GetMaxShields() {
        return shield.max;
    }
    
	void OnTakeDamage(HitDamage damage)
	{
		TakeDamage(damage.Amount);
	}

    void Update()
    {
        const float HEAL_TICK_RATE = 1.0f;
        const int HEAL_AMOUNT = 1;
        
        lastHealTick += Time.deltaTime;

        if (lastHealTick > HEAL_TICK_RATE)
        {
            lastHealTick -= HEAL_TICK_RATE;

            HealShield(HEAL_AMOUNT);
        }
    }
}
