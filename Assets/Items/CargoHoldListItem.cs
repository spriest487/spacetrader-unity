using UnityEngine;
using UnityEngine.UI;

public class CargoHoldListItem : MonoBehaviour
{
    [SerializeField]
    private Text label;

    [SerializeField]
    private Image icon;

    [SerializeField]
    private CargoItemType itemType;

    [SerializeField]
    private int quantity;

    public static CargoHoldListItem CreateFromPrefab(CargoHoldListItem prefab, CargoItemType itemType, int quantity)
    {
        var result = Instantiate(prefab);
        result.quantity = quantity;

        result.label.text = itemType.DisplayName;
        result.icon.sprite = itemType.Icon;
        result.itemType = itemType;

        return result;
    }

    void Start()
    {
        label.text = itemType.DisplayName; 

        if (quantity > 1)
        {
            label.text += " (" + quantity + ")";
        }
    }
}
