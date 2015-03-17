using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ScreenBarButton : MonoBehaviour
{
    private Button button;
    
    [SerializeField]
    private bool requiresPlayer;

    void Start()
    {
        button = GetComponent<Button>();
    }

    void Update()
    {
        button.interactable = PlayerStart.ActivePlayer || !requiresPlayer;
    }
}
