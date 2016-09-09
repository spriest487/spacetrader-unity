using UnityEngine;
using System.Collections;

public abstract class ActionOnActivate : MonoBehaviour
{
    public abstract string ActionName { get; }

    public abstract void Activate(Ship activator);
    public abstract bool CanBeActivatedBy(Ship activator);

    public bool TryActivate(Ship activator)
    {
        if (!CanBeActivatedBy(activator))
        {
            return false;
        }

        Activate(activator);
        return true;
    }
}
