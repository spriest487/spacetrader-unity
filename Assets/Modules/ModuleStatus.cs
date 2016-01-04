using UnityEngine;

public class ModuleStatus : ScriptableObject
{
    [SerializeField]
    private float cooldown;
    
    [SerializeField]
    private Vector3 aim;

    [SerializeField]
    private ModuleDefinition definition;

    public Vector3 Aim
    {
        get { return aim; }
        set { aim = value; }
    }

    public ModuleDefinition Definition
    {
        get { return definition; }
    }

    public float Cooldown
    {
        get { return cooldown; }
    }
    
    public ModuleStatus()
    {
        cooldown = 0;
    }

    public static ModuleStatus Create(ModuleDefinition definition)
    {
        ModuleStatus result = CreateInstance<ModuleStatus>();
        result.definition = definition;

        return result;
    } 

    public void Activate(Ship activator, WeaponHardpoint hardpoint)
    {
        if (cooldown <= Mathf.Epsilon) //cd check
        {
            Definition.Behaviour.Activate(activator, hardpoint, this);
            cooldown = Definition.CooldownLength; 
        }
    }
     
    public void Update()
    {
        cooldown -= Time.deltaTime; 
    }
}