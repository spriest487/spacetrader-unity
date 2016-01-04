using System.Collections.Generic;
using UnityEngine;

public class CrewConfiguration : ScriptableObject
{
#if UNITY_EDITOR
    [UnityEditor.MenuItem("Assets/Create/SpaceTrader/Crew/Crew configuration")]
    public static void Create()
    {
        ScriptableObjectUtility.CreateAsset<CrewConfiguration>();
    }
#endif

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
