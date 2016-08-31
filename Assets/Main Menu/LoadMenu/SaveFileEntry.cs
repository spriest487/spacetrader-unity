#pragma warning disable 0649

using UnityEngine;
using UnityEngine.UI;
using SavedGames;

public class SaveFileEntry : Toggle {

    [SerializeField]
    private Text entryText;
    
    private LoadGameMenu loadGameMenu;
        
    public SavesFolder.Entry SaveEntry { get; private set; }

    protected override void Start()
    {
        Debug.Assert(entryText);        
        
        base.Start();
    }

    public void Assign(LoadGameMenu menu, SavesFolder.Entry save)
    {
        Debug.Assert(menu);
        Debug.Assert(save != null);

        loadGameMenu = menu;
        SaveEntry = save;

        var incompatible = save.Header.Version != SaveHeader.CURRENT_VERSION;
        if (incompatible)
        {
            entryText.text = "Incompatible save";
        }
        else
        {
            entryText.text = string.Format("{0} - {1}", save.Header.CharacterName, save.Header.TimeStamp);
        }
    }

    public void SelectInMenu()
    {
        loadGameMenu.SelectEntry(this);
    }    
}
