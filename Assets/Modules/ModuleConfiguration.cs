using UnityEngine;
using System.Collections.Generic;

public class ModuleConfiguration : MonoBehaviour
{
	private readonly Dictionary<string, ModuleDefinition> definitions;

	public Transform bullet;
	public Transform bulletMuzzleFlash;

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
    
	ModuleConfiguration()
	{
		definitions = new Dictionary<string, ModuleDefinition>();

		definitions.Add("Laser Gun", new ModuleDefinition("Laser Gun",
			new GunBehaviour(1, bullet, bulletMuzzleFlash),
			0.5f
		));
		definitions.Add("Heavy Laser Gun", new ModuleDefinition("Heavy Laser Gun",
			new GunBehaviour(2, bullet, bulletMuzzleFlash),
			1.0f
		));
	}
}
