#pragma warning disable 0649

using System.Collections.Generic;
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

    public IList<Sprite> Portraits { get { return portraits; } }
    public Sprite DefaultPortrait { get { return defaultPortrait; } }

    private static string[] LoadNamesFromTextAsset(TextAsset asset)
    {
        return asset.text.Split('\n');
    }

    private void OnEnable()
    {
        forenames = LoadNamesFromTextAsset(forenameList);
        surnames = LoadNamesFromTextAsset(surnameList);
    }

    private IEnumerable<string> Forenames { get { return forenames; } }
    private IEnumerable<string> Surnames { get { return surnames; } }
}
