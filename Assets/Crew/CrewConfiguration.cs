#pragma warning disable 0649

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CrewConfiguration : ScriptableObject
{
    [SerializeField]
    private TextAsset forenameList;

    [SerializeField]
    private TextAsset surnameList;

    private string[] forenames;
    private string[] surnames;

    [SerializeField]
    private List<Sprite> portraits;

    [SerializeField]
    private Sprite defaultPortrait;
    
    //global list of all characters in the game!
    [SerializeField]
    private List<CrewMember> characters;

    public IList<Sprite> Portraits { get { return portraits; } }
    public Sprite DefaultPortrait { get { return defaultPortrait; } }

    private IEnumerable<string> Forenames { get { return forenames; } }
    private IEnumerable<string> Surnames { get { return surnames; } }

    public IEnumerable<CrewMember> Characters
    {
        get { return characters; }
    }

    public static CrewConfiguration Create(CrewConfiguration prefab)
    {
        var result = Instantiate(prefab);

        result.forenames = LoadNamesFromTextAsset(result.forenameList);
        result.surnames = LoadNamesFromTextAsset(result.surnameList);
        
        result.characters = new List<CrewMember>();

        return result;
    }

    private static string[] LoadNamesFromTextAsset(TextAsset asset)
    {
        return asset.text.Split('\n');
    }
   
    public CrewMember NewCharacter(string name, Sprite portrait)
    {
        Debug.Assert(portrait == defaultPortrait || portraits.Contains(portrait), 
            "portrait for character must be in the portraits list");

        var result = CreateInstance<CrewMember>();
        result.name = name;
        result.Portrait = portrait;

        characters.Add(result);

        return result;
    }

    public CrewMember NewCharacter(CrewMember source)
    {
        var result = Instantiate(source);

        characters.Add(result);

        return result;
    }

    public void DestroyCharacter(CrewMember character)
    {
        Debug.Assert(characters.Contains(character), "character should be registered in global characters list");

        characters.Remove(character);
    }
}
