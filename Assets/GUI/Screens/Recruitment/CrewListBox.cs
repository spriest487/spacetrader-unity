using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CrewListBox : MonoBehaviour
{
    [SerializeField]
    private CrewListItem itemPrefab;

    [SerializeField]
    private Transform contentArea;

    [SerializeField]
    private TargetCrew targetCrew;

    [SerializeField]
    private GameObject emptyLabel;

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

        if (PlayerShip.LocalPlayer)
        {
            if (targetCrew == TargetCrew.Current)
            {
                var playerShip = PlayerShip.LocalPlayer.Ship;

                crew = new List<CrewMember>();
                if (playerShip.CrewAssignments.Captain)
                {
                    crew.Add(playerShip.CrewAssignments.Captain);
                }
                
                crew.AddRange(playerShip.CrewAssignments.Passengers);
            }
            else
            {
                var moorable = PlayerShip.LocalPlayer.GetComponent<Moorable>();

                if (moorable && moorable.SpaceStation)
                {
                    crew = moorable.SpaceStation.AvailableCrew;
                }
            }
        }

        if (crew == null || crew.Count == 0)
        {
            Clear();
            emptyLabel.gameObject.SetActive(true);
            return;
        }

        if (!crew.ElementsEquals(currentItems))
        {
            Clear();
            emptyLabel.gameObject.SetActive(false);

            foreach (var member in crew)
            {
                var newItem = CrewListItem.CreateFromPrefab(itemPrefab, member);
                newItem.transform.SetParent(contentArea.transform, false);
            }

            currentItems = new List<CrewMember>(crew);
        }
    }
}