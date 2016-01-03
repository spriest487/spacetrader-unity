using UnityEngine;

public struct DamageNotification
{
    public int Amount { get; private set; }
    public Hitpoints Target { get; private set; }

    public DamageNotification(int amount, Hitpoints target)
    {
        Amount = amount;
        Target = target;
    }
}
