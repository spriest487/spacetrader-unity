public enum RadioMessageType
{
    Greeting,

    FollowMe,
    Attack,

    AcknowledgeOrder,
    HelpMe,
}

public struct RadioMessage
{
    private readonly Ship source;
    private RadioMessageType messageType;

    public Ship SourceShip { get { return source; } }
    public RadioMessageType MessageType { get { return messageType; } }

    public RadioMessage(Ship source, RadioMessageType messageType)
    {
        this.source = source;
        this.messageType = messageType;
    }
}
