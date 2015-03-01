using UnityEngine;

class HitDamage
{
	public Vector3 Location;
	public int Amount;
    public GameObject Owner;

    public HitDamage(Vector3 location, int amount, GameObject owner)
	{
		this.Location = location;
		this.Amount = amount;
        this.Owner = owner;
	}
}
