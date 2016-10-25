#pragma warning disable 0649

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
        get { return cutscene != null ? cutscene.CurrentPage : null; }
    }

    public bool HasCutscene
    {
        get { return cutscene != null; }
    }

    private Cutscene cutscene;

    public void OnEnable()
    {
        Refresh();
    }
    
    public void PlayCutscene(Cutscene cutscene)
    {
        Debug.Assert(!this.cutscene);
        this.cutscene = Instantiate(cutscene);

        Refresh();
    }

    private void Refresh()
    {
        if (CurrentCutscenePage != null)
        {
            content.gameObject.SetActive(true);

            dialogText.text = CurrentCutscenePage.Text;
            speakerText.text = CurrentCutscenePage.Speaker.ToUpper();
        }
        else
        {
            content.gameObject.SetActive(false);
        }
    }

    public void AdvanceCutscene()
    {
        cutscene.Next();

        Refresh();
    }
}