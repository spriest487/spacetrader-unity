#pragma warning disable 0649

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(Button))]
public class ShipModuleController : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [Header("UI Elements")]

    [SerializeField]
    private Image icon;

    [SerializeField]
    private Transform highlight;

    [SerializeField]
    private Image rarityGlow;
    
    [HideInInspector]
    [SerializeField]
    private Ship ship;
    
    [HideInInspector]
    [SerializeField]
    private int moduleSlot;

    public Ship Ship
    {
        get { return ship; }
    }

    public int ModuleSlot
    {
        get { return moduleSlot; }
    }

    public HardpointModule Module
    {
        get { return Ship.ModuleLoadout.GetSlot(ModuleSlot); }
    }

    public bool Highlighted
    {
        get { return highlight.gameObject.activeSelf; }
        set { highlight.gameObject.SetActive(value); }
    }

    public void OnClickModule()
    {
        SendMessageUpwards("OnSelectShipModule", this);
    }

    public void OnDrag(PointerEventData pointerData)
    {
    }

    public void OnBeginDrag(PointerEventData pointerData)
    {
        if (!ship.ModuleLoadout.IsFreeSlot(moduleSlot))
        {
            Debug.Log("dragged from hardpoint slot " + moduleSlot);
            SendMessageUpwards("OnDragHardpointModule", this);
        }
    }

    public void OnEndDrag(PointerEventData pointerData)
    {
        if (!ship.ModuleLoadout.IsFreeSlot(moduleSlot))
        {
            var hits = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, hits);

            if (hits.Count != 0)
            {
                hits[0].gameObject.SendMessage("OnDropHardpointModule", this, SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    public void Assign(Ship ship, int moduleIndex, bool highlighted)
    {
        this.ship = ship;
        this.moduleSlot = moduleIndex;
        
        var module = Module;
        var itemType = module.ModuleType;
        if (itemType)
        {
            icon.sprite = itemType.Icon;
            icon.gameObject.SetActive(true);
            rarityGlow.gameObject.SetActive(true);
            rarityGlow.color = itemType.Rarity.Color();
        }
        else
        {
            icon.gameObject.SetActive(false);
            rarityGlow.gameObject.SetActive(false);
        }
        
        Highlighted = highlighted;
    }
}
