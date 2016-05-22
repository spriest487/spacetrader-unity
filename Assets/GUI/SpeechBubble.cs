using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class SpeechBubble : MonoBehaviour
{
    [SerializeField]
    private RectTransform text;

    [SerializeField]
    private RectTransform icon;

    public void Update()
    {
        var pos = transform.position;
        float scaleX;
        float scaleY;
        
        if (pos.x > Screen.width / 2)
        {
            scaleX = -1;
        }
        else
        {
            scaleX = 1;
        }

        if (pos.y > Screen.height / 2)
        {
            scaleY = -1;
        }
        else
        {
            scaleY = 1;
        }

        var scale = new Vector3(scaleX, scaleY, 1);
        transform.localScale = scale;
        text.transform.localScale = scale;
        icon.transform.localScale = scale;
    }
}
