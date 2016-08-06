#pragma warning disable 0649

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Ship))]
public class LootContainer : MonoBehaviour
{
    public Ship Ship { get; private set; }

    void Start()
    {
        Ship = GetComponent<Ship>();
    }

    void OnActivated(Ship activator)
    {
        ScreenManager.Instance.BroadcastScreenMessage(PlayerStatus.Flight, ScreenID.None, "OnPlayerActivatedLoot", this);
    }
}
