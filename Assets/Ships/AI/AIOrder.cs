public enum AIOrder
{
    Wait = RadioMessageType.Wait,
    Follow = RadioMessageType.FollowMe,
    Attack = RadioMessageType.Attack,
    Move,
}

public static class AIOrderUtility
{
    public static string GetHUDLabel(this AIOrder order)
    {
        switch (order)
        {
            case AIOrder.Attack: return "ATTACK";
            case AIOrder.Follow: return "FOLLOW";
            default: return "WAIT";
        }
    }
}