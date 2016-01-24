using UnityEngine;

public class HUD : MonoBehaviour
{
    [SerializeField]
    public Transform content;

    private void Update()
    {
        bool visible = true;

        if (ScreenManager.Instance.CurrentCutscenePage != null)
        {
            visible = false;
        }

        content.gameObject.SetActive(visible);
    }
}