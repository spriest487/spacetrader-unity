using UnityEngine;
using UnityEngine.UI;

public class CargoHoldListItem : MonoBehaviour
{
    [SerializeField]
    private Text label;

    [SerializeField]
    private Image icon;

    [SerializeField]
    private string itemName;

    [SerializeField]
    private int quantity;

    public static CargoHoldListItem CreateFromPrefab(CargoHoldListItem prefab, CargoItemType itemType, int quantity)
    {
        var result = Instantiate(prefab);
        result.quantity = quantity;

        result.label.text = itemType.DisplayName;
        result.icon.sprite = itemType.Icon;

        return result;
    }

    void Start()
    {
        label.text = itemName; 

        if (quantity > 1)
        {
            label.text += " (" + quantity + ")";
        }
    }
}
