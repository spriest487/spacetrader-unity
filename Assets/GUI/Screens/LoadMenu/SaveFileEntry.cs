#pragma warning disable 0649

using UnityEngine;
using UnityEngine.UI;
using SavedGames;
using System.Globalization;

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
            var time = save.Header.TimeStamp.ToString("yyyy/MM/dd HH:mm", CultureInfo.InvariantCulture);

            entryText.text = string.Format("{0} - {1}", save.Header.CharacterName, time);
        }
    }

    public void SelectInMenu()
    {
        loadGameMenu.SelectEntry(this);
    }
}
