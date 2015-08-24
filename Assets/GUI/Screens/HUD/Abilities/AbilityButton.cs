using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AbilityButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private Image image;

    [SerializeField]
    private Text cooldownLabel;
    
    [SerializeField]
    private Ability ability;

    private bool pointerOver;

    public Ability Ability
    {
        get { return ability; }
        set
        {
            ability = value;
            image.sprite = ability.Icon;
        }
    }

    public bool PointerOver
    {
        get { return pointerOver; }
    }

    public void ActivateAbility()
    {
        if (ability.Cooldown > 0)
        {
            return;
        }

        var player = PlayerShip.LocalPlayer;
        if (player)
        {
            var ship = player.GetComponent<Ship>();

            ability.Use(ship);
        }
    }

    void Update()
    {
        if (ability.Cooldown > 0)
        {
            image.color = new Color(1, 1, 1, 0.5f);

            cooldownLabel.text = ability.Cooldown.ToString("n1");
            cooldownLabel.gameObject.SetActive(true);
        }
        else
        {
            image.color = new Color(1, 1, 1, 1);
            cooldownLabel.gameObject.SetActive(false);
        }
    }

    public void OnPointerEnter(PointerEventData ev)
    {
        pointerOver = true;
    }

    public void OnPointerExit(PointerEventData ev)
    {
        pointerOver = false;
    }
}