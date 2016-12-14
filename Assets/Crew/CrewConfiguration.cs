#pragma warning disable 0649

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "SpaceTrader/Crew/Crew Config")]
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

    [SerializeField]
    private List<CrewMember> characters;

    public IEnumerable<Sprite> Portraits { get { return portraits; } }
    public int PortraitCount { get { return portraits.Count; } }

    public Sprite DefaultPortrait { get { return defaultPortrait; } }

    private IEnumerable<string> Forenames { get { return forenames; } }
    private IEnumerable<string> Surnames { get { return surnames; } }

    public IEnumerable<CrewMember> Characters
    {
        get { return characters; }
    }

    public Sprite GetPortrait(int portrait)
    {
        return portraits[portrait];
    }

    public int GetPortraitIndex(Sprite portrait)
    {
        return portraits.IndexOf(portrait);
    }

    public static CrewConfiguration Create(CrewConfiguration prefab)
    {
        var result = Instantiate(prefab);

        Debug.Assert(result.Portraits.Contains(result.DefaultPortrait), "crew config portrait list must contain the default portrait");

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
        if (!portrait)
        {
            portrait = DefaultPortrait;
        }

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
        result.name = source.name; //get rid of (Clone)

        characters.Add(result);

        return result;
    }

    public void DestroyCharacter(CrewMember character)
    {
        Debug.Assert(characters.Contains(character), "character should be registered in global characters list");

        characters.Remove(character);
    }
}
