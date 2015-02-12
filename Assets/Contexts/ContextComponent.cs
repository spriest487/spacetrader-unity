using System;
using UnityEngine;

public abstract class ContextComponent<TObject> where TObject : Component
{
    public abstract TObject Value { get; }
    public abstract bool HasValue { get; }

    public override string ToString()
    {
        return HasValue ? "ContextRef [" + Value.ToString() + "]" : "ContextRef (empty)";
    }
}
