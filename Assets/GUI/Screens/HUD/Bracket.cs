﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(CanvasGroup))]
public class Bracket : MonoBehaviour
{
    [SerializeField]
    private Targetable target;

    [SerializeField]
    private Text nameplate;
    [SerializeField]
    private Image[] childImages;
    [SerializeField]
    private Healthbar healthbar;

    [SerializeField]
    private CanvasGroup canvasGroup;
    [SerializeField]
    private RectTransform rectTransform;

    //owner bracket manager
    [SerializeField]
    private BracketManager bracketManager;

    private Canvas canvas;

    public Targetable Target { get { return target; } }

    [SerializeField]
    [HideInInspector]
    private Collider targetCollider;

    [SerializeField]
    [HideInInspector]
    private Hitpoints targetHitpoints;

    [SerializeField]
    private Ship targetShip;

    public static Bracket CreateFromPrefab(Bracket prefab,
        BracketManager manager,
        Targetable target)
    {
        var newBracket = Instantiate(prefab);

        newBracket.bracketManager = manager;
        newBracket.name = "Bracket for " + target.name;
        newBracket.target = target;

        newBracket.targetCollider = target.GetComponent<Collider>();
        newBracket.targetHitpoints = target.GetComponent<Hitpoints>();

        return newBracket;
    }

    void Start()
    {
        canvas = GetComponentInParent<Canvas>();
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

    private void CalculateBoundingBoxSize(out int width, out int height)
    {
        var extents = targetCollider.bounds.extents;
        var corners = new[]
        {
            new Vector3(extents.x, extents.y, extents.z),
            new Vector3(-extents.x, extents.y, extents.z),
            new Vector3(-extents.x, -extents.y, extents.z),
            new Vector3(extents.x, -extents.y, extents.z),
            new Vector3(extents.x, extents.y, -extents.z),
            new Vector3(-extents.x, extents.y, -extents.z),
            new Vector3(-extents.x, -extents.y, -extents.z),
            new Vector3(extents.x, -extents.y, -extents.z),
        };

        var xs = new float[8];
        var ys = new float[8];

        for (int i = 0; i < 8; ++i)
        {
            var cornerScreenPos = Camera.main.WorldToScreenPoint(target.transform.position + corners[i]);
            xs[i] = cornerScreenPos.x;
            ys[i] = cornerScreenPos.y;
        }

        var minCorner = new Vector2(Mathf.Min(xs), Mathf.Min(ys));
        var maxCorner = new Vector2(Mathf.Max(xs), Mathf.Max(ys));

        var diff = maxCorner - minCorner;
        width = (int)(diff.x / canvas.scaleFactor);
        height = (int)(diff.y / canvas.scaleFactor);

        width = Mathf.Clamp(width, bracketManager.DefaultWidth, bracketManager.DefaultWidth * 3);
        height = Mathf.Clamp(height, bracketManager.DefaultHeight, bracketManager.DefaultHeight * 3);
    }
    
    void LateUpdate()
	{
		if (!target)
		{
			return;
		}

        Targetable playerTargetable = null;
        Ship playerShip = null;

        if (PlayerShip.LocalPlayer)
        {
            playerTargetable = PlayerShip.LocalPlayer.GetComponent<Targetable>();
            playerShip = PlayerShip.LocalPlayer.Ship;
        }
        
        var pos = Camera.main.WorldToScreenPoint(target.transform.position);
        
        var x = Mathf.Clamp(pos.x, 0, Screen.width);
        var y = Mathf.Clamp(pos.y, 0, Screen.height);

        /*if it's behind us...
            -things above and behind stick to the top of the screen
            -things below and behind stick to the bottom of the screen 
        */
        if (pos.z < 0)
        {
            var halfway = Screen.height * 0.5f;
            if (y < halfway)
            {
                y = Screen.height;
            }
            else
            {
                y = 0;
            }

            x = Screen.width - x;
        }

        transform.position = new Vector3((int) x, (int) y, transform.position.z);
		canvasGroup.interactable = true;
		canvasGroup.blocksRaycasts = true;
		canvasGroup.alpha = 1;
			
		//todo: calculate screen size of object
		var width = bracketManager.DefaultWidth;
		var height = bracketManager.DefaultHeight;

        if (targetCollider)
        {
            CalculateBoundingBoxSize(out width, out height);
        }

        bool isTarget;
        
        if (playerShip)
        {
            isTarget = playerShip.Target == target;            
        }
        else
        {
            isTarget = false;            
        }
        
        Color reactionColor = bracketManager.GetBracketColor(playerTargetable, target);

        if (isTarget)
        {
            width = (int)(width * bracketManager.SelectedExpand);
            height = (int)(height * bracketManager.SelectedExpand);
        }


		if (childImages != null)
		{
			foreach (var childImage in childImages)
			{
				childImage.color = reactionColor;

				if (isTarget)
				{
					childImage.sprite = bracketManager.SelectedCorner;
				}
				else
				{
					childImage.sprite = bracketManager.Corner;
				}
			}
		}

		if (nameplate)
		{
            nameplate.gameObject.SetActive(isTarget);
			nameplate.color = new Color(reactionColor.r, reactionColor.g, reactionColor.b, 1);

			nameplate.text = target.name.ToUpper();
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
		/*}
		else
		{
			//not visible or interactible
			canvasGroup.interactable = false;
			canvasGroup.blocksRaycasts = false;
			canvasGroup.alpha = 0;
		}*/
	}

    public void SetPlayerTarget()
    {
        var player = PlayerShip.LocalPlayer;
        if (player)
        {
            player.GetComponent<Ship>().Target = target;
        }
    }
}
