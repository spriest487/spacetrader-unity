using UnityEngine;
using UnityEngine.UI;

public class CargoHoldListItem : MonoBehaviour
{
    [SerializeField]
    Text label;

    [SerializeField]
    string itemName;

    [SerializeField]
    int quantity;

    public static CargoHoldListItem CreateFromPrefab(CargoHoldListItem prefab, string name, int quantity)
    {
        var result = Instantiate(prefab);
        result.itemName = name;
        result.quantity = quantity;

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
