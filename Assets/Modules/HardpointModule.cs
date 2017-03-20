#pragma warning disable 0649

using UnityEngine;
using System;
using Object = UnityEngine.Object;

[Serializable]
public class HardpointModule
{
    [SerializeField]
    private ModuleBehaviour behaviour;
    
    [SerializeField]
    private Vector3 aim;

    [SerializeField]
    private ModuleItemType definition;

    public ModuleBehaviour Behaviour
    {
        get { return behaviour; }
    }

    public Vector3 Aim
    {
        get { return aim; }
        set { aim = value; }
    }

    public ModuleItemType ModuleType
    {
        get { return definition; }
    }

    public float Cooldown
    {
        get { return behaviour? behaviour.Cooldown : 0; }
        set
        {
            if (!behaviour)
            {
                Debug.LogWarning("setting cooldown on module with no behaviour");
                return;
            }
            behaviour.Cooldown = value;
        }
    }
    
    public HardpointModule()
    {
        definition = null;
        behaviour = null;
    }

    public HardpointModule(ModuleItemType definition): this()
    {
        if (definition)
        {
            this.definition = definition;
            behaviour = Object.Instantiate(definition.Behaviour);
            behaviour.Equip(this);
        }
    }

    public void Activate(Ship activator, int slot)
    {
        if (Cooldown <= Mathf.Epsilon) //cd check
        {
            behaviour.Activate(activator, slot);
        }
    }

    public void UpdateBehaviour(Ship owner)
    {
        if (behaviour)
        {
            behaviour.UpdateForOwner(owner);
        }
    }
}