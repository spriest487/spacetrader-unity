using UnityEngine;
using UnityEngine.UI;

public class CargoHoldListItem : MonoBehaviour
{
    [SerializeField]
    private Text label;

    [SerializeField]
    private string itemName;

    [SerializeField]
    private int quantity;

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
