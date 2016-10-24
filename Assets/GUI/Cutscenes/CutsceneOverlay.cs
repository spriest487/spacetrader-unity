using UnityEngine;
using UnityEngine.UI;

public class CutsceneOverlay : MonoBehaviour
{
    [SerializeField]
    private Transform content;

    [SerializeField]
    private Text dialogText;

    [SerializeField]
    private Text speakerText;

    public CutscenePage CurrentCutscenePage
    {
        get { return cutscene.CurrentPage; }
    }

    private Cutscene cutscene;

    public void PlayCutscene(Cutscene cutscene)
    {
        Debug.Assert(!this.cutscene);
        this.cutscene = Instantiate(cutscene);
    }

    private void Refresh()
    {
        content.gameObject.SetActive(true);

        dialogText.text = CurrentCutscenePage.Text;
        speakerText.text = CurrentCutscenePage.Speaker.ToUpper();
    }

    public void AdvanceCutscene()
    {
        cutscene.Next();
    }
}