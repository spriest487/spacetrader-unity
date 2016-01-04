using System.Collections.Generic;
using UnityEngine;

public class CrewManager : MonoBehaviour
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

    void Start()
    {
        forenames = LoadNamesFromTextAsset(forenameList);
        surnames = LoadNamesFromTextAsset(surnameList);
    }

    private string[] Forenames { get { return forenames; } }
    private string[] Surnames { get { return surnames; } }
}
