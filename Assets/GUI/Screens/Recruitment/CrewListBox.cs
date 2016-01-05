using UnityEngine;
using System.Collections.Generic;

public class CrewListBox : MonoBehaviour
{
    [SerializeField]
    private CrewListItem itemPrefab;

    [SerializeField]
    private Transform contentArea;

    [SerializeField]
    private TargetCrew targetCrew;

    //cache to save unnecessary reloads
    private List<CrewMember> currentItems;

    public enum TargetCrew
    {
        Current,
        Available
    }

    private void Clear()
    {
        foreach (var child in contentArea.GetComponentsInChildren<CrewListItem>())
        {
            Destroy(child.gameObject);
        }
    }

    private void Update()
    {
        List<CrewMember> crew = null;
        if (targetCrew == TargetCrew.Current)
        {
            //TODO
        }
        else
        {
            if (PlayerShip.LocalPlayer)
            {
                var moorable = PlayerShip.LocalPlayer.GetComponent<Moorable>();

                if (moorable && moorable.SpaceStation)
                {
                    crew = moorable.SpaceStation.AvailableCrew;
                }
            }
        }

        if (crew == null)
        {
            Clear();
            return;
        }

        if (!crew.ElementsEquals(currentItems))
        {
            Clear();

            foreach (var member in crew)
            {
                var newItem = CrewListItem.CreateFromPrefab(itemPrefab, member);
                newItem.transform.SetParent(contentArea.transform, false);
            }

            currentItems = new List<CrewMember>(crew);
        }
    }
}