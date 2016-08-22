using UnityEngine;

class HitDamage
{
	public Vector3 Location;
	public int Amount;
    public Ship Owner;

    public HitDamage(Vector3 location, int amount, Ship owner)
	{
		this.Location = location;
		this.Amount = amount;
        this.Owner = owner;
	}
}
