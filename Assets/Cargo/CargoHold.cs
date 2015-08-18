using UnityEngine;
using System.Collections;

public class CargoHold : MonoBehaviour
{
    [SerializeField]
    string[] items;

    public string[] Items {
        get { return items; }
        set { items = (string[]) value.Clone(); }
    }
}
