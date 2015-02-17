using UnityEngine;

public class ModuleStatus : ScriptableObject
{
    public static ModuleStatus None { get; private set; }

    static ModuleStatus()
    {
        None = ScriptableObject.CreateInstance<ModuleStatus>();
    }
    
    [SerializeField]
    private float cooldown;

    [SerializeField]
    private string definitionName;

    [SerializeField]
    private ModuleGroup moduleGroup;

    /* don't serialize the current definition, if we lose the instance
     just look it up in the global map again */
    private ModuleDefinition definition;

    public ModuleDefinition Definition
    {
        get
        {
            return definition != null?
                definition :
                (definition = ModuleConfiguration.Instance.GetDefinition(definitionName));
        }
    }

    public float Cooldown
    {
        get
        {
            return cooldown;
        }
    }

    public string Name
    {
        get
        {
            return definitionName;
        }
    }

    public bool Empty { get; private set; }   

#if UNITY_EDITOR
    public bool FoldoutState { get; set; }
#endif

    public ModuleStatus()
    {
        cooldown = 0;

#if UNITY_EDITOR
        FoldoutState = true;
#endif
    }


    public static ModuleStatus Create(string moduleName, ModuleGroup moduleGroup)
    {
        if (moduleName == null || moduleGroup == null)
        {
            throw new UnityException("bad arguments to Setup()");
        }

        ModuleStatus result = ScriptableObject.CreateInstance<ModuleStatus>();
        result.moduleGroup = moduleGroup;
        result.definitionName = moduleName;
        return result;
    }

    public void Activate(Ship activator, WeaponHardpoint hardpoint)
    {
        if (cooldown <= Mathf.Epsilon) //cd check
        {
            Definition.Behaviour.Activate(activator, hardpoint);
            cooldown = Definition.CooldownLength;
        }
    }

    public void Update()
    {
        cooldown -= Time.deltaTime;
    }
}