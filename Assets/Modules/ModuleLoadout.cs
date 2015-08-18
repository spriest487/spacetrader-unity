using UnityEngine;

[RequireComponent(typeof(Ship))]
public class ModuleLoadout : MonoBehaviour
{
    [SerializeField]
    private ModuleGroup frontModules;

    private WeaponHardpoint[] hardpoints;
    
    private Ship ship;

    public WeaponHardpoint[] Hardpoints
    {
        get { return hardpoints; }
        private set { hardpoints = value; }
    }
    
    public ModuleGroup FrontModules
    { 
        get
        {
            if (frontModules == null)
            {
                frontModules = ScriptableObject.CreateInstance<ModuleGroup>();
                frontModules.SetParent(this);
            }

            return frontModules;
        }
    }
	
	public void Activate(int index)
	{
		var module = frontModules[index];

		if (!module.Empty)
		{
            WeaponHardpoint hardpoint;
            if (Hardpoints.Length == 0)
            {
                hardpoint = null;                
            }
            else
            {
                hardpoint = Hardpoints[index % Hardpoints.Length];
            }

            module.Activate(ship, hardpoint);
        }
		else
		{
			throw new System.ArgumentException("tried to activate an empty module slot");
		}
	}

	void Start()
	{
		ship = GetComponent<Ship>();
		Hardpoints = ship.GetComponentsInChildren<WeaponHardpoint>();
	}

	void OnShipTypeChange()
	{

	}

	void Update()
	{
		foreach (var module in FrontModules)
		{
			if (!module.Empty)
			{
				module.Update();
			}
		}
	}
}
