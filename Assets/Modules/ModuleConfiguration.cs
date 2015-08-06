using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ModuleConfiguration : MonoBehaviour
{
    [SerializeField]
    private ModuleDefinition[] definitionsArray;

    private Dictionary<string, ModuleDefinition> definitions;
    private Dictionary<string, ModuleDefinition> Definitions
    { 
        get
        {
            if (definitions == null)
            {
                definitions = new Dictionary<string, ModuleDefinition>();
                foreach (var definition in definitionsArray)
                {
                    definitions.Add(definition.Name, definition);
                }
            }

            return definitions;
        }
    }

	public Transform bullet;
	public Transform bulletMuzzleFlash;

    public static ModuleConfiguration Instance { get; private set; }

	public ModuleDefinition GetDefinition(string moduleType)
	{
		ModuleDefinition definition;
		Definitions.TryGetValue(moduleType, out definition);

		if (definition == null)
		{
			throw new System.ArgumentException("definition not found for module type: " +moduleType);
		}

		return definition;
	} 

	public string[] DefinitionNames
	{
		get
		{
            string[] names = new string[Definitions.Count];
			int i = 0;
			foreach (var name in Definitions.Keys)
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

            definitionsList.Add(ModuleDefinition.Create("Training Laser",
                GunBehaviour.Create(1, bullet, bulletMuzzleFlash),
                0.2f
            ));

            definitionsArray = definitionsList.ToArray();
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
