using UnityEngine;
using UnityEngine.UI;

public class CrewListItem : MonoBehaviour
{
    [SerializeField]
    private Text name;

    public static CrewListItem CreateFromPrefab(CrewListItem prefab, CrewMember member)
    {
        var instance = Instantiate(prefab);
        instance.name.text = member.name;

        return instance;
    }
}