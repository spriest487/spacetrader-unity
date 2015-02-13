using UnityEngine;
using System.Collections;

public class SunTint : MonoBehaviour
{
    public string tintAttributeName = "_TintColor";
    public Color baseTint = Color.white;

    public Light sunLight;

    void Update()
    {
        if (sunLight && renderer && renderer.material)
        {
            renderer.material.SetColor(tintAttributeName, baseTint * sunLight.color);
        }
    }
}
