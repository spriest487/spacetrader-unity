public enum AIOrder
{
    /// <summary>
    /// wait idle (or other default behaviour)
    /// </summary>
    Wait = RadioMessageType.FollowMe,
    //Follow = RadioMessageType.FollowMe,

    /// <summary>
    /// attack your own target
    /// </summary>
    Attack = RadioMessageType.Attack,

    /// <summary>
    /// move to point
    /// </summary>
    Move,
}

public static class AIOrderUtility
{
    public static string GetHUDLabel(this AIOrder order)
    {
        switch (order)
        {
            case AIOrder.Attack: return "ATTACK";
            case AIOrder.Wait: return "FOLLOW";
            default: return "WAIT";
        }
    }
}
