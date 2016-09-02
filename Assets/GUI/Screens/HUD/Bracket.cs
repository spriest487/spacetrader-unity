#pragma warning disable 0649

using UnityEngine;
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
    private List<Image> childImages;
    [SerializeField]
    private Image edgeMarker;
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
   
    public void Assign(BracketManager manager, Targetable target)
    {
        bracketManager = manager;
        name = "Bracket for " + target.name;
        this.target = target;

        targetCollider = target.GetComponent<Collider>();
        targetHitpoints = target.GetComponent<Hitpoints>();
    }

    void Start()
    {
        Debug.Assert(target, "bracket must have a target");

        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
        
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

    private Camera CameraForTarget()
    {
        if (target.TargetSpace == TargetSpace.Distant)
        {
            return BackgroundCamera.Current.Camera;
        }
        else
        {
            return Camera.main;
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
            var cornerScreenPos = CameraForTarget().WorldToScreenPoint(target.transform.position + corners[i]);
            xs[i] = cornerScreenPos.x;
            ys[i] = cornerScreenPos.y;
        }

        var minCorner = new Vector2(Mathf.Min(xs), Mathf.Min(ys));
        var maxCorner = new Vector2(Mathf.Max(xs), Mathf.Max(ys));

        var diff = maxCorner - minCorner;
        width = (int)(diff.x / canvas.scaleFactor);
        height = (int)(diff.y / canvas.scaleFactor);

        width = Mathf.Clamp(width, bracketManager.DefaultWidth, bracketManager.DefaultWidth * 2);
        height = Mathf.Clamp(height, bracketManager.DefaultHeight, bracketManager.DefaultHeight * 2);
    }

    private void RotateEdgeMarkerToTarget(int x, int y)
    {
        var halfWidth = Screen.width * 0.5f;
        var halfHeight = Screen.height * 0.5f;
        var center = new Vector2(halfWidth, halfHeight);
        var between = new Vector2(x, y) - center;
        
        var angle = Vector2.Angle(Vector2.up, between);
        if (x > halfWidth)
        {
            angle = 360 - angle;
        }

        edgeMarker.transform.rotation = Quaternion.AngleAxis(angle, new Vector3(0, 0, 1));
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

        var pos = CameraForTarget().WorldToScreenPoint(target.transform.position);

        var x = (int) Mathf.Clamp(pos.x, 0, Screen.width);
        var y = (int) Mathf.Clamp(pos.y, 0, Screen.height);
        var z = -pos.z; //closer things should have higher values

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

        if (x == 0 
            || x == Screen.width
            || y == 0 
            || y == Screen.height)
        {
            nameplate.gameObject.SetActive(false);
            healthbar.gameObject.SetActive(false);
            childImages.ForEach(childImage => childImage.gameObject.SetActive(false));
            edgeMarker.gameObject.SetActive(true);

            if (isTarget)
            {
                edgeMarker.sprite = bracketManager.SelectedEdgeMarker;
                edgeMarker.color = reactionColor;
            }
            else
            {
                edgeMarker.sprite = bracketManager.EdgeMarker;
                edgeMarker.color = reactionColor * new Color(1, 1, 1, 0.5f);
            }

            RotateEdgeMarkerToTarget(x, y);
        }
        else
        {
            Color cornerColor;
            Sprite cornerSprite;

            if (isTarget)
            {
                cornerSprite = bracketManager.SelectedCorner;
                cornerColor = reactionColor;
            }
            else
            {
                cornerSprite = bracketManager.Corner;
                cornerColor = reactionColor * new Color(1, 1, 1, 0.5f);
            }

            childImages.ForEach(childImage =>
            {
                childImage.gameObject.SetActive(true);
                childImage.color = cornerColor;
                childImage.sprite = cornerSprite;
            });

            nameplate.gameObject.SetActive(true);

            float nameplateAlpha = isTarget ? 1 : 0.75f;
            nameplate.color = new Color(reactionColor.r, reactionColor.g, reactionColor.b, nameplateAlpha);

            nameplate.text = target.name.ToUpper();
            nameplate.fontSize = isTarget ? 24 : 16;
            
            edgeMarker.gameObject.SetActive(false);

            var width = bracketManager.DefaultWidth;
            var height = bracketManager.DefaultHeight;

            if (targetCollider)
            {
                CalculateBoundingBoxSize(out width, out height);
            }

            if (isTarget)
            {
                width = (int)(width * bracketManager.SelectedExpand);
                height = (int)(height * bracketManager.SelectedExpand);
            }

            rectTransform.sizeDelta = new Vector2(width, height);

            if (targetHitpoints && isTarget)
            {
                healthbar.gameObject.SetActive(true);
                healthbar.ship = targetHitpoints;
            }
            else
            {
                healthbar.gameObject.SetActive(false);
            }
        }

        transform.position = new Vector3(x, y, z);
		canvasGroup.interactable = true;
		canvasGroup.blocksRaycasts = true;
		canvasGroup.alpha = 1;
	}

    public void SetPlayerTarget()
    {
        var player = PlayerShip.LocalPlayer;
        if (player)
        {
            player.Ship.Target = target;
        }
    }
}
