#pragma warning disable 0649

using UnityEngine;
using UnityEngine.UI;

public class CutscenePanel : MonoBehaviour
{
    [SerializeField]
    private Transform content;

    [SerializeField]
    private Text dialogText;

    [SerializeField]
    private Text speakerText;

    private void Update()
    {
        var cutscene = ScreenManager.Instance.CurrentCutscenePage;

        if (cutscene != null)
        {
            content.gameObject.SetActive(true);

            dialogText.text = cutscene.Text;
            speakerText.text = cutscene.Speaker.ToUpper();
        }
        else
        {
            content.gameObject.SetActive(false);
        }
    }

    public void Advance()
    {
        ScreenManager.Instance.AdvanceCutscene();
    }
}