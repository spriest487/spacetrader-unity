using UnityEngine;

struct HitDamage
{
	public Vector3 source;
	public int amount;

	public HitDamage(Vector3 source, int amount)
	{
		this.source = source;
		this.amount = amount;
	}
}
