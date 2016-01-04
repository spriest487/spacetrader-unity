using UnityEngine;
using System.Collections;

public class CargoHold : MonoBehaviour
{
    [SerializeField]
    string[] items;

    [SerializeField]
    int size;

    public string[] Items {
        get { return items; }
        set { items = (string[]) value.Clone(); }
    }

    public int Size
    {
        get { return size; }
        set { size = value; }
    }
}
