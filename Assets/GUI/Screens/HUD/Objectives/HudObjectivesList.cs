using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(LayoutGroup))]
public class HudObjectivesList : MonoBehaviour
{
    [SerializeField]
    private HudObjectivesItem objectiveItemPrefab;

    private void ClearItems()
    {
        //clear children
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
        
    private void ReloadItems()
    {
        ClearItems();
    }

    void Update()
    {
        var player = PlayerShip.LocalPlayer;
        
        if (player)
        {
            var targetable = player.GetComponent<Targetable>();

            if (targetable)
            {
                var currentItems = gameObject.GetComponentsInChildren<HudObjectivesItem>();
                var objectives = MissionManager.ActiveMission.FindObjectives(targetable.Faction);

                var currentCount = currentItems.Length;
                var newCount = objectives.Length;
                
                /* if necessary recreate the items */
                if (newCount != currentCount)
                {
                    ClearItems();
                    currentItems = new HudObjectivesItem[newCount];

                    for (int item = 0; item < newCount; ++item)
                    {
                        var newItem = Instantiate(objectiveItemPrefab);
                        newItem.transform.SetParent(transform, false);

                        currentItems[item] = newItem;
                    }
                }

                /* give each item in the list an objective (we should have one per
                objective now) */
                for (int item = 0; item < newCount; ++item)
                {
                    currentItems[item].Objective = objectives[item];
                }
            }
            else
            {
                ClearItems();
            }
        }
        else
        {
            ClearItems();
        }
    }
}