using System.Collections.Generic;
using UnityEngine;

sealed class Box
{
    public object obj;
}
    
sealed class BoxRef<TObject> : ContextComponent<TObject> where TObject : Component
{
    public Box box;
        
    public override TObject Value
    {
        get
        {
            return HasValue ? (TObject) box.obj : null;
        }
    }

    public override bool HasValue
    {
        get
        {
            return box.obj != null && ((TObject)box.obj).gameObject.activeInHierarchy;
        }
    }    
}
    
public class Context
{
    private IDictionary<System.Type, Box> objects;

    public Context(object[] values)
    {
        objects = new Dictionary<System.Type, Box>();

        foreach (var value in values)
        {
            Install(value.GetType(), value);
        }
    }

    public ContextComponent<TObject> Get<TObject>() where TObject : Component
    {
        var type = typeof(TObject);
        var newRef = (BoxRef<TObject>) System.Activator.CreateInstance(typeof(BoxRef<>).MakeGenericType(type));
            
        Box box;
        if (!objects.TryGetValue(type, out box))
        {
            objects.Add(type, box = new Box());
        }

        newRef.box = box;

        return (ContextComponent<TObject>)newRef;
    }
    
    public void Install(System.Type type, object value)
    {
        if (value == null)
        {
            throw new System.ArgumentException("can't install a null value in a context - for type " +type);
        }

        Box box;
        if (!objects.TryGetValue(type, out box))
        {
            objects.Add(type, box = new Box());
        }

        if (box.obj != null)
        {
            throw new System.ArgumentException("can't overwrite context value of type " +type + " (unset it first?)");
        }

        box.obj = value;
    }

    public void Uninstall(System.Type type)
    {
        Box box;
        if (!objects.TryGetValue(type, out box))
        {
            objects.Add(type, box = new Box());
        }

        if (box.obj == null)
        {
            throw new System.ArgumentException("can't uninstall a value that's already null - for type " +type);
        }

        box.obj = null;
    }

    public void Clear()
    {
        objects.Clear();
    }
}
