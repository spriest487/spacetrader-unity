using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(CanvasScaler))]
public class MinimumSizeCanvasScaler : MonoBehaviour
{
    [SerializeField]
    private int minWidth = 1024;

    [SerializeField]
    private int minHeight = 768;

    private CanvasScaler canvasScaler;

    private void Start()
    {
        canvasScaler = GetComponent<CanvasScaler>();
    }

    private void Update()
    {
        float scale;
        if (Screen.height < minHeight || Screen.width < minWidth)
        {
            scale = Mathf.Min(Screen.height / (float)minHeight, Screen.width / (float)minWidth);
        }
        else
        {
            scale = 1;
        }

        canvasScaler.scaleFactor = scale;
    }

    private void OnScreenActive()
    {
        Start();
        Update();
    }

#if UNITY_EDITOR
    private void OnGUI()
    {
        Update();
    }
#endif
}
