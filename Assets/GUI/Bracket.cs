using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class Bracket : MonoBehaviour
{
	public Targetable target;

	public Text nameplate;
	public Image[] childImages; 
	public Healthbar healthbar;

	private CanvasGroup canvasGroup;
	private RectTransform rectTransform;
	
	//owner bracket manager
	public BracketManager bracketManager;

    public int defaultWidth = 64;
    public int defaultHeight = 64;
    public float selectedExpand = 1.25f;

	void Start()
	{
		canvasGroup = GetComponent<CanvasGroup>();
		rectTransform = GetComponent<RectTransform>();

		if (!target)
		{
			throw new UnityException("no target for bracket");
		}

		var hitpoints = target.GetComponent<Hitpoints>();
		foreach (var healthbar in GetComponentsInChildren<Healthbar>())
		{
			if (hitpoints)
			{
				healthbar.ship = hitpoints;
			}
			else
			{
				healthbar.gameObject.SetActive(false);
			}
		}
	}

	void LateUpdate()
	{
		if (!target)
		{
			return;
		}

        Targetable playerTargetable = null;
        Ship playerShip = null;

        if (PlayerStart.ActivePlayer)
        {
            playerTargetable = PlayerStart.ActivePlayer.GetComponent<Targetable>();
            playerShip = PlayerStart.ActivePlayer.GetComponent<Ship>();
        }		

		var targetHitpoints = target.GetComponent<Hitpoints>();
		
		var pos = Camera.main.WorldToScreenPoint(target.transform.position);
		if (pos.z > 0)
		{
			transform.position = new Vector3(pos.x, pos.y, transform.position.z);
			canvasGroup.interactable = true;
			canvasGroup.blocksRaycasts = true;
			canvasGroup.alpha = 1;
			
			//todo: calculate screen size of object
			var width = defaultWidth;
			var height = defaultHeight;

			bool isTarget = playerShip && playerShip.target == target;
			bool sameFaction = playerTargetable && string.Equals(playerTargetable.faction, target.faction);

            if (isTarget)
            {
                width = (int)(width * selectedExpand);
                height = (int)(height * selectedExpand);
            }
			
			Color reactionColor;
			if (bracketManager)
			{
				reactionColor = sameFaction ? bracketManager.friendlyColor : bracketManager.hostileColor;
			}
			else
			{
				reactionColor = Color.white;
			}

			if (childImages != null)
			{
				foreach (var childImage in childImages)
				{
					childImage.color = reactionColor;

					if (isTarget)
					{
						childImage.sprite = bracketManager.selectedCorner;
					}
					else
					{
						childImage.sprite = bracketManager.corner;
					}
				}
			}
			if (nameplate)
			{
				nameplate.color = reactionColor;
				nameplate.text = target.name;
			}
			if (healthbar)
			{
				if (targetHitpoints)
				{
					healthbar.gameObject.SetActive(true);
					healthbar.ship = targetHitpoints;
				}
				else
				{
					healthbar.gameObject.SetActive(false);
				}
			}
			
			rectTransform.sizeDelta = new Vector2(width, height);
		}
		else
		{
			//not visible or interactible
			canvasGroup.interactable = false;
			canvasGroup.blocksRaycasts = false;
			canvasGroup.alpha = 0;
		}
	}
}
