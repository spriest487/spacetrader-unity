#pragma warning disable 0649

using UnityEngine;

public class AutoPlayCutscene : MonoBehaviour
{
    [SerializeField]
    private Cutscene cutscene;

    private void Start()
    {
        ScreenManager.Instance.PlayCutscene(cutscene);
        Destroy(gameObject);
    }
}
