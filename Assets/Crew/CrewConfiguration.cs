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

    private static string[] LoadNamesFromTextAsset(TextAsset asset)
    {
        return asset.text.Split('\n');
    }

    private void OnEnable()
    {
        forenames = LoadNamesFromTextAsset(forenameList);
        surnames = LoadNamesFromTextAsset(surnameList);
    }

    private string[] Forenames { get { return forenames; } }
    private string[] Surnames { get { return surnames; } }
}
