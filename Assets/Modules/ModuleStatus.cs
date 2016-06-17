#pragma warning disable 0649

using UnityEngine;
using System;

[Serializable]
public class HardpointModule
{
    [SerializeField]
    private ModuleBehaviour behaviour;
    
    [SerializeField]
    private Vector3 aim;

    [SerializeField]
    private ModuleItemType definition;

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
    }
    
    public HardpointModule()
    {
        definition = null;
        behaviour = null;
    }

    public HardpointModule(ModuleItemType definition): this()
    {
        this.definition = definition;
        if (definition)
        {
            this.behaviour = definition ? ScriptableObject.Instantiate(definition.Behaviour) : null;
        }
    }

    public void Activate(Ship activator, int slot)
    {
        if (Cooldown <= Mathf.Epsilon) //cd check
        {
            ModuleType.Behaviour.Activate(activator, slot);
        }
    }
}