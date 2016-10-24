#pragma warning disable 0649

using System.Collections;
using SavedGames;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;

public class LoadGameMenu : MonoBehaviour
{
    public class SaveDateOrder : IComparer<SavesFolder.Entry>
    {
        public int Compare(SavesFolder.Entry x, SavesFolder.Entry y)
        {
            return x.Header.TimeStamp.CompareTo(y.Header.TimeStamp);
        }
    }

    [SerializeField]
    private ToggleGroup fileList;

    [SerializeField]
    private Image selectedPortrait;

    [SerializeField]
    private SaveFileEntry entryPrefab;

    [SerializeField]
    private Button loadButton;

    [SerializeField]
    private Button deleteButton;

    private GUIController guiController;

    private PooledList<SaveFileEntry, string> entries;

    private SaveFileEntry selectedEntry;

    private void Start()
    {
        guiController = GetComponentInParent<GUIController>();
    }
    
    private void OnEnable()
    {
        Debug.Assert(fileList);
        Debug.Assert(entryPrefab);
        
        Refresh();
    }

    private void Refresh()
    {
        if (entries == null)
        {
            entries = new PooledList<SaveFileEntry, string>(fileList.transform, entryPrefab);
        }

        var saves = SavesFolder.GetSaves().ToList();
        saves.Sort(new SaveDateOrder());

        var paths = saves.Select(s => s.Path);

        entries.Refresh(paths, (i, entry, path) =>
        {
            entry.group = fileList;
            entry.Assign(this, saves[i]);
        });

        SelectEntry(entries.FirstOrDefault());

        foreach (var entry in entries)
        {
            entry.isOn = entry == selectedEntry;
        }
    }
    
    public void Load()
    {
        if (selectedEntry == null)
        {
            return;
        }

        SpaceTraderConfig.Instance.StartCoroutine(LoadGameRoutine(selectedEntry.SaveEntry.Path));
    }

    private IEnumerator LoadGameRoutine(string path)
    {
        yield return null;
        yield return guiController.SwitchTo(ScreenID.LoadInProgress);

        var loadSave = SavesFolder.LoadGame(path);
        yield return loadSave;

        if (loadSave.Error != null)
        {
            Debug.LogException(loadSave.Error);
        }

        yield return guiController.SwitchTo(ScreenID.None);
    }

    public void Delete()
    {
        if (selectedEntry == null)
        {
            return;
        }

        SavesFolder.DeleteGame(selectedEntry.SaveEntry.Path);
        Refresh();
    }

    public void SelectEntry(SaveFileEntry entry)
    {
        selectedEntry = entry;

        if (entry != null)
        {
            selectedPortrait.sprite = entry.SaveEntry.Header.GetPortraitSprite();
            if (!selectedPortrait.sprite)
            {
                selectedPortrait.sprite = SpaceTraderConfig.CrewConfiguration.DefaultPortrait;
            }
        }

        bool hasEntry = entry;

        loadButton.interactable = hasEntry;
        deleteButton.interactable = hasEntry;
        selectedPortrait.gameObject.SetActive(hasEntry);
    }
}
