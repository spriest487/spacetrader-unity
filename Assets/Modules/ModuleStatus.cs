using UnityEngine;

public class ModuleStatus : ScriptableObject
{
    [SerializeField]
    private float cooldown;
    
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
        get { return cooldown; }
    }
    
    public ModuleStatus()
    {
        cooldown = 0;
    }

    public static ModuleStatus Create(ModuleItemType definition)
    {
        ModuleStatus result = CreateInstance<ModuleStatus>();
        result.definition = definition;

        return result;
    } 

    public void Activate(Ship activator, int slot)
    {
        if (cooldown <= Mathf.Epsilon) //cd check
        {
            ModuleType.Behaviour.Activate(activator, slot);
            cooldown = ModuleType.CooldownLength; 
        }
    }
     
    public void Update()
    {
        cooldown -= Time.deltaTime; 
    }
}