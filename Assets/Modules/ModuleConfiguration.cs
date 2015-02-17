using UnityEngine;
using System.Collections.Generic;

public class ModuleConfiguration : MonoBehaviour
{
    [SerializeField]
    private ModuleDefinition[] definitionsArray;

    private Dictionary<string, ModuleDefinition> definitions;

	public Transform bullet;
	public Transform bulletMuzzleFlash;

    public static ModuleConfiguration Instance { get; private set; }

	public ModuleDefinition GetDefinition(string moduleType)
	{
		ModuleDefinition definition;
		definitions.TryGetValue(moduleType, out definition);

		if (definition == null)
		{
			throw new System.ArgumentException("definition not found for module type: " +moduleType);
		}

		return definition;
	}

	public string[] definitionNames
	{
		get
		{
            string[] names = new string[definitions.Count];
			int i = 0;
			foreach (var name in definitions.Keys)
			{
				names[i++] = name;
			}

			return names;
		}
	}
    
    void Start()
    {
        if (definitionsArray == null || definitionsArray.Length == 0)
        {
            List<ModuleDefinition> definitionsList = new List<ModuleDefinition>();

            definitionsList.Add(ModuleDefinition.Create("Laser Gun",
                GunBehaviour.Create(1, bullet, bulletMuzzleFlash),
                0.1f
            ));
            
            definitionsList.Add(ModuleDefinition.Create("Heavy Laser Gun",
                GunBehaviour.Create(2, bullet, bulletMuzzleFlash),
                1.0f
            ));

            definitionsArray = definitionsList.ToArray();
        }

        definitions = new Dictionary<string, ModuleDefinition>();
        foreach (ModuleDefinition definition in definitionsArray)
        {
            definitions.Add(definition.Name, definition);
        }
    }
    
	void OnEnable()
	{
        if (Instance != null)
        {
            throw new UnityException("can't have more than one instance of ModuleConfiguration per scene");   
        }

        Instance = this;
	}

    void OnDisable()
    {
        if (Instance != this)
        {
            throw new UnityException("failed to remove ModuleConfiguration singleton, it's set to another instance");
        }

        Instance = null;
    }
}
